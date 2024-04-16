using System;
using System.Collections.Generic;
using Game.Script.Character;
using Game.Script.Character.Skill;
using UnityEngine;
using UnityEngine.Serialization;

namespace Skill
{
    [Serializable]
    public class SkillActonConfig
    {
        [SerializeField] public float time = 0;
        [SerializeField] public SkillType skillType;
        [SerializeField] public string param;
    }

    public enum SkillStatus
    {
        None,
        Executing,
    }
    
    [CreateAssetMenu(fileName = "skill", menuName = "技能配置文件", order = 0)]
    public class Skill : ScriptableObject
    {
        [SerializeField] public float maxTime = 1;
        [SerializeField] public List<SkillActonConfig> actions;
        [NonSerialized] private List<SkillAction> _skillActions = new();
        [NonSerialized] float _curTime = 0;

        private List<SkillAction> _executeActions;
        private SkillStatus _skillStatus = SkillStatus.None;
        public void Init()
        {
            foreach (var action in actions)
            {
                if (SkillMgr.Instance.ActionTypes.TryGetValue(action.skillType, out var type))
                {
                    if (JsonUtility.FromJson(action.param, type) is SkillAction skillAction)
                    {
                        skillAction.Init(action.time);
                        _skillActions.Add(skillAction);
                    }
                }
            }
            
            _skillActions.Sort((SkillAction a, SkillAction b) =>
            {
                if (a.ExecuteTime < b.ExecuteTime)
                    return -1;

                if (a.ExecuteTime > b.ExecuteTime)
                    return 1;

                return 0;
            });
        }

        public void StartSkill()
        {
            _curTime = 0;
            _executeActions.Clear();

            foreach (var action in _skillActions)
            {
                _executeActions.Add(action);
            }

            _skillStatus = SkillStatus.Executing;
        }

        public bool ExecuteSkill(float deltaTime, Pawn controller)
        {

            if (_skillStatus != SkillStatus.Executing)
            {
                return false;
            }
            
            _curTime += deltaTime;

            while (_executeActions.Count > 0)
            {
                var top = _executeActions[0];

                if (top.ExecuteTime >= _curTime)
                {
                    top.Execute(controller);
                    _executeActions.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            if (_curTime >= maxTime)
            {
                _skillStatus = SkillStatus.None;
                return false;
            }
            
            return true;
        }
    }
}
