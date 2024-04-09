
using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Actor;

using Game.Script.UI.Ext;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class ActorDataEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/ActorDataEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/params")] private Transform _paramsRoot;
        private ActorData _curActorData;
        
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
        }

        public ActorData CurActorData
        {
            set
            {
                _curActorData = value;
                RefreshActorUI();
            }
        }
    

        void RefreshActorUI()
        {
            
            if (_curActorData.go == null)
            {
                return;
            }

            var mapActor = _curActorData.go.GetComponent<MapActor>();

            if (null == mapActor)
            {
                return;
            }
            FieldDrawer.BeginDraw(_paramsRoot);
            FieldDrawer.Draw(_paramsRoot, mapActor, (info, o) =>
            {
                _curActorData.Set(info.Name, o);
            }, typeof(ActorDataDesAttribute));
            
        }
    }
}