/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System;
using System.Collections.Generic;

namespace XLuaDemo
{
    /// <summary>
    /// 类对象池公工厂
    /// </summary>
    public class ClassPoolFactor : Singleton<ClassPoolFactor>
    {
        private Dictionary<Type,object> AllClassPool = new Dictionary<Type, object>();
        
        private ClassPoolFactor()
        {
            
        }

        /// <summary>
        /// 创建一个类对象池
        /// </summary>
        /// <param name="initCount"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ClassObjectPool<T> CreateClassPool<T>(int initCount) where T : IPoolItem, new()
        {
            var type = typeof(T);
            object pool;
            if (!AllClassPool.TryGetValue(type,out pool))
            {
                pool = new ClassObjectPool<T>(initCount);
                AllClassPool.Add(type,pool);
            }
            
            return pool as ClassObjectPool<T>;
        }
    }
}