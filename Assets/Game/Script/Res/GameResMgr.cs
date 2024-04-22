using System.Collections.Generic;
using CSVHelper;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Script.Res
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    public static class GameResInitializer
    {
        static GameResInitializer()
        {
            GameResMgr.Instance.Init();
        }
    }

#endif
    public class GameResMgr : Singleton<GameResMgr>
    {
#if !UNITY_EDITOR
 [RuntimeInitializeOnLoadMethod]
    static void RuntimeLoad()
        {
        GameResMgr.Instance.Init();
}
#endif

        public void Init()
        {
            CsvHelper.mLoader = OnCsvRead;
            NetworkClient.OnSpawnHook = OnSpawnNetGo;
        }

        GameObject OnSpawnNetGo(SpawnMessage message)
        {
            var template = LoadAssetSync<GameObject>(message.assetPath);

            if (null != template)
            {
                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                var go = Object.Instantiate(template, mapSubsystem.Root.transform);
                return go;
            }

            return null;
        }

        private void OnCsvRead(string szName, System.Action<string, string, System.Action<List<CsvRow>>> readCallBack, System.Action<List<CsvRow>> userCallBack)
        {
            var path = System.IO.Path.Combine("Assets/Game/Res/Config/", szName + ".csv");
            var textAsset = LoadAssetSync<TextAsset>(path);
            var content = System.Text.Encoding.GetEncoding("GBK").GetString(textAsset.bytes);
            readCallBack(szName, content, userCallBack);
        }

        public T LoadAssetSync<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
            var op = Addressables.LoadAssetAsync<T>(path);

            return op.WaitForCompletion();
#endif
        }
    }
}