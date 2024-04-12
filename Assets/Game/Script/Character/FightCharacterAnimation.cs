using System;
using Game.Script.Common;
using Game.Script.Res;
using Mirror;
using Spine.Unity;
using UnityEngine;

namespace Game.Script.Character
{
    public class FightCharacterAnimation : CharacterAnimation
    {
        private void Start()
        {
            var go = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Animation/Animation_Test.prefab");
            var skeletonGo = Instantiate(go, transform);
            _skeletonAnimation = skeletonGo.GetComponent<SkeletonAnimation>();
        }
    }
}