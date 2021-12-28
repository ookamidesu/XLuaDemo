using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLuaDemo
{
    
    public class LogManager
    {
        //private static Action<string,string, LogType> log_callback;
        private static bool _EnableLog = true;

        private static List<ILogHandler> _logHandlers = new List<ILogHandler>();


        static LogManager()
        {
            Application.logMessageReceived += HandlerLog;
            Debug.unityLogger.logEnabled = _EnableLog;

        }

        public static bool EnableLog
        {
            get { return _EnableLog; }
            set
            {
                _EnableLog = value;
                Debug.unityLogger.logEnabled = _EnableLog;
            }
        }


        public static void RegisterLogHandler(ILogHandler handler)
        {
            _logHandlers.Add(handler);
        }
    
        public static void UnregisterLogHandler(ILogHandler handler)
        {
            _logHandlers.Remove(handler);
        }

        public static void HandlerLog(string msg,string stackTrace,LogType logType)
        {
            foreach (var logHandler in _logHandlers)
            {
                if (logType >= logHandler.FilterLogType)
                {
                    logHandler.SendMessage(msg,stackTrace,logType);
                }
            }
        }
    }
}


