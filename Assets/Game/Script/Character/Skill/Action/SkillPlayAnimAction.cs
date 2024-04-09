using Game.Script.Attribute;
using UnityEngine;

namespace Game.Script.Character.Skill.Action
{
    [SkillDes(SkillType.PlayAnimation, "播放动作")]
    public class SkillPlayAnimSkillAction : SkillAction
    {
        [Label("动作名")]
        public string aniName;
    }
}