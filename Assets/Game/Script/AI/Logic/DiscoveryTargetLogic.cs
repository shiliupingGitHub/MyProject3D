using BehaviorDesigner.Runtime;
using Game.Script.Character;
using Game.Script.Common;
using UnityEngine;

namespace Game.Script.AI.Logic
{
    public class DiscoveryTargetLogic : AILogic
    {
        public override void Tick(AICharacter character, float deltaTime)
        {
            if (Common.Game.Instance.MyController != null)
            {
                if (character.Target == null)
                {
                    var curDisSqt = float.MaxValue;
                    var mePosition = character.Position;
                    FightCharacter player = null;
                    foreach (var fight in Common.Game.Instance.Fights)
                    {
                        var fightPosition = fight.Position;
                        var tempSqt = (fightPosition - mePosition).sqrMagnitude;
                        if (tempSqt < curDisSqt)
                        {
                            curDisSqt = tempSqt;
                            player = fight;
                        }
                    }

                    if (player != null)
                    {
                        GameLoop.RunGameThead(() => { character.Target = player.gameObject; });
                    }
                }
            }
        }
    }
}