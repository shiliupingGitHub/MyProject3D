﻿using System;
using Game.Script.Setting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Game.Editor
{
    [EditorTool("游戏工具")]
    class GaveViewEditorTool : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);
            Handles.BeginGUI();
            GameSetting.Instance.ShowGrid = GUILayout.Toggle(GameSetting.Instance.ShowGrid, "地图网格");
            GameSetting.Instance.ShowBlock = GUILayout.Toggle(GameSetting.Instance.ShowBlock, "地图阻挡");
            GameSetting.Instance.ShowFps = GUILayout.Toggle(GameSetting.Instance.ShowFps, "显示FPS");
            GameSetting.Instance.ShowPath = GUILayout.Toggle(GameSetting.Instance.ShowPath, "显示路径");
            Handles.EndGUI();
        }
    }
}