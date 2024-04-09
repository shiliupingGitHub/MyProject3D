namespace Game.Script.Attribute
{
    public class UIPathAttribute : System.Attribute
    {
        public string Path { get; set; }
        public UIPathAttribute(string path)
        {
            Path = path;
        }
    }
}