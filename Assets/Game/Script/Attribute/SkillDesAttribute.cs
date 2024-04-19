namespace Game.Script.Character.Skill
{
    public class SkillDesAttribute : System.Attribute
    {
        public string SkillType { get; }
        public SkillDesAttribute(string skillType)
        {
            this.SkillType = skillType;
        }
    }
}