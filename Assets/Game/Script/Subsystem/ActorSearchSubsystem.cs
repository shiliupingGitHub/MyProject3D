using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Character;
using Game.Script.Common;
using Game.Script.Map;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class ActorSearchSubsystem : GameSubsystem
    {
        class Area
        {
            private const float LeafSize = 5;
            float Width { get; set; }
            float Height { get; set; }
            Vector3 Center { get; set; }

            private List<Area> Children { get; set; } = new();

            List<Pawn> _pawns = new();

            bool IsInside(Vector3 position)
            {
                return position.x >= Center.x - Width / 2.0f && position.x <= Center.x + Width / 2.0f && position.z >= Center.z - Height / 2 && position.z <= Center.z + Height / 2;
            }

            public void Clear()
            {
                _pawns.Clear();
            }

            public void Create(Vector3 center, float width, float height,  List<Area> leafQueue)
            {
                Width = width;
                Height = height;
                Center = center;

                if (Width > Height && Width > LeafSize)
                {
                    float childWidth = Width * 0.5f;
                    float childHeight = height;
                    var centerOffset = new Vector3(childWidth * 0.5f, 0, 0);
                    var leftCenter = center - centerOffset;
                    var rightCenter = center + centerOffset;
                    var leftArea = new Area();
                    var rightArea = new Area();
                    Children.Add(leftArea);
                    Children.Add(rightArea);
                    leftArea.Create(leftCenter, childWidth, childHeight, leafQueue);
                    rightArea.Create(rightCenter, childWidth, childHeight, leafQueue);
                }
                else if (Height > LeafSize)
                {
                    float childWidth = Width;
                    float childHeight = height * 0.5f;
                    var centerOffset = new Vector3(0, 0, childHeight * 0.5f);
                    var downCenter = center - centerOffset;
                    var upCenter = center + centerOffset;
                    var downArea = new Area();
                    var upArea = new Area();
                    Children.Add(downArea);
                    Children.Add(downArea);
                    downArea.Create(downCenter, childWidth, childHeight, leafQueue);
                    upArea.Create(upCenter, childWidth, childHeight, leafQueue);
                }
                else
                {
                    leafQueue.Add(this);
                }
            }

            public void Enter(Pawn pawn)
            {
                if (Children.Count == 0)
                {
                    lock (this)
                    {
                        _pawns.Add(pawn);
                    }
                }
                else
                {
                    foreach (var child in Children)
                    {
                        if (child.IsInside(pawn.Position))
                        {
                            child.Enter(pawn);
                            break;
                        }
                    }
                }
            }
        }


        readonly List<Pawn> _pawns = new();
        Area _root;
        private readonly List<Area> _leafs = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            gameEventSubsystem.Subscribe("AllMapLoaded", OnAllMapLoaded);
            gameEventSubsystem.Subscribe("LeaveLevel", OnLeaveLevel);
            gameEventSubsystem.Subscribe("AllMapUnLoaded", OnAllMapUnLoaded);
        }

        void OnAllMapLoaded(System.Object o)
        {
            GameLoop.Add(OnUpdate);
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            if (mapSubsystem.MapBk)
            {
                CreateTree(mapSubsystem.MapBk);
            }
            
        }

        void OnAllMapUnLoaded(System.Object o)
        {
            Clear();
        }

        void OnLeaveLevel(System.Object o)
        {
            LevelType lt = o is LevelType ? (LevelType)o : LevelType.None;

            if (lt == LevelType.Fight || lt == LevelType.Home)
            {
                Clear();
            }
        }

        public void Search(Vector3 position, ref List<Pawn> results)
        {
            
        }

        void CreateTree(MapBk mapBk)
        {
            _root = new Area();
            var center = mapBk.Offset;
            float width = mapBk.xGridNum * mapBk.xGridSize + 1;
            float height = mapBk.zGridNum * mapBk.zGridSize + 1;
            center.x += width * 0.5f;
            center.z += height * 0.5f;
            _root.Create(center, width, height, _leafs);
        }

        void Clear()
        {
            GameLoop.Remove(OnUpdate);
            _root = null;
            _leafs.Clear();
        }

        void OnUpdate(float deltaTime)
        {
            BuildTree();
        }

        void BuildTree()
        {
            if (_leafs.Count > 0 && _root != null)
            {
                Parallel.ForEach(_leafs, leaf => { leaf.Clear(); });
                Parallel.ForEach(_pawns, pawn => { _root.Enter(pawn); });
            }
        }

        public void Add(Pawn pawn)
        {
            _pawns.Add(pawn);
        }

        public void Remove(Pawn pawn)
        {
            _pawns.Remove(pawn);
        }
    }
}