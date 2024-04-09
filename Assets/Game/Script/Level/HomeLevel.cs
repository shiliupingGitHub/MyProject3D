using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using Mirror;
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
            var networkSubsystem = Common.Game.Instance.GetSubsystem<NetworkSubsystem>();
            networkSubsystem.LoadNetWorkManager(GameMode.Home);
            Common.Game.Instance.Mode = GameMode.Home;
            Common.Game.Instance.LoadMapName = "map_test_1";
            NetworkManager.singleton.StartHost();
            UIManager.Instance.Show<HomeFrame>();
            
        }
        
    }
}