
using Game.Script.Attribute;
using Game.Script.Common;
using Mirror;
using UnityEngine;

namespace Game.Script.Map.Actor
{
    public class PlayerStartActor : MapActor
    {
        public GameObject Monster;
        
        protected override void Start()
        {
            base.Start();

            if (Common.Game.Instance.Mode == GameMode.Host)
            {
                var position = transform.position;
                position.x += 0.5f;
                position.z += 0.5f;
                position.y += 1;
                var go = Instantiate(Monster, position, Quaternion.identity);
                NetworkServer.Spawn(go);
            }
            
        }
    }
}