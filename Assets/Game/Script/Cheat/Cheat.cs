using Game.Script.Attribute;
using Game.Script.Common;

namespace Game.Script.Cheat
{
    public class Cheat
    {

        public void Test(int k)
        {
          
        }
        [CheatServerOnly]
        public void TestHost(float k)
        {
            
        }
    }
}