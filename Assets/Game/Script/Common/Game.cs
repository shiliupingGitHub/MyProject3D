using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Game.Script.Character;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.Subsystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Common
{
    public enum GameMode
    {
        Hall,
        Host,
        Edit,
        Client,
        Home,
    }

    public class Game : SingletonWithOnInstance<Game>
    {

        public System.Func<GameObject> serverFightNewPlayer;
        public GameMode Mode { set; get; } = GameMode.Host;

        public string FightMap { get; set; }
        public List<FightCharacter> Fights { get; } = new();
        private readonly Dictionary<System.Type, GameSubsystem> _subsystems = new();
        private readonly List<Pawn> _pawns = new();
        private FightCharacter _myController;
        private MapBk _mapBk;
        private const long TickMaxTime = 5;
        public void RegisterPawn(Pawn pawn)
        {
            if (!_pawns.Contains(pawn))
            {
                _pawns.Add(pawn);
                if (pawn is FightCharacter character)
                {
                    Fights.Add(character);
                }
            }
        }




        public FightCharacter MyController
        {
            set
            {
                _myController = value;
                
                var eventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                eventSubsystem.Raise("localPlayerLoad", _myController);
            }
            get => _myController;
        }


        async void DoTick()
        {
            while (true)
            {
                Stopwatch stopwatch = new Stopwatch();
                long executeMilliseconds = 0;
                int curIndex = 0;
                while (curIndex < _pawns.Count)
                {
                   
                    stopwatch.Restart();
                    var pawn = _pawns[curIndex];
                    curIndex++;
                    float curTime = Time.unscaledTime;
                    float delta = curTime - pawn.LastTickTime;
                    pawn.Tick(delta);
                    pawn.LastTickTime = curTime;
                    
                    stopwatch.Stop();
                    
                    executeMilliseconds += (int)stopwatch.ElapsedMilliseconds;
                    
                    if(executeMilliseconds > TickMaxTime)
                    {
                        executeMilliseconds = 0;
                        await TimerSubsystem.Delay(1);
                    };

                }
                
                await TimerSubsystem.Delay(1);
            }
        }

        public void UnRegisterPawn(Pawn pawn)
        {
            _pawns.Remove(pawn);
            if (pawn is FightCharacter character)
            {
                Fights.Remove(character);
            }
        }

        public T GetSubsystem<T>() where T : GameSubsystem
        {
            var type = typeof(T);
            _subsystems.TryGetValue(type, out var ret);
            return ret as T;
        }

        public override void OnInstance()
        {
            base.OnInstance();
            var baseType = typeof(GameSubsystem);
            var assem = baseType.Assembly;
            foreach (var type in assem.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && type != baseType)
                {
                    if (System.Activator.CreateInstance(type) is GameSubsystem subsystem)
                    {
                        _subsystems.Add(type, subsystem);
                    }
                }
            }

            foreach (var subsystem in _subsystems)
            {
                subsystem.Value.OnInitialize();
            }

            DoTick();
        }
    }
}