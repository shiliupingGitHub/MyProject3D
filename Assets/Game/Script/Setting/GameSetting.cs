using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Setting
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    public static class GameSettingInitializer
    {
        
        static GameSettingInitializer()
        {
            GameSetting.Instance.Init();
        }
    }
#endif
    public class GameSetting : Singleton<GameSetting>
    {
#if !UNITY_EDITOR
 [UnityEngine.RuntimeInitializeOnLoadMethod]
    static void RuntimeLoad()
        {
       GameSetting.Instance.Init();
}
#endif
        const string gameSettingAssetPath = "Assets/Game/Res/Misc/GameSettingConfig.prefab";
        public bool ShowGrid { get; set; }
        public bool ShowBlock { get; set; }
        public bool ShowFps { get; set; }
        public bool ShowPath { get; set; }
        private GameSettingConfig _gameSettingConfig;

        public GameSettingConfig Config
        {
            get
            {
                if (_gameSettingConfig == null)
                {
                    var template = GameResMgr.Instance.LoadAssetSync<GameObject>(gameSettingAssetPath);
                    var go = Object.Instantiate(template);
                    Object.DontDestroyOnLoad(go);
                    _gameSettingConfig = go.GetComponent<GameSettingConfig>();
                }

                return _gameSettingConfig;
            }
        }
        
        public void Init()
        {
            ShowGrid = false;
            ShowBlock = false;
            ShowFps = false;
            ShowPath = false;
        }
    }
}