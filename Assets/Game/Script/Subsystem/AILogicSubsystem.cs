using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Game.Editor;
using Game.Script.AI.Logic;
using Game.Script.Character;
using Game.Script.Common;
using UnityEngine;


namespace Game.Script.Subsystem
{
    public class AILogicSubsystem : GameSubsystem
    {
        private readonly Dictionary<string, AILogic> _logics = new();
        private readonly List<AICharacter> _characters = new();
        private float _lastTickTime;

        public override void OnInitialize()
        {
            base.OnInitialize();
            var baseType = typeof(AILogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attribute = type.GetCustomAttribute<AILogicDesAttribute>();

                    if (null != attribute)
                    {
                        var logic = System.Activator.CreateInstance(type) as AILogic;
                        _logics.Add(attribute.Des, logic);
                    }
                }
            }

            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Subscribe("addMonster", o => { _characters.Add(o as AICharacter); });

            eventSubsystem.Subscribe("removeMonster", o => { _characters.Remove(o as AICharacter); });

            Tick();
        }

        void TickCharacters(float deltaTime)
        {
            if (_characters.Count > 0)
            {
                int num = _characters.Count;
                Parallel.For(0, num, (i, _) =>
                {
                    var character = _characters[i];
                    TickLogic(character, deltaTime);
                });
            }
        }

        void TickLogic(AICharacter character, float deltaTime)
        {
            var logicConfig = character.AILogic;
            if (null != logicConfig)
            {
                foreach (var logicName in logicConfig.logics)
                {
                    if (_logics.TryGetValue(logicName, out var logic))
                    {
                        logic.Tick(character, deltaTime);
                    }
                }
            }
        }

        async void Tick()
        {
            while (true)
            {
                if (Common.Game.Instance.Mode == GameMode.Host)
                {
                    if (_lastTickTime == 0)
                    {
                        _lastTickTime = Time.unscaledTime;
                    }
                    else
                    {
                        float curTime = Time.unscaledTime;
                        float delta = curTime - _lastTickTime;
                        _lastTickTime = curTime;
                        TickCharacters(delta);
                    }
                }


                await TimerSubsystem.Delay(1);
            }
        }
    }
}