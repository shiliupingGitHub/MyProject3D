using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Actor;
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
        List<string> _options = new List<string>();
        
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
            _logicRoot.onItemReload += (o, i) =>
            {

            };
            _btnAddLogic.onClick.AddListener(() =>
            {
                if (_options.Count > _ddLogics.value)
                {
                    var logicName = _options[_ddLogics.value];
                    if (!_curMapData.logics.Contains(logicName))
                    {
                        _curMapData.logics.Add(logicName);
                        RefreshDDLogic();
                        _logicRoot.Setup(_curMapData.logics.Count);
                    }
                    
                }
            });
        }

        void RefreshDDLogic()
        {
            var logicSubsystem = Common.Game.Instance.GetSubsystem<MapLogicSubsystem>();
            _ddLogics.ClearOptions();
            foreach (var logicName in logicSubsystem.AllLogicNames)
            {
                if (!_curMapData.logics.Contains(logicName))
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
                RefreshDDLogic();
            }
           
        }

        void RefreshActorUI()
        {
            FieldDrawer.BeginDraw(_paramsRoot);
            FieldDrawer.Draw(_paramsRoot, _curMapData.BaseSetting, (info, o) =>
            {
               
            });
            
            _logicRoot.Setup(_curMapData.logics.Count);
        }
    }
}