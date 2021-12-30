using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace XLuaDemo
{
    public class ComponentTemplate
    {
        public static void WriteBindInfo(GenerateCodeTool codeInfo)
        {
            var scriptFile = codeInfo.ScriptGenerateFilePath;

            var codeFolder = $"{codeInfo.m_GenerateCodePath}/{codeInfo.m_Folder}";
            if (!Directory.Exists(codeFolder)) //如果不存在就创建 dir 文件夹  
                Directory.CreateDirectory(codeFolder);

            if (File.Exists(scriptFile))
            {
                File.WriteAllText(scriptFile, "");
            }

            var stringBuilder = new StringBuilder();
            //var writer = File.CreateText(scriptFile);
            //声明title，用上时间以便每次都能重新编译
            stringBuilder.AppendLine($"--[[\n-- OOKAMI\n-- 脚本由生成器生成\n-- {DateTime.Now.ToString()}\n-- 请勿更改\n-- 请勿更改\n-- 请勿更改 \n]]\n");
            //生成类基础信息

            stringBuilder.AppendLine($"---@class {codeInfo.m_ClassName} : {"Base" + codeInfo.viewType}");
            stringBuilder.AppendLine($"local {codeInfo.m_ClassName} = BaseClass({"Base" + codeInfo.viewType})");

            //生成构造器
            stringBuilder.AppendLine(
                $"function {codeInfo.m_ClassName}:Constructor()\n\tself.conf = {{\n\t\tprefabPath = {"\"" + codeInfo.prefabPath + "\";"}\n\t\troot ={"\"" + codeInfo.showLayer + "\";"}\n\t}}\nend");
            stringBuilder.AppendLine("");
            //生成初始化方法
            stringBuilder.AppendLine($"function {codeInfo.m_ClassName}:InitUIComponent(root)");
            foreach (var component in codeInfo.m_AllComponents)
            {
                if (string.IsNullOrEmpty(component.ComponentName))
                {
                    //没有组件.添加obj
                    stringBuilder.AppendLine(
                        $"\tself.{component.m_FildName} = root:Find({"\"" + component.nodePath + "\""}).gameObject");
                }
                else
                {
                    //添加组件
                    stringBuilder.AppendLine(
                        $"\tself.{component.m_FildName} = root:Find({"\"" + component.nodePath + "\""}):GetComponent({"\"" + component.ComponentName + "\""})");
                }

            }

            stringBuilder.AppendLine($"end");
            stringBuilder.AppendLine($"return {codeInfo.m_ClassName}");

            File.WriteAllText(scriptFile, stringBuilder.ToString());
            
            Debug.Log($"生成代码成功{scriptFile}");
            
            var normalScriptFile = codeInfo.ScriptFilePath;

            //生成正常脚本
            if (!File.Exists(normalScriptFile))
            {
                //只有不存在才生成
                stringBuilder.Clear();
                string luaHome = "Assets/LuaScripts/";

                string requirePath = scriptFile.Replace("Assets/LuaScripts/", "");
                var index = requirePath.LastIndexOf(".");
                requirePath = requirePath.Substring(0, index);

                stringBuilder.AppendLine($"---@class {codeInfo.m_ClassName} : {"Base" + codeInfo.viewType}");
                
                stringBuilder.AppendLine($"local {codeInfo.m_ClassName} = require({"\"" +requirePath + "\""})");
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine($"return {codeInfo.m_ClassName}");
                File.WriteAllText(normalScriptFile, stringBuilder.ToString());
            }

        }

    }
}
