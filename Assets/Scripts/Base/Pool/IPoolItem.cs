/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

namespace XLuaDemo
{
    /// <summary>
    /// 对象池资源
    /// </summary>
    public interface IPoolItem
    {
        /// <summary>
        /// 第一次初始化时调用
        /// </summary>
        void OnInit();

        /// <summary>
        /// 回收时调用
        /// </summary>
        void OnRecycle();
        
        /// <summary>
        /// 使用前调用
        /// </summary>
        void Spawn();
    }
}