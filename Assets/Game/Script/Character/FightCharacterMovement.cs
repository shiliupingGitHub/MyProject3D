using System;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Res;
using Game.Script.Subsystem;
using Game.Script.UI;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Game.Script.Character
{
    public class FightCharacterMovement : NetworkBehaviour
    {
        public System.Action movingChanged;
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference MoveLeftAction;
        public InputActionReference MoveRightAction;
        public float MoveSpeed = 1;
        private CharacterController _characterController;
        private Vector3 moveDir = Vector3.zero;
        private Camera _camera;
        CinemachineBrain _cinemachineBrain;
        private CinemachineVirtualCamera _cinemachineVirtualCamera;

        [SyncVar(hook = nameof(OnMovingChanged))]
        private bool _bMoving = false;

        public bool IsMoving => _bMoving;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        [Command]
        void SetMoving(bool moving)
        {
            _bMoving = moving;
        }

        void OnMovingChanged(bool oldValue, bool newValue)
        {
            movingChanged?.Invoke();
        }

        public void StartControl()
        {
            var virtualCameraTemplate = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Player/CameraSetting.prefab");
            var virtualCameraGo = Object.Instantiate(virtualCameraTemplate);
            var brainTransform = virtualCameraGo.transform.Find("CinemachineBrain");
            _cinemachineBrain = brainTransform.GetComponent<CinemachineBrain>();
            var vTr = virtualCameraGo.transform.Find("VirtualCamera");
            var cinemachineConfiner = vTr.GetComponent<CinemachineConfiner>();
            _cinemachineVirtualCamera = vTr.GetComponent<CinemachineVirtualCamera>();
            _cinemachineVirtualCamera.Follow = transform;
            _cinemachineVirtualCamera.LookAt = transform;
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            var CameraBounds = mapSubsystem.MapBk.transform.Find("CameraBounds").GetComponent<Collider>();
            cinemachineConfiner.m_BoundingVolume = CameraBounds;

            SetUpInput();
            GameLoop.Add(OnUpdate);
        }

        public void EndControl()
        {
            GameLoop.Remove(OnUpdate);
        }

        private void OnUpdate(float deltaTime)
        {
            if (isLocalPlayer)
            {
                DoMove(deltaTime);
            }
        }

        void SetUpInput()
        {
            MoveUpAction.action.Enable();
            MoveDownAction.action.Enable();
            MoveLeftAction.action.Enable();
            MoveRightAction.action.Enable();

            MoveUpAction.action.started += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject)
                    moveDir.z = 1;
            };
            MoveUpAction.action.canceled += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject)
                    moveDir.z = 0;
            };

            MoveDownAction.action.started += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject)
                    moveDir.z = -1;
            };
            MoveDownAction.action.canceled += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject) moveDir.z = 0;
            };

            MoveLeftAction.action.started += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject) moveDir.x = -1;
            };
            MoveLeftAction.action.canceled += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject) moveDir.x = 0;
            };

            MoveRightAction.action.started += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject) moveDir.x = 1;
            };
            MoveRightAction.action.canceled += context =>
            {
                if (!UIManager.Instance.UIEventSystem.currentSelectedGameObject) moveDir.x = 0;
            };
        }

        void DoMove(float deltaTime)
        {
            if(!UIManager.Instance.IsInit)
                return;
            if (UIManager.Instance.UIEventSystem.currentSelectedGameObject != null)
            {
                moveDir = Vector3.zero;
                return;
            }
            var dir = moveDir;
            dir.Normalize();

            bool moving = dir != Vector3.zero;

            if (moving != _bMoving)
            {
                SetMoving(moving);
                _bMoving = moving;
                movingChanged?.Invoke();
            }

            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            if (mapSubsystem.MapBk == null)
            {
                return;
            }

            _characterController.Move(dir * MoveSpeed * deltaTime);
        }
    }
}