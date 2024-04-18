namespace Game.Editor
{
    public class LogicDesAttribute : System.Attribute
    {
        public string Des { get; }

        public LogicDesAttribute(string des)
        {
            Des = des;
        }
    }
}