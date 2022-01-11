using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using XLuaDemo;
using Debug = UnityEngine.Debug;

public class ThreadManager : MonoSingleton<ThreadManager>
{
    public class ThreadEvent
    {
        public ThreadEventType type;
        public ConcurrentQueue<object> evParams = new ConcurrentQueue<object>();
        //public ConcurrentQueue<Action> actions;
        public Action<object> onFinish;
        public Action<SyncEventData> syncEvent;

        public void AddEventParam(object obj)
        {
            evParams.Enqueue(obj);
            allCount++;
        }

        internal int finishCount;
        internal int allCount;

        internal bool isFinish => finishCount == allCount;

        internal bool hasParam = true;

        public float CompleteProgress => finishCount / (float)allCount;

        internal bool FinishOne()
        {
            finishCount++;
            return isFinish;

        }
    }

    public class SyncEventData
    {
        public SyncEventType syncEventType;
        public object eventParam;

        public SyncEventData(SyncEventType syncEventType, object eventParam)
        {
            this.syncEventType = syncEventType;
            this.eventParam = eventParam;
        }
    }
    
    public enum SyncEventType
    {
        DownFileFinish,
        DeleteFileFinish,
        UpdateDownload,
    }
    
    public enum ThreadEventType
    {
        UpdateExtract,
        UpdateDownload,
    }
    
    public enum ThreadState
    {
        Null,
        Running,
        SlowModel,
        Suspend,
        Stop,
    }
    
    public class DownloadFileParam
    {
        public FileUpdateModel model;

        public string localPath;

        public string serverPath;

        public ABRes localRes;

        public ABRes serverRes;

        internal long lastBytes;

        public DownloadFileParam(FileUpdateModel model,  string serverPath,string localPath,ABRes localRes,ABRes serverRes)
        {
            this.model = model;
            this.serverPath = serverPath;
            this.localPath = localPath;
            this.localRes = localRes;
            this.serverRes = serverRes;
        }
        
        public DownloadFileParam(FileUpdateModel model,  string serverPath,string localPath)
        {
            this.model = model;
            this.serverPath = serverPath;
            this.localPath = localPath;
        }
    }
    
    public enum FileUpdateModel
    {
        Delete,
        Download,
        Update,
    }

    
    public object lockObj = new object();
    
    public ConcurrentQueue<ThreadEvent> threadEvents = new ConcurrentQueue<ThreadEvent>();

    public int suspendDelay = 100;

    private int initThreadCount = 1;

    private int sleepTime = 10;

    private int slowSleepTime = 1000;

    public float refreshTime = 1;

    private double currentByte = 0;
    
    private long lastByte;
    

    
    public bool isStart;

    private ThreadState _state = ThreadState.Null;
    void Awake()
    {
        InitThread();
        
    }

    /*private void Start()
    {
        var threadEvent = new ThreadEvent();
        /*for (int i = 0; i < 10000; i++)
        {
            string severPath =  "http://localhost:8080/Pc/AB/2187726194";

            string localPath = $"{Application.dataPath.Replace("/Assets","")}/tem/{i}";
            
            threadEvent.AddEventParam(new DownloadFileParam(FileUpdateModel.Download, severPath, localPath));
        }#1#
        
        for (int i = 0; i < 10000; i++)
        {
            string severPath =  "http://localhost:8080/Pc/AB/2187726194";

            string localPath = $"{Application.dataPath.Replace("/Assets","")}/tem/{i}";
            
            threadEvent.AddEventParam(new DownloadFileParam(FileUpdateModel.Delete, severPath, localPath));
        }

        threadEvent.onFinish = OnDownloadFinish;
        threadEvent.syncEvent = OnDownloadFile;
        
        
        threadEvent.type = ThreadEventType.UpdateDownload;
    }*/

    public void AddThreadEvent(ThreadEvent threadEvent)
    {
        threadEvents.Enqueue(threadEvent);
    }

    private void OnDownloadFinish(object obj)
    {
        //Debug.Log("下载完成");
        Debug.Log("全部完成" + Time.frameCount);
    }

    
    private void OnDownloadFile(object obj)
    {
        var syncEventData = obj as SyncEventData;
        switch (syncEventData.syncEventType)
        {
            case SyncEventType.DownFileFinish:
                Debug.Log("下载完成");
                break;
            case SyncEventType.DeleteFileFinish:
                Debug.Log("删除完成");
                break;
            case SyncEventType.UpdateDownload:
                //Debug.Log(syncEventData.eventParam);
                currentByte += (double) syncEventData.eventParam;
                //text = syncEventData.eventParam.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
       
    }
    
    void Update()
    {
        if (threadEvents.TryPeek(out var threadEvent))
        {
            if (threadEvent.isFinish)
            {
                if(threadEvents.TryDequeue(out threadEvent))
                {
                    threadEvent.onFinish(threadEvent);
                }
            }
        }
        
    }


    public void InitThread()
    {
        ThreadPool.SetMaxThreads(4,4);
        for (int i = 0; i < initThreadCount; i++)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(OnThreadUpdate),_state);
           
        }
        /*Thread t = new Thread(OnThreadUpdate);
        t.Start();*/
        _state = ThreadState.Stop;
    }

    private void OnThreadUpdate(object obj)
    {
        while (true)
        {
            lock (lockObj)
            {
                if (_state == ThreadState.Suspend)
                {
                    Thread.Sleep(suspendDelay);
                    continue;
                }
                
                if(_state == ThreadState.Stop)
                    break;
                ExeFunc();
            }
            Thread.Sleep(sleepTime);
        }
    }

    public void ExeFunc()
    {
        //Debug.Log(threadEvents.Count);
        if (threadEvents.TryPeek(out var threadEvent) && threadEvent.hasParam)
        {
            //Debug.Log(threadEvent.evParams.Count);
            switch (threadEvent.type)
            {
                case ThreadEventType.UpdateExtract:
                    break;
                case ThreadEventType.UpdateDownload:
                    UpdateDownload(threadEvent);
                    break;
            }
            
            //Debug.Log("执行");
            
            
            if (threadEvent.evParams.Count == 0)
            {
                threadEvent.hasParam = false;
            }
        }
    }

    public void UpdateDownload(ThreadEvent threadEvent)
    {
        //Debug.Log("执行"+threadEvent.evParams.Count);
        if (threadEvent.evParams.TryDequeue(out var para))
        {
            var downFileParam = para as DownloadFileParam;

            switch (downFileParam.model)
            {
                case FileUpdateModel.Delete:
                    DeleteFile(downFileParam,threadEvent);
                    break;
                case FileUpdateModel.Download:
                    DownloadFile(downFileParam,threadEvent);
                    break;
                case FileUpdateModel.Update:
                    break;
            }

            //threadEvent.syncEvent(downFileParam);
        }

       
    }

    private void DownloadFile(DownloadFileParam fileParam,ThreadEvent threadEvent)
    {
        //Debug.Log($"下载 {fileParam.serverPath}");
        using (WebClient client = new WebClient())
        {
            
            //var time = sw.Elapsed.TotalSeconds;
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender,e)=> ProgressChanged(sender,e,threadEvent,fileParam));
            client.DownloadFileCompleted += new AsyncCompletedEventHandler((sender,e)=> ProgressFinish(sender,e,threadEvent,fileParam));
            client.DownloadFileAsync(new System.Uri(fileParam.serverPath), fileParam.localPath);
        }
    }
    
    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e,ThreadEvent threadEvent,DownloadFileParam fileParam)
    {
        var currentDownloadByte = (e.BytesReceived - fileParam.lastBytes)/1024d;
        fileParam.lastBytes = e.BytesReceived;
        threadEvent.syncEvent(new SyncEventData(SyncEventType.UpdateDownload,currentDownloadByte));
    }
    
    private void ProgressFinish(object sender, AsyncCompletedEventArgs e,ThreadEvent threadEvent,DownloadFileParam fileParam)
    {
        //UnityEngine.Debug.Log("ThreadManager ProgressFinish");
        //sw.Reset();
        //Debug.Log("完成一个");
        threadEvent.FinishOne();
        threadEvent.syncEvent(new SyncEventData(SyncEventType.DownFileFinish,fileParam));
    }


    private void DeleteFile(DownloadFileParam fileParam,ThreadEvent threadEvent)
    {
        if (File.Exists(fileParam.localPath))
        {
            File.Delete(fileParam.localPath);
        }
        
        threadEvent.syncEvent(new SyncEventData(SyncEventType.DeleteFileFinish,fileParam));
        threadEvent.FinishOne();
    }

    public void ExeFinish()
    {
        Debug.Log("执行完毕");
        GameObject obj = new GameObject("123");
        Debug.Log(obj.name);
    }

    private void OnEnable()
    {
        _state = ThreadState.Running;
    }

    private void OnDisable()
    {
        _state = ThreadState.Suspend;
    }

    private void OnDestroy()
    {
        _state = ThreadState.Stop;
    }

   
}
