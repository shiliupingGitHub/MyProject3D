using System;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    public class CharacterMovement : NetworkBehaviour
    {
        public float moveSpeed = 1;
        protected CharacterController CharacterController;
        public Action MovingChanged;
        [SyncVar(hook = nameof(OnMovingChanged))]
        protected bool BMoving;
        [SyncVar(hook = nameof(OnMoveDirChange))]
        protected Vector3 _moveDir = Vector3.zero;
        
        public bool IsMoving => BMoving;
        public Vector3 MoveDir => _moveDir;
        void OnMoveDirChange(Vector3 oldValue, Vector3 newValue)
        {
            MovingChanged?.Invoke();
        }
        void OnMovingChanged(bool oldValue, bool newValue)
        {
            MovingChanged?.Invoke();
        }
        
        protected virtual void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
        }
    }
}