using System.Collections.Generic;
using System.IO;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.Setting;
using Game.Script.Subsystem;
using OneP.InfinityScrollView;
using Rewired;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";
        private MapData _curMapData;
        private ActorConfig _curSelectActorConfig;
        private GameObject _curSelectShadow;
        private bool _bTicking;
        private readonly List<int> _bkIds = new();
        [UIPath("ddBk")] private Dropdown _ddBk;
        [UIPath("InputSaveName")] private InputField _inputSaveName;
        [UIPath("ddSaveMaps")] private Dropdown _ddSaveMaps;
        [UIPath("btnNew")] private Button _btnNew;
        [UIPath("btnLoad")] private Button _btnLoad;
        [UIPath("btnSave")] private Button _btnSave;
        [UIPath("ActorTemplate")] private GameObject _actorTemplate;
        [UIPath("svActors")] private InfinityScrollView _scrollViewActors;
        [UIPath("btnReturnHall")] private Button _btnReturnHall;
        [UIPath("btnEventEdit")] private Button _btnEventEdit;
        [UIPath("btnSetting")] private Button _btnSetting;
        [UIPath("sdZoomFactor")] private Slider _sdZoomFactor;
        [UIPath("sdMoveFactor")] private Slider _sdMoveFactor;
        [UIPath("Op/TogglePut")] private Toggle _togglePut;
        [UIPath("Op/ToggleErase")] private Toggle _toggleErase;
        [UIPath("Op/ToggleEdit")] private Toggle _toggleEdit;

        private string SavePath
        {
            get
            {
                string path = Path.Combine(Application.persistentDataPath, "Map");
                if (Application.isEditor)
                {
                    path = Path.Combine(Application.dataPath, "Game/Res/Map/Data");
                }

                return path;
            }
        }

        private string MapExtension => ".txt";

        void OnUpdate(float delta)
        {
            TickDrag();
            TickShadow();
            TickZoom();
        }

        void TickShadow()
        {
            if (_curSelectShadow != null)
            {
                if (Camera.main != null)
                {
                    var layer = LayerMask.NameToLayer("LandScape");
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, 1 << layer))
                    {
                        MapBk mapBk = Object.FindObjectOfType<MapBk>();

                        if (null != mapBk)
                        {
                            _curSelectShadow.transform.position = mapBk.ConvertToGridPosition(hitInfo.point);
                        }
                    }
                }
            }
        }


        protected override void OnDestroy()
        {
            //DisableInput();
            base.OnDestroy();
            GameLoop.Remove(OnUpdate);
            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            gameInputSubsystem.UnRegisterButtonDown(OnEditOperate, "EditOperate");
            gameInputSubsystem.UnRegisterButtonDown(OnEditCancel, "EditCancel");
        }

        void InitActors()
        {
            List<ActorConfig> actorConfigs = new();
            foreach (var actorConfig in ActorConfig.dic)
            {
                actorConfigs.Add(actorConfig.Value);
            }

            _scrollViewActors.onItemReload += (go, i) =>
            {
                var btn = go.transform.Find("btn").GetComponent<Button>();
                var text = go.transform.Find("btn/txt").GetComponent<Text>();
                var actorConfig = actorConfigs[i];
                text.text = actorConfig.name;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    if (!_togglePut.isOn) return;
                    SetSelectActor(actorConfig);
                });
            };
            _scrollViewActors.Setup(actorConfigs.Count);
        }

        void SetSelectActor(ActorConfig actorConfig)
        {
            if (null != _curSelectShadow)
            {
                Object.Destroy(_curSelectShadow);
                _curSelectShadow = null;
                _curSelectActorConfig = null;
            }

            if (actorConfig != null)
            {
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(actorConfig.path);

                if (template)
                {
                    _curSelectShadow = Object.Instantiate(template);
                    _curSelectActorConfig = actorConfig;
                    _curSelectShadow.tag = "Shadow";

                    var actor = _curSelectShadow.GetComponent<Actor>();

                    if (null != actor)
                    {
                        actor.ActorType = ActorType.Shadow;
                    }
                }
            }
        }


        void InitBks()
        {
            var mapConfigs = MapBKConfig.dic;
            _bkIds.Clear();
            List<string> mapDdContent = new();
            _ddBk.ClearOptions();
            for (int i = 1; i <= mapConfigs.Count; i++)
            {
                var mapConfig = mapConfigs[i];

                mapDdContent.Add(mapConfig.name);
                _bkIds.Add(mapConfig.id);
            }

            _ddBk.AddOptions(mapDdContent);
        }

        void InitSavedMaps()
        {
            RefreshSaveMaps();
        }

        void RefreshSaveMaps()
        {
            _ddSaveMaps.ClearOptions();
            List<string> allMaps = new();
            var path = SavePath;
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (file.EndsWith(MapExtension))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    allMaps.Add(fileName);
                }
            }

            _ddSaveMaps.AddOptions(allMaps);
        }

        void SetCameraCenter(MapBk mapBk)
        {
            Vector3 center = mapBk.transform.position;
            center += new Vector3(mapBk.xGridSize * mapBk.xGridNum * 0.5f, 0, 0);

            if (Camera.main != null)
            {
                var transform = Camera.main.transform;
                Vector3 cameraPosition = transform.position;
                cameraPosition.x = center.x;
                cameraPosition.z = center.z;
                transform.position = cameraPosition;
            }
        }

        void InitFactor()
        {
            _sdMoveFactor.value = GameSetting.Instance.EditMoveFactor;
            _sdZoomFactor.value = GameSetting.Instance.EditZoomFactor;
            _sdMoveFactor.onValueChanged.AddListener(value => { GameSetting.Instance.EditMoveFactor = value; });
            _sdZoomFactor.onValueChanged.AddListener(value => { GameSetting.Instance.EditZoomFactor = value; });
        }

        void NewMap()
        {
            if (_curMapData != null)
            {
                _curMapData.UnLoadSync();
            }

            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            _curMapData = mapSubsystem.New(_bkIds[_ddBk.value]);
            _curMapData.LoadSync();
            //EnableInput();
            MapBk mapBk = Object.FindObjectOfType<MapBk>();
            if (mapBk != null)
            {
                SetCameraCenter(mapBk);
                GameSetting.Instance.ShowGrid = true;
            }
        }

        void LoadMap()
        {
            if (_curMapData != null)
            {
                _curMapData.UnLoadSync();
                _curMapData = null;
            }

            var fileName = _ddSaveMaps.captionText.text;

            var path = Path.Combine(SavePath, fileName + MapExtension);

            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);
                var mapData = MapData.DeSerialize(data);

                if (MapBKConfig.dic.ContainsKey(mapData.bkId))
                {
                    _curMapData = mapData;
                    _curMapData.LoadSync();
                    MapBk mapBk = Object.FindObjectOfType<MapBk>();

                    if (mapBk != null)
                    {
                        //EnableInput();
                        SetCameraCenter(mapBk);
                        GameSetting.Instance.ShowGrid = true;
                        _inputSaveName.text = fileName;
                    }
                }
            }
        }

        void SaveMap()
        {
            if (_curMapData != null && null != _inputSaveName)
            {
                if (!string.IsNullOrEmpty(_inputSaveName.text))
                {
                    var data = _curMapData.Serialize();
                    string path = SavePath;

                    path = Path.Combine(path, _inputSaveName.text + MapExtension);

                    File.WriteAllText(path, data);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    RefreshSaveMaps();
                }
            }
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnReturnHall.onClick.AddListener(() =>
            {
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                levelSubsystem.Enter(LevelType.Hall);
            });
            _btnSetting.onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    var frame = UIManager.Instance.Show<MapSettingEditFrame>();
                    frame.CurMapData = _curMapData;
                }
            });
            _btnNew.onClick.AddListener(NewMap);

            _btnEventEdit.onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    var frame = UIManager.Instance.Show<MapEventEditFrame>();
                    frame.SetData(_curMapData);
                }
            });

            _btnLoad.onClick.AddListener(LoadMap);
            _btnSave.onClick.AddListener(SaveMap);

            InitBks();
            InitSavedMaps();
            InitActors();
            GameLoop.Add(OnUpdate);
            InitFactor();
            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            gameInputSubsystem.RegisterButtonDown(OnEditOperate, "EditOperate");
            gameInputSubsystem.RegisterButtonDown(OnEditCancel, "EditCancel");
        }
        
        

        void TickDrag()
        {
            if (_curSelectShadow)
                return;
            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            float x = gameInputSubsystem.TouchDelta.x;
            float z = gameInputSubsystem.TouchDelta.y;
            if (Camera.main != null)
            {
                if (!UIManager.Instance.UIEventSystem.IsPointerOverGameObject())
                {
                    if (gameInputSubsystem.GetMouseButton(0))
                    {
                        var transform = Camera.main.transform;
                        var cameraPosition = transform.position;
                        cameraPosition.x -= x * 0.01f;
                        cameraPosition.z -= z * 0.01f;
                        transform.position = cameraPosition;
                    }
                }
            }
        }

        void TickZoom()
        {
            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            var delta = gameInputSubsystem.WheelFactor;
            if (Camera.main != null && delta != 0 && _curMapData != null)
            {
                var transform = Camera.main.transform;
                transform.position += transform.forward * delta * GameSetting.Instance.EditZoomFactor;
            }
        }
        
        void RemoveActor()
        {
            if (null != _curMapData && null != Camera.main)
            {
                var layer = LayerMask.NameToLayer("LandScape");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, 1 << layer))
                {
                    _curMapData.RemoveActor(hitInfo.point);
                }
            }
        }

        void EditActor()
        {
            if (null != _curMapData && null != Camera.main)
            {
                var layer = LayerMask.NameToLayer("LandScape");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, 1 << layer))
                {
                    var actorData = _curMapData.GetActorData(hitInfo.point);
                    if (null != actorData)
                    {
                        var frame = UIManager.Instance.Show<ActorDataEditFrame>();
                        frame.CurActorData = actorData;
                    }
                }
            }
        }

        void OnEditCancel(InputActionEventData _)
        {
            if (UIManager.Instance.UIEventSystem.IsPointerOverGameObject())
            {
                return;
            }
            
            SetSelectActor(null);
            
        }

        void OnEditOperate(InputActionEventData _)
        {
            if (UIManager.Instance.UIEventSystem.IsPointerOverGameObject())
                return;
            
            if (!_togglePut.isOn)
            {
                SetSelectActor(null);
            }
            if (null != _curSelectShadow && null != _curSelectActorConfig && null != _curMapData)
            {
                if (_togglePut.isOn)
                {
                    _curMapData.AddActor(_curSelectShadow.transform.position, _curSelectActorConfig);
                }
            }

            if (_toggleEdit.isOn)
            {
                EditActor();
            }

            if (_toggleErase.isOn)
            {
                RemoveActor();
            }
        }
    }
}