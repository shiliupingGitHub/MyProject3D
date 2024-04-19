using Game.Script.Map;

namespace Game.Script.Attribute
{
    public class MapActionDesAttribute : System.Attribute
    {
        public  string Des { get; }
        public MapActionDesAttribute(string des)
        {
            Des = des;
        }
    }
}