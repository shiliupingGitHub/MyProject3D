using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using Game.Script.Common;
using Game.Script.Res;
using Rewired;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class GameInputSubsystem : GameSubsystem
    {
        private const string InputManagerPath = "Assets/Game/Res/Misc/InputManager.prefab";
        Player Player => ReInput.players.GetPlayer("System");
        private bool _bSetMousePosition = false;
        private Vector2 _mouseDelta = Vector2.zero;
        private Vector3 _lastMousePosition;
        public Vector2 MouseDelta => _mouseDelta;
        private InputManager _inputManager;
        public override void OnInitialize()
        {
            base.OnInitialize();
            if (_inputManager == null)
            {
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(InputManagerPath);
                var go = Object.Instantiate(template);
                Object.DontDestroyOnLoad(go);
                _inputManager = go.GetComponent<InputManager>();
               
            }
            GameLoop.Add(OnUpdate);
        }

        void OnUpdate(float deltaTime)
        {
            if (!_bSetMousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                _bSetMousePosition = true;
            }
            else
            {
                _mouseDelta = Input.mousePosition - _lastMousePosition;
                _lastMousePosition = Input.mousePosition;
            }
            
        }

        

        public float GetAxis(string name)
        {
            return Player.GetAxis(name);
        }
        public float GetAxisDelta(string name)
        {
            return Player.GetAxisDelta(name);
        }
        public Vector2 GetAxis2D(string xName, string yName)
        {
            return Player.GetAxis2D(xName, yName);
        }

        public bool GetButtonDown(string name)
        {
            
            return Player.GetButtonDown(name);
        }

        public bool GetButton(string name)
        {
            return Player.GetButtonDown(name);
        }
        
        public bool GetButtonSinglePressHold(string name)
        {
            return Player.GetButtonSinglePressHold(name);
        }

        public bool GetMouseButton(int index)
        {
            return Input.GetMouseButton(index);
        }

        public bool GetKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }
        
        public bool GetMouseButtonUp(int index)
        {
            return Input.GetMouseButtonUp(index);
        }

        public float WheelFactor
        {
            get
            {
                return GetAxis("Wheel");
            }
        }

        public void AddActionCallback(System.Action<InputActionEventData> callback, string actionName)
        {
            Player.AddInputEventDelegate(callback, UpdateLoopType.Update, actionName);
        }

        public void RemoveActionCallback(System.Action<InputActionEventData> callback, string actionName)
        {
            Player.RemoveInputEventDelegate(callback, UpdateLoopType.Update, actionName);
        }
    }
}