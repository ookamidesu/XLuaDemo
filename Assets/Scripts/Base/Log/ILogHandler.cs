using UnityEngine;

namespace XLuaDemo
{
    public interface ILogHandler
    {
        LogType FilterLogType { get; }

        void SendMessage(string msg, string stackTrace, LogType logType);
    }
}