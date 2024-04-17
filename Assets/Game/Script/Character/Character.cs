using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Character
{
    public class Character : Pawn
    {
        public List<global::Skill.Skill> skills = new();

        private readonly List<global::Skill.Skill> _instanceSkills = new();
        private readonly global::Skill.Skill _curSkill = null;

        private Vector3 _lastPosition = new Vector3(-1000, -1000, 0);

        public override bool IsAddToSearch => true;

        protected override void Awake()
        {
            base.Awake();
            if (null != skills)
            {
                foreach (var skill in skills)
                {
                    var instanceSkill = Instantiate(skill);
                    _instanceSkills.Add(instanceSkill);
                    instanceSkill.Init();
                }
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            UpdateGrid();

            if (null != _curSkill)
            {
                _curSkill.ExecuteSkill(deltaTime, this);
            }

            if (_lastPosition != CacheTransform.position)
            {
                _lastPosition = CacheTransform.position;
                positionChanged?.Invoke();
            }
        }
    }
}