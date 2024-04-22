using System;
using System.Collections.Generic;
using Game.Script.Common;
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
        private Dictionary<string, IconAtlas> _atlasDic = new();

        [RuntimeInitializeOnLoadMethod]
        static void RuntimeLoad()
        {
            IconManager.Instance.Init();
        }

        void Init()
        {
#if UNITY_EDITOR
            LoadFromAsset();
#endif
        }
#if UNITY_EDITOR
        void LoadFromAsset()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:SpriteAtlas", new[] { RootPath });
            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var atlas = GameResMgr.Instance.LoadAssetSync<SpriteAtlas>(assetPath);
                Sprite[] sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                foreach (var sprite in sprites)
                {
                    var name = sprite.name.Remove(sprite.name.Length - 7); ;
                    _atlasDic.Add(name, new IconAtlas(){Path = assetPath, Atlas = atlas});
                }
            }
        }
#endif
    }
}