using Game.Script.Common;
using Game.Script.Res;
using Rewired;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class GameInputSubsystem : GameSubsystem
    {
        private const string InputManagerPath = "Assets/Game/Res/Misc/InputManager.prefab";
        Player Player => ReInput.players?.GetPlayer("System");
        private bool _bSetTouchPosition = false;
        private Vector2 _touchDelta = Vector2.zero;
        private Vector3 _lastTouchPosition;
        public Vector2 TouchDelta => _touchDelta;
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
            if (!_bSetTouchPosition)
            {
                _lastTouchPosition = Input.mousePosition;
                _bSetTouchPosition = true;
            }
            else
            {
                _touchDelta = Input.mousePosition - _lastTouchPosition;
                _lastTouchPosition = Input.mousePosition;
            }
            
        }
        
        public float GetAxis(string name)
        {
            if (Player != null)
            {
                return  Player.GetAxis(name);
            }
            return 0;
        }
        public bool GetMouseButton(int index)
        {
           
            return Input.GetMouseButton(index);
        }
        public float WheelFactor => GetAxis("Wheel");
        
        public void RegisterButtonDown(System.Action<InputActionEventData> callback, string actionName)
        {
            if(Player == null)
                return;
            Player.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionName);
        }
        
        public void RegisterButtonUp(System.Action<InputActionEventData> callback, string actionName)
        {
            if(Player == null)
                return;
            Player.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, actionName);
        }

        public void UnRegisterButtonDown(System.Action<InputActionEventData> callback, string actionName)
        {
            if(Player == null)
                return;
            Player.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionName);
        }
        
        public void UnRegisterButtonUp(System.Action<InputActionEventData> callback, string actionName)
        {
            if(Player == null)
                return;
            Player.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, actionName);
        }
    }
}