using System.IO;
using UnityEngine;

namespace XLuaDemo
{
    public class AppConfig
    {
        public static string DataPath;

        public static string AppDataPath;
        
        public static string AssetDir;

        public static string LuaAssetsDir;
        public static string ABPath;
        public static bool IsBundle { get; set; }

        public static void Init()
        {
            //string game = AppConfig.AppName.ToLower();
            
            
            //如果为移动平台,路径为persistentDataPath
            if (Application.isMobilePlatform) {
                if (DataPath == null)
                    DataPath = Application.persistentDataPath + "/Mobile" + "/";
            }
            /*else if (AppConfig.DebugMode) {
                if (DataPath == null)
                    DataPath = AppConfig.AppDataPath + "/" + AppConfig.AssetDir + "/";
            }*/
            /*else if (Application.platform == RuntimePlatform.OSXEditor) {
                if (DataPath == null)
                {
                    int i = Application.dataPath.LastIndexOf('/');
                    DataPath = Application.dataPath.Substring(0, i + 1) + game + "/";
                }
            }*/
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform ==   RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                
                string dataPath = Application.dataPath;
                AppDataPath = dataPath.Replace("/Assets", "");
                AssetDir = "PC";
                DataPath = AppConfig.AppDataPath+"/"+AppConfig.AssetDir+"/";
            }
            /*else
            if (DataPath == null)
                DataPath = "c:/" + game + "/";*/
            LuaAssetsDir =Application.dataPath + "/LuaScripts";
            
            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);
            
            Debug.Log(LuaAssetsDir);
        }
    }
}