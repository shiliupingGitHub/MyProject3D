using System.Collections.Generic;

namespace Game.Script.Subsystem
{

        
    public class EventSubsystem : GameSubsystem
    {
        private Dictionary<string, System.Action<System.Object>> _subscribers = new();
        private List<string> _fightEvents = new();
        public void Raise(string eventName, System.Object o = null)
        {
            if(_subscribers.TryGetValue(eventName, out var subscriber))
                subscriber(o);
        }
        public  void Subscribe(string eventName, System.Action<System.Object> subscriber)
        {
            if(_subscribers.ContainsKey(eventName))
                _subscribers[eventName] += subscriber;
            else
            {
                _subscribers[eventName] = subscriber;
            }
        }
        
        public  void UnSubscribe(string eventName, System.Action<System.Object> subscriber)
        {
            if(_subscribers.ContainsKey(eventName))
                _subscribers[eventName] -= subscriber;
        }
    }
}