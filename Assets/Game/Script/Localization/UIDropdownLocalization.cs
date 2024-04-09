using System.Collections.Generic;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Extern
{
    public class UIDropdownLocalization : MonoBehaviour
    {
        private Dropdown _dropdown;
        public List<string> Keys;
        private void Awake()
        {
            _dropdown = GetComponent<Dropdown>();
            Localize();
            var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
            localizationSubsystem.onLanguageChanged += Localize;
        }

        public void Localize()
        {
            if (null != _dropdown)
            {
                _dropdown.ClearOptions();
                List<string> values = new();
                var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
                foreach (var key in Keys)
                {
                    values.Add(localizationSubsystem.Get(key));
                }
                _dropdown.AddOptions(values);
            }
        }

        private void OnDestroy()
        {
            var localizationSubsystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();
            localizationSubsystem.onLanguageChanged += Localize;
        }
    }
}