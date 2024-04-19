using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Setting;

namespace Game.Script.Cheat
{
    public class Cheat
    {

        public void Test(int k)
        {
          
        }

        public void ShowFps()
        {
            GameSetting.Instance.ShowFps = true;
        }
        [CheatServerOnly]
        public void TestHost(float k)
        {
           
            
        }
    }
}