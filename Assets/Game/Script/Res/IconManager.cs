using System.Collections.Generic;
using System.IO;
using Game.Script.Common;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.Script.Res
{
    public class IconAtlas
    {
        public string Path;
        public SpriteAtlas Atlas;
    }

    public class IconManager : Singleton<IconManager>
    {
        private const string RootPath = "Assets/Game/Res/Icon";
        private const string ConfigPath = "Assets/Game/Res/Misc/IconConfig.txt";
        private Dictionary<string, string> _iconPath = new();
        private Dictionary<string, Sprite> _iconSprites = new();

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeLoad()
        {
            Instance.Init();
        }

        public Sprite GetIcon(string name)
        {
            if (_iconSprites.TryGetValue(name, out var ret))
            {
                return ret;
            }
            else
            {
                if (_iconPath.TryGetValue(name, out var atlasPath))
                {
                    var atlas = GameResMgr.Instance.LoadAssetSync<SpriteAtlas>(atlasPath);
                   
                        Sprite[] sprites = new Sprite[atlas.spriteCount];
                        atlas.GetSprites(sprites);
                        foreach (var sprite in sprites)
                        {
                            var tempName = sprite.name.Remove(sprite.name.Length - 7);
                            _iconSprites.Add(tempName, sprite);
                        }
                        
                        if (_iconSprites.TryGetValue(name, out var temp))
                        {
                            return temp;
                        }
                }
            }
            return null;
        }

        void Init()
        {
#if UNITY_EDITOR
            LoadFromAsset();
#else
            LoadFromConfig();
#endif
        }

        void LoadFromConfig()
        {
            var asset = GameResMgr.Instance.LoadAssetSync<TextAsset>(ConfigPath);
            _iconPath = SerializationUtility.DeserializeValue<Dictionary<string, string>>(asset.bytes, DataFormat.JSON);
            
        }
#if UNITY_EDITOR
        public void WriteToConfig()
        {
            var data = SerializationUtility.SerializeValue(_iconPath, DataFormat.JSON);
            File.WriteAllBytes(ConfigPath, data);
            UnityEditor.AssetDatabase.Refresh();
        }

        public void LoadFromAsset()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:SpriteAtlas", new[] { RootPath });
            _iconSprites.Clear();
            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var atlas = GameResMgr.Instance.LoadAssetSync<SpriteAtlas>(assetPath);
                Sprite[] sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                foreach (var sprite in sprites)
                {
                    var name = sprite.name.Remove(sprite.name.Length - 7);
                    _iconSprites.Add(name, sprite);
                    _iconPath.Add(name, assetPath);
                }
            }
        }
#endif
    }
}