using System;
using System.Globalization;
using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Res;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Ext
{
    public static class FieldDrawer
    {
        private const string FloatTemplate = "Assets/Game/Res/UI/Extern/floatParam.prefab";
        private const string IntTemplate = "Assets/Game/Res/UI/Extern/intParam.prefab";
        private const string StringTemplate = "Assets/Game/Res/UI/Extern/stingParam.prefab";
        private const string boolTemplate = "Assets/Game/Res/UI/Extern/boolParam.prefab";
        public static void BeginDraw(Transform tr)
        {
            for (int i = tr.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(tr.GetChild(i).gameObject);
            }
        }

        public static void Draw(Transform tr, System.Object obj, System.Action<FieldInfo,System.Object> valueChanged, System.Type attributeTag = null)
        {
            var type = obj.GetType();
            var typeInfo = (TypeInfo)type;
            foreach (var field in typeInfo.DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (!field.IsPublic)
                {
                    continue;
                }
                
                if(null != attributeTag)
                {
                    if (!field.IsDefined(attributeTag))
                    {
                        continue;
                    }
                }
                Draw(tr, field, obj, o =>
                {
                   valueChanged.Invoke(field, o);
                });
                
            }
        }
        public static void Draw(Transform tr, FieldInfo fieldInfo, object obj, System.Action<System.Object> valueChanged)
        {
            if (fieldInfo.FieldType == typeof(float))
            {
                DrawFloat(tr, fieldInfo, obj, (value) =>
                {
                    valueChanged(value);
                });
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                DrawInt(tr, fieldInfo, obj, ( value) =>
                {
                    valueChanged(value);
                });
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                DrawString(tr, fieldInfo, obj, valueChanged);
            }
            else if(fieldInfo.FieldType == typeof(bool))
            {
                DrawBool(tr, fieldInfo, obj, valueChanged);
            }
        }
       static string GetHeaderName(FieldInfo fieldInfo)
        {
            string header = fieldInfo.Name;
            var attr = fieldInfo.GetCustomAttribute<LabelAttribute>();

            if (null != attr)
            {
                header = attr.Name;
            }
            
            var localizationSystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();

            return localizationSystem.Get(header);
        }
        
        private static void DrawBool(Transform tr, FieldInfo fieldInfo, object obj, Action<object> valueChanged)
        {
            var curValue =  (bool)fieldInfo.GetValue(obj);
            string header = GetHeaderName(fieldInfo);
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(boolTemplate);
            var go = Object.Instantiate(template, tr);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var toggle = go.transform.Find("toggleValue").GetComponent<Toggle>();
            toggle.isOn = curValue;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(value =>
            {
                fieldInfo.SetValue(obj, value);
                valueChanged.Invoke(value);
            });
            
        }
        private static void DrawFloat(Transform tr, FieldInfo fieldInfo, object obj, System.Action<float> valueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is float ? (float)fieldInfo.GetValue(obj) : 0;
            string header = GetHeaderName(fieldInfo);
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(FloatTemplate);
            var go = Object.Instantiate(template, tr);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue.ToString(CultureInfo.InvariantCulture);
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(str =>
            {
                float.TryParse(str, result: out var value);
                fieldInfo.SetValue(obj, value);
                valueChanged.Invoke(value);
            });
        }

        private static void DrawString(Transform tr,FieldInfo fieldInfo, object obj, System.Action<string> valueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is string ? (string)fieldInfo.GetValue(obj) : string.Empty;
            string header = GetHeaderName(fieldInfo);
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(StringTemplate);
            var go = Object.Instantiate(template, tr);
            go.SetActive(true);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue;
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(str =>
            {
                fieldInfo.SetValue(obj, str);
                valueChanged.Invoke(str);
            });
        }

        private static void DrawInt(Transform tr,FieldInfo fieldInfo, object obj, System.Action<int> valueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is int ? (int)fieldInfo.GetValue(obj) : 0;
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(IntTemplate);
            var go = Object.Instantiate(template, tr);
            go.SetActive(true);
            string header = GetHeaderName(fieldInfo);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue.ToString();
            input.onValueChanged.RemoveAllListeners();
            input.onValueChanged.AddListener(str =>
            {
                int.TryParse(str, result: out var value);
                fieldInfo.SetValue(obj, value);
                valueChanged.Invoke(value);
            });
        }
    }
}