/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System.Collections.Generic;
using UnityEngine;

namespace XLuaDemo
{
    /// <summary>
    /// 类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassObjectPool<T> where T : IPoolItem,new() 
    {

        /// <summary>
        /// 对象池
        /// </summary>
        protected Stack<T> m_UnusePool = new Stack<T>();
        
        /// <summary>
        /// 正在使用的对象池
        /// </summary>
        protected List<T> m_UsePool = new List<T>();
        
        
        /// <summary>
        /// 初始化个数
        /// </summary>
        protected int m_InitCount = 0;
        
        /// <summary>
        /// 没有回收的对象个数
        /// </summary>
        protected int m_NoRecycleCount = 0;

        
        public int NoRecycleCount
        {
            get { return m_NoRecycleCount; }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="initCount">初始化数量</param>
        public ClassObjectPool(int initCount)
        {
            m_InitCount = initCount;
            for (int i = 0; i < initCount; i++)
            {
                var data = new T();
                data.OnInit();
                m_UnusePool.Push(data);
            }
        }

        /// <summary>
        /// 从池里面取类对象
        /// </summary>
        /// <returns></returns>
        public T Spawn()
        {
            if (m_UnusePool.Count > 0)
            {
                T rtn = m_UnusePool.Pop();
                if (rtn == null)
                {
                    rtn = new T();
                    rtn.OnInit();
                }
                m_UsePool.Add(rtn);
                m_NoRecycleCount++;
                rtn.Spawn();
                return rtn;
            }
            else
            {
                T rtn = new T();
                rtn.OnInit();
                m_UsePool.Add(rtn);
                m_NoRecycleCount++;
                rtn.Spawn();
                return rtn;
            }
        }

        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Recycle(T obj)
        {
            if (obj == null)
                return false;

            m_NoRecycleCount--;
            
            m_UsePool.Remove(obj);
            m_UnusePool.Push(obj);
            obj.OnRecycle();
            return true;
        }
    }
}