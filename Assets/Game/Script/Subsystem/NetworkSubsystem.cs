using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Subsystem
{
    public class NetworkSubsystem : GameSubsystem
    {
        private const string KcpNetMgrPath = "Assets/Game/Res/Net/KcpFightNetworkManager.prefab";
        private GameObject _networkMgrGo;
        public void LoadNetWorkManager()
        {
            if (_networkMgrGo != null)
            {
                Object.Destroy(_networkMgrGo);
            }
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(KcpNetMgrPath);
            _networkMgrGo = Object.Instantiate(template);
            _networkMgrGo.name = "NetworkMgr";
            Object.DontDestroyOnLoad(_networkMgrGo);
        }
    }
}