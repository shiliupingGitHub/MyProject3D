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
        private int _areaIndex = -1;
        private readonly List<(int, int)> _nowArea = new();
        private readonly List<(int, int)> _tempArea = new();
        protected System.Action positionChanged;
        public Vector3 centerOffset = new Vector3(0.5f, 0.5f, 0);
        public virtual Vector3 CenterOffset => centerOffset;

        public ActorType ActorType { get; set; } = ActorType.Normal;
        private int xStep = 0;
        private int zStep = 0;
        private Collider _collider;

        protected virtual void Start()
        {
            CalculateStep();
            UpdateArea();
            positionChanged += UpdateArea;
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
        }

        protected virtual void OnDestroy()
        {
            LeaveAllArea();
        }

        void LeaveAllArea()
        {
            var mapSubsystem = Game.Instance.GetSubsystem<MapSubsystem>();
            foreach (var area in _nowArea)
            {
                var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2);

                if (mapArea != null)
                {
                    mapArea.Leave(this, IsBlock);
                }
            }

            _nowArea.Clear();
        }

        protected virtual void UpdateArea()
        {
            var mapSubsystem = Game.Instance.GetSubsystem<MapSubsystem>();
            var position = CacheTransform.position;
            var (nowAreaIndex, x, y) = mapSubsystem.CreateAreaIndex(position);

            if (nowAreaIndex != _areaIndex)
            {
                _areaIndex = nowAreaIndex;

                if (_areaIndex >= 0)
                {
                    _tempArea.Clear();

                    if (_collider != null)
                    {
                        for (int i = 0; i < xStep; i++)
                        {
                            for (int j = 0; j < zStep; j++)
                            {
                                int gridX = x + i;
                                int gridY = y + j;

                                if (!_tempArea.Contains(((gridX, gridY))))
                                {
                                    _tempArea.Add((gridX, gridY));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!_tempArea.Contains(((x, y))))
                        {
                            _tempArea.Add((x, y));
                        }
                    }

                    foreach (var area in _tempArea)
                    {
                        if (!_nowArea.Contains(area))
                        {
                            var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2, true);
                            mapArea.Enter(this, IsBlock);
                        }
                    }

                    foreach (var area in _nowArea)
                    {
                        if (!_tempArea.Contains(area))
                        {
                            var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2);

                            if (mapArea != null)
                            {
                                mapArea.Leave(this, IsBlock);
                            }
                        }
                    }

                    _nowArea.Clear();
                    _tempArea.CopyTo(_nowArea);
                }
                else
                {
                    LeaveAllArea();
                }
            }
        }
    }
}