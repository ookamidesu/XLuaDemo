using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace XLuaDemo
{
    public class AssetsHotFixManager : Singleton<AssetsHotFixManager>
    {
        private Action onFinish;

        private Action<float> updateProgress;

        private Action<object> updateDownload;

        private ThreadManager.ThreadEvent currentThreadEvent;

        private ResList localResList;

        private VerInfo localVerInfo;
        
        private ResList serverResList;

        private VerInfo serverVerInfo;
        
        /// <summary>
        /// 检测热更,当前没有进行本地资源错误检测
        /// </summary>
        /// <param name="onUpdate"></param>
        /// <param name="onFinish"></param>
        public void CheckHotFix(Action<float> updateProgress, Action<object> syncProgress, Action onFinish)
        {
            //获取服务器最新的版本信息
            this.onFinish = onFinish;
            this.updateProgress = updateProgress;
            this.updateDownload = syncProgress;
            GameManager.Instance.StartCoroutine(CheckInfoNew());
        }

        public IEnumerator CheckInfoNew()
        {
            UnityWebRequest request = UnityWebRequest.Get(AppConfig.ResUrl + AppConfig.PlatformName+ AppConfig.VerInfoName);
            yield return request.SendWebRequest();
            var download = request.downloadHandler;
            serverVerInfo = FileUtil.DeserializeByString<VerInfo>(download.text);
            
            if (File.Exists(AppConfig.VerInfoPath))
            {
                localVerInfo = FileUtil.DeserializeByFile<VerInfo>(AppConfig.VerInfoPath);
                if (localVerInfo.ResVer != serverVerInfo.ResVer)
                {
                    yield return UpdateResList();
                }
                else
                {
                    OnUpdateFinish(null);
                }
            }
            else
            {
                localVerInfo = new VerInfo();
                SaveVerInfo();
               
                yield return UpdateResList(true);
            }
            
        }

        private IEnumerator UpdateResList(bool isAll = false)
        {
            var listRequest = UnityWebRequest.Get(AppConfig.ResUrl + AppConfig.PlatformName+ AppConfig.ResListName);
            yield return listRequest.SendWebRequest();
            var download = listRequest.downloadHandler;
            serverResList = FileUtil.DeserializeByString<ResList>(download.text);
            if (isAll)
            {
                localResList = new ResList();
                SaveResList();
                UpdateResList(serverResList,null);
            }
            else
            {
                localResList = FileUtil.DeserializeByFile<ResList>(AppConfig.ResListPath);
                UpdateResList(serverResList,localResList);
            }
        }
        
        
        private void UpdateResList(ResList serverResList, ResList localResList)
        {
            //使用创建一个新的线程事件
            currentThreadEvent = new ThreadManager.ThreadEvent();
            currentThreadEvent.onFinish = OnUpdateFinish;
            currentThreadEvent.syncEvent = OnSyncEvent;
            currentThreadEvent.type = ThreadManager.ThreadEventType.UpdateDownload;
            
            
            //本地列表不存在,代表没有下载过/主动重新下载
            if (localResList == null)
            {
                foreach (var serverRes in serverResList.AllRes)
                {
                    currentThreadEvent.AddEventParam(new ThreadManager.DownloadFileParam(
                        ThreadManager.FileUpdateModel.Download, AppConfig.ResPlatformUrl + "AB/" + serverRes.Path,
                        Path.Combine(AppConfig.ABPath, serverRes.Path),null,serverRes));
                }
            }
            else
            {
                foreach (var serverRes in serverResList.AllRes)
                {
                    var localRes = localResList.AllRes.FirstOrDefault(res => serverRes.Path == res.Path);
                    if (localRes == null || localRes.Crc != serverRes.Crc)
                    {
                        currentThreadEvent.AddEventParam(new ThreadManager.DownloadFileParam(
                            ThreadManager.FileUpdateModel.Download, AppConfig.ResPlatformUrl + "AB/" + serverRes.Path,
                            Path.Combine(AppConfig.ABPath, serverRes.Path),localRes,serverRes));
                        //本地没有文件 

                    }
                    
                    
                }
                //删除检测..
                foreach (var abRese in localResList.AllRes.Where(res => localResList.AllRes.Any(servetRes => servetRes.Path == res.Path)))
                {
                    currentThreadEvent.AddEventParam(new ThreadManager.DownloadFileParam(
                        ThreadManager.FileUpdateModel.Delete, "",
                        Path.Combine(AppConfig.ABPath, abRese.Path),abRese,null));
                }
            }
            
            ThreadManager.Instance.AddThreadEvent(currentThreadEvent);

        }

        private void OnUpdateFinish(object obj)
        {
            Debug.Log("更新完成");
            //替换版本信息和资源
            if (serverResList != null && serverVerInfo != null)
            {
                FileUtil.SerializeObjToFile(AppConfig.VerInfoPath,serverVerInfo);
                FileUtil.SerializeObjToFile(AppConfig.ResListPath,serverResList);
            }
            onFinish();
        }
        

        private void OnSyncEvent(ThreadManager.SyncEventData syncEventData)
        {
            switch (syncEventData.syncEventType)
            {
                case ThreadManager.SyncEventType.DownFileFinish:
                    Debug.Log(currentThreadEvent.CompleteProgress);

                    var downloadFileParam = (ThreadManager.DownloadFileParam) syncEventData.eventParam;
                    //完成一个下载,更新资源列表
                    //代表新创建
                    if (downloadFileParam.localRes == null && downloadFileParam.serverRes != null)
                    {
                        localResList.AllRes.Add(downloadFileParam.serverRes);
                    }
                    else if(downloadFileParam.localRes != null && downloadFileParam.serverRes != null)
                    {
                        //代表更新
                        //应为传入的为引用,直接更改lcoalRes会同步到localResList
                        downloadFileParam.localRes.Update(downloadFileParam.serverRes);
                    }
                    SaveResList();
                    updateProgress(currentThreadEvent.CompleteProgress);
                    break;
                case ThreadManager.SyncEventType.DeleteFileFinish:
                    var deleteFileParam = (ThreadManager.DownloadFileParam) syncEventData.eventParam;
                    localResList.AllRes.Remove(deleteFileParam.localRes);
                    SaveResList();
                    updateProgress(currentThreadEvent.CompleteProgress);
                    break;
                case ThreadManager.SyncEventType.UpdateDownload:
                    updateDownload(syncEventData.eventParam);
                    break;
            }
        }

        private void SaveResList()
        {
            FileUtil.SerializeObjToFile(AppConfig.ResListPath,localResList);
        }
        
        private void SaveVerInfo()
        {
            FileUtil.SerializeObjToFile(AppConfig.VerInfoPath,localVerInfo);
        }
        

    }
}