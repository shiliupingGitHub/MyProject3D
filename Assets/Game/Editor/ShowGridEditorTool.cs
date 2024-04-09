using System;
using Game.Script.Setting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Game.Editor
{
    [EditorTool("显示游戏地图网格")]
    class ShowGridEditorTool : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            Handles.BeginGUI();
            GameSetting.Instance.ShowGrid = GUILayout.Toggle(GameSetting.Instance.ShowGrid, "显示游戏地图网格");
            Handles.EndGUI();
        }
    }
}