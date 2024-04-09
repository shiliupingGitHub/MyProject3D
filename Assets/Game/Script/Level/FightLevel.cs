using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using Mirror;

namespace Game.Script.Level
{
    [CustomLevel(LevelType.Fight)]
    public class FightLevel : Level
    {
        public override void Leave()
        {
            base.Leave();
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.FightStart = false;
        }

        public override void Enter()
        {
            base.Enter();
            switch (Common.Game.Instance.Mode)
            {
                case GameMode.Host:
                {
                    NetworkManager.singleton.StartHost();
                }
                break;
                case GameMode.Client:
                {
                    NetworkManager.singleton.StartClient();
                }
                break;
            }

            UIManager.Instance.Show<FightFrame>();
        }
    }
}