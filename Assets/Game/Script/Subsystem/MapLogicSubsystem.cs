using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Common;
using Game.Script.Map.Logic;
using Game.Script.Map.Logic.Fight;
using Game.Script.Map.Logic.Home;

namespace Game.Script.Subsystem
{
    public class MapLogicSubsystem : GameSubsystem
    {
        private readonly List<HomeMapLogic> _homeLogics = new();
        private readonly List<FightMapLogic> _fightLogics = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
            InitHomeLogic();
            InitFightLogic();
        }

        void InitHomeLogic()
        {
            var baseType = typeof(HomeMapLogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var logic = System.Activator.CreateInstance(type) as HomeMapLogic;
                    _homeLogics.Add(logic);
                }
            }
        }

        void InitFightLogic()
        {
            var baseType = typeof(FightMapLogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var logic = System.Activator.CreateInstance(type) as FightMapLogic;
                    _fightLogics.Add(logic);
                }
            }
        }

        void OnUpdate(float deltaTime)
        {
            int num = _homeLogics.Count;
            Parallel.For(0, num, (i, _) =>
            {
                var logic = _homeLogics[i];
                logic.Tick(deltaTime);
            });

            if (Common.Game.Instance.Mode == GameMode.Host)
            {
                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                if (mapSubsystem.MapLoaded)
                {
                    Parallel.For(0, _fightLogics.Count, (i, _) =>
                    {
                        var logic = _fightLogics[i];
                        logic.Tick(deltaTime);
                    });
                }
              
            }
        }
    }
}