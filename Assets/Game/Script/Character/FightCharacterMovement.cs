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
    public class FightCharacterMovement : CharacterMovement
    {
        
        private Camera _camera;
        private CinemachineVirtualCamera _virtualCamera;
        
        [Command]
        void Cmd_SetMoving(bool moving)
        {
            BMoving = moving;
        }

        [Command]
        void Cmd_SetMoveDir(Vector3 moveDir)
        {
            _moveDir = moveDir;
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
        void DoMove(float deltaTime)
        {
            if (!UIManager.Instance.IsInit)
                return;
            if (UIManager.Instance.UIEventSystem.currentSelectedGameObject != null)
            {
                return;
            }
            var gameInputSubsystem = Common.Game.Instance.GetSubsystem<GameInputSubsystem>();
            var dir = Vector3.zero;
            dir.x = gameInputSubsystem.GetAxis("MoveHorizontal");
            dir.z = gameInputSubsystem.GetAxis("MoveVertical");
            if (dir != base.MoveDir)
            {
                Cmd_SetMoveDir(dir);
                _moveDir = dir;
                MovingChanged?.Invoke();
            }

            dir.Normalize();

            bool moving = dir != Vector3.zero;

            if (moving != BMoving)
            {
                Cmd_SetMoving(moving);
                BMoving = moving;
                MovingChanged?.Invoke();
            }

            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            if (mapSubsystem.MapBk == null)
            {
                return;
            }

            CharacterController.Move(dir * moveSpeed * deltaTime);
        }
    }
}