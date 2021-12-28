using System;
using System.Collections.Generic;
using System.IO;

namespace XLuaDemo
{
    public class ComponentTemplate
    {
        public static void WriteBindInfo(GenerateCodeTool codeInfo)
        {
            var scriptFile = codeInfo.ScriptFilePath;

            var codeFolder = $"{codeInfo.m_GenerateCodePath}/{codeInfo.m_Folder}";
            if (!Directory.Exists(codeFolder))//如果不存在就创建 dir 文件夹  
                Directory.CreateDirectory(codeFolder);

            if (File.Exists(scriptFile))
            {
                File.Delete(scriptFile);
            }
            FileStream fs = new FileStream(scriptFile, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs);
            //var writer = File.CreateText(scriptFile);
            //声明title，用上时间以便每次都能重新编译
            writer.WriteLine("--[[\n* OOKAMI\n* 脚本由生成器生成\n* " + DateTime.Now.ToString()+ "\n]]"  );
               
            //writer.WriteLine("namespace " + (string.IsNullOrEmpty(info.GenerateCodeNamespace)?"Default":info.GenerateCodeNamespace) );
            //writer.WriteLine("{");
            
            /*writer.WriteLine("\tpublic partial class " + (string.IsNullOrEmpty(info.GenerateScriptName)?info.name:info.GenerateScriptName));
            writer.WriteLine("\t{");*/
            
            List<string> allListName = new List<string>();
            foreach (var bindCodeInfo in codeInfo.m_AllComponents)
            {
                /*if (bindCodeInfo.IsList)
                {
                    if (allListName.Contains(bindCodeInfo.ListName))
                    {
                        continue;
                    }
                    writer.WriteLine(string.Format("\t\tpublic global::System.Collections.Generic.List<global::{0}> m_{1};",bindCodeInfo.obj?bindCodeInfo.obj.GetType().ToString():"UnityEngine.GameObject",bindCodeInfo.ListName));
                    allListName.Add(bindCodeInfo.ListName);
                }
                else
                {
                    writer.WriteLine(string.Format("\t\tpublic global::{0} m_{1};",bindCodeInfo.obj?bindCodeInfo.obj.GetType().ToString():"UnityEngine.GameObject",bindCodeInfo.Name));
                }*/
               
            }
            
            //writer.WriteLine("\t}");
            //writer.WriteLine("}");
            writer.Close();

        }
        
        /*public static void WriteMono(GenerateCodeInfo info,List<BindCodeInfo> bindInfos)
        {
            var scriptFile = info.GenerateCodeFolder +  "/" +(string.IsNullOrEmpty(info.GenerateScriptName)?info.name:info.GenerateScriptName) + ".cs";

            if (!Directory.Exists(info.GenerateCodeFolder))//如果不存在就创建 dir 文件夹  
                Directory.CreateDirectory(info.GenerateCodeFolder);
            if (File.Exists(scriptFile))
            {
                return;
            }
            
            var writer = File.CreateText(scriptFile);
            
            writer.WriteLine("using UnityEngine;");
            
            writer.WriteLine("/*\n* OOKAMI\n* 脚本由生成器生成\n#1#" );

            writer.WriteLine("namespace " + (string.IsNullOrEmpty(info.GenerateCodeNamespace)?"Default":info.GenerateCodeNamespace) );
            writer.WriteLine("{");
            writer.WriteLine("\tpublic partial class " + (string.IsNullOrEmpty(info.GenerateScriptName)?info.name:info.GenerateScriptName) + " : MonoBehaviour" );
            writer.WriteLine("\t{");
           
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Close();
        }*/
    }
}
