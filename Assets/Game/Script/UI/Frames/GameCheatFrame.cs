using Game.Script.Attribute;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class GameCheatFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/GameCheatFrame.prefab";
        [UIPath("inputCheat")] private InputField _inputCheat;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _inputCheat.onSubmit.AddListener(str =>
            {
                var cheatSubsystem = Common.Game.Instance.GetSubsystem<GameCheatSubsystem>();
                cheatSubsystem.Execute(str);
                UIManager.Instance.UIEventSystem.SetSelectedGameObject(null);
                Hide();
            });
        }

        protected override void OnShow()
        {
            base.OnShow();
            UIManager.Instance.UIEventSystem.SetSelectedGameObject(_inputCheat.gameObject);
        }
        
    }
}