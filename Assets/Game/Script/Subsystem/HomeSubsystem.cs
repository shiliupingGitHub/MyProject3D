using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Home.Actor;

namespace Game.Script.Subsystem
{
    public class HomeSubsystem : GameSubsystem
    {
        private List<HomeActor> _homeActors = new();
        const string _homeMap = "map_test_1";

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.AddQuit(SaveAchieve);
        }

        public void ShowHome()
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            mapSubsystem.LoadMap(_homeMap, false);
        }

        public void LoadAchieve()
        {
            
        }

        public void SaveAchieve()
        {
            
        }
    }
}