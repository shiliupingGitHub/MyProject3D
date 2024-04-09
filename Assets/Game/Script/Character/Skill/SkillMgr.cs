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
        public Dictionary<SkillType, SkillAction> DefaultActions { get; } = new();
        public Dictionary<SkillType, System.Type> ActionTypes { get; } = new();
        public Dictionary<SkillType, string> Descriptions = new();

#if UNITY_EDITOR
 [RuntimeInitializeOnLoadMethod]
    static void RuntimeLoad()
        {
        SkillMgr.Instance.Init();
}
#endif
        public List<string> GetSortDes()
        {
            List<string> ret = new();

            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                ret.Add(Descriptions.TryGetValue(skillType, out var Des) ? Des : "Unknow");
            }

            return ret;
        }

        public void Init()
        {
            DefaultActions.Clear();
            ActionTypes.Clear();
            Descriptions.Clear();
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
                            ActionTypes.Add(skillDesAttribute.SkillType, type);
                            DefaultActions.Add(skillDesAttribute.SkillType, System.Activator.CreateInstance(type) as SkillAction);
                            Descriptions.Add(skillDesAttribute.SkillType, skillDesAttribute.Des);
                        }
                    }
                }
            }
        }
    }
}