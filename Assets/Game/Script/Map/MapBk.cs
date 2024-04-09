using Cinemachine;
using UnityEngine;
using Game.Script.Attribute;
using Game.Script.Subsystem;
using Mirror;

namespace Game.Script.Map
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapBk : MonoBehaviour
    {
        [Label("X方向数量")] public int xGridNum = 35;
        [Label("Z方向数量")] public int zGridNum = 35;
        [Label("x方向格子大小")] public float xGridSize = 1;
        [Label("Z方向格子大小")] public float zGridSize = 1;

        public CinemachineVirtualCamera virtualCamera;
        public CinemachineBrain brain;
        
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
            if (worldPosition.x < 0 || worldPosition.y <= 0)
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


        public Vector3 Offset => transform.position;

        public (int, int) GetGridIndex(Vector3 worldPos)
        {
            int retX = -1;
            int retY = -1;

            Vector3 o = transform.position;

            var offset = (worldPos - o);

            offset.x /= xGridSize;
            offset.z /= zGridSize;

            if (offset.x >= 0 && offset.x < xGridNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }

            if (offset.y >= 0 && offset.y < zGridNum)
            {
                retY = Mathf.FloorToInt(offset.y);
            }

            retX = Mathf.Clamp(retX, 0, xGridNum - 1);
            retY = Mathf.Clamp(retY, 0, zGridNum - 1);

            return (retX, retY);
        }
    }
}