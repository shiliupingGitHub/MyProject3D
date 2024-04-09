using UnityEngine;

namespace Game.Script.Common
{
    public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (null == _instance)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    Object.DontDestroyOnLoad(go);
                    _instance = go.AddComponent<T>();
                }

                return _instance;
            }
            
        }
    }
}