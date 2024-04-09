using System.Collections.Generic;

namespace Game.Script.Async
{
    public static class GameTaskHelper
    {
        private class CoroutineBlocker
        {
            private int count;

            private List<GameTaskCompletionSource> tcss = new List<GameTaskCompletionSource>();

            public CoroutineBlocker(int count)
            {
                this.count = count;
            }

            public async GameTask WaitAsync()
            {
                --this.count;
                if (this.count < 0)
                {
                    return;
                }

                if (this.count == 0)
                {
                    List<GameTaskCompletionSource> t = this.tcss;
                    this.tcss = null;
                    foreach (GameTaskCompletionSource ttcs in t)
                    {
                        ttcs.SetResult();
                    }

                    return;
                }

                GameTaskCompletionSource tcs = new GameTaskCompletionSource();
                tcss.Add(tcs);
                await tcs.Task;
            }
        }

        public static async GameTask WaitAny<T>(GameTask<T>[] tasks)
        {
            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);
            foreach (GameTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async GameVoid RunOneTask(GameTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }
        }

        public static async GameTask WaitAny(GameTask[] tasks)
        {
            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);
            foreach (GameTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async GameVoid RunOneTask(GameTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }
        }

        public static async GameTask WaitAll<T>(GameTask<T>[] tasks)
        {
            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);
            foreach (GameTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async GameVoid RunOneTask(GameTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }
        }

        public static async GameTask WaitAll(GameTask[] tasks)
        {
            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);
            foreach (GameTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async GameVoid RunOneTask(GameTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }
        }
    }
}