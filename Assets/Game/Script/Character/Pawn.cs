using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class Pawn : Actor
    {
        public virtual bool IsAddToSearch => true;
        public Vector3 Position { get; private set; }
        public float LastTickTime;
        protected override void Awake()
        {
            base.Awake();
            Common.Game.Instance.RegisterPawn(this);
            LastTickTime = Time.unscaledTime;
            if (IsAddToSearch)
            {
                var actorSearchSubsystem = Common.Game.Instance.GetSubsystem<ActorSearchSubsystem>();
                actorSearchSubsystem.Add(this);
            }
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            Common.Game.Instance.UnRegisterPawn(this);
            if (IsAddToSearch)
            {
                var actorSearchSubsystem = Common.Game.Instance.GetSubsystem<ActorSearchSubsystem>();
                actorSearchSubsystem.Remove(this);
            }
        }
        
        public virtual void Tick(float deltaTime)
        {
            Position = CacheTransform.position;
        }
    }   
}