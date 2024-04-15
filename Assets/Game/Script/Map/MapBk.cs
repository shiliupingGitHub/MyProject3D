using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace Game.Script.Map
{
    [ExecuteAlways]
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapBk : MonoBehaviour
    {
        [Label("X方向数量")] public int xGridNum = 35;
        [Label("Z方向数量")] public int zGridNum = 35;
        [Label("x方向格子大小")] public float xGridSize = 1;
        [Label("Z方向格子大小")] public float zGridSize = 1;
        public List<int> blocks = new();

        public (int, int) GetGrid(Vector3 worldPosition)
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

            var (gridX, gridZ) = GetGrid(worldPosition);
            Vector3 ret = transform.position;

            ret += new Vector3(gridX * xGridSize, 0, gridZ * zGridSize);

            return ret;
        }

        private void Awake()
        {
            MapBkManager.Instance.Add(this);
            if (Application.isPlaying)
            {
                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                mapSubsystem.MapBk = this;
            }
         
        }
        
        private void OnDestroy()
        {
            MapBkManager.Instance.Remove(this);
        }

        void AddBlock(ushort x, ushort z)
        {
            if (x >= xGridNum)
            {
                return;
            }
            if(z >= zGridNum)
            {
                return;
            }
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


        [Button("生成阻挡")]
        public void GenerateObstacle()
        {
            blocks.Clear();

            var colliders = GetComponentsInChildren<Collider>();
            foreach (var childCollider in colliders)
            {
                if (childCollider.gameObject.layer != LayerMask.NameToLayer("Default"))
                    continue;
                
                if(childCollider.isTrigger)
                    continue;
                var (x, z, xStep, zStep) = GameUtil.CalculateGridStep(childCollider, this);
               
                for (int i = 0; i < xStep; i++)
                {
                    for (int j = 0; j < zStep; j++)
                    {
                        AddBlock((ushort)(x + i), (ushort)(z + j));
                    }
                }
                AddBlock((ushort)x, (ushort)z);
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