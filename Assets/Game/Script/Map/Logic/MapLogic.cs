namespace Game.Script.Map.Logic
{
    public abstract class MapLogic
    {
        public abstract void Tick(float deltaTime) ;
        public abstract void Enter();
        public abstract void Exit();
    }
}