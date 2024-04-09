using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Script.Map.Actor
{
    public class MapActor : Common.Actor
    {
        public ActorConfig Config { get; set; }
        public GameObject PreviewGo;
        public GameObject GameGo;
        public bool isNet = false;
        protected override void Start()
        {
            base.Start();

            switch (ActorType)
            {
                case Common.ActorType.Normal:
                {
                    if (GameGo != null)
                    {
                        GameGo.SetActive(true);
                    }

                    if (null != PreviewGo)
                    {
                        PreviewGo.SetActive(false);
                    }
                   
                }
                break;
                default:
                {
                    if (GameGo != null)
                    {
                        GameGo.SetActive(false);
                    }

                    if (null != PreviewGo)
                    {
                        PreviewGo.SetActive(true);
                    }
                }
                break;
            }
        }
    }
}