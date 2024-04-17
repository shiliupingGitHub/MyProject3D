using System.Collections.Generic;
using Game.Script.Common;

namespace Game.Script.Subsystem
{
    public class ActorSearchSubsystem : GameSubsystem
    {
        List<Actor> _actors = new List<Actor>();
        public override void OnInitialize()
        {
            base.OnInitialize();
            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            gameEventSubsystem.Subscribe("AllMapLoaded", OnAllMapLoaded);
            gameEventSubsystem.Subscribe("LeaveLevel", OnLeaveLevel);
            gameEventSubsystem.Subscribe("AllMapUnLoaded", OnAllMapUnLoaded);
        }

        void OnAllMapLoaded(System.Object o)
        {
            GameLoop.Add(OnUpdate);
        }

        void OnAllMapUnLoaded(System.Object o)
        {
            GameLoop.Remove(OnUpdate);
        }
        
        void OnLeaveLevel(System.Object o)
        {
            LevelType lt = o is LevelType ? (LevelType)o : LevelType.None;

            if (lt == LevelType.Fight || lt == LevelType.Home)
            {
                
                GameLoop.Remove(OnUpdate);
            }
        }

        void OnUpdate(float deltaTime)
        {
            
        }
        public void Add(Actor actor)
        {
            _actors.Add(actor);
        }

        public void Remove(Actor actor)
        {
            _actors.Remove(actor);
        }
    }
}