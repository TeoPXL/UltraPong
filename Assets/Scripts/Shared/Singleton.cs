using UnityEngine;

namespace Shared
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (!_instance) Debug.LogError(typeof(T) + " is missing.");
                }

                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("Second instance of " + typeof(T) + " created. Automatic self-destruct triggered.");
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            Init();
        }


        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }


        public virtual void Init()
        {
        }
    }
}