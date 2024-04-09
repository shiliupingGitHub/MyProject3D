using System.Collections.Generic;
using Game.Script.Res;
using Game.Script.UI.Extern;
using UnityEngine;

namespace Game.Script.Subsystem
{
    
    public sealed class LocalizationSubsystem : GameSubsystem
    {
        private Dictionary<SystemLanguage, LanguageData> _languageDatas = new();

        public System.Action onLanguageChanged;
        private SystemLanguage _curLanguage = SystemLanguage.English;
        public override void OnInitialize()
        {
            var languageGo =GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Localization/LocalizationConfig.prefab");
            var localizationConfig = languageGo.GetComponent<LocalizationConfig>();

            for (int i = 0; i < localizationConfig._keys.Count; i++)
            {
                var key = localizationConfig._keys[i];
                var value = localizationConfig._values[i];
                var content = System.Text.Encoding.GetEncoding("GBK").GetString(value.bytes);
                LanguageData data = new LanguageData();
                data.Load(content);
                _languageDatas.Add(key, data);
            }
        }

        public void SetLanguage(SystemLanguage language)
        {
            if (_curLanguage != language)
            {
                _curLanguage = language;
                if (null != onLanguageChanged)
                {
                    onLanguageChanged.Invoke();
                }
            }
        }
        

        public string Get(string key)
        {
            if (_languageDatas.TryGetValue(_curLanguage, out var data))
            {
                if (data.dic.TryGetValue(key, out var ret))
                {
                    return ret.data;
                }
            }
            return key;
        }
    }
}