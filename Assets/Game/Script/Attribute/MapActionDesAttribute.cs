using Game.Script.Map;

namespace Game.Script.Attribute
{
    public class MapActionDesAttribute : System.Attribute
    {
        public  MapActionType ActionType { get; }
        public MapActionDesAttribute(MapActionType type)
        {
            ActionType = type;
        }
    }
}