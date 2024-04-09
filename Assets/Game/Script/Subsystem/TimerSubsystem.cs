using Game.Script.Async;
using Game.Script.Common;
using Priority_Queue;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class TimerSubsystem : GameSubsystem
    {

        public static GameTask Delay(float time)
        {
            var timerSubsystem = Common.Game.Instance.GetSubsystem<TimerSubsystem>();
            return timerSubsystem.WaitTime(time);
        }
        struct TimerData
        {
            public float time;
            public GameTaskCompletionSource tcs;
        }

        private readonly SimplePriorityQueue<TimerData> _queue = new();

        GameTask WaitTime(float time)
        {
            TimerData data = new TimerData();
            data.tcs = new GameTaskCompletionSource();
            data.time = Time.unscaledTime + time / 1000;
            
            _queue.Enqueue(data, data.time);

            return data.tcs.Task;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            
            GameLoop.Add(OnUpdate);
            
        }

        void OnUpdate(float deltaTime)
        {
            while (_queue.Count > 0)
            {
                if(_queue.TryFirst(out var data))
                {
                    if (data.time <= Time.unscaledTime)
                    {
                        _queue.Remove(data);
                        data.tcs.SetResult();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}