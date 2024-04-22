using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor
{
    public class GameAssetPreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; } = 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            RefreshAsset();
        }
        
        [MenuItem("Tools/Refresh Addressables")]
        public static void RefreshAsset()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var group = settings.DefaultGroup;
            var rootPath = "Assets/" + AutoBundles.autoBundlesFolderName;
            var subFolders = AssetDatabase.GetSubFolders(rootPath);
            var entriesAdded = new List<AddressableAssetEntry>();
            foreach (var subFolder in subFolders)
            {
                var guids = AssetDatabase.FindAssets(AutoBundles.assetFilter, new[] {subFolder});

                var folderName = Path.GetFileName(subFolder);
                
                for (int i = 0; i < guids.Length; i++)
                {
                    var curGroup= settings.FindGroup(AutoBundles.autoGroupPrefix + folderName);

                    if (curGroup == null)
                    {
                        curGroup = group;
                    }
                
                    var entry = settings.CreateOrMoveEntry(guids[i], curGroup, readOnly: false, postEvent: false);
                    entry.address = AssetDatabase.GUIDToAssetPath(guids[i]);
                    entriesAdded.Add(entry);
                }
            }
         
 
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
        }
    }
}