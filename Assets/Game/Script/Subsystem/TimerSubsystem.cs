using Cysharp.Threading.Tasks;
using Game.Script.Common;
using Priority_Queue;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class TimerSubsystem : GameSubsystem
    {

        public static UniTask Delay(float time)
        {
            var timerSubsystem = Common.Game.Instance.GetSubsystem<TimerSubsystem>();
            return timerSubsystem.WaitTime(time);
        }
        struct TimerData
        {
            public float Time;
            public UniTaskCompletionSource Tcs;
        }

        private readonly SimplePriorityQueue<TimerData> _queue = new();

        UniTask WaitTime(float time)
        {
            TimerData data = new TimerData
            {
                Tcs = new UniTaskCompletionSource(),
                Time = Time.unscaledTime + time / 1000
            };

            _queue.Enqueue(data, data.Time);

            return data.Tcs.Task;
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
                    if (data.Time <= Time.unscaledTime)
                    {
                        _queue.Remove(data);
                        data.Tcs.TrySetResult();
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