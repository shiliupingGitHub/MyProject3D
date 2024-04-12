using System;
using Game.Script.Common;
using Game.Script.Res;
using Mirror;
using Spine.Unity;
using UnityEngine;

namespace Game.Script.Character
{
    public class FightCharacterAnimation : NetworkBehaviour
    {
        private SkeletonAnimation _skeletonAnimation;
        public  string idleAnimation = "idle";
        public string walkAnimation = "walk";
        private FightCharacterMovement _movement;
        private void Start()
        {
            var go = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Animation/Animation_Test.prefab");
            var skeletonGo = Instantiate(go, transform);
            _skeletonAnimation = skeletonGo.GetComponent<SkeletonAnimation>();
            _movement = GetComponent<FightCharacterMovement>();
            _movement.MovingChanged += OnMovingChanged;
        }

        void OnMovingChanged()
        {
            UpdateBaseAnimation();
        }

        void UpdateBaseAnimation()
        {
            _skeletonAnimation.AnimationState.SetAnimation(0, _movement.IsMoving?walkAnimation:idleAnimation, true);
        }

   
    }
}