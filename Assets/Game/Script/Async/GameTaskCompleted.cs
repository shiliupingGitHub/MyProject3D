using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Game.Script.Async
{
    [AsyncMethodBuilder(typeof (AsyncGameTaskCompletedMethodBuilder))]
    public struct GameTaskCompleted: ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public GameTaskCompleted GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [DebuggerHidden]
        public void GetResult()
        {
        }

        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
        }
    }
}