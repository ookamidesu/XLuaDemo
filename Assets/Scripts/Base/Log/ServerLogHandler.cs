using UnityEngine;

namespace XLuaDemo
{
    /// <summary>
    /// log信息,会发送到服务器
    /// </summary>
    public class ServerLogHandler : MonoBehaviour ,ILogHandler
    {
        public LogType m_FilterLogType = LogType.Exception;

        public LogType FilterLogType { get; }

     

        void OnEnable()
        {
            LogManager.RegisterLogHandler(this);
        }
    

        void OnDisable()
        {
            LogManager.UnregisterLogHandler(this);
        }
        
    
        
        public void SendMessage(string msg, string stackTrace, LogType logType)
        {
           //todo 有错误报告,发送到服务器
        }
    }
}