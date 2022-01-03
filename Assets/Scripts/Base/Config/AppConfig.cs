using System;
using System.IO;
using UnityEngine;

namespace XLuaDemo
{
    public class AppConfig
    {
        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string AppVersion = "0.1";
        
        public static string ResVersion = "0.1";
        
        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string DataPath;

        /// <summary>
        /// Log输出目录
        /// </summary>
        public static string LogPath;

        public static string PlatformName;

        public static string ResListPath;

        public static string ResListName = "ResList.json";


        public static string VerInfoPath;
        
        public static string VerInfoName = "VerInfo.json";

        

        /// <summary>
        /// 
        /// </summary>
        public static string AppDataPath;
        

        /// <summary>
        /// 本地lua位置
        /// </summary>
        public static string LuaAssetsDir;
        
        
        /// <summary>
        /// 本地ab目录
        /// </summary>
        public static string ABPath;
        
        /// <summary>
        /// 使用bundle加载
        /// </summary>
        public static bool IsBundle = true;

        public static bool IsLocal = true;

        /// <summary>
        /// 资源服务器地址
        /// 
        /// </summary>
        public static string ResUrl = "http://localhost:8080/";
        
        public static string ResPlatformUrl;
        

        public static void Init()
        {
            DataPath = Application.dataPath.Replace("/Assets", "");
            LogPath = DataPath + "/Log";
            
            AppDataPath = Application.streamingAssetsPath;

            ResListPath = Path.Combine(AppDataPath, ResListName);
            VerInfoPath = Path.Combine(AppDataPath, VerInfoName);

            ABPath = AppDataPath+ "/AB/";
            
            //lua会从ab/Assets加载,使用Assets能读取的路径
            LuaAssetsDir = "Assets/LuaScript";
            
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WebGLPlayer:
                    PlatformName = "Pc/";
                    break;
                case RuntimePlatform.Android:
                    PlatformName = "Android/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    PlatformName = "IOS/";
                    break;
                default:
                    PlatformName = "Default/";
                    break;
            }

            ResPlatformUrl = ResUrl + PlatformName + "/";

            if (!Directory.Exists(ABPath))
            {
                Directory.CreateDirectory(ABPath);
            }
            //Debug.Log(LuaAssetsDir);
        }

        
        
        private static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        
        
        /// <summary>
        /// 打包后的位置
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetBuildPath(RuntimePlatform platform,string fileName = "")
        {
            string dataPath = Application.dataPath.Replace("/Assets", "/AssetsBundle");
            if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer ||
                platform == RuntimePlatform.WebGLPlayer)
                dataPath = dataPath + "/Pc/" + fileName;
            else if (platform == RuntimePlatform.Android)
                dataPath = dataPath + "/Android/" + fileName;
            else if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.OSXEditor ||
                     platform == RuntimePlatform.OSXPlayer)
                dataPath = dataPath + "/IOS/"+ fileName;
            else
                Debug.Log("Unspport System!");
            return dataPath;
        }
        
   
    }
}