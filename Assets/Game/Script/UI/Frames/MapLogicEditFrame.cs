using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Logic;
using Game.Script.Subsystem;
using Game.Script.UI.Ext;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class MapLogicEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/MapLogicEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/svBaseParams/Viewport/Content/params")] private Transform _paramsRoot;
        [UIPath("offset/lbName")] private Text _lbName;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
        }

        public void Edit(mapLogicData data)
        {
            var mapLogicSubsystem = Common.Game.Instance.GetSubsystem<MapLogicSubsystem>();
            var logic = mapLogicSubsystem.Create(data.Name);

            if (null != logic)
            {
                JsonUtility.FromJsonOverwrite(data.Data, logic);
                FieldDrawer.BeginDraw(_paramsRoot);
                FieldDrawer.Draw(_paramsRoot, logic, (_, _) =>
                {
                    data.Data = JsonUtility.ToJson(logic);
                });
            }
        }
    }
}