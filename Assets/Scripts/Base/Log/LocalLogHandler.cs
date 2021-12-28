using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XLuaDemo
{

    /// <summary>
    /// log信息,存储在本地
    /// </summary>
    public class LocalLogHandler : MonoBehaviour ,ILogHandler
    {
        private Thread thread;
        public LogType m_FilterLogType = LogType.Log;
        
        private Queue<LogInfo> _logInfos = new Queue<LogInfo>();

        public LogType FilterLogType { get; }

        private string outputPath;
        
        private object lockObj = new object();
        

        
        public struct LogInfo
        {
            public LogType LogType;
            public string msg;
            public string stackTrace;
            public DateTime Time;

            public LogInfo(LogType logType, string msg, string stackTrace, DateTime time)
            {
                LogType = logType;
                this.msg = msg;
                this.stackTrace = stackTrace;
                Time = time;
            }
        }
        
        /*public struct Log
        {
            
        }*/

        void Start()
        {
            thread = new Thread(OnUpdateThread);
            
            string log_dir = Path.Combine(AppConfig.DataPath, "log");
            if (!Directory.Exists(log_dir))
                Directory.CreateDirectory(log_dir);

            outputPath = Path.Combine(log_dir, "out_put.txt");
            
            //每次启动先删除旧的
            File.WriteAllText(outputPath, "");
            
            thread.Start();
        }
        

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
            lock (lockObj)
            {
                //TestText.Instance.text.text = TestText.Instance.text.text + "\nLocalLogHandler"  + stackTrace + "*****"+  msg;
                _logInfos.Enqueue(new LogInfo(logType, msg,stackTrace,DateTime.Now));
            }
           
        }

        public void OnUpdateThread()
        {
            while (true)
            {
                lock (lockObj)
                {
                    if (_logInfos.Count > 0)
                    {
                        WriteFile(_logInfos);
                        _logInfos.Clear();
                    }
                   
                   
                }
                Thread.Sleep(1);
            }
        }

        private void WriteFile(IEnumerable<LogInfo> logInfos)
        {
            using (StreamWriter writer = new StreamWriter(outputPath, true, System.Text.Encoding.UTF8) )
            {
                
                foreach (var info in logInfos)
                {
                    writer.WriteLine($"{info.LogType} {info.Time:yyyy-MM-dd HH:mm:ss} *********{info.msg} \n {info.stackTrace}");
                }
            }
        }
    }
}