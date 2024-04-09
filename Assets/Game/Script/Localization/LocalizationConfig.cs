using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.UI.Extern
{
    public class LocalizationConfig : MonoBehaviour
    {
        public List<SystemLanguage> _keys = new();
        public List<TextAsset> _values = new();
    }
}