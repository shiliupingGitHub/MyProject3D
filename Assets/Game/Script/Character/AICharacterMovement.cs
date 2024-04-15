using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Script.Common;
using Game.Script.Setting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Script.Character
{
    public enum PathState
    {
        None,
        Moving,
        Success,
        Fail,
    }

    public class AICharacterMovement : CharacterMovement
    {
        private PathState CurPathState { get; set; } = PathState.None;
        private List<Vector3> _path;
        private int _curPathIndex = -1;
        private float _curAcceptRadius = 0.5f;
        private GameObject _targetGo;
        private UniTaskCompletionSource<PathState> _pathTcl;
        private Vector3 _lasChangePosition;
        private float _lastChangePositionTime;
        private LineRenderer _lineRenderer;
        private GameObject CurtHitGo { get; set; }

        public UniTask<PathState> Move(List<Vector3> path, float acceptRadius = 1.2f, GameObject targetGo = null)
        {
            _pathTcl = new UniTaskCompletionSource<PathState>();
            _curPathIndex = 2;
            _path = path;
            CurPathState = PathState.Moving;
            _curAcceptRadius = acceptRadius;
            _targetGo = targetGo;
            _lasChangePosition = transform.position;
            _lastChangePositionTime = Time.unscaledTime;
            DisplayPath(path);

            return _pathTcl.Task;
        }

        void CheckLineRenderValid()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                Color drawColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.8f);
                _lineRenderer.startColor = drawColor;
                _lineRenderer.endColor = drawColor;
                _lineRenderer.startWidth = 0.03f;
                _lineRenderer.endWidth = 0.03f;
                _lineRenderer.material = new Material(GameSetting.Instance.Config.pathMat);
                _lineRenderer.useWorldSpace = true;
                _lineRenderer.material.SetColor("_BaseColor", drawColor);
            }
        }

        void DisplayPath(List<Vector3> path)
        {
            if (!GameSetting.Instance.ShowPath)
            {
                EndDrawPath();
                return;
            }

            CheckLineRenderValid();
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(path.ToArray());
        }

        void EndDrawPath()
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            GameLoop.Add(OnUpdate);
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

        void CleatPathInfo()
        {
            _curPathIndex = -1;
            _path = null;
            if (null != _pathTcl)
            {
                _pathTcl.TrySetResult(CurPathState);
                _pathTcl = null;
                EndDrawPath();
            }
        }

        private bool NeedPath => _path != null && _curPathIndex >= 0;

        bool CheckPathValid()
        {
            if (!NeedPath)
            {
                return false;
            }

            if (_curPathIndex >= _path.Count)
            {
                CurPathState = PathState.Success;

                CleatPathInfo();

                return false;
            }

            return true;
        }

        void DoMove(float deltaTime)
        {
            if (!CheckPathValid())
            {
                return;
            }

            var targetPosition = _path[_curPathIndex];
            var curPosition = transform.position;
            targetPosition.y = curPosition.y;
            var dir = targetPosition - curPosition;

            if (dir.sqrMagnitude < 0.1)
            {
                _curPathIndex++;
            }
            else
            {
                var endPosition = _path[^1];
                endPosition.y = curPosition.y;
                if (_curAcceptRadius >= Vector3.Distance(curPosition, endPosition))
                {
                    CurPathState = PathState.Success;

                    CleatPathInfo();
                    EndDrawPath();
                }
                else
                {
                    var dis = Vector3.Distance(targetPosition, curPosition);
                    float pathSpeed = dis / deltaTime;
                    float speed = Mathf.Min(pathSpeed, moveSpeed);
                    var flag = CharacterController.Move(dir.normalized * speed * deltaTime);

                    if (flag != CollisionFlags.None)
                    {
                        CurPathState = CurtHitGo == _targetGo ? PathState.Success : PathState.Fail;
                        CleatPathInfo();
                        EndDrawPath();
                    }
                    else
                    {
                        CurtHitGo = null;
                    }
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
                    CurPathState = PathState.Fail;
                    CleatPathInfo();
                }
            }
            else
            {
                _lasChangePosition = transform.position;
                _lastChangePositionTime = Time.unscaledTime;
            }
        }

        public void CancelMove()
        {
            CurPathState = PathState.None;
            CleatPathInfo();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            CurtHitGo = hit.gameObject;
        }
    }
}