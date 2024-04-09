using System;
using UnityEngine;

namespace Game.Script.Common
{
    
    public class GameLoop : UnitySingleton<GameLoop>
    {
        public bool drawFps = true;
        public System.Action<float> doUpdate;
        public System.Action<float> doFixedUpdate;
        private System.Action _threadAction;
        public System.Action applicationQuit;
        private int _frame;
        // 上一次计算帧率的时间
        private float _lastTime;
        // 平均每一帧的时间
        private float _frameDeltaTime;
        // 间隔多长时间(秒)计算一次帧率
        private float _Fps;
        private const float _timeInterval = 0.5f;
        
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeLoad()
        {
            Instance.Init();
        }

        void Init()
        {
            doUpdate = null;
        }

         void OnApplicationQuit()
        {
            applicationQuit.Invoke();
        }

        private void Update()
        {
            
            if (null != doUpdate)
            {
                doUpdate.Invoke(Time.unscaledDeltaTime);
            }

            lock (this)
            {
                if (null != _threadAction)
                {
                    _threadAction.Invoke();
                }

                _threadAction = null;
            }

            Physics2D.Simulate(Time.unscaledDeltaTime);
            Physics2D.SyncTransforms();
            FrameCalculate();
        }

        private void FrameCalculate()
        {
            _frame++;
            if (Time.realtimeSinceStartup - _lastTime < _timeInterval)
            {
                return;
            }

            float time = Time.realtimeSinceStartup - _lastTime;
            _Fps = _frame / time;
            _frameDeltaTime = time / _frame;

            _lastTime = Time.realtimeSinceStartup;
            _frame = 0;
        }

        private void Start()
        {
            _lastTime = Time.realtimeSinceStartup;
            //Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

        }

        private void OnGUI()
        {
            if (drawFps)
            {
                string msg = string.Format("Fps:{0}  FpsDeltaTime:{1}", _Fps, _frameDeltaTime);
                GUI.Label(new Rect(0, 0, 300, 50), msg);
              
            }
        }

        private void FixedUpdate()
        {
            if (null != doFixedUpdate)
            {
                doFixedUpdate.Invoke(Time.fixedUnscaledDeltaTime);
            }
        }

        public static void RunGameThead(System.Action action)
        {
            if (!_instance)
            {
                return;
            }
            lock (_instance)
            {
                _instance._threadAction += action;
            }
        }

        public static void Add(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doUpdate += action;
            }
        }

        public static void AddQuit(System.Action action)
        {
            if (_instance)
            {
                _instance.applicationQuit += action;
            }
        }
        
        public static void RemoveQuit(System.Action action)
        {
            if (_instance)
            {
                _instance.applicationQuit -= action;
            }
        }

        public static void Remove(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doUpdate -= action;
            }
        }
        
        public static void AddFixed(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doFixedUpdate += action;
            }
        }

        public static void RemoveFixed(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doFixedUpdate -= action;
            }
        }

        
    }
}