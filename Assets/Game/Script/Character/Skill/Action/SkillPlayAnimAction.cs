using Game.Script.Attribute;
using UnityEngine;

namespace Game.Script.Character.Skill.Action
{
    [SkillDes("SkillActionPlayAnimation")]
    public class SkillPlayAnimSkillAction : SkillAction
    {
        [Label("动作名")]
        public string aniName;
    }
}