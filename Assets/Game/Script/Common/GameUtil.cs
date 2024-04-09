using UnityEngine;

namespace Game.Script.Common
{
    public static class GameUtil
    {
        public static Vector3 ConvertPointToWorldPosition((int, int) p, Vector3 offset, float cellX, float cellZ)
        {
            Vector3 ret = offset;
            
            ret.x += cellX * 0.5f;
            ret.z += cellZ * 0.5f;

            ret.x += p.Item1 * cellX;
            ret.z += p.Item2 * cellZ;
            
            
            return ret;
        }
    }
}