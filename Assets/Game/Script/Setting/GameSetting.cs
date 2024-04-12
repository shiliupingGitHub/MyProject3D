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
        const string GameSettingAssetPath = "Assets/Game/Res/Misc/GameSettingConfig.prefab";
        public bool ShowGrid { get; set; }
        public bool ShowBlock { get; set; }
        public bool ShowFps { get; set; }
        public bool ShowPath { get; set; }

        public float EditMoveFactor
        {
            get => PlayerPrefs.GetFloat("EditMoveFactor", 1f);
            set => PlayerPrefs.SetFloat("EditMoveFactor", value);
        }
        
        public float EditZoomFactor
        {
            get => PlayerPrefs.GetFloat("EditZoomFactor", 1f);
            set => PlayerPrefs.SetFloat("EditZoomFactor", value);
        }
        
        private GameSettingConfig _gameSettingConfig;

        public GameSettingConfig Config
        {
            get
            {
                if (_gameSettingConfig == null)
                {
                    var template = GameResMgr.Instance.LoadAssetSync<GameObject>(GameSettingAssetPath);
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