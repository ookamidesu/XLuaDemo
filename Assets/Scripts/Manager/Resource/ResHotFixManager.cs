using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace XLuaDemo
{
    public class AssetsHotFixManager : Singleton<AssetsHotFixManager>
    {
        public enum FileUpdateModel
        {
            Delete,
            Download,
            Update,
        }

        public class FileUpdateInfo
        {
            public FileUpdateModel model;

            public string localPath;

            public string serverPath;
        }
        
        public List<FileUpdateInfo> fileUpdateInfos = new List<FileUpdateInfo>();
        
        public void CheckHotFix(Action<float> onUpdate,Action onFinish)
        {
            //获取服务器最新的版本信息
            GameManager.Instance.StartCoroutine(CheckInfo(onFinish));

        }

        public IEnumerator CheckInfo(Action onFinish)
        {
            WWW www = new WWW(AppConfig.ResUrl + AppConfig.PlatformName+ AppConfig.VerInfoName);
            yield return www;
            var versionByServer = FileUtil.DeserializeByString<VerInfo>(www.text.Substring(1));
            
            if (File.Exists(AppConfig.VerInfoPath))
            {
                var versionByLocal = FileUtil.DeserializeByFile<VerInfo>(AppConfig.VerInfoPath);
                //版本号不一样.更新
                if (versionByServer.ResVer != versionByLocal.ResVer)
                {
                    Debug.Log("开始更新");
                    //判断资源列表
                    var resListByLocal = FileUtil.DeserializeByFile<ResList>(AppConfig.ResListPath);
                    www = new WWW(AppConfig.ResUrl + AppConfig.PlatformName+ AppConfig.ResListName);
                    yield return www;
                    var resListByServer = FileUtil.DeserializeByString<ResList>(www.text.Substring(1));
                    
                    FileUtil.SerializeObjToFile(AppConfig.ResListPath,resListByServer);
                    foreach (var serverRes in resListByServer.AllRes)
                    {
                        var localRes = resListByLocal.AllRes.FirstOrDefault(res => serverRes.Path == res.Path);
                        if (localRes== null)
                        {
                            //本地没有文件 
                            fileUpdateInfos.Add(new FileUpdateInfo()
                            {
                                model = FileUpdateModel.Download,
                                localPath = Path.Combine(AppConfig.ABPath , serverRes.Path),
                                serverPath = AppConfig.ResPlatformUrl + "AB/" + serverRes.Path,
                            });
                        }
                        else
                        {
                            //本地有文件
                            if (localRes.Crc != serverRes.Crc)
                            {
                                fileUpdateInfos.Add(new FileUpdateInfo()
                                {
                                    model = FileUpdateModel.Update,
                                    localPath = Path.Combine(AppConfig.ABPath , localRes.Path),
                                    serverPath = AppConfig.ResPlatformUrl + "AB/" + serverRes.Path,
                                });
                            }
                        }
                    }
                    
                    /*//删除一些数据
                    foreach (var localRes in resListByLocal.AllRes)
                    {
                        if ()
                        {
                            
                        }
                    }*/

                }
            }
            else
            {
                //不存在.代表第一次启动
                www = new WWW(AppConfig.ResUrl + AppConfig.PlatformName+ AppConfig.ResListName);
                yield return www;
                Debug.Log(www.text);
                var resListByServer = FileUtil.DeserializeByString<ResList>(www.text.Substring(1));
                FileUtil.SerializeObjToFile(AppConfig.ResListPath,resListByServer);
                //将所有资源加到下载队列
                foreach (var serverRes in resListByServer.AllRes)
                {
                    fileUpdateInfos.Add(new FileUpdateInfo()
                    {
                        model = FileUpdateModel.Download,
                        localPath = Path.Combine(AppConfig.ABPath , serverRes.Path),
                        serverPath = AppConfig.ResPlatformUrl + "AB/" + serverRes.Path,
                    });
                }
            }
            
            foreach (var fileUpdateInfo in fileUpdateInfos)
            {
                Debug.LogFormat("更新 : {0}",fileUpdateInfo.localPath);
                switch (fileUpdateInfo.model)
                {
                    case FileUpdateModel.Delete:
                        break;
                    case FileUpdateModel.Download:
                        WWW wwwDown = new WWW(fileUpdateInfo.serverPath);
                        Debug.Log(fileUpdateInfo.serverPath);
                        yield return wwwDown;
                        Debug.Log(wwwDown.bytes.Length);
                        File.WriteAllBytes(fileUpdateInfo.localPath,wwwDown.bytes);
                        break;
                    case FileUpdateModel.Update:
                        WWW wwwUpdate = new WWW(fileUpdateInfo.serverPath);
                        yield return wwwUpdate;
                        File.WriteAllBytes(fileUpdateInfo.localPath,wwwUpdate.bytes);
                        break;
                   
                }
            }
            FileUtil.SerializeObjToFile(AppConfig.VerInfoPath,versionByServer);

            yield return new WaitForSeconds(2);
            onFinish();

        }
        
    }
}