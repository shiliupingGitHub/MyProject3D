namespace Game.Script.Common
{
    public interface IOnInstance
    {

        void OnInstance();
    }

    public class SingletonWithOnInstance<T>  : IOnInstance where T : IOnInstance, new()
    {
        private static T _instance;

        public virtual void OnInstance(){}
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                    _instance.OnInstance();
                }

                return _instance;
            }
        }
    }
    public class Singleton<T>   where T:new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}