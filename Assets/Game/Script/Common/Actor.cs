using System;
using System.Collections.Generic;
using Game.Script.Map;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;


namespace Game.Script.Common
{
    public enum ActorType
    {
        Normal,
        Shadow,
        Preview,
    }

    public class Actor : NetworkBehaviour
    {
        public Transform CacheTransform { get; set; }
        protected virtual bool IsBlock => false;
        private int _gridIndex = -1;
        private readonly List<(int, int)> _nowGrid = new();
        private readonly List<(int, int)> _tempGrid = new();
        protected System.Action positionChanged;
        public Vector3 centerOffset = new Vector3(0.5f, 0.5f, 0);
        public virtual Vector3 CenterOffset => centerOffset;

        public ActorType ActorType { get; set; } = ActorType.Normal;
        private int xStep = 0;
        private int zStep = 0;
        private Collider _collider;
        public virtual bool IsAddToSearch => false;

        protected virtual void Start()
        {
            CalculateStep();
            UpdateGrid();
            positionChanged += UpdateGrid;
        }

        void CalculateStep()
        {
            _collider = GetComponent<Collider>();
            var mapSubsystem = Game.Instance.GetSubsystem<MapSubsystem>();
            var mapBk = mapSubsystem.MapBk;

            if (null == mapBk)
                return;

            if (null == _collider)
                return;


            var gridStepInfo = GameUtil.CalculateGridStep(_collider, mapBk);

            xStep = gridStepInfo.xStep;
            zStep = gridStepInfo.zStep;
        }

        protected virtual void Awake()
        {
            CacheTransform = transform;

            if (IsAddToSearch)
            {
                var actorSearchSubsystem = Common.Game.Instance.GetSubsystem<ActorSearchSubsystem>();
                actorSearchSubsystem.Add(this);
            }
        }

        protected virtual void OnDestroy()
        {
            LeaveAllGrid();
            if (IsAddToSearch)
            {
                var actorSearchSubsystem = Common.Game.Instance.GetSubsystem<ActorSearchSubsystem>();
                actorSearchSubsystem.Remove(this);
            }
        }

        void LeaveAllGrid()
        {
            var mapSubsystem = Game.Instance.GetSubsystem<MapSubsystem>();
            foreach (var grid in _nowGrid)
            {
                var mapGrid = mapSubsystem.GetGrid(grid.Item1, grid.Item2);

                if (mapGrid != null)
                {
                    mapGrid.Leave(this, IsBlock);
                }
            }

            _nowGrid.Clear();
        }

        protected virtual void UpdateGrid()
        {
            var mapSubsystem = Game.Instance.GetSubsystem<MapSubsystem>();
            var position = CacheTransform.position;
            var (gridIndex, x, y) = mapSubsystem.CreateGridIndex(position);

            if (gridIndex != _gridIndex)
            {
                _gridIndex = gridIndex;

                if (_gridIndex >= 0)
                {
                    _tempGrid.Clear();

                    if (_collider != null)
                    {
                        for (int i = 0; i < xStep; i++)
                        {
                            for (int j = 0; j < zStep; j++)
                            {
                                int gridX = x + i;
                                int gridY = y + j;

                                if (!_tempGrid.Contains(((gridX, gridY))))
                                {
                                    _tempGrid.Add((gridX, gridY));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!_tempGrid.Contains(((x, y))))
                        {
                            _tempGrid.Add((x, y));
                        }
                    }

                    foreach (var grid in _tempGrid)
                    {
                        if (!_nowGrid.Contains(grid))
                        {
                            var mapGrid = mapSubsystem.GetGrid(grid.Item1, grid.Item2, true);
                            mapGrid.Enter(this, IsBlock);
                        }
                    }

                    foreach (var grid in _nowGrid)
                    {
                        if (!_tempGrid.Contains(grid))
                        {
                            var mapGrid = mapSubsystem.GetGrid(grid.Item1, grid.Item2);

                            if (mapGrid != null)
                            {
                                mapGrid.Leave(this, IsBlock);
                            }
                        }
                    }

                    _nowGrid.Clear();
                    _tempGrid.CopyTo(_nowGrid);
                }
                else
                {
                    LeaveAllGrid();
                }
            }
        }
    }
}