using System;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Res;
using Game.Script.Subsystem;
using Game.Script.UI;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Character
{
    public class FightCharacterMovement : NetworkBehaviour
    {
        public Action MovingChanged;
        public float moveSpeed = 1;
        private CharacterController _characterController;
        private Vector3 moveDir = Vector3.zero;
        private Camera _camera;
        private CinemachineVirtualCamera _virtualCamera;

        [SyncVar(hook = nameof(OnMovingChanged))]
        private bool _bMoving;

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
            MovingChanged?.Invoke();
        }

        public void StartControl()
        {
            var virtualCameraTemplate = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Player/CameraSetting.prefab");
            var virtualCameraGo = Object.Instantiate(virtualCameraTemplate);
            var vTr = virtualCameraGo.transform.Find("VirtualCamera");
            var confiner = vTr.GetComponent<CinemachineConfiner>();
            _virtualCamera = vTr.GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = transform;
            _virtualCamera.LookAt = transform;
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            var cameraBounds = mapSubsystem.MapBk.transform.Find("CameraBounds").GetComponent<Collider>() ?? throw new ArgumentNullException("mapSubsystem.MapBk.transform.Find(\"CameraBounds\").GetComponent<Collider>()");
            confiner.m_BoundingVolume = cameraBounds;

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
        }

        void DoMove(float deltaTime)
        {
            if (!UIManager.Instance.IsInit)
                return;
            if (UIManager.Instance.UIEventSystem.currentSelectedGameObject != null)
            {
                moveDir = Vector3.zero;
                return;
            }

            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            moveDir.x = gameInputSubsystem.GetAxis("MoveHorizontal");
            moveDir.z = gameInputSubsystem.GetAxis("MoveVertical");
            var dir = moveDir;
            dir.Normalize();

            bool moving = dir != Vector3.zero;

            if (moving != _bMoving)
            {
                SetMoving(moving);
                _bMoving = moving;
                MovingChanged?.Invoke();
            }

            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            if (mapSubsystem.MapBk == null)
            {
                return;
            }

            _characterController.Move(dir * moveSpeed * deltaTime);
        }
    }
}