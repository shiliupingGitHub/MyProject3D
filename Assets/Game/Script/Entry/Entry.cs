using Game.Script.Common;
using Game.Script.Res;
using Game.Script.Subsystem;
using Game.Script.UI;
using UnityEngine;

namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        public GameMode entryMode = GameMode.Hall;

        private void Start()
        {
            Common.Game.Instance.Mode = entryMode;
            var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
            switch (entryMode)
            {
                case GameMode.Hall:
                {
                    levelSubsystem.Enter(LevelType.Hall);
                }
                    break;
                case GameMode.Edit:
                    levelSubsystem.Enter(LevelType.Edit);
                    break;
            }
        }
    }
}