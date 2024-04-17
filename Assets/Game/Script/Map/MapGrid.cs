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

        bool ActorBlocked
        {
            get
            {
                return _blockActors.Count > 0;
            }
        }
        
    }
}