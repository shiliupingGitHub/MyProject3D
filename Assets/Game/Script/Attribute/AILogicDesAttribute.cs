namespace Game.Editor
{
    public class AILogicDesAttribute : System.Attribute
    {
        public string Des { get; }

        public AILogicDesAttribute(string des)
        {
            Des = des;
        }
    }
}