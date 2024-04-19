using Cysharp.Threading.Tasks;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    [CustomLevel(LevelType.Edit)]
    public class EditLevel : Level
    {
        private const string SceneName = "Edit";
        public override void Enter()
        {
            base.Enter();
            Common.Game.Instance.Mode = GameMode.Edit;

            _ = Start();

        }
        
        async UniTaskVoid Start()
        {
            
            await SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            UIManager.Instance.Show<EditFrame>();
            _ = LoadComplete();
        }

        async UniTaskVoid LoadComplete()
        {
            await TimerSubsystem.Delay(1000);
            UIManager.Instance.Hide<LoadingFrame>();
        }
    }
}