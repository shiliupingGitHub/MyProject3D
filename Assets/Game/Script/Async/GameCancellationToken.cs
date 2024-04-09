using System;

namespace Game.Script.Async
{
    public class GameCancellationToken
    {
        private Action action;

        public void Register(Action callback)
        {
            this.action = callback;
        }

        public void Cancel()
        {
            action.Invoke();
        }
    }
}