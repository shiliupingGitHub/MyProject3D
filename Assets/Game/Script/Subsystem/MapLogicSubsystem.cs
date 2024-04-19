﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map.Logic;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class MapLogicSubsystem : GameSubsystem
    {
        private readonly Dictionary<string, MapLogic> _logics = new();
        private Dictionary<string, Type> _logicTypes = new();
        public List<string> AllLogicNames { get; set; } = new();

        private bool _bEnter;
        private List<MapLogic> _workLogics = new();

        public MapLogic Create(string name)
        {
            if (_logicTypes.TryGetValue(name, out var type))
            {
                var logic = (MapLogic)Activator.CreateInstance(type);
                return logic;
            }
            
            return null;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
            InitLogic();
            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            gameEventSubsystem.Subscribe("AllMapLoaded", OnAllMapLoaded);
            gameEventSubsystem.Subscribe("LeaveLevel", OnLeaveLevel);
            gameEventSubsystem.Subscribe("AllMapUnLoaded", OnAllMapUnload);
        }

        void OnAllMapLoaded(System.Object _)
        {
            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
            if(mapSubsystem.CurMapData == null)
                return;
            foreach (var logicData in mapSubsystem.CurMapData.logics)
            {
                if (_logics.TryGetValue(logicData.Name, out var logic))
                {
                    if (!_workLogics.Contains(logic))
                    {
                        _workLogics.Add(logic);
                        JsonUtility.FromJsonOverwrite(logicData.Data, logic);
                        logic.Enter();
                    }
                    
                }
              
            }
            
        }
        void OnLeaveLevel(System.Object o)
        {
            LevelType lt = o is LevelType ? (LevelType)o : LevelType.None;

            if (lt == LevelType.Fight || lt == LevelType.Home)
            {
                Exit();
            }
        }

        void OnAllMapUnload(System.Object o)
        {
            Exit();
        }

        void Exit()
        {
            foreach (var logic in _workLogics)
            {
                logic.Exit();
            }
            _workLogics.Clear();
        }
        
        void InitLogic()
        {
            var baseType = typeof(MapLogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attribute = type.GetCustomAttribute<MapLogicDesAttribute>();

                    if (null != attribute)
                    {
                        var logic = System.Activator.CreateInstance(type) as MapLogic;
                        _logics.Add(attribute.Des, logic);
                        AllLogicNames.Add(attribute.Des);
                        _logicTypes.Add(attribute.Des, type);
                    }
                }
            }
        }

        void OnUpdate(float deltaTime)
        {
            if (_workLogics.Count > 0)
            {
                Parallel.ForEach(_workLogics, logic =>
                {
                    logic.Tick(deltaTime);
                });
            }
            
        }
    }
}