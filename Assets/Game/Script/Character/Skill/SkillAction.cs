using System;
using UnityEngine;

namespace Game.Script.Character.Skill
{
    public class SkillAction
    {
        [NonSerialized]
        private float _executeTime = 0;
        public float ExecuteTime => _executeTime;
        
        public virtual void Init(float time)
        {
            _executeTime = time;
        }
        public virtual void Execute(Pawn controller){}
    }
}