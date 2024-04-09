using System;
using System.Collections.Generic;
using Game.Script.Async;
using Game.Script.Common;
using UnityEngine;

namespace Game.Script.Character
{
    public enum PathState
    {
        None,
        Moving,
        Success,
        Fail,
    }
    public class AICharacterMovement : MonoBehaviour
    {
        public float moveSpeed = 0.5f;
        private Rigidbody2D _rigidbody;
        public PathState CurPathState { get; set; } = PathState.None;
        private List<Vector3> _path;
        private int _curPathIndex = -1;
        private float _curAcceptRadius = 1f;

        private GameObject _targetGo ;
        private GameTaskCompletionSource<PathState> _pathTcl;
        private Vector3 _lasChangePosition;
        private float _lastChangePositionTime;
        public GameTask<PathState> Move(List<Vector3> path, float acceptRadius = 1.2f, GameObject targetGo = null)
        {
            _pathTcl = new GameTaskCompletionSource<PathState>();
            _curPathIndex = 2;
            _path = path;
            CurPathState = PathState.Moving;
            _curAcceptRadius = acceptRadius;
            _targetGo = targetGo;
            _lasChangePosition = transform.position;
            _lastChangePositionTime = Time.unscaledTime;
            
            return _pathTcl.Task;
        }

        private void Awake()
        {
            GameLoop.Add(OnUpdate);
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnDestroy()
        {
            GameLoop.Remove(OnUpdate);
        }

        void OnUpdate(float deltaTime)
        {
            DoMove(deltaTime);
            DoCheckMove();
        }
        void DoMove(float deltaTime)
        {
            if (null == _path)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            if (_curPathIndex < 0)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            if (_curPathIndex >= _path.Count)
            {
                _rigidbody.velocity = Vector3.zero;
                CurPathState = PathState.Success;

                _curPathIndex = -1;
                _path = null;
                if (null != _pathTcl)
                {
                    _pathTcl.SetResult(CurPathState);
                    _pathTcl = null;
                }

                return;
            }

            var targetPosition = _path[_curPathIndex];
            var curPosition = transform.position;
            var dir = targetPosition - curPosition;

            if (dir.sqrMagnitude < 0.1)
            {
                _rigidbody.velocity = Vector3.zero;
                _curPathIndex++;
            }
            else
            {
                var endPosition = _path[^1];
                if (_curAcceptRadius >= Vector3.Distance(curPosition, endPosition))
                {
                    CurPathState = PathState.Success;
                    _rigidbody.velocity = Vector3.zero;
                    _path = null;
                    _curPathIndex = -1;
                    if (null != _pathTcl)
                    {
                        _pathTcl.SetResult(CurPathState);
                        _pathTcl = null;
                    }
                }
                else
                {
                    var dis = Vector3.Distance(targetPosition, curPosition);
                    float pathSpeed = dis / deltaTime;
                    float speed = Mathf.Min(pathSpeed, moveSpeed);

                    _rigidbody.velocity = dir.normalized * speed;
                }
            }
        }
        
        void DoCheckMove()
        {
            if (CurPathState != PathState.Moving)
            {
                return;
            }
            if (transform.position == _lasChangePosition)
            {
                if (Time.unscaledTime - _lastChangePositionTime > 0.5f)
                {
                    _rigidbody.velocity = Vector3.zero;

                    if (null != _pathTcl)
                    {
                        _pathTcl.SetResult(CurPathState);
                    }

                    _curPathIndex = -1;
                    _path = null;
                    _pathTcl = null;
                }
            }
            else
            {
                _lasChangePosition = transform.position;
                _lastChangePositionTime = Time.unscaledTime;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CurPathState == PathState.Moving)
            {
                CurPathState = other.gameObject == _targetGo ? PathState.Success : PathState.Fail;
            }

            _rigidbody.velocity = Vector3.zero;

            if (null != _pathTcl)
            {
                _pathTcl.SetResult(CurPathState);
            }

            _curPathIndex = -1;
            _path = null;
            _pathTcl = null;
        }
        public void CancelMove()
        {
            _rigidbody.velocity = Vector3.zero;
            CurPathState = PathState.None;
            if (null != _pathTcl)
            {
                _pathTcl.SetResult(CurPathState);
            }

            _curPathIndex = -1;
            _path = null;
            _pathTcl = null;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (CurPathState == PathState.Moving)
            {
                CurPathState = other.gameObject == _targetGo ? PathState.Success : PathState.Fail;
                _rigidbody.velocity = Vector3.zero;

                if (null != _pathTcl)
                {
                    _pathTcl.SetResult(CurPathState);
                }

                _curPathIndex = -1;
                _path = null;
                _pathTcl = null;
            }
        }
    }
}