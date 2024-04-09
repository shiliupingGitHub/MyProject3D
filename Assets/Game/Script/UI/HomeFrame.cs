using Game.Script.Attribute;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI
{
    public class HomeFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/HomeFrame.prefab";
        [UIPath("offset/btnReturnHall")]
        private Button _btnReturnHall;

        public override void Init(Transform parent)
        {
            base.Init(parent);
            
            _btnReturnHall.onClick.AddListener(() =>
            {
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                levelSubsystem.Enter(LevelType.Hall);
            });
        }
    }
}