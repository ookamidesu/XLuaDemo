using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace XLuaDemo
{
    public class FileUtil
    {
        public static void Copy(string srcPath, string targetPath)
        {
            try
            {
                Debug.Log(targetPath);
                Debug.Log(srcPath);
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

                string[] files = Directory.GetFileSystemEntries(srcPath);
                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                    {
                        Copy(file, Path.Combine(targetPath,GetPathName(file)));
                    }
                    else
                    {
                        File.Copy(file, targetPath + Path.DirectorySeparatorChar + Path.GetFileName(file), true);
                    }
                }

            }
            catch
            {
                Debug.LogError("无法复制：" + srcPath + "  到" + targetPath);
            }
        }
        
        public static string GetPathName(string path)
        {
            var last = path.LastIndexOf('.');
            if (last < 0)
            {
                last = path.Length;
            }
            var start1 = path.LastIndexOf('/')+1;
            var start2= path.LastIndexOf('\\')+1;

            var start = start1 > start2 ? start1 : start2;
        
            return path.Substring(start, last - start);
        }

        static Encoding utf8WithoutNoBom =new UTF8Encoding(false);
        public static void SerializeObjToFile(string filePath,object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
           
            File.WriteAllText(filePath,json,utf8WithoutNoBom);
        }
        public static T DeserializeByFile<T>(string filePath)
        {
            if(File.Exists(filePath))
                return DeserializeByString<T>(File.ReadAllText(filePath));
            else
                return DeserializeByString<T>("");
        }
        
        public static T  DeserializeByString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}