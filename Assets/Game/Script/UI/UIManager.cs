using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Script.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private GameObject _uiRoot;
        private bool _bInit;
        private readonly List<Frame> _queueFrames = new List<Frame>();
        private Transform _baseRoot;
        private Transform _topRoot;
        public bool IsInit => _bInit;
        public EventSystem UIEventSystem { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeLoad()
        {
            Instance.Init();
        }
         void Init()
        {
            if (!_bInit)
            {
                var rootTemplate = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/UI/UIRoot.prefab");
                _uiRoot = Object.Instantiate(rootTemplate);
                Object.DontDestroyOnLoad(_uiRoot);
                _baseRoot = _uiRoot.transform.Find("Canvas/base");
                _topRoot = _uiRoot.transform.Find("Canvas/top");
                UIEventSystem = _uiRoot.GetComponentInChildren<EventSystem>();
                _bInit = true;
            }
        }

        public void Clear()
        {
            foreach (var frame in _queueFrames)
            {
                frame.Destroy();
              
            }
            _queueFrames.Clear();
        }

        public void Hide<T>() where T : Frame
        {
            T curFrame = null;
            foreach (var frame in _queueFrames)
            {
                if (frame.GetType() == typeof(T))
                {
                    curFrame = frame as T;
                    break;
                }
            }

            if (curFrame != null)
            {
                curFrame.Hide();
            }
        }

        public T Get<T>() where T : Frame
        {
            T ret = null;
            foreach (var frame in _queueFrames)
            {
                if (frame.GetType() == typeof(T))
                {
                    ret = frame as T;
                    break;
                }
            }

            return ret;
        }
        
        public T Show<T>(bool bUseQueue = true, bool top = false) where  T : Frame
        {
            T ret = null;
            if (bUseQueue)
            {
                foreach (var frame in _queueFrames)
                {
                    if (frame.GetType() == typeof(T))
                    {
                        ret = frame as T;
                        _queueFrames.Remove(frame);
                        break;
                    }
                }
            }

            if (ret == null)
            {
                ret = System.Activator.CreateInstance<T>();
            
                ret.Init(top?_topRoot:_baseRoot);
            }
            
            ret.FrameGo.transform.SetAsLastSibling();
            _queueFrames.Add(ret);
            ret.Show();
            
            return ret;
        }
    }
}