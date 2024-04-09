using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Script.Character;
using Game.Script.Subsystem;
using UnityEngine;

namespace Game.Script.AI.Action
{
    public class MoveToTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        enum MoveStatus
        {
            None,
            Success,
            Fail,
            Path,
            Moving,
        }

        public SharedGameObject target;
        public float acceptRadius = 1.0f;
        public float rePathDistance = 2;
        private MoveStatus _moveStatus = MoveStatus.None;
        private ulong _pathId;
        private Vector3 _oldTargetPathPosition;

        void StartPath()
        {
            _moveStatus = MoveStatus.Path;
            var end = target.Value.transform.position;
            var start = gameObject.transform.position;
            _oldTargetPathPosition = end;
            FindPath(start, end);
        }

        public override void OnStart()
        {
            base.OnStart();

            if (target.Value != null)
            {
                StartPath();
            }
            else
            {
                _moveStatus = MoveStatus.Fail;
            }
        }

        async void FindPath(Vector3 start, Vector3 end)
        {
            var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
            var path = await pathSystem.AddPath(start, end, ref _pathId);
            DoPath(path);
        }

        async void DoPath(List<Vector3> path)
        {
            if (_moveStatus != MoveStatus.Path)
            {
                return;
            }

            if (path ==null ||  path.Count == 0)
            {
                _moveStatus = MoveStatus.Fail;
                return;
            }

            var characterMovement = GetComponent<AICharacterMovement>();
            _pathId = 0;
            _moveStatus = MoveStatus.Moving;
            var result = await characterMovement.Move(path, acceptRadius, target.Value);

            DoResult(result);
        }

        void DoResult(PathState result)
        {
            if (_moveStatus == MoveStatus.Moving)
            {
                _moveStatus = ConvertPathState(result);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _moveStatus = MoveStatus.None;
            if (_pathId > 0)
            {
                var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
                pathSystem.RemovePath(_pathId);
            }

            var movement = GetComponent<AICharacterMovement>();
            movement.CancelMove();
        }

        MoveStatus ConvertPathState(PathState state)
        {
            switch (state)
            {
                case PathState.Fail:
                    return MoveStatus.Fail;
            }

            return MoveStatus.Success;
        }

        void CheckRePath()
        {
            if (target.Value == null)
                return;

            var disSqt = (target.Value.transform.position - _oldTargetPathPosition).sqrMagnitude;


            if (disSqt >= rePathDistance * rePathDistance)
            {
                var characterMovement = GetComponent<AICharacterMovement>();
                _pathId = 0;
                characterMovement.CancelMove();
                StartPath();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null)
            {
                return TaskStatus.Failure;
            }

            switch (_moveStatus)
            {
                case MoveStatus.Fail:
                {
                    return TaskStatus.Failure;
                }
                case MoveStatus.Success:
                {
                    return TaskStatus.Success;
                }
                case MoveStatus.Moving:
                {
                    CheckRePath();
                    return TaskStatus.Running;
                }
                default:
                    return TaskStatus.Running;
            }
        }
    }
}