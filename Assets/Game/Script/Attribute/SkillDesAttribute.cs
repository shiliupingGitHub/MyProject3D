namespace Game.Script.Character.Skill
{
    public class SkillDesAttribute : System.Attribute
    {
        public SkillType SkillType { get; }
        public string Des { get; }
        public SkillDesAttribute(SkillType skillType, string des)
        {
            this.SkillType = skillType;
            this.Des = des;
        }
    }
}