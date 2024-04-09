using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Actor;
using Game.Script.UI.Ext;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class MapSettingEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/MapSettingEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/Scroll View/Viewport/Content/params")] private Transform _paramsRoot;
        private MapData _curMapData;
        
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
        }
        public MapData CurMapData
        {
            set
            {
                _curMapData = value;
                RefreshActorUI();
            }
           
        }

        void RefreshActorUI()
        {
            FieldDrawer.BeginDraw(_paramsRoot);
            FieldDrawer.Draw(_paramsRoot, _curMapData.BaseSetting, (info, o) =>
            {
               
            });
            
        }
    }
}