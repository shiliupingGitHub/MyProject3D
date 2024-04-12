using Game.Script.Subsystem;
using Mirror;

namespace Game.Script.Misc
{
    public class HomeNetworkManager : NetworkManager
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

            if (sceneName == onlineScene)
            {
                var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                eventSubsystem.Raise("serverSceneChanged");
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