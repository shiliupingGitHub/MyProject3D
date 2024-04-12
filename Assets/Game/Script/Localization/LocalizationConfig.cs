using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Script.UI.Extern
{
    public class LocalizationConfig : SerializedMonoBehaviour
    {
        [SerializeField]
        public Dictionary<SystemLanguage, TextAsset> tables = new();
    }
    
}