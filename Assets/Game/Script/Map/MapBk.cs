using Cinemachine;
using UnityEngine;
using Game.Script.Attribute;
using Game.Script.Subsystem;
using Mirror;

namespace Game.Script.Map
{
    [RequireComponent(typeof(Grid))]
    [RequireComponent(typeof(NetworkIdentity))]
    public class MapBk : MonoBehaviour
    {
        [Label("X方向数量")] public int xGridNum = 100;

        [Label("Y方向数量")] public int yGridNum = 100;

        public CinemachineVirtualCamera virtualCamera;
        public Transform blockTilesRoot;

        private Grid _grid;

        public Grid MyGrid
        {
            get
            {
                if (null == _grid)
                {
                    _grid = GetComponent<Grid>();
                }

                return _grid;
            }
        }

        (int, int) GetGrid(Vector3 worldPosition)
        {
            Vector3 relative = worldPosition - Offset;

            int gridX = Mathf.FloorToInt(relative.x / MyGrid.cellSize.x);
            int gridY = Mathf.FloorToInt(relative.y / MyGrid.cellSize.y);

            return (gridX, gridY);
        }

        public Vector3 GetPosition(int x, int y)
        {
            Vector3 ret = transform.position;

            ret += new Vector3(x * MyGrid.cellSize.x, y * MyGrid.cellSize.y, 0);

            return ret;
        }

        public Vector3 ConvertToGridPosition(Vector3 worldPosition)
        {
            if (worldPosition.x < 0 || worldPosition.y <= 0)
            {
                return Vector3.zero;
            }

            (var gridX, var gridY) = GetGrid(worldPosition);
            Vector3 ret = transform.position;

            ret += new Vector3(gridX * MyGrid.cellSize.x, gridY * MyGrid.cellSize.y, 0);

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
            var cellSize = MyGrid.cellSize;

            var offset = (worldPos - o);

            offset.x /= cellSize.x;
            offset.y /= cellSize.y;

            if (offset.x >= 0 && offset.x < xGridNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }

            if (offset.y >= 0 && offset.y < yGridNum)
            {
                retY = Mathf.FloorToInt(offset.y);
            }

            retX = Mathf.Clamp(retX, 0, xGridNum - 1);
            retY = Mathf.Clamp(retY, 0, yGridNum - 1);

            return (retX, retY);
        }
    }
}