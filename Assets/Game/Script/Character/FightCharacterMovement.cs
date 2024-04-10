﻿using System;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Res;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Game.Script.Character
{
    public class FightCharacterMovement : MonoBehaviour
    {
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference MoveLeftAction;
        public InputActionReference MoveRightAction;
        public float MoveSpeed = 1;
        private CharacterController _characterController;
        private Vector3 moveDir = Vector3.zero;
        private Camera _camera;
        CinemachineBrain  _cinemachineBrain;
        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            
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
            
            DoMove(deltaTime);
        }
        void SetUpInput()
        {
            MoveUpAction.action.Enable();
            MoveDownAction.action.Enable();
            MoveLeftAction.action.Enable();
            MoveRightAction.action.Enable();

            MoveUpAction.action.started += context => { moveDir.z += 1; };
            MoveUpAction.action.canceled += context => { moveDir.z -= 1; };

            MoveDownAction.action.started += context => { moveDir.z -= 1; };
            MoveDownAction.action.canceled += context => { moveDir.z += 1; };

            MoveLeftAction.action.started += context => { moveDir.x -= 1; };
            MoveLeftAction.action.canceled += context => { moveDir.x += 1; };

            MoveRightAction.action.started += context => { moveDir.x += 1; };
            MoveRightAction.action.canceled += context => { moveDir.x -= 1; };
        }
        void DoMove(float deltaTime)
        {
            var dir = moveDir;
            dir.Normalize();
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            if (mapSubsystem.MapBk == null)
            {
                return;
            }

            _characterController.Move(dir * MoveSpeed * deltaTime);
            
        }
    }
}