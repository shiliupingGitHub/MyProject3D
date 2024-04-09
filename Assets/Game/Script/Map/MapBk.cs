using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Game.Script.Attribute;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine.Serialization;

namespace Game.Script.Map
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapBk : MonoBehaviour
    {
        [Label("X方向数量")] public int xGridNum = 35;
        [Label("Z方向数量")] public int zGridNum = 35;
        [Label("x方向格子大小")] public float xGridSize = 1;
        [Label("Z方向格子大小")] public float zGridSize = 1;
        public List<int> blocks = new();
        
        (int, int) GetGrid(Vector3 worldPosition)
        {
            Vector3 relative = worldPosition - Offset;

            int gridX = Mathf.FloorToInt(relative.x / xGridSize);
            int gridY = Mathf.FloorToInt(relative.z / zGridSize);

            return (gridX, gridY);
        }

        public Vector3 GetPosition(int x, int z)
        {
            Vector3 ret = transform.position;

            ret += new Vector3(x * xGridSize, 0, z * zGridSize);

            return ret;
        }

        public Vector3 ConvertToGridPosition(Vector3 worldPosition)
        {
            if (worldPosition.x < 0 || worldPosition.z <= 0)
            {
                return Vector3.zero;
            }

            var ( gridX,  gridZ) = GetGrid(worldPosition);
            Vector3 ret = transform.position;

            ret += new Vector3(gridX * xGridSize, 0, gridZ * zGridSize);

            return ret;
        }

        private void Awake()
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            mapSubsystem.MapBk = this;
        }

        void AddBlock(ushort x, ushort z)
        {
            int data = x << 16;
            data |= z;

            if (blocks.Contains(data))
            {
                return;
            }
            blocks.Add(data);
        }

        public (int, int) BlockToGrid(int data)
        {
            int retX = 0;
            int retZ = 0;
            
            retX = data >> 16;
            retZ = data & 0xFFFF;
            
            return (retX, retZ);
        }
        

        [ContextMenu("生成阻挡")]
        public void GenerateObstacle()
        {
            blocks.Clear();
            var pathBlocks = transform.GetComponentsInChildren<PathBlock>();
            foreach (var pathBlock in pathBlocks)
            {
                var colliders = pathBlock.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    var min = collider.bounds.min;
                    var max = collider.bounds.max;
                    
                    int xStep = Mathf.CeilToInt((max.x - min.x) / xGridSize);
                    int zStep = Mathf.CeilToInt((max.z - min.z) / zGridSize);
                    var (x, z) = GetGrid(min);
                    for (int i = 0; i < xStep; i++)
                    {
                        for (int j = 0; j < zStep; j++)
                        {
                           AddBlock( (ushort)(x + i), (ushort)(z+j));
                        }
                    }
                    AddBlock((ushort)x, (ushort)z);
                }
            }
            
        }


        public Vector3 Offset => transform.position;

        public (int, int) GetGridIndex(Vector3 worldPos)
        {
            int retX = -1;
            int retZ = -1;

            Vector3 o = transform.position;

            var offset = (worldPos - o);

            offset.x /= xGridSize;
            offset.z /= zGridSize;

            if (offset.x >= 0 && offset.x < xGridNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }

            if (offset.z >= 0 && offset.z < zGridNum)
            {
                retZ = Mathf.FloorToInt(offset.z);
            }

            retX = Mathf.Clamp(retX, 0, xGridNum - 1);
            retZ = Mathf.Clamp(retZ, 0, zGridNum - 1);

            return (retX, retZ);
        }
    }
}