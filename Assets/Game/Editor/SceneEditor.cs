

using Game.Script.Res;
using Game.Script.Setting;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    
    public class SceneEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/CreateMapBk")]
        static void CreateMapBk()
        {
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Map/Bk/SceneTemplate.prefab");
            var go = PrefabUtility.InstantiatePrefab(template) as GameObject;
            go.name = "newMapBk";
            PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            GameSetting.Instance.ShowGrid = true;
        }

        [MenuItem("GameObject/复制Hierarchy路径")]
        static void CopyHierarchyPath()
        {
            var gameObject = Selection.activeGameObject;
            string path = gameObject.name;
            Transform t = gameObject.transform;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
 
            // 将路径复制到剪贴板
            GUIUtility.systemCopyBuffer = path;
        }
    }
}