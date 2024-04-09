using Game.Script.Attribute;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is LabelAttribute attr && attr.Name.Length > 0)
            {
                label.text = attr.Name;
            }
            EditorGUI.PropertyField(position, property, label);
        }
    }
}