using Game.Script.Character;

namespace Game.Script.AI.Logic
{
    public abstract class AILogic
    {
        public abstract void Tick(AICharacter character, float deltaTime) ;
    }
}