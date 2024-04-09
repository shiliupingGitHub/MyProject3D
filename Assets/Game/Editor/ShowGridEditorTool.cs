using System;
using Game.Script.Setting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Game.Editor
{
    [EditorTool("网格和阻挡")]
    class ShowGridEditorTool : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            Handles.BeginGUI();
            GameSetting.Instance.ShowGrid = GUILayout.Toggle(GameSetting.Instance.ShowGrid, "地图网格");
            GameSetting.Instance.ShowBlock = GUILayout.Toggle(GameSetting.Instance.ShowBlock, "地图阻挡");
            Handles.EndGUI();
        }
    }
}