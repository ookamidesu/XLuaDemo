using System.Collections;
using UnityEngine;
using XLua;

namespace XLuaDemo
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private WaitForSeconds wait = new WaitForSeconds(0.5f);
        
        public void SendMessage(string message,LuaFunction callback)
        {
            XLuaManager.Instance.StartCoroutine(Send(message, callback));
        }

        public IEnumerator Send(string message,LuaFunction callback)
        {

            //模拟延迟,直接返回
            yield return wait;
            if (callback != null)
            {
                
                callback.Call();
            }
            
        }
        
        
    }
}