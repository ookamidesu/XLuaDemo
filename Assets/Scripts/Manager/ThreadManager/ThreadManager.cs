using System.Collections.Generic;
using System.Threading;

namespace XLuaDemo
{
  
    
    
    public class ThreadManager : MonoSingleton<ThreadManager>
    {
        public class ThreadEvent
        {
            public ThreadEventType type;
            public List<object> evParams = new List<object>();
        }
        
        public enum ThreadEventType
        {
            UpdateExtract,    //更新解包
            UpdateDownload,    //更新下载
            UpdateProgress,    //更新进度
        }
        
        private Queue<ThreadEvent> events = new Queue<ThreadEvent>();

        private bool openThread = true;
        
        static readonly object m_lockObject = new object();

        private int threadCount = 3;
        
        public void Test()
        {
            ThreadPool.SetMaxThreads(threadCount, threadCount);
            ThreadPool.QueueUserWorkItem(new WaitCallback(OnUpdate), 3);
        }

        public void OnUpdate(object obj)
        {
            while (openThread)
            {
                lock (m_lockObject)
                {
                    
                }
                
                Thread.Sleep(10);
            }
           
        }
    }
}