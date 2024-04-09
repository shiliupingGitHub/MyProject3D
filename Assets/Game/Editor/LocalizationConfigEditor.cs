using Game.Script.UI.Extern;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(LocalizationConfig))]
    public class LocalizationConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            var config = target as LocalizationConfig;
            EditorGUILayout.BeginVertical();
            if (config != null)
            {
                int delete = -1;
                for (int i = 0; i < config._keys.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    config._keys[i] = (SystemLanguage)EditorGUILayout.EnumPopup(config._keys[i], GUILayout.Width(100));
                    config._values[i] = (TextAsset)EditorGUILayout.ObjectField(config._values[i], typeof(TextAsset), false);
                    if (GUILayout.Button("x"))
                    {
                        delete = i;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(100)))
                {
                    config._keys.Add(SystemLanguage.Afrikaans);
                    config._values.Add(null);
                }

                GUILayout.EndHorizontal();

                if (delete >= 0)
                {
                    config._keys.RemoveAt(delete);
                    config._values.RemoveAt(delete);
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}