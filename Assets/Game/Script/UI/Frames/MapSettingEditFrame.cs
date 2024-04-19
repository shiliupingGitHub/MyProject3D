using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Subsystem;
using Game.Script.UI.Ext;
using OneP.InfinityScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class MapSettingEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/MapSettingEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/svBaseParams/Viewport/Content/params")] private Transform _paramsRoot;
        [UIPath("offset/svLogics")] private InfinityScrollView _logicRoot;
        [UIPath("offset/ddLogics")] private Dropdown _ddLogics;
        [UIPath("offset/btnAddLogic")] private Button _btnAddLogic;
        private MapData _curMapData;
        List<string> _options = new ();
        
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
            var localizeSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
            _logicRoot.onItemReload += (o, i) =>
            {
                var logicData = _curMapData.logics[i];
                var name = logicData.Name;
                var lbName = o.transform.Find("lbName").GetComponent<Text>();
                var btnRemove = o.transform.Find("btnRemove").GetComponent<Button>();
                var btnEdit = o.transform.Find("btnEdit").GetComponent<Button>();
                btnEdit.onClick.RemoveAllListeners();
                lbName.text = localizeSubsystem.Get(name);
                btnRemove.onClick.RemoveAllListeners();
                btnRemove.onClick.AddListener(() =>
                {
                    if (_curMapData.ContainLogic(name))
                    {
                        _curMapData.RemoveLogic(name);
                        RefreshDdLogic();
                        _logicRoot.Setup(_curMapData.logics.Count);
                    }
                });
                btnEdit.onClick.AddListener(() =>
                {
                    var frame = UIManager.Instance.Show<MapLogicEditFrame>();
                    frame.Edit(logicData);
                });
            };
            _btnAddLogic.onClick.AddListener(() =>
            {
                if (_options.Count > _ddLogics.value)
                {
                    var logicName = _options[_ddLogics.value];
                    if (!_curMapData.ContainLogic(logicName))
                    {
                        _curMapData.AddLogic(logicName, null);
                        RefreshDdLogic();
                        _logicRoot.Setup(_curMapData.logics.Count);
                    }
                    
                }
            });
        }

        void RefreshDdLogic()
        {
            var logicSubsystem = Common.Game.Instance.GetSubsystem<MapLogicSubsystem>();
            _ddLogics.ClearOptions();
            _options.Clear();
            foreach (var logicName in logicSubsystem.AllLogicNames)
            {
                if (!_curMapData.ContainLogic(logicName))
                {
                    _options.Add(logicName);
                }
            }

            if (_options.Count > 0)
            {
                _ddLogics.gameObject.SetActive(true);
                _ddLogics.AddOptions(_options);
                _btnAddLogic.gameObject.SetActive(true);
            }
            else
            {
                _ddLogics.gameObject.SetActive(false);
                _btnAddLogic.gameObject.SetActive(false);
            }
        }
        public MapData CurMapData
        {
            set
            {
                _curMapData = value;
                RefreshActorUI();
                RefreshDdLogic();
            }
           
        }

        void RefreshActorUI()
        {
            FieldDrawer.BeginDraw(_paramsRoot);
            FieldDrawer.Draw(_paramsRoot, _curMapData.BaseSetting, (_, _) =>
            {
               
            });
            
            _logicRoot.Setup(_curMapData.logics.Count);
        }
    }
}