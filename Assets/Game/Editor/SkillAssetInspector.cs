using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.DemiEditor;
using Game.Script.Attribute;
using Game.Script.Character.Skill;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    [CustomEditor(typeof(Skill))]
    public class SkillAssetInspector : UnityEditor.Editor
    {
        private Skill _skill = null;
        private float curTime = 0;
        private int selectActionIndex = 0;
        private Dictionary<System.Type, Action<System.Object, FieldInfo>> _typeDraw = new();
        private Texture2D _backGround;
        private GUIStyle _style;
        private void OnEnable()
        {
            _skill = target as Skill;
            _typeDraw.Clear();
            _typeDraw.Add(typeof(string), OnDrawStringField);
            _typeDraw.Add(typeof(float), OnDrawFloatField);
            _typeDraw.Add(typeof(int), OnDrawIntField);
        }
        

        private void Awake()
        {
            _backGround = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Game/Editor/Img/squareBorderCurvedEmptyThick.png");
            _style = new GUIStyle();
            _style.normal.background = _backGround;
            _style.border = new RectOffset(4, 4, 4, 4);
        }

        string GetHeaderName(FieldInfo fieldInfo)
        {
            string header = fieldInfo.Name;
            var attr = fieldInfo.GetCustomAttribute<LabelAttribute>();

            if (null != attr)
            {
                header = attr.Name;
            }

            return header;
        }
        void OnDrawStringField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is string ? (string)fieldInfo.GetValue(action) : string.Empty;
            string header = GetHeaderName(fieldInfo);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(header, GUILayout.Width(100));
            var newValue = EditorGUILayout.TextField(curValue, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            fieldInfo.SetValue(action, newValue);
            
        }
        
        void OnDrawFloatField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is float ? (float)fieldInfo.GetValue(action) : 0;
            string header = GetHeaderName(fieldInfo);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(header, GUILayout.Width(100));
            var newValue = EditorGUILayout.FloatField(curValue, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            fieldInfo.SetValue(action, newValue);
            
        }
        
        void OnDrawIntField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is int ? (int)fieldInfo.GetValue(action) : 0;
            string header = GetHeaderName(fieldInfo);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(header, GUILayout.Width(100));
            var newValue = EditorGUILayout.IntField(curValue, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            fieldInfo.SetValue(action, newValue);
            
        }

        string DrawAction(SkillType skill, string param)
        {
            
            var type = SkillMgr.Instance.ActionTypes[skill];
            var action = JsonUtility.FromJson(param, type);

            if (null == action)
            {
                action = System.Activator.CreateInstance(type);
            }
            var typeInfo = (System.Reflection.TypeInfo)type;
            
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
                var fieldType = field.FieldType;

                if (_typeDraw.TryGetValue(fieldType, out var drawAction))
                {
                    drawAction.Invoke(action, field);
                }
                
            }
            return JsonUtility.ToJson(action);
        }
        
        
        void DrawTime()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("持续时间", GUILayout.Width(100));
            _skill.maxTime = EditorGUILayout.FloatField( _skill.maxTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }

        void DrawOp()
        {
    
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
          
            curTime = GUILayout.HorizontalSlider(curTime, 0, _skill.maxTime);
            curTime = EditorGUILayout.FloatField(curTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            var sortDes = SkillMgr.Instance.GetSortDes();
            GUILayout.Label("行为:");
            selectActionIndex = EditorGUILayout.Popup(selectActionIndex, sortDes.ToArray(), GUILayout.Width(100));
            if (GUILayout.Button("添加行为", GUILayout.Width(100)))
            {
                var type = (SkillType)selectActionIndex;
                if (SkillMgr.Instance.DefaultActions.TryGetValue(type, out var defaultAction))
                {
                    var param = JsonUtility.ToJson(defaultAction);
                    _skill.actions.Add(new SkillActonConfig()
                    {
                        param = param, skillType = type, time = curTime,
                    });
                }
               
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }

        void DrawActions()
        {
            var sortDes = SkillMgr.Instance.GetSortDes();
            EditorGUILayout.Space(50);
            // style.normal.background = _backGround;
            EditorGUILayout.BeginVertical();
            
            bool bRemove = false;
            SkillActonConfig removeActionConfig = null;
            foreach (var action in _skill.actions)
            {
                // style.normal.background = _backGround;
                EditorGUILayout.BeginHorizontal();
                int skillTypeIndex = (int)action.skillType;

                if (sortDes.Count() > skillTypeIndex)
                {
                    
                    action.skillType = (SkillType)EditorGUILayout.Popup(skillTypeIndex, sortDes.ToArray(), GUILayout.Width(100));
                    action.time = EditorGUILayout.FloatField(action.time , GUILayout.Width(50));
                    EditorGUILayout.Space(50);
                    var oldColor = GUI.color;
                    GUI.color = Color.green;
                    // style.normal.background = _backGround;
                    EditorGUILayout.BeginVertical(_style);
                    action.param = DrawAction(action.skillType, action.param);
                   // action.param = SkillMgr.Instance.DefaultActions[action.skillType].OnGui(action.param);
                    EditorGUILayout.EndVertical();
                    GUI.color = oldColor;
                    if (GUILayout.Button("X", GUILayout.Width(100)))
                    {
                        bRemove = true;
                        removeActionConfig = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(20);
            }

            if (bRemove)
            {
                _skill.actions.Remove(removeActionConfig);
            }
            EditorGUILayout.EndVertical();
            
            
        }

        void DrawContent()
        {
            
            DrawOp();
            DrawActions();
    
        }
        public override void OnInspectorGUI()
        {
            
            EditorGUILayout.BeginVertical(_style);

            DrawTime();
            EditorGUILayout.Space(50);
            DrawContent();
            EditorGUILayout.Space(50);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(50);
        }
    }
}