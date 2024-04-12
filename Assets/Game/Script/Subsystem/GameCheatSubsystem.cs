using System;
using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class GameCheatSubsystem : GameSubsystem
    {
        private Cheat.Cheat _cheat = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
        }

        public void Execute(string str)
        {
            DoExecute(str);
        }
        void DoExecute(string str)
        {
            var args = ConvertToArgs(str);

            if (args.Count == 0)
            {
                return;
            }

            var method = typeof(Cheat.Cheat).GetMethod(args[0]);

            if (method == null)
            {
                return;
            }

            var attrs = method.GetCustomAttributes(typeof(CheatServerOnlyAttribute), true);

            if (attrs.Length > 0)
            {
                switch (Common.Game.Instance.Mode)
                {
                    case GameMode.Client:
                    {
                        if (Common.Game.Instance.MyController != null)
                        {
                            Common.Game.Instance.MyController.Cmd_ExecuteCheat(str);
                        }
                    }
                    return;
                }
            }

            var parameters = method.GetParameters();

            if (parameters.Length > args.Count - 1)
            {
                return;
            }

            int index = 1;
            List<object> parameterObjects = new();
            foreach (var parameter in parameters)
            {
                var arg = GameUtil.ConvertStringToType(args[index], parameter.ParameterType);
                parameterObjects.Add(arg);
                index++;
            }

            method.Invoke(_cheat, parameterObjects.ToArray());
        }


        List<string> ConvertToArgs(string str)
        {
            List<string> ret = new();
            var args = str.Split(' ');

            foreach (var arg in args)
            {
                var val = arg.Trim();
                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                ret.Add(val);
            }

            return ret;
        }
        
        void OnUpdate(float deltaTime)
        {
            if (!UIManager.Instance.IsInit)
            {
                return;
            }

            if (UIManager.Instance.UIEventSystem.currentSelectedGameObject != null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote))
            {
                var frame = UIManager.Instance.Get<GameCheatFrame>();

                if (null == frame || !frame.IsVisible)
                {
                    UIManager.Instance.Show<GameCheatFrame>();
                }
                else
                {
                    frame.Hide();
                }
            }
        }
    }
}