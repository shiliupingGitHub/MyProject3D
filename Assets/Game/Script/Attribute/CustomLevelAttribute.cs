using Game.Script.Subsystem;

namespace Game.Script.Attribute
{
    public class CustomLevelAttribute : System.Attribute
    {
        public  LevelType lT { get; }
        public CustomLevelAttribute(LevelType type)
        {
            lT = type;
        }
    }
}