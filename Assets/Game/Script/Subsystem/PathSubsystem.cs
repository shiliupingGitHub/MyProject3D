using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Async;
using Game.Script.Common;
using Game.Script.Map;
using Priority_Queue;
using UnityEngine;

namespace Game.Script.Subsystem
{
    struct PathRequest
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public ulong pathId;
        public  GameTaskCompletionSource<List<Vector3>>  tls;
    }

    public class PathSubsystem : GameSubsystem
    {
        private readonly List<PathRequest> _pathRequestList = new();
        private ulong _pathId = 1;
        private const int PathNumPerFrame = 20;
        public override void OnInitialize()
        {
            base.OnInitialize();

            var levelSubsystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
            levelSubsystem.preLevelChange += (_, _) =>
            {
                foreach (var request in _pathRequestList)
                {
                    if (request.tls != null)
                    {
                        request.tls.SetResult(null);
                    }
                }
                _pathRequestList.Clear();
                _pathId = 1;
            };
            DoTick();
        }

        private async void DoTick()
        {
            while (true)
            {
                OnTick();
                await TimerSubsystem.Delay(1);
            }
        }

        public GameTask<List<Vector3>> AddPath(Vector3 start, Vector3 end,  ref ulong pathId)
        {
             pathId = _pathId;
            _pathId++;

            GameTaskCompletionSource<List<Vector3>> curtls = new();
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            (int sX, int sY) = mapSubsystem.MapBk.GetGridIndex(start);
            (int eX, int eY) = mapSubsystem.MapBk.GetGridIndex(end);

            _pathRequestList.Add(new PathRequest() { tls = curtls,startPosition = start, endPosition = end, pathId = pathId , startX = sX, startY = sY, endX = eX, endY = eY});
            return curtls.Task;
        }

        public void RemovePath(ulong pathId)
        {
            var request = _pathRequestList.Find(x => x.pathId == pathId);

            if (request.tls != null)
            {
                request.tls.SetResult(null);
                _pathRequestList.Remove(request);
            }
            
        }
        
        Vector3 ConvertPointToWorldPosition((int, int) p, Vector3 offset, float cellX, float cellZ)
        {
            Vector3 ret = offset;
            
            ret.x += cellX * 0.5f;
            ret.z += cellZ * 0.5f;

            ret.x += p.Item1 * cellX;
            ret.z += p.Item2 * cellZ;
            
            
            return ret;
        }
        void OnTick()
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            if (mapSubsystem.MapBk == null)
            {
                return;
            }

            int num = Mathf.Min(PathNumPerFrame, _pathRequestList.Count);
            var offset = mapSubsystem.MapBk.Offset;
            var cellX =  mapSubsystem.MapBk.xGridSize;
            var cellZ =  mapSubsystem.MapBk.zGridSize;
            if (num > 0)
            {
                Parallel.For(0, num, (i, _) =>
                {
                    var request = _pathRequestList[i];
                    var path = GeneratePath(request.startX, request.startY, request.endX, request.endY);
                    
                    List<Vector3> finalPath = new();

                    if (null != path && path.Count > 0)
                    {
                        finalPath.Add(request.startPosition);
                        foreach (var p in path)
                        {
                            var position = ConvertPointToWorldPosition(p, offset, cellX, cellZ);
                            finalPath.Add(position);
                        }
                        finalPath.Add(request.endPosition);
                    }
                    
                    GameLoop.RunGameThead(() =>
                    {
                        request.tls.SetResult(finalPath);
                    });
                    
                    
                });
                _pathRequestList.RemoveRange(0, num);
            }
        }

        private List<(int, int)> DoPath(Vector3 start, Vector3 end, MapBk mapBk)
        {
            (int startX, int startY) = mapBk.GetGridIndex(start);
            (int endX, int endY) = mapBk.GetGridIndex(end);
            return GeneratePath(startX, startY, endX, endY);
        }

        private static float CalcHeuristicManhattan(int nodeX, int nodeY, int goalX, int goalY)
        {
            return Mathf.Abs(nodeX - goalX) + Mathf.Abs(nodeY - goalY);
        }

        bool IsBlock(int x, int y)
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            if (null != mapSubsystem)
            {
                var area = mapSubsystem.GetArea(x, y);

                return area?.Blocked ?? false;
            }

            return false;

        }

        // Calculates the Euclidean heuristic distance between two nodes
        private static float CalcHeuristicEuclidean(int nodeX, int nodeY, int goalX, int goalY)
        {
            return Mathf.Sqrt(Mathf.Pow(nodeX - goalX, 2) + Mathf.Pow(nodeY - goalY, 2));
        }

        private static List<(int, int)> BacktracePath((int, int)[,] parentMap, int goalX, int goalY)
        {
            List<(int, int)> path = new List<(int, int)>();

            (int, int) current = (goalX, goalY);

            while (current != (-1, -1))
            {
                path.Add(current);
                current = parentMap[current.Item2, current.Item1];
            }

            path.Reverse();
            return path;
        }

        private (bool, (int, int))[] GetNeighbours(int xCordinate, int yCordinate, bool walkableDiagonals = false)
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            List<(bool, (int, int))> neighbourCells = new List<(bool, (int, int))>();

            int heigth = mapSubsystem.MapBk.zGridNum;
            int width = mapSubsystem.MapBk.xGridNum;

            int range = 1;
            int yStart = (int)MathF.Max(0, yCordinate - range);
            int yEnd = (int)MathF.Min(heigth - 1, yCordinate + range);

            int xStart = (int)MathF.Max(0, xCordinate - range);
            int xEnd = (int)MathF.Min(width - 1, xCordinate + range);

            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    if (x == xCordinate && y == yCordinate)
                    {
                        continue;
                    }

                    if (IsBlock(x, y))
                    {
                        continue;
                    }

                    if (!walkableDiagonals)
                    {
                        if ((x == xCordinate - range) && (y == yCordinate - range || y == yCordinate + range))
                        {
                            if (IsBlock(xCordinate, y))
                                continue;
                            if(IsBlock(x, yCordinate))
                                continue;
                        }

                        if ((x == xCordinate + range) && (y == yCordinate - range || y == yCordinate + range))
                        {
                            if (IsBlock(xCordinate, y))
                                continue;
                            if(IsBlock(x, yCordinate))
                                continue;
                        }
                    }

                    neighbourCells.Add((!IsBlock(x, y), (x, y)));
                }
            }

            return neighbourCells.ToArray();
        }


        private List<(int, int)> GeneratePath(int startX, int startY, int goalX, int goalY, bool manhattanHeuristic = true, bool walkableDiagonals = false)
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            // Set the heuristic function to use based on the manhattanHeuristic parameter
            Func<int, int, int, int, float> heuristic = manhattanHeuristic ? CalcHeuristicManhattan : CalcHeuristicEuclidean;

            // Get the dimensions of the map
            int mapHeight = mapSubsystem.MapBk.zGridNum;
            int mapWidth = mapSubsystem.MapBk.xGridNum;

            // Define constants and arrays needed for the algorithm
            float sqrt2 = Mathf.Sqrt(2);
            float[,] gCostMap = new float[mapHeight, mapWidth];
            float[,] fCostMap = new float[mapHeight, mapWidth];
            (int, int)[,] parentMap = new (int, int)[mapHeight, mapWidth];

            // Initialize the open set with the starting node
            SimplePriorityQueue<(int, int)> openSet = new SimplePriorityQueue<(int, int)>();
            gCostMap[startY, startX] = 0.0000001f;
            fCostMap[startY, startX] = heuristic(startX, startY, goalX, goalY);
            parentMap[startY, startX] = (-1, -1);
            openSet.Enqueue((startX, startY), fCostMap[startY, startX]);

            // Start the A* algorithm
            while (openSet.Count > 0)
            {
                // Get the node with the lowest f cost from the open set
                (int, int) current = openSet.First;
                int currentX = current.Item1;
                int currentY = current.Item2;

                // If we have reached the goal node, backtrack to get the path
                if (current.Item1 == goalX && current.Item2 == goalY)
                {
                    var path = BacktracePath(parentMap, goalX, goalY);
                    return path;
                }

                // Remove the current node from the open set
                openSet.Dequeue();

                // Get the neighbours of the current node
                (bool, (int, int))[] neighbours = GetNeighbours(currentX, currentY, walkableDiagonals);

                // Process each neighbour
                for (int i = 0; i < neighbours.Length; i++)
                {
                    (bool, (int, int)) neighbour = neighbours[i];

                    // Get the x and z coordinates of the neighbour
                    int neighbourX = neighbour.Item2.Item1;
                    int neighbourY = neighbour.Item2.Item2;

                    // Calculate the tentative g cost of the neighbour
                    float dist = currentX - neighbourX == 0 || currentY - neighbourY == 0 ? 1 : sqrt2;
                    float tentativeGCost = gCostMap[currentY, currentX] + dist;

                    // If the neighbour has not been visited yet, or the tentative g cost is lower than its current g cost,
                    // update its g, f and parent values and add it to the open set
                    if (gCostMap[neighbourY, neighbourX] == 0 || tentativeGCost < gCostMap[neighbourY, neighbourX])
                    {
                        parentMap[neighbourY, neighbourX] = current;
                        gCostMap[neighbourY, neighbourX] = tentativeGCost;
                        fCostMap[neighbourY, neighbourX] = tentativeGCost + heuristic(neighbourX, neighbourY, goalX, goalY);

                        // If the neighbor node is not in the open set, add it
                        if (!openSet.Contains(neighbour.Item2))
                        {
                            openSet.Enqueue(neighbour.Item2, fCostMap[neighbourY, neighbourX]);
                        }
                    }
                }
            }

            return null;
        }
    }
}