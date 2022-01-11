using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using XLuaDemo;
using FileUtil = XLuaDemo.FileUtil;

namespace MyBundle
{
    public enum ResourceType
    {
        Default,
        Fiexd,
        Cache,
    }
    
    public class BundleBuild
    {
        public static string m_BunleTargetPath;
        public static string m_BunleLocalPath = Application.streamingAssetsPath;

        public static string ABCONFIGPATH = "Assets/Editor/BuildAssetBundle/ABConfig.asset";

        public static string ABDataPath =  "Assets/GameData/ABConfig/";
        
        public static string abResourcePath =ABDataPath+"AssetBundleConfig.bytes";
        
        public static string xmlPath = ABDataPath+"AssetbundleConfig.xml";
        
        
        private static string prefabOtherABName = "Other";
        
        private List<string> Filter = new List<string>();
        

        private static Dictionary<string, uint> m_ABCrcName = new Dictionary<string, uint>();

        private static List<uint> m_allBundleNameCrc = new List<uint>();

        private static List<uint> m_allBundleResourceCrc = new List<uint>();

        private static List<string> allBundlePath = new List<string>();

        private static Dictionary<string, string> m_AllBundle = new Dictionary<string, string>();

        private static List<string> m_fixedABName = new List<string>();
        
        private static List<string> m_cacheABName = new List<string>();

        private static ABConfig abConfig;

        
        public static void Build(BuildTarget target)
        {
            try
            {
                var platform = BuildTargetToPlatform(target);
                m_BunleTargetPath = AppConfig.GetBuildPath(platform,"AB");
                FileCheck();
                //ClearABName();
                m_fixedABName.Clear();
                m_ABCrcName.Clear();
                m_allBundleNameCrc.Clear();
                allBundlePath.Clear();
                m_AllBundle.Clear();
                m_allBundleResourceCrc.Clear();
                abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
                Debug.Log(abConfig);

                //先将单独打包的资源进行设置
                BundleResource();

                //依赖打包
                BundlePrefab();

                BuildAssetBundle(target);

                ClearABName();

                //EncryptAB();

                GenerateResList(platform);
                
                //MoveAB(platform);
            
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                throw;
            }


        }
        
        public static RuntimePlatform BuildTargetToPlatform(BuildTarget target)
        {
            if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows)
                return RuntimePlatform.WindowsEditor;
            else if (target == BuildTarget.Android)
                return RuntimePlatform.Android;
            else if (target == BuildTarget.iOS)
                return RuntimePlatform.IPhonePlayer;
            else
                return RuntimePlatform.WindowsEditor;
        }
        
        [MenuItem("AssetsBundle/BuildAndroid")]
        public static void BuildAndroid()
        {
            
        }
        
        [MenuItem("AssetsBundle/BuildIos")]
        public static void BuildIos()
        {
            
        }      
        
        [MenuItem("AssetsBundle/BuildPc")]
        public static void BuildPc()
        {
            Build(BuildTarget.StandaloneWindows64);

        }

        public static void CopyLua(string source,string targetPath)
        {
            string[] files = Directory.GetFileSystemEntries(source);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            //删除目标中的文件
            foreach (var file in Directory.GetFiles(targetPath))
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            foreach (string file in files)
            {
                
                if (Directory.Exists(file))
                {
                    CopyLua(file, Path.Combine(targetPath,file.Substring(file.LastIndexOf("\\"))));
                }
                else
                {
                    if(file.EndsWith(".meta"))
                        continue;
                    File.Copy(file, targetPath + Path.DirectorySeparatorChar + Path.GetFileName(file)+".txt", true);
                }
            }
        }

       

        private static void FileCheck()
        {
            if (!Directory.Exists(m_BunleTargetPath))
            {
                Directory.CreateDirectory(m_BunleTargetPath);
            }
            
            if (!Directory.Exists(ABDataPath))
            {
                Directory.CreateDirectory(ABDataPath);
            }
            
            if (File.Exists(abResourcePath))
            {
                File.Delete(abResourcePath);
            }
            
            var fileStream = File.Create(abResourcePath);
            fileStream.Close();
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            
            
        }

        private static void BuildAssetBundle(BuildTarget target)
        {
            string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
            //key为全路径，value为包名
            Dictionary<string, string> resPathDic = new Dictionary<string, string>();
            for (int i = 0; i < allBundles.Length; i++)
            {
                //根据Bundle名获取所有资源路径
                string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
                for (int j = 0; j < allBundlePath.Length; j++)
                {
                    if (allBundlePath[j].EndsWith(".cs"))
                        continue;

                    //Debug.Log("此AB包：" + allBundles[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                    resPathDic.Add(allBundlePath[j], allBundles[i]);
                    
                }
            }


            DeleteAB();
            //生成自己的配置表
            WriteData(resPathDic);

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BunleTargetPath,
                BuildAssetBundleOptions.ChunkBasedCompression, target);

            if (manifest == null)
            {
                Debug.LogError("AssetBundle 打包失败！");
            }
            else
            {
                Debug.Log("AssetBundle 打包完毕");
            }
        }


        static void WriteData(Dictionary<string, string> resPathDic)
        {
            AssetBundleConfig config = new AssetBundleConfig();
            config.ABList = new List<ABBase>();
            foreach (string path in resPathDic.Keys)
            {
                /*if (!ValidPath(path))
                    continue;*/

                ABBase abBase = new ABBase();
                abBase.Path = path;
                abBase.Crc = Crc32.GetCrc32(path);
                if (m_allBundleResourceCrc.Contains(abBase.Crc))
                {
                    Debug.LogErrorFormat("Crc 校验出错,请尝试更改资源名称 {0},或筛选资源清单", path);
                    return;
                }

                abBase.ABName = resPathDic[path];
                abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
                abBase.ABDependce = new List<string>();
                string[] resDependce = AssetDatabase.GetDependencies(path);
                for (int i = 0; i < resDependce.Length; i++)
                {
                    string tempPath = resDependce[i];
                    if (tempPath == path || path.EndsWith(".cs"))
                        continue;

                    string abName = "";
                    if (resPathDic.TryGetValue(tempPath, out abName))
                    {
                        if (abName == resPathDic[path])
                            continue;

                        if (!abBase.ABDependce.Contains(abName))
                        {
                            abBase.ABDependce.Add(abName);
                        }
                    }
                }

                config.ABList.Add(abBase);
            }

            config.FixedAB = m_fixedABName.Distinct().ToList();
            config.CacheAB = m_cacheABName.Distinct().ToList();
            

            //写入xml
            if (File.Exists(xmlPath)) File.Delete(xmlPath);
            FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(config.GetType());
            xs.Serialize(sw, config);
            sw.Close();
            fileStream.Close();

            //AssetDatabase.Refresh();
            //SetABName("assetbundleconfig", xmlPath);

            //写入二进制
            foreach (ABBase abBase in config.ABList)
            {
                abBase.Path = "";
            }
            FileStream fs = new FileStream(abResourcePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, config);
            fs.Close();
            //AssetDatabase.Refresh();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            SetABName("assetbundleconfig", abResourcePath);
        }


        private static void BundleResource()
        {
            BundleResource(abConfig.m_FixedFileConfigs, ResourceType.Fiexd, false);
            BundleResource(abConfig.m_AllFileConfigs, ResourceType.Default, false);
            BundleResource(abConfig.m_FixedFileConfigs, ResourceType.Fiexd, true);
            BundleResource(abConfig.m_AllFileConfigs, ResourceType.Default, true);
        }

        private static void BundleResource(List<ABConfig.ABFileConfig> resources, ResourceType type, bool isDir)
        {
            foreach (var fileConfig in resources)
            {
                if (fileConfig.isDir == isDir)
                {
                    if (fileConfig.isSingle)
                    {
                        List<string> subfilePath = Directory
                            .GetFiles(fileConfig.Path, "*", SearchOption.AllDirectories)
                            .Where(path => !path.EndsWith(".meta") && !path.EndsWith(".cs") && path != fileConfig.Path)
                            .ToList();

                        for (var i = 0; i < subfilePath.Count; i++)
                        {
                            subfilePath[i] = subfilePath[i].Replace('\\', '/');
                        }


                        AddBundle(subfilePath);

                        if (type == ResourceType.Fiexd)
                        {
                            AddFixedABPath(subfilePath);
                        } else if(type == ResourceType.Cache)
                        {
                            AddCacheABPath(subfilePath);
                        }
                    }
                    else
                    {
                        if (ContainsAB(fileConfig.ABName))
                        {
                            Debug.LogError("AB包配置名字重复，请检查！:" + fileConfig.ABName);
                            return;
                        }
                        else
                        {
                            AddBundle(fileConfig.ABName, fileConfig.Path);

                            if (type == ResourceType.Fiexd)
                            {
                                AddFixedAB(fileConfig.ABName);
                            }
                            else if(type == ResourceType.Cache)
                            {
                                AddCacheABPath(fileConfig.ABName);
                            }
                        }
                    }
                }
            }
        }

        private static void BundlePrefab()
        {
            BundlePrefab(abConfig.m_FixedPrefabConfigs, ResourceType.Fiexd);
            BundlePrefab(abConfig.m_AllPrefabConfigs, ResourceType.Default);
            
            BundlePrefab(abConfig.m_CacheFileConfigs, ResourceType.Cache);
        }

        private static void BundlePrefab(List<ABConfig.ABPrefabConfig> resources, ResourceType type)
        {
            List<string> allPrefabPath = new List<string>();

            allPrefabPath.AddRange(resources.Where(path => !path.isDir).Select(path => path.Path));
            string[] paths = resources.Where(path => path.isDir).Select(path => path.Path).ToArray();
            if (paths.Length > 0)
            {
                string[] allStr = AssetDatabase.FindAssets("t:Prefab", paths);
                for (int i = 0; i < allStr.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
                    if (!allPrefabPath.Contains(path))
                    {
                        allPrefabPath.Add(path);
                    }
                }
            }


            for (int i = 0, cout = allPrefabPath.Count; i < allPrefabPath.Count; i++)
            {
                var path = allPrefabPath[i];
                EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / cout);
                if (!ContainsBundle(path))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string[] allDepend = AssetDatabase.GetDependencies(path)
                        .Where(prefabPath => !prefabPath.EndsWith(".cs")).ToArray();
                    
                    string abName = obj.name;

                    if (type == ResourceType.Fiexd)
                    {
                        AddFixedAB(obj.name);
                    }else if(type == ResourceType.Cache)
                    {
                        AddCacheABPath(obj.name);
                    }

                    for (int j = 0; j < allDepend.Length; j++)
                    {
                        AddBundle(abName,allDepend[j]);
                    }
                }
            }
        }


        private static void AddFixedAB(string abName)
        {
            if (abConfig.isGenerateName)
            {
                m_fixedABName.Add(GetCrcName(abName).ToString());
            }
            else
            {
                m_fixedABName.Add(abName);
            }
        }

        private static void AddFixedABPath(string path)
        {
            if (abConfig.isGenerateName)
            {
                m_fixedABName.Add(GetCrcName(FileUtil.GetPathName(path)).ToString());
            }
            else
            {
                m_fixedABName.Add(FileUtil.GetPathName(path));
            }
        }

        private static void AddFixedABPath(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddFixedABPath(path);
            }
        }
        
        private static void AddCacheABPath(string path)
        {
            
            if (abConfig.isGenerateName)
            {
                m_cacheABName.Add(GetCrcName(FileUtil.GetPathName(path)).ToString());
            }
            else
            {
                m_cacheABName.Add(FileUtil.GetPathName(path));
            }
        }
        
        private static void AddCacheABPath(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddCacheABPath(path);
            }
        }

        private static void AddBundle(string path)
        {
            AddBundle(FileUtil.GetPathName(path), path);
        }

        private static void AddBundle(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddBundle(path);
            }
        }

        private static void AddBundle(string name, string path)
        {
            if (!ContainsBundle(path))
            {
                allBundlePath.Add(path);
                m_AllBundle.Add(path, name);
                SetABName(name, path);
            }
        }

        private static void AddBundle(string name, IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddBundle(name, path);
            }
        }

        static void SetABName(string name, string path)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError("不存在此路径文件：" + path);
            }
            else
            {
                /*if (name.Equals(prefabOtherABName))
                {
                    Debug.LogFormat("建议将此文件 {0} 单独打成AB包名",path);
                }*/
                
                if (abConfig.isGenerateName)
                {
                    assetImporter.assetBundleName = GetCrcName(name).ToString();
                }
                else
                {
                    assetImporter.assetBundleName = name;
                }
            }
        }

        static void SetABName(string name, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                SetABName(name, paths[i]);
            }
        }


        /// <summary>
        /// 删除无用AB包
        /// </summary>
        static void DeleteAB()
        {
            //获取当前所有配置的AB包名字
            string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
            DirectoryInfo direction = new DirectoryInfo(m_BunleTargetPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (allBundlesName.Contains(files[i].Name) || files[i].Name.EndsWith(".meta") ||
                    files[i].Name.EndsWith(".manifest") || files[i].Name.EndsWith("assetbundleconfig"))
                {
                    continue;
                }
                else
                {
                    //删除多余AB
                    Debug.Log("此AB包已经被删或者改名了：" + files[i].Name);
                    if (File.Exists(files[i].FullName))
                    {
                        File.Delete(files[i].FullName);
                    }

                    if (File.Exists(files[i].FullName + ".manifest"))
                    {
                        File.Delete(files[i].FullName + ".manifest");
                    }
                }
            }
        }

        private static void ClearABName()
        {
            string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < oldABNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
                EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1.0f / oldABNames.Length);
            }
        }

        private static bool ContainsBundle(string path)
        {
            return allBundlePath.Any(abPath =>
                path.Equals(abPath) || (path.Contains(abPath) && (path.Replace(abPath, "")[0] == '/')));
        }

        /// <summary>
        /// 提供给依赖打包的方法
        /// 当前依赖已经在Bundle中
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private static bool ContainsAB(string abName)
        {
            return !abConfig.isGenerateName && m_AllBundle.ContainsKey(abName);
        }


        private static uint GetCrcName(string name)
        {
            if (m_ABCrcName.ContainsKey(name))
            {
                return m_ABCrcName[name];
            }

            var crc = Crc32.GetCrc32(name);
            m_ABCrcName.Add(name, crc);
            if (m_allBundleNameCrc.Contains(crc))
            {
                throw new Exception(string.Format("生成Crc出错.{0}已有重复的,请重命名", name));
            }

            m_allBundleNameCrc.Add(crc);
            return crc;
        }
        
        
        /// <summary>
        /// 加密AB包
        /// </summary>
        public static void EncryptAB()
        {
            var abPaths = Directory.GetFiles(m_BunleTargetPath);
            foreach (var abPath in abPaths)
            {
                if (!abPath.EndsWith("meta") && !abPath.EndsWith(".manifest"))
                {
                    AESEncrypt.AESFileEncrypt(abPath);
                }
            }
            Debug.Log("加密完成");
        }

        /// <summary>
        /// 生成资源清单方便热更
        /// </summary>
        public static void GenerateResList(RuntimePlatform platform)
        {
            var resList = new ResList();
            resList.AllRes = new List<ABRes>();
            
            var abPaths = Directory.GetFiles(m_BunleTargetPath);
            foreach (var abPath in abPaths)
            {
                if (!abPath.EndsWith("meta") && !abPath.EndsWith(".manifest") && File.Exists(abPath))
                {
                    var abRes = new ABRes();
                    var bytes = File.ReadAllBytes(abPath);
                   
                    abRes.Crc = Crc32.GetCrc32(bytes);
                    abRes.Path =  FileUtil.GetPathName(abPath);
                    abRes.Length = bytes.Length;
                    resList.AllRes.Add(abRes);

                }
            }
            VerInfo verInfo = new VerInfo(){AppVer = AppConfig.AppVersion,ResVer = AppConfig.ResVersion};
            FileUtil.SerializeObjToFile(AppConfig.GetBuildPath(platform,AppConfig.ResListName),resList);
            FileUtil.SerializeObjToFile(AppConfig.GetBuildPath(platform,AppConfig.VerInfoName),verInfo);
        }

        public static bool isClearScript;
        
       

        /// <summary>
        /// 撤销操作.以还原编辑器工具代码
        /// </summary>
        public static void UndoEditorScript()
        {
            if (isClearScript)
            {
                Undo.PerformUndo();
                isClearScript = false;
            }
        }
        

        public static void MoveAB(RuntimePlatform platform)
        {
            
            FileUtil.Copy(AppConfig.GetBuildPath(platform),m_BunleLocalPath);
        }
        
    }
}