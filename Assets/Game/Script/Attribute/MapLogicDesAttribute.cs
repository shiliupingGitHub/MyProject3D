namespace Game.Script.Attribute
{
    public class MapLogicDesAttribute : System.Attribute
    {
        public string Des { get; }

        public MapLogicDesAttribute(string des)
        {
            Des = des;
        }
    }
}