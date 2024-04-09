using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Game.Script.Async
{
    [AsyncMethodBuilder(typeof (ETAsyncTaskMethodBuilder))]
    public struct GameTask
    {
        public static GameTaskCompleted CompletedTask => new GameTaskCompleted();

        private readonly GameTaskCompletionSource awaiter;

        [DebuggerHidden]
        public GameTask(GameTaskCompletionSource awaiter)
        {
            this.awaiter = awaiter;
        }

        [DebuggerHidden]
        public GameTaskCompletionSource GetAwaiter()
        {
            return this.awaiter;
        }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        private async GameVoid InnerCoroutine()
        {
            await this;
        }
    }

    [AsyncMethodBuilder(typeof (ETAsyncTaskMethodBuilder<>))]
    public struct GameTask<T>
    {
        private readonly GameTaskCompletionSource<T> awaiter;

        [DebuggerHidden]
        public GameTask(GameTaskCompletionSource<T> awaiter)
        {
            this.awaiter = awaiter;
        }

        [DebuggerHidden]
        public GameTaskCompletionSource<T> GetAwaiter()
        {
            return this.awaiter;
        }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        private async GameVoid InnerCoroutine()
        {
            await this;
        }
    }
}