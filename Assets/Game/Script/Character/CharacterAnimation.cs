using System;
using Spine.Unity;
using UnityEngine;

namespace Game.Script.Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        protected SkeletonAnimation _skeletonAnimation;
        public string idleAnimation = "idle";
        public string walkAnimation = "walk";
        private CharacterMovement _movement;

        protected void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _movement.MovingChanged += OnMovingChanged;
        }
        
        void OnMovingChanged()
        {
            UpdateBaseAnimation();
        }

        string GetBaseAnimationName()
        {
            return _movement.IsMoving ? walkAnimation : idleAnimation;
        }

        void UpdateBaseAnimation()
        {
            if (_skeletonAnimation)
            {
                var baseEntry = _skeletonAnimation.AnimationState.GetCurrent(0);
                var baseAni = GetBaseAnimationName();
                if (baseEntry.Animation.Name != baseAni)
                {
                    _skeletonAnimation.AnimationState.SetAnimation(0, baseAni, true);
                }
            }
        }
    }
}