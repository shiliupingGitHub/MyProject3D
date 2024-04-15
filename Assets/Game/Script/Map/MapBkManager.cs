using System.Collections.Generic;
using Game.Script.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Script.Map
{
    public class MapBkManager : Singleton<MapBkManager>
    {
        private List<MapBk> _mapBks = new();
        public void Add(MapBk mapBk)
        {
            _mapBks.Add(mapBk);
        }

        public MapBk FindMapBk(Scene scene)
        {
            return _mapBks.Find(x => scene.IsValid()?x.gameObject.scene == scene: x.gameObject.scene == SceneManager.GetActiveScene());
        }

        public void Remove(MapBk mapBk)
        {
            _mapBks.Remove(mapBk);
        }
    }
}