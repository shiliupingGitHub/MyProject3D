using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    [CustomLevel(LevelType.Home)]
    public class HomeLevel : Level
    {
        private const string SceneName = "Home";
        public override void Enter()
        {
            base.Enter();
            SceneManager.sceneUnloaded += OnLoadedScene;
            SceneManager.LoadScene(SceneName);
            Common.Game.Instance.Mode = GameMode.Home;
            UIManager.Instance.Show<HomeFrame>();
            
        }

        void OnLoadedScene(Scene scene)
        {
            SceneManager.sceneUnloaded -= OnLoadedScene;
            LoadComplete();
            
        }
        async void LoadComplete()
        {
            await TimerSubsystem.Delay(1000);
            UIManager.Instance.Hide<LoadingFrame>();
            var homeSubsystem = Common.Game.Instance.GetSubsystem<HomeSubsystem>();
            homeSubsystem.ShowHome();
        }
    }
}