
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Script.Map.Actor
{
    public class PlayerStartActor : MapActor
    {
        public GameObject TestMonster;
        protected override void Start()
        {
            base.Start();
            if (Common.Game.Instance.Mode == GameMode.Host)
            {
                if (null != TestMonster)
                {
                    var position = transform.position;
                    position.x += 0.5f;
                    position.z += 0.5f;
                    var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                    var monster = Instantiate(TestMonster, mapSubsystem.Root.transform);
                    monster.transform.position = position;
                    NetworkServer.Spawn(monster);
                }
              
            }
        }
    }
}