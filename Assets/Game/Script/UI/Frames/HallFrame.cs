
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class HallFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/HallFrame.prefab";
        [UIPath("offset/InputIp")]
        private InputField _inputIp;
        [UIPath("offset/btnFight")]
        private Button _btnFight;
        [UIPath("offset/btnJoin")]
        private Button _btnJoin;
        [UIPath("offset/btnEdit")]
        private Button _btnEdit;
        [UIPath("offset/btnHome")]
        private Button _btnHome;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnHome.onClick.AddListener(() =>
            {
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                levelSubsystem.Enter(LevelType.Home);
            });
            _btnFight.onClick.AddListener(() =>
            {
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                var networkSubsystem = Common.Game.Instance.GetSubsystem<NetworkSubsystem>();
                networkSubsystem.LoadNetWorkManager();
                Common.Game.Instance.Mode = GameMode.Host;
                Common.Game.Instance.FightMap = "map_test_1";
                NetworkManager.singleton.networkAddress = "localhost";
                levelSubsystem.Enter(LevelType.Fight);
            });
            _btnJoin.onClick.AddListener(() =>
            {
                var networkSubsystem = Common.Game.Instance.GetSubsystem<NetworkSubsystem>();
                networkSubsystem.LoadNetWorkManager();
                NetworkManager.singleton.networkAddress = !string.IsNullOrEmpty(_inputIp.text) ? _inputIp.text : "localhost";
                Common.Game.Instance.Mode = GameMode.Client;
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                levelSubsystem.Enter(LevelType.Fight);
            });
            _btnEdit.onClick.AddListener(() =>
            {
                var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
                Common.Game.Instance.Mode = GameMode.Edit;
                levelSubsystem.Enter(LevelType.Edit);
            });
        }
    }
}