﻿using System;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Res;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Game.Script.Character
{
    [RequireComponent(typeof(FightCharacterMovement))]
    [RequireComponent(typeof(FightCharacterAnimation))]
    [RequireComponent(typeof(CharacterController))]
    public class FightCharacter : Character
    {
        
        [TargetRpc]
        void TargetRpc_SetStartFightLeftTime(float leftTime)
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.StartLeftTime = leftTime;
        }

        [Command]
        void Cmd_RequestEnterInfo()
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            TargetRpc_SetStartFightLeftTime(fightSubsystem.StartLeftTime);
        }

        [Command]
        public void Cmd_ExecuteCheat(string str)
        {
            var gameCheatSubsystem = Common.Game.Instance.GetSubsystem<GameCheatSubsystem>();
            
            gameCheatSubsystem.Execute(str);
        }

        protected override bool IsBlock { get; } = false;
        
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            Common.Game.Instance.MyController = this;
            Cmd_RequestEnterInfo();
            
        }
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            var moveComponent = GetComponent<FightCharacterMovement>();
            
            moveComponent.StartControl();
            
        }
        
       

        public override void OnStopLocalPlayer()
        {
            base.OnStopLocalPlayer();
            var moveComponent = GetComponent<FightCharacterMovement>();
            
            moveComponent.EndControl();
        }
    }
}