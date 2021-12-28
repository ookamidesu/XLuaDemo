/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System.Collections.Generic;

namespace XLuaDemo
{
    /// <summary>
    /// 对象池 资源集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PoolItemList<T> : List<T> ,IPoolItem
    {
        public void OnInit()
        {
            
        }

        public void OnRecycle()
        {
            Clear();
        }

        public void Spawn()
        {
            
        }
    }
}