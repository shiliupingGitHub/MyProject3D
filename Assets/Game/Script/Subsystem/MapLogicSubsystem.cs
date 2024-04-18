using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map.Logic;
namespace Game.Script.Subsystem
{
    public class MapLogicSubsystem : GameSubsystem
    {
        private readonly Dictionary<string, MapLogic> _logics = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
            InitLogic();
        }
        
        void InitLogic()
        {
            var baseType = typeof(MapLogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attribute = type.GetCustomAttribute<MapLogicDesAttribute>();

                    if (null != attribute)
                    {
                        var logic = System.Activator.CreateInstance(type) as MapLogic;
                        _logics.Add(attribute.Des, logic);
                    }
                    
                    
                }
            }
        }

        void OnUpdate(float deltaTime)
        {
            if (Common.Game.Instance.Mode != GameMode.Host && Common.Game.Instance.Mode != GameMode.Home)
                return;
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            if(mapSubsystem.CurMapData == null)
                return;
            
            int num = mapSubsystem.CurMapData.logics.Count;

            if (num > 0)
            {
                Parallel.ForEach(mapSubsystem.CurMapData.logics, loigcName =>
                {
                    if (_logics.TryGetValue(loigcName, out var logic))
                    {
                        logic.Tick(deltaTime);
                    }
                });
            }
            
        }
    }
}