
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class FightFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightFrame.prefab";
        
        [UIPath("offset/lbLeftTime")] private Text _lbLeftTime;
        [UIPath("offset/btnReturnHall")] private Button _btnReturnHall;
        private LineRenderer _lineRenderer;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            UpdateStartFightLeftTime();
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.startLeftTimeChanged += UpdateStartFightLeftTime;
            _btnReturnHall.onClick.AddListener(() =>
            {
                switch (Common.Game.Instance.Mode)
                {
                    case GameMode.Host:
                    {
                        NetworkManager.singleton.StopHost();
                    }
                        break;
                    case GameMode.Client:
                    {
                        NetworkManager.singleton.StopClient();
                    }
                        break;
                }
            });
          

        }

        void UpdateStartFightLeftTime()
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();

            if (fightSubsystem.StartLeftTime > 0)
            {
                _lbLeftTime.enabled = true;
                _lbLeftTime.text = Mathf.RoundToInt(fightSubsystem.StartLeftTime).ToString();
            }
            else
            {
                _lbLeftTime.enabled = false;
            }
        }
        
        public override void Destroy()
        {
            base.Destroy();
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.startLeftTimeChanged -= UpdateStartFightLeftTime;
        }
    }
}