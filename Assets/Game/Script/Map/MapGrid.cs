using System.Collections.Generic;
namespace Game.Script.Map
{
    public class MapGrid
    {
        public bool MapBlocked { get; set; } = false;
        private List<Common.Actor> _actors= new();
        private List<Common.Actor> _blockActors = new();

        public void Enter(Common.Actor actor, bool block)
        {
            if (!_actors.Contains(actor))
            {
                _actors.Add(actor);
            }

            if (block)
            {
                if (!_blockActors.Contains(actor))
                {
                    _blockActors.Add(actor);
                }
            }
        }
        
        public void Leave(Common.Actor actor, bool block)
        {
            _actors.Remove(actor);
            if (block)
            {
                _blockActors.Remove(actor);
            }
        }

        public bool Blocked
        {
            get
            {
                if (MapBlocked)
                {
                    return true;
                }
                return ActorBlocked;
            }
        }

        public bool IsBlocked(Common.Actor ignoreActor)
        {
            if (MapBlocked)
            {
                return true;
            }

            if (_blockActors.Count > 0)
            {
                return !_blockActors.Contains(ignoreActor);
            }

            return false;

        }

        bool ActorBlocked => _blockActors.Count > 0;
    }
}