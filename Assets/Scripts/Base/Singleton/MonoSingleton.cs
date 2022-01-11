using System;
using UnityEngine;

namespace XLuaDemo
{
    /// <summary>
    /// mono单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T _instance = null;
        private static bool _applicationIsQuit = false;
        public static T Instance
        {
            get
            {
                if (_instance == null && !_applicationIsQuit)
                {
                    _instance = FindObjectOfType<T>();
                    if (FindObjectsOfType<T>().Length >= 1)
                    {
                        Debug.LogWarning("More than 1");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var instanceName = typeof(T).Name;
                        Debug.LogFormat("Instance Name: {0}", instanceName);
                        var instanceObj = GameObject.Find(instanceName);

                        if (!instanceObj)
                            instanceObj = new GameObject(instanceName);

                        _instance = instanceObj.AddComponent<T>();
                        _instance.OnCreate();
                        DontDestroyOnLoad(instanceObj); //保证实例不会被释放

                        Debug.LogFormat("Add New Singleton {0} in Game!", instanceName);
                    }
                    else
                    {
                        Debug.LogFormat("Already exist: {0}", _instance.name);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = (T)this;
        }

        protected virtual void OnCreate()
        {
            
        }
        
        protected void OnApplicationQuit()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }

        protected virtual void OnDestroy()
        {
            _applicationIsQuit = true;
        }
    }
}