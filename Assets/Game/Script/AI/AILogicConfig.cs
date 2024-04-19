using System.Collections.Generic;
using System.Reflection;
using Game.Editor;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Script.AI.Logic
{
    public class AILogicConfig : MonoBehaviour
    {
        [ValueDropdown("GetLogicNames")] public List<string> logics = new();
#if UNITY_EDITOR
        private List<string> GetLogicNames()
        {
            var list = new List<string>();
            var baseType = typeof(AILogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attribute = type.GetCustomAttribute<AILogicDesAttribute>();
                    if (null != attribute)
                    {
                        list.Add(attribute.Des);
                    }
                }
            }

            return list;
        }
#endif
    }

}