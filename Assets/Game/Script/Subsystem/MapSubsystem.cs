using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Map.Logic;
using Game.Script.Res;
using Game.Script.UI;
using Game.Script.UI.Frames;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Game.Script.Subsystem
{
    public class MapSubsystem : GameSubsystem
    {
        string AssetMapPath => "Assets/Game/Res/Map/Data/";
        private string playerAssetPath => "Assets/Game/Res/Player/PlayerPrefab.prefab";

        string MapExtension => ".txt";
        private MapBk _mapBk;
        public MapData CurMapData { get; private set; }

        private readonly Dictionary<uint, MapArea> _areas = new();
        public bool MapLoaded { get; private set; }

        public MapBk MapBk
        {
            set
            {
                _mapBk = value;

                var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                eventSubsystem.Raise("mapBkLoad", _mapBk);
            }
            get => _mapBk;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();


            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Subscribe("localPlayerLoad", o => { CheckMap(); });
            eventSubsystem.Subscribe("mapBkLoad", script =>
            {
                _mapBk = script as MapBk;
                CheckMap();
                GenerateInitAreas();
            });
            eventSubsystem.Subscribe("AllMapLoaded", _ =>
            {
                MapLoaded = true;
            });
            eventSubsystem.Subscribe("serverFightSceneChanged", o => { LoadMap(Common.Game.Instance.FightMap, true); });
            Common.Game.Instance.serverFightNewPlayer += () =>
            {
                var bornPosition = GetRandomBornPosition();
                var playerPrefab = GameResMgr.Instance.LoadAssetSync<GameObject>(playerAssetPath);
                GameObject player = Object.Instantiate(playerPrefab, bornPosition, quaternion.identity);
                return player;
            };
        }

        public Vector3 GetRandomBornPosition()
        {
            int chooseX = _mapBk.xGridNum / 2;
            int chooseY = _mapBk.yGridNum / 2;

            for (int i = 0; i < 2; i++)
            {
                int x = Random.Range(0, _mapBk.xGridNum - 1);
                int y = Random.Range(0, _mapBk.yGridNum - 1);

                var area = GetArea(x, y);

                if (null == area || !area.Blocked)
                {
                    chooseX = x;
                    chooseY = y;
                    break;
                }
            }

            Vector3 ret = _mapBk.Offset;

            var cellSize = _mapBk.MyGrid.cellSize;
            ret.x += chooseX * cellSize.x + cellSize.x * 0.5f;
            ret.y += chooseY * cellSize.y + cellSize.y * 0.5f;
            return ret;
        }


        public MapArea GetArea(int x, int y, bool create = false)
        {
            uint areaKey = CreateAreaIndex((uint)x, (uint)y);

            if (!_areas.TryGetValue(areaKey, out var ret))
            {
                ret = new MapArea();
                _areas.Add(areaKey, ret);
            }

            return ret;
        }

        bool IsFighting => Common.Game.Instance.Mode == GameMode.Client || Common.Game.Instance.Mode == GameMode.Host;

        void CheckMap()
        {
            if (IsFighting)
            {
                if (MapBk != null && Common.Game.Instance.MyController != null)
                {
                    var tr = Common.Game.Instance.MyController.transform;
                    MapBk.virtualCamera.Follow = tr;
                    MapBk.virtualCamera.LookAt = tr;
                    MapBk.virtualCamera.gameObject.SetActive(true);


                    StartGame();
                }
            }
        }

        async void StartGame()
        {
            await TimerSubsystem.Delay(1000);
            UIManager.Instance.Hide<LoadingFrame>();

            if (Common.Game.Instance.Mode == GameMode.Host)
            {
                var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
                fightSubsystem.StartFight();
            }
        }

        public MapData New(int bkId)
        {
            MapData mapData = new MapData
            {
                bkId = bkId
            };

            return mapData;
        }

        uint CreateAreaIndex(uint x, uint y)
        {
            x = x << 16;

            uint ret = x | y;

            return ret;
        }

        public (int, int, int) CreateAreaIndex(Vector3 position)
        {
            if (_mapBk == null)
            {
                return (-1, -1, -1);
            }


            Vector3 relative = position - _mapBk.transform.position;

            int x = Mathf.FloorToInt(relative.x / _mapBk.MyGrid.cellSize.x);
            int y = Mathf.FloorToInt(relative.y / _mapBk.MyGrid.cellSize.y);

            if (x < 0)
            {
                return (-1, -1, -1);
                ;
            }

            if (y < 0)
            {
                return (-1, -1, -1);
                ;
            }

            return ((int)CreateAreaIndex((uint)x, (uint)y), x, y);
        }

        void AddAreaMapBlock(uint x, uint y)
        {
            uint areaIndex = CreateAreaIndex(x, y);
            if (!_areas.TryGetValue(areaIndex, out var area))
            {
                area = new MapArea();
                _areas.Add(areaIndex, area);
            }

            area.MapBlocked = true;
        }

        void GenerateInitAreas()
        {
            _areas.Clear();

            if (_mapBk == null)
            {
                return;
            }

            if (_mapBk.blockTilesRoot == null)
                return;

            var tileMaps = _mapBk.blockTilesRoot.GetComponentsInChildren<Tilemap>();

            foreach (var tilemap in tileMaps)
            {
                var bound = tilemap.cellBounds;
                foreach (var pos in bound.allPositionsWithin)
                {
                    var sp = tilemap.GetSprite(pos);

                    if (null != sp)
                    {
                        if (pos.x >= 0
                            && pos.x < _mapBk.xGridNum
                            && pos.y >= 0
                            && pos.y < _mapBk.yGridNum
                           )
                        {
                            AddAreaMapBlock((uint)pos.x, (uint)pos.y);
                        }
                    }
                }
            }
        }


        public void LoadMap(string mapName, bool net, bool inAsset = true)
        {
            MapLoaded = false;
            var path = AssetMapPath + mapName + MapExtension;
            var content = GameResMgr.Instance.LoadAssetSync<TextAsset>(path);
            var mapData = MapData.DeSerialize(content.text);
            CurMapData = mapData;
            mapData.LoadAsync(false, net);
        }
    }
}