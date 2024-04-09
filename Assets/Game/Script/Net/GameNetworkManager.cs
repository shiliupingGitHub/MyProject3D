using Game.Script.Subsystem;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Script.Misc
{
    public class GameNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {

            var player = Common.Game.Instance.serverFightNewPlayer.Invoke();
            
            player.name = $"{player.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            if (sceneName.Contains("Fight"))
            {
                var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                eventSubsystem.Raise("serverFightSceneChanged");
            }
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            eventSubsystem.Raise("LeaveMap");
            var levelSystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
            levelSystem.Enter(LevelType.Hall);
            
        }
    }
}