using System.Collections.Generic;
using System.IO;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.Setting;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";
        [UIPath("ddBk")] private Dropdown _ddBk;

        private MapData _curMapData;
        private const float ZoomFactor = 400;
        private const float OrthographicSizeMin = 0.5f;
        private const float OrthographicSizeMax = 20;
        private const float MoveFactor = 0.01f;
        private bool _bEnabledInput;
        private readonly Dictionary<string, InputActionReference> _inputActionReferences = new();
        private bool _bDrag;
        private bool _bAddTick;
        private Vector3 _lastDragPosition = Vector3.zero;
        private ActorConfig _curSelectActorConfig;
        private GameObject _curSelectShadow;
        private bool _bTicking;
        private readonly List<int> _bkIds = new();
        [UIPath("InputSaveName")] private InputField _inputSaveName;
        [UIPath("Load/ddSaveMaps")] private Dropdown _ddSaveMaps;
        [UIPath("btnNew")] private Button _btnNew;
        [UIPath("Load/btnLoad")] private Button _btnLoad;
        [UIPath("btnSave")] private Button _btnSave;
        [UIPath("ActorTemplate")] private GameObject _actorTemplate;
        [UIPath("svActors/Viewport/Content")] private Transform _contentRoot;
        [UIPath("btnReturnHall")] private Button _btnReturnHall;
        [UIPath("btnEventEdit")] private Button _btnEventEdit;
        [UIPath("btnSetting")] private Button _btnSetting;

        void AddToTick()
        {
            if (!_bAddTick)
            {
                _bTicking = true;
                _bAddTick = true;
                DoTick();
            }
        }

        void RemoveTick()
        {
            if (_bAddTick)
            {
                _bTicking = false;
                _bAddTick = false;
            }
        }

        async void DoTick()
        {
            while (_bTicking)
            {
                Tick();
                await TimerSubsystem.Delay(1);
            }
        }

        void Tick()
        {
            if (_bDrag)
            {
                TickDrag();
            }

            TickShadow();
        }

        void TickShadow()
        {
            if (_curSelectShadow != null)
            {
                if (Camera.main != null)
                {
                    var layer = LayerMask.NameToLayer("LandScape");
                    var ray= Camera.main.ScreenPointToRay(Input.mousePosition);
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

        void TickDrag()
        {
            if (_inputActionReferences.TryGetValue("EditDrag", out var inputDrag))
            {
                if (inputDrag.action.IsPressed())
                {
                    var newPosition = Input.mousePosition;
                    var delta = newPosition - _lastDragPosition;

                    delta.z = delta.y;
                    delta.y = 0;

                    if (Camera.main != null)
                    {
                        var transform = Camera.main.transform;
                        var cameraPosition = transform.position;

                        cameraPosition -= delta * MoveFactor;
                        transform.position = cameraPosition;
                    }

                    _lastDragPosition = newPosition;
                }
            }
        }

        void OnZoom(InputAction.CallbackContext callbackContext)
        {
            var delta = callbackContext.ReadValue<Vector2>();
            if (Camera.main != null)
            {
                var orthographicSize = Camera.main.orthographicSize;

                orthographicSize -= delta.y / ZoomFactor;
                orthographicSize = Mathf.Clamp(orthographicSize, OrthographicSizeMin, OrthographicSizeMax);
                Camera.main.orthographicSize = orthographicSize;
            }
        }

        void InitInput()
        {
            // hookActions.Add("Zoom", OnZoom);
            // hookActions.Add("EditMove", OnMove);
            var hooks = FrameGo.GetComponents<InputActionHook>();
            foreach (var hook in hooks)
            {
                if (hook.inputAction != null)
                {
                    _inputActionReferences.Add(hook.inputAction.action.name, hook.inputAction);
                }
            }

            if (_inputActionReferences.TryGetValue("Zoom", out var inputZoom))
            {
                inputZoom.action.performed += OnZoom;
            }

            if (_inputActionReferences.TryGetValue("EditDrag", out var inputDrag))
            {
                inputDrag.action.started += delegate
                {
                    _bDrag = true;
                    _lastDragPosition = Input.mousePosition;
                };

                inputDrag.action.canceled += delegate { _bDrag = false; };
            }

            if (_inputActionReferences.TryGetValue("EditAddActor", out var inputAddActor))
            {
                inputAddActor.action.started += delegate
                {
                    if (null != _curSelectShadow && null != _curSelectActorConfig && null != _curMapData)
                    {
                        _curMapData.AddActor(_curSelectShadow.transform.position, _curSelectActorConfig);
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            if (null != _curMapData && null != Camera.main)
                            {
                                var layer = LayerMask.NameToLayer("LandScape");
                                var ray= Camera.main.ScreenPointToRay(Input.mousePosition);
                                if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, 1 << layer))
                                {
                                    _curMapData.RemoveActor(hitInfo.point);
                                }
                               
                            }
                        }
                        else if (Input.GetKey(KeyCode.LeftAlt))
                        {
                            if (null != _curMapData && null != Camera.main)
                            {
                                var layer = LayerMask.NameToLayer("LandScape");
                                var ray= Camera.main.ScreenPointToRay(Input.mousePosition);
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
                    }
                };
            }

            if (_inputActionReferences.TryGetValue("CancelEditActor", out var inputCancelEditActor))
            {
                inputCancelEditActor.action.started += delegate { SetSelectActor(null); };
            }
        }

        void EnableInput()
        {
            if (!_bEnabledInput)
            {
                foreach (var input in _inputActionReferences)
                {
                    input.Value.action.Enable();
                }

                _bEnabledInput = true;
            }
        }

        void DisableInput()
        {
            if (_bEnabledInput)
            {
                foreach (var input in _inputActionReferences)
                {
                    input.Value.action.Disable();
                }

                _bEnabledInput = false;
            }
        }

        protected override void OnDestroy()
        {
            DisableInput();
            base.OnDestroy();
            RemoveTick();
        }

        void InitActors()
        {
            var actorConfigs = ActorConfig.dic;
            foreach (var actorConfig in actorConfigs)
            {
                var actorGo = Object.Instantiate(_actorTemplate, _contentRoot);
                actorGo.transform.localScale = Vector3.one;
                actorGo.name = actorConfig.Value.id.ToString();
                var text = actorGo.transform.Find("Name").GetComponent<Text>();
                text.text = actorConfig.Value.name;
                actorGo.SetActive(true);
                var btn = actorGo.GetComponent<Button>();

                if (btn != null)
                {
                    btn.onClick.AddListener(() => { SetSelectActor(actorConfig.Value); });
                }
            }
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

        void SetCameraCenter(MapBk mapBk)
        {
            Vector3 center = mapBk.transform.position;
            center += new Vector3(mapBk.xGridSize * mapBk.xGridNum * 0.5f,0, 0);

            if (Camera.main != null)
            {
                var transform = Camera.main.transform;
                Vector3 cameraPosition = transform.position;
                cameraPosition.x = center.x;
                cameraPosition.z = center.z;
                transform.position = cameraPosition;
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
            _btnNew.onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    _curMapData.UnLoadSync();
                }

                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                _curMapData = mapSubsystem.New(_bkIds[_ddBk.value]);
                _curMapData.LoadSync();
                EnableInput();
                MapBk mapBk = Object.FindObjectOfType<MapBk>();
                if (mapBk != null)
                {
                    SetCameraCenter(mapBk);
                    GameSetting.Instance.ShowGrid = true;
                }
            });

            _btnEventEdit.onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    var frame = UIManager.Instance.Show<MapEventEditFrame>();
                    frame.SetData(_curMapData);
                }
            });

            _btnLoad.onClick.AddListener(() =>
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
                            EnableInput();
                            SetCameraCenter(mapBk);
                            GameSetting.Instance.ShowGrid = true;
                            _inputSaveName.text = fileName;
                        }
                    }
                }
            });

            _btnSave.GetComponent<Button>().onClick.AddListener(() =>
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
            });

            InitBks();
            InitSavedMaps();
            InitActors();
            InitInput();
            AddToTick();
        }
    }
}