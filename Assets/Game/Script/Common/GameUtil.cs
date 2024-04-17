using System;
using Game.Script.Map;
using UnityEngine;

namespace Game.Script.Common
{
    public static class GameUtil
    {
        public static (int x,  int z, int xStep, int zStep) CalculateGridStep(Collider collider, MapBk bk)
        {
            var min = collider.bounds.min;
            var max = collider.bounds.max;

            min -= bk.Offset;
            max -= bk.Offset;


            int xStep = Mathf.CeilToInt((max.x - min.x) / bk.xGridSize);
            int zStep = Mathf.CeilToInt((max.z - min.z) / bk.zGridSize);
            var (x, z) = bk.GetGrid(min);

            return (x, z, xStep, zStep);
        }
        public  static (int , int ) IndexSplit(uint areaIndex)
        {
            uint x = areaIndex >> 16;
            uint z = areaIndex & 0xffff;
            return ((int)x, (int)z);
        }
        public static uint CombineToIndex(uint x, uint z)
        {
            x = x << 16;

            uint ret = x | z;

            return ret;
        }
        public static Vector3 ConvertPointToWorldPosition((int, int) p, Vector3 offset, float cellX, float cellZ)
        {
            Vector3 ret = offset;
            
            ret.x += cellX * 0.5f;
            ret.z += cellZ * 0.5f;

            ret.x += p.Item1 * cellX;
            ret.z += p.Item2 * cellZ;
            
            
            return ret;
        }
        public static object ConvertStringToType(string value, Type targetType)
        {
            // 对于值类型，需要考虑转换失败的情况
            if (targetType.IsValueType)
            {
                // 尝试将字符串转换为目标类型
                try
                {
                    // 对于整数类型
                    if (targetType == typeof(int))
                    {
                        return Convert.ToInt32(value);
                    }

                    // 对于浮点数类型
                    if (targetType == typeof(double))
                    {
                        return Convert.ToDouble(value);
                    }

                    // 对于布尔类型
                    if (targetType == typeof(bool))
                    {
                        return Convert.ToBoolean(value);
                    }

                    if (targetType == typeof(string))
                    {
                        return value;
                    }

                    if (targetType == typeof(float))
                    {
                        return Convert.ToSingle(value);
                    }
                    // 对于其他值类型可以继续添加
                }
                catch
                {
                    // 转换失败，可以抛出异常或返回默认值
                    return Activator.CreateInstance(targetType);
                }
            }

            // 对于引用类型，直接转换（通常不会成功，因为会转换为对应的字符串）
            return Convert.ChangeType(value, targetType);
        }
    }
}