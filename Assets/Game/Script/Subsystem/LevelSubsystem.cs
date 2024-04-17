using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Level;
using Game.Script.UI;
using Game.Script.UI.Frames;

namespace Game.Script.Subsystem
{
    public enum LevelType
    {
        None,
        Hall,
        Fight,
        Edit,
        Home,
    }
    public class LevelSubsystem : GameSubsystem
    {
        public System.Action<LevelType, LevelType> preLevelChange;
        private readonly Dictionary<LevelType, Level.Level> _levels = new();
            
        public override void OnInitialize()
        {
            base.OnInitialize();
            var baseType = typeof(Level.Level);
            var types = baseType.Assembly.GetTypes();
            
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                  var lTAttrs = type.GetCustomAttributes(typeof(CustomLevelAttribute), true);

                  if (lTAttrs.Length > 0)
                  {
                        _levels.Add(((CustomLevelAttribute)lTAttrs[0]).lT, (Level.Level)System.Activator.CreateInstance(type));
                  }
                }
            }


        }

        private LevelType _curLevel = LevelType.None;
        
        public void Enter(LevelType levelType)
        {
            if (preLevelChange != null)
            {
                preLevelChange.Invoke(_curLevel, levelType);
            }
            if (_levels.TryGetValue(_curLevel, out var level))
            {
                level.Leave();
                
                var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                if (null != gameEventSubsystem)
                {
                    gameEventSubsystem.Raise("LeaveLevel", _curLevel);
                }
            }
            UIManager.Instance.Clear();
            UIManager.Instance.Show<LoadingFrame>(true, true);
            DoEnter(levelType);
            
        }

        async void DoEnter(LevelType levelType)
        {
            await TimerSubsystem.Delay(1);
            _levels[levelType].Enter();
            _curLevel = levelType;
            System.GC.Collect();
        }
    }
}