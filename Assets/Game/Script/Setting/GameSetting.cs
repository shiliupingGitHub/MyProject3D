using Game.Script.Common;

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
        public bool ShowGrid { get; set; }
        public bool ShowBlock { get; set; }
        public bool ShowFps { get; set; }
        public void Init()
        {
            ShowGrid = false;
            ShowBlock = false;
            ShowFps = false;
        }
    }
}