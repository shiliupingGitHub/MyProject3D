using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    [CustomLevel(LevelType.Hall)]
    public class HallLevel : Level
    {
        private const string SceneName = "Hall";
        public override void Enter()
        {
            base.Enter();
            Common.Game.Instance.Mode = GameMode.Hall;
            SceneManager.LoadScene(SceneName);
            UIManager.Instance.Show<HallFrame>();
            LoadComplete();
        }
        async void LoadComplete()
        {
            await TimerSubsystem.Delay(1000);
            UIManager.Instance.Hide<LoadingFrame>();
        }
    }
}