using System;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Extern
{
    public class UILocalization : MonoBehaviour
    {

        public string Key;
        private Text _text;
        private void Awake()
        {
            var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
            _text = GetComponent<Text>();
            Localize();
            localizationSubsystem.onLanguageChanged += Localize;
        }

        void Localize()
        {
            if (null != _text)
            {
                var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
                _text.text = localizationSubsystem.Get(Key);
            }
        }

        private void OnDestroy()
        {
            var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
            localizationSubsystem.onLanguageChanged += Localize;
        }
    }
}