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
        public Dictionary<uint, MapArea> Areas => _areas;
        public bool MapLoaded { get; private set; }
        private GameObject _root;

        public GameObject Root
        {
            get
            {
                if (null == _root)
                {
                    _root = new GameObject("MapRoot");
                }

                return _root;
            }
        }

        public void ClearGo()
        {
            if (null != _root)
            {
                Object.Destroy(_root);
                _root = null;
            }
        }

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
            eventSubsystem.Subscribe("serverSceneChanged", o => { LoadMap(Common.Game.Instance.LoadMapName, true); });
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
            int chooseY = _mapBk.zGridNum / 2;

            for (int i = 0; i < 2; i++)
            {
                int x = Random.Range(0, _mapBk.xGridNum - 1);
                int z = Random.Range(0, _mapBk.zGridNum - 1);

                var area = GetArea(x, z);

                if (null == area || !area.Blocked)
                {
                    chooseX = x;
                    chooseY = z;
                    break;
                }
            }

            Vector3 ret = _mapBk.Offset;
            
            ret.x += chooseX * _mapBk.xGridSize + _mapBk.xGridSize * 0.5f;
            ret.z += chooseY * _mapBk.zGridSize + _mapBk.zGridSize * 0.5f;
            return ret;
        }


        public MapArea GetArea(int x, int y, bool create = false)
        {
            MapArea  ret = null;
            uint areaKey = CreateAreaIndex((uint)x, (uint)y);
            lock (this)
            {
                if (!_areas.TryGetValue(areaKey, out  ret))
                {
                    ret = new MapArea();
                    _areas.Add(areaKey, ret);
                }
            }
            
            return ret;
        }

        bool IsCanStart => Common.Game.Instance.Mode == GameMode.Client || Common.Game.Instance.Mode == GameMode.Host || Common.Game.Instance.Mode == GameMode.Home;

        void CheckMap()
        {
            if (IsCanStart)
            {
                if (MapBk != null && Common.Game.Instance.MyController != null)
                {
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

        uint CreateAreaIndex(uint x, uint z)
        {
            x = x << 16;

            uint ret = x | z;

            return ret;
        }

       public (int , int ) AreaIndexToGrid(uint areaIndex)
        {
            uint x = areaIndex >> 16;
            uint z = areaIndex & 0xffff;
            return ((int)x, (int)z);
        }

        public (int, int, int) CreateAreaIndex(Vector3 position)
        {
            if (_mapBk == null)
            {
                return (-1, -1, -1);
            }


            Vector3 relative = position - _mapBk.transform.position;

            int x = Mathf.FloorToInt(relative.x / _mapBk.xGridSize);
            int z = Mathf.FloorToInt(relative.z / _mapBk.zGridSize);

            if (x < 0)
            {
                return (-1, -1, -1);
            }

            if (z < 0)
            {
                return (-1, -1, -1);
            }

            return ((int)CreateAreaIndex((uint)x, (uint)z), x, z);
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

            foreach (var block in _mapBk.blocks)
            {
                var (x, z) = _mapBk.BlockToGrid(block);
                AddAreaMapBlock((uint)x, (uint)z);
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