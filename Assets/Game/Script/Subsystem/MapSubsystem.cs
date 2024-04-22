using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.Setting;
using Game.Script.UI;
using Game.Script.UI.Frames;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Script.Subsystem
{
    public class MapSubsystem : GameSubsystem
    {
        const string AssetMapPath = "Assets/Game/Res/Map/Data/";
        const string PlayerAssetPath = "Assets/Game/Res/Misc/PlayerPrefab.prefab";

        string MapExtension => ".txt";
        private MapBk _mapBk;
        public MapData CurMapData { get; private set; }

        private readonly Dictionary<uint, MapGrid> _grids = new();
        public Dictionary<uint, MapGrid> Grids => _grids;
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
                GenerateInitGrids();
            });
            eventSubsystem.Subscribe("AllMapLoaded", _ =>
            {
                MapLoaded = true;
                GameSetting.Instance.ShowGrid = Common.Game.Instance.Mode == GameMode.Edit;
            });
            eventSubsystem.Subscribe("serverSceneChanged", o => { LoadMap(Common.Game.Instance.LoadMapName, true); });
            Common.Game.Instance.serverFightNewPlayer += () =>
            {
                var bornPosition = GetRandomBornPosition();
                var playerPrefab = GameResMgr.Instance.LoadAssetSync<GameObject>(PlayerAssetPath);
                GameObject player = Object.Instantiate(playerPrefab, bornPosition, quaternion.identity);
                return player;
            };
        }

        private Vector3 GetRandomBornPosition()
        {
            int chooseX = _mapBk.xGridNum / 2;
            int chooseY = _mapBk.zGridNum / 2;

            for (int i = 0; i < 2; i++)
            {
                int x = Random.Range(0, _mapBk.xGridNum - 1);
                int z = Random.Range(0, _mapBk.zGridNum - 1);

                var grid = GetGrid(x, z);

                if (null == grid || !grid.Blocked)
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


        public MapGrid GetGrid(int x, int y, bool create = false)
        {
            MapGrid  ret;
            uint gridKey = GameUtil.CombineToIndex((uint)x, (uint)y);
            lock (this)
            {
                if (!_grids.TryGetValue(gridKey, out  ret))
                {
                    ret = new MapGrid();
                    _grids.Add(gridKey, ret);
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


        
        public (int, int, int) CreateGridIndex(Vector3 position)
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

            return ((int)GameUtil.CombineToIndex((uint)x, (uint)z), x, z);
        }

        void AddGridBlock(uint x, uint y)
        {
            uint gridIndex = GameUtil.CombineToIndex(x, y);
            if (!_grids.TryGetValue(gridIndex, out var grid))
            {
                grid = new MapGrid();
                _grids.Add(gridIndex, grid);
            }

            grid.MapBlocked = true;
        }

        void GenerateInitGrids()
        {
            _grids.Clear();

            if (_mapBk == null)
            {
                return;
            }

            foreach (var block in _mapBk.blocks)
            {
                var (x, z) = _mapBk.BlockToGrid(block);
                AddGridBlock((uint)x, (uint)z);
            }
            
        }


        private void LoadMap(string mapName, bool net, bool inAsset = true)
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