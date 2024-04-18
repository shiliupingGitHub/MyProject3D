
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Script.AI;
using Game.Script.AI.Logic;
using Game.Script.Subsystem;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Game.Script.Character
{
    [RequireComponent(typeof(AICharacterMovement))]
    [RequireComponent(typeof(AICharacterAnimation))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(LogicConfig))]
    public class AICharacter : Character
    {
        public LogicConfig Logic { get; private set; }
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

        protected override void Awake()
        {
            base.Awake();
            Logic = GetComponent<LogicConfig>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Raise("removeMonster", this);
          
        }
        



  
    }
}