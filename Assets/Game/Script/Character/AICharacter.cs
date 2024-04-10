
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Script.AI;
using Game.Script.Async;
using Game.Script.Common;
using Game.Script.Subsystem;
using UnityEngine;


namespace Game.Script.Character
{
    public class AICharacter : Character
    {
        
        private GameBehaviorTree _gameBehaviorTree;
        public ExternalBehavior externalBehaviorTree;
        public GameBehaviorTree BehaviorTree => _gameBehaviorTree;

        private GameObject _target;
        public GameObject Target
        {
            get => _target;
            set
            {
                if (null != _gameBehaviorTree)
                {
                    _target = value;
                    _gameBehaviorTree.SetVariableValue("Target", value);
                }
                
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            _gameBehaviorTree = gameObject.AddComponent<GameBehaviorTree>();
            var etbt = UnityEngine.Object.Instantiate(externalBehaviorTree);
            _gameBehaviorTree.RestartWhenComplete = true;
            _gameBehaviorTree.DisableBehavior();
            _gameBehaviorTree.ExternalBehavior = etbt;
            _gameBehaviorTree.EnableBehavior();
        }

        protected override bool IsBlock => true;
        protected override void Start()
        {
            base.Start();
            
            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Raise("addMonster", this);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Raise("removeMonster", this);
          
        }
        



  
    }
}