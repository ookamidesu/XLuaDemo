using System;
using System.Reflection;
using UnityEngine;

namespace XLuaDemo
{
    /// <summary>
    /// 单例父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T:Singleton<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if (ctor == null)
                        throw new Exception("没有公共的构造器");

                    _instance = ctor.Invoke(null) as T;
                    
                    _instance.OnCreate();
                }
                return _instance;
                
            }
        }

        protected Singleton()
        {
            
        }

        protected virtual void OnCreate()
        {
            
        }
    }

}

