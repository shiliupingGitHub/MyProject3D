using UnityEngine;

namespace Game.Script.Attribute
{
    public class LabelAttribute : PropertyAttribute
    {
        public string Name { get; }

        public LabelAttribute(string name)
        {
            this.Name = name;
        }
    }
}