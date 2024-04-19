using System;
using System.Collections.Generic;
using Game.Script.Common;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Game.Script.Character.Skill
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class SkillInitializer
    {
        static SkillInitializer()
        {
            SkillMgr.Instance.Init();
        }
    }
#endif
    public class SkillMgr : Singleton<SkillMgr>
    {
        public Dictionary<string, SkillAction> DefaultActions { get; } = new();
        public Dictionary<string, System.Type> ActionTypesDic { get; } = new();
        public List<string> ActionTypesList { get; set; } = new();

#if UNITY_EDITOR
 [RuntimeInitializeOnLoadMethod]
    static void RuntimeLoad()
        {
        SkillMgr.Instance.Init();
}
#endif


        public void Init()
        {
            DefaultActions.Clear();
            ActionTypesDic.Clear();
            var baseType = typeof(SkillAction);
            var types = baseType.Assembly.GetTypes();

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attrs = type.GetCustomAttributes(typeof(SkillDesAttribute), false);

                    foreach (var attr in attrs)
                    {
                        if (attr is SkillDesAttribute skillDesAttribute)
                        {
                            ActionTypesDic.Add(skillDesAttribute.SkillType, type);
                            DefaultActions.Add(skillDesAttribute.SkillType, System.Activator.CreateInstance(type) as SkillAction);
                            ActionTypesList.Add(skillDesAttribute.SkillType);
                        }
                    }
                }
            }
        }
    }
}