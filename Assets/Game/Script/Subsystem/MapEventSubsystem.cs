using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Character.Skill;
using Game.Script.Common;
using Game.Script.Map;
using Priority_Queue;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class MapEventSubsystem : GameSubsystem
    {
        class ExecuteEvent
        {
            public string Name { get; set; }
            public List<MapAction> actions = new();
        }

        class TimeExecuteEvent : ExecuteEvent
        {
            public float Time { get; set; }
        }
        public Dictionary<MapActionType, MapAction> DefaultActions { get; } = new();
        public Dictionary<MapActionType, System.Type> ActionTypes { get; } = new();
       
        SimplePriorityQueue<TimeExecuteEvent> _timeEvents = new();
        Dictionary<string, List<ExecuteEvent>> _executeEvents = new();
        Queue<string> _eventQueue = new();

        void OnAllMapLoaded(System.Object o)
        {
            MapData mapData = o as MapData;
           
            LoadTimeEvent(mapData);
            LoadCustomEvent(mapData);

        }

        public void Raise(string eventName)
        {
            _eventQueue.Enqueue(eventName);
        }

        void DoEvent(string eventName)
        {
            if (_executeEvents.TryGetValue(eventName, out var events))
            {
                events.ForEach(eventData =>
                {
                    eventData.actions.ForEach(action =>
                    {
                        action.Execute();
                    });
                });
            }
        }

        void OnLeaveLevel(System.Object o)
        {
            LevelType lt = o is LevelType ? (LevelType)o : LevelType.None;
            
            if(lt == LevelType.Fight)
            {
                _timeEvents.Clear();
                _executeEvents.Clear();
            }
        }

        void OnUpdate(float deltaTime)
        {
            while (_timeEvents.Count > 0)
            {
                if(_timeEvents.TryFirst(out var data))
                {
                    if (data.Time <= Time.unscaledTime)
                    {
                        _timeEvents.Remove(data);
                        data.actions.ForEach(action =>
                        {
                            action.Execute();
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            }

            while (_eventQueue.Count > 0)
            {
                DoEvent(_eventQueue.Dequeue());
            }
        }

        void LoadCustomEvent(MapData mapData)
        {
            _timeEvents.Clear();
            float curTime = Time.unscaledTime;
            foreach (var timeEventData in mapData.timeEvents)
            {
                var timeEvent = new TimeExecuteEvent();
                timeEvent.Time = curTime + timeEventData.time;
                timeEvent.Name = timeEventData.name;
                foreach (var actionData in timeEventData.actions)
                {
                    if(ActionTypes.TryGetValue(actionData.type, out var type))
                    {
                        
                        var action = JsonUtility.FromJson(actionData.data, type) as MapAction;
                        if (action == null)
                        {
                            action = System.Activator.CreateInstance(type) as MapAction;
                        }
                        timeEvent.actions.Add(action);
                    }
                    
                }
                
                if(_executeEvents.TryGetValue(timeEvent.Name, out var events))
                {
                    events.Add(timeEvent);
                }
                else
                {
                    _executeEvents.Add(timeEvent.Name, new List<ExecuteEvent>() { timeEvent });
                }
            }
        }

        void LoadTimeEvent(MapData mapData)
        {
            _timeEvents.Clear();
            float curTime = Time.unscaledTime;
            foreach (var timeEventData in mapData.timeEvents)
            {
                var timeEvent = new TimeExecuteEvent();
                timeEvent.Time = curTime + timeEventData.time;
                timeEvent.Name = timeEventData.name;
                foreach (var actionData in timeEventData.actions)
                {
                    if(ActionTypes.TryGetValue(actionData.type, out var type))
                    {
                        
                        var action = JsonUtility.FromJson(actionData.data, type) as MapAction;

                        if (action == null)
                        {
                            action = System.Activator.CreateInstance(type) as MapAction;
                        }
                        timeEvent.actions.Add(action);
                    }
                    _timeEvents.Enqueue(timeEvent, timeEvent.Time);
                }
            }
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            gameEventSubsystem.Subscribe("AllMapLoaded", OnAllMapLoaded);
            gameEventSubsystem.Subscribe("LeaveLevel", OnLeaveLevel);
            DefaultActions.Clear();
            ActionTypes.Clear();
            var baseType = typeof(MapAction);
            var types = baseType.Assembly.GetTypes();

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attrs = type.GetCustomAttributes(typeof(MapActionDesAttribute), false);

                    foreach (var attr in attrs)
                    {
                        if (attr is MapActionDesAttribute desAttribute)
                        {
                            ActionTypes.Add(desAttribute.ActionType, type);
                            DefaultActions.Add(desAttribute.ActionType, System.Activator.CreateInstance(type) as MapAction);
                        }
                    }
                }
            }
        }
    }
}