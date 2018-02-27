using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Generic class to create managers. It contains all the SINGLETON declaration
    /// as well as the initialization.
    /// </summary>
    /// <typeparam name="T">Class of the inheriting manager</typeparam>
    public class GenericManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Singleton declaration 

        static T _instance = null;
        public static T SINGLETON
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        #region Event functions

        public virtual void Awake()
        {
            IniSingleton();
        }

        public virtual void OnEnable()
        {
            IniSingleton();
        }

        #endregion

        #region Public methods

        public virtual void IniSingleton()
        {
            if (_instance != null) return;

            _instance = (T)FindObjectOfType(typeof(T));
        }

        #endregion
    }
}
