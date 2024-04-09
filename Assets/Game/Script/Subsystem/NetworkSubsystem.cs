using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class NetworkSubsystem : GameSubsystem
    {
        private const string KcpFightNetMgrPath = "Assets/Game/Res/Net/KcpFightNetworkManager.prefab";
        private const string KcpHomeNetMgrPath = "Assets/Game/Res/Net/KcpHomeNetworkManager.prefab";
        private GameObject _networkMgrGo;

        public void LoadNetWorkManager(GameMode mode = GameMode.Host)
        {
            if (_networkMgrGo != null)
            {
                Object.Destroy(_networkMgrGo);
            }

            GameObject template = null;
            switch (mode)
            {
                case GameMode.Home:
                    template = GameResMgr.Instance.LoadAssetSync<GameObject>(KcpHomeNetMgrPath);
                    break;
                default:
                {
                    template = GameResMgr.Instance.LoadAssetSync<GameObject>(KcpFightNetMgrPath);
                }
                    break;
            }

            _networkMgrGo = Object.Instantiate(template);
            _networkMgrGo.name = "NetworkMgr";
            Object.DontDestroyOnLoad(_networkMgrGo);
        }
    }
}