using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Game.Editor
{
    public class SkillEditor : OdinMenuEditorWindow
    {
        private const string AssetPath = "Assets/Game/Res/Skill";
        [MenuItem("Tools/技能配置")]           
        private static void OpenWindow()           
        {               
            var window = GetWindow<SkillEditor>();               
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);     
        }
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(supportsMultiSelect: true);
            tree.AddAllAssetsAtPath("数据", AssetPath, typeof(Skill.Skill));
            return tree;
        }
    }
}