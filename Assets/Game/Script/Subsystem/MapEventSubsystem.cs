using System.Collections.Generic;
using Game.Script.Attribute;
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
            public readonly List<MapAction> Actions = new();
        }

        class TimeExecuteEvent : ExecuteEvent
        {
            public float Delay { get; set; }
            public float WorkDelay { get; set; }
        }

        public Dictionary<MapActionType, MapAction> DefaultActions { get; } = new();
        public Dictionary<MapActionType, System.Type> ActionTypes { get; } = new();

        SimplePriorityQueue<TimeExecuteEvent> _timeEvents = new();
        Dictionary<string, List<ExecuteEvent>> _executeEvents = new();
        Queue<string> _eventQueue = new();
        private SimplePriorityQueue<TimeExecuteEvent> _workTimeExecuteEvents = new();
        private float _curEventTime = 0;
        private bool _bResetTimeEvent = false;
        private float _eventPeriod = 0;
        private bool _bWork = false;

        void OnAllMapLoaded(System.Object o)
        {
            MapData mapData = o as MapData;

            LoadTimeEvent(mapData);
            LoadCustomEvent(mapData);
            PutWorkTimeExecuteEvents();
            _curEventTime = 0;
            _bResetTimeEvent = mapData.BaseSetting.reSetTimeAfterEnd;
            _eventPeriod = mapData.BaseSetting.eventPeriod;
            _bWork = true;
            GameLoop.Add(OnUpdate);
        }

        void PutWorkTimeExecuteEvents()
        {
            _workTimeExecuteEvents.Clear();

            foreach (var timeEvent in _timeEvents)
            {
                if (timeEvent.Delay < 0)
                {
                    timeEvent.WorkDelay = Random.Range(0, _eventPeriod);
                }
                _workTimeExecuteEvents.Enqueue(timeEvent, timeEvent.Delay);
            }
        }

        public void Raise(string eventName)
        {
            _eventQueue.Enqueue(eventName);
        }

        void DoEvent(string eventName)
        {
            if (_executeEvents.TryGetValue(eventName, out var events))
            {
                events.ForEach(eventData => { eventData.Actions.ForEach(action => { action.Execute(); }); });
            }
        }

        void OnLeaveLevel(System.Object o)
        {
            LevelType lt = o is LevelType ? (LevelType)o : LevelType.None;

            if (lt == LevelType.Fight || lt == LevelType.Home)
            {
                _timeEvents.Clear();
                _executeEvents.Clear();
                _workTimeExecuteEvents.Clear();
                _bWork = false;
                GameLoop.Remove(OnUpdate);
            }
        }

        void OnUpdate(float deltaTime)
        {
            if (_bWork)
            {
                UpdateMapTime(deltaTime);
                UpdateEvents(deltaTime);
            }
           
        }

        void UpdateMapTime(float deltaTime)
        {
            _curEventTime += deltaTime;
            if (_curEventTime > _eventPeriod)
            {
                _curEventTime = 0;

                if (_bResetTimeEvent)
                {
                    PutWorkTimeExecuteEvents();
                }
            }
        }

        void UpdateEvents(float deltaTime)
        {
            while (_workTimeExecuteEvents.Count > 0)
            {
                if (_workTimeExecuteEvents.TryFirst(out var data))
                {
                    if (data.WorkDelay <= _curEventTime)
                    {
                        _workTimeExecuteEvents.Remove(data);
                        data.Actions.ForEach(action => { action.Execute(); });
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
                timeEvent.Name = timeEventData.name;
                foreach (var actionData in timeEventData.actions)
                {
                    if (ActionTypes.TryGetValue(actionData.type, out var type))
                    {
                        var action = JsonUtility.FromJson(actionData.data, type) as MapAction;
                        if (action == null)
                        {
                            action = System.Activator.CreateInstance(type) as MapAction;
                        }

                        timeEvent.Actions.Add(action);
                    }
                }

                if (_executeEvents.TryGetValue(timeEvent.Name, out var events))
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
                timeEvent.Name = timeEventData.name;
                timeEvent.Delay = timeEventData.time;
                foreach (var actionData in timeEventData.actions)
                {
                    if (ActionTypes.TryGetValue(actionData.type, out var type))
                    {
                        var action = JsonUtility.FromJson(actionData.data, type) as MapAction;

                        if (action == null)
                        {
                            action = System.Activator.CreateInstance(type) as MapAction;
                        }

                        timeEvent.Actions.Add(action);
                    }

                    _timeEvents.Enqueue(timeEvent, timeEvent.Delay);
                }
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

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