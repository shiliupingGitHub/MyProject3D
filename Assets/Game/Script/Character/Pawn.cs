using Game.Script.Common;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class Pawn : Actor
    {
        public Vector3 Position { get; private set; }
        public float LastTickTime;
        protected override void Awake()
        {
            base.Awake();
            Common.Game.Instance.RegisterPawn(this);
            LastTickTime = Time.unscaledTime;
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            Common.Game.Instance.UnRegisterPawn(this);
        }
        
        public virtual void Tick(float deltaTime)
        {
            Position = CacheTransform.position;
        }
    }   
}