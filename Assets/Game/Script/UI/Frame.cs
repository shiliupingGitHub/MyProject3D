using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.UI
{
    public class Frame
    {
        private GameObject _gameObject;

        public GameObject FrameGo => _gameObject;

        protected virtual string ResPath => string.Empty;

        public virtual void Init(Transform parent)
        {
            if (!string.IsNullOrEmpty(ResPath))
            {
                var asset = GameResMgr.Instance.LoadAssetSync<GameObject>(ResPath);
                _gameObject = Object.Instantiate(asset,parent);
                InitField(_gameObject, this);
            }
        }
        
        

        public static void InitField(GameObject go, System.Object o)
        {
            var typeInfo = o.GetType() as TypeInfo;

            if (typeInfo == null)
            {
                return;
            }
               
            foreach (var field in typeInfo.DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                var pathAttribute = field.GetCustomAttribute<UIPathAttribute>();

                if (null != pathAttribute)
                {
                    var transform = go.transform.Find(pathAttribute.Path);

                    if (null != transform)
                    {
                        if (field.FieldType == typeof(GameObject))
                        {
                            field.SetValue(o, transform.gameObject);
                        }
                        else if (field.FieldType == typeof(Transform))
                        {
                            field.SetValue(o, transform);
                        }
                        else
                        {
                            var component = transform.GetComponent(field.FieldType);
                            field.SetValue(o, component);
                        }
                    }
                    else
                    {
                        Debug.LogError($@"path {pathAttribute.Path}does not int {go.name}");
                    }
                  
                }
            }
        }

        public virtual void Destroy()
        {
            if (null != FrameGo)
            {
                Object.Destroy(FrameGo);
                _gameObject = null;
            }
            OnDestroy();
        }

        public void Show()
        {
            if (null != FrameGo)
            {
                FrameGo.SetActive(true);
            }
            OnShow();
        }

        public void Hide()
        {
            if (null != FrameGo)
            {
                FrameGo.SetActive(false);
            }
            OnHide();
        }

        protected virtual void OnShow()
        {
          
        }

        protected virtual void OnHide()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }
        
    }
}