using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ABConfig", menuName = "CreateABConfig", order = 4)]
public class ABConfig : ScriptableObject
{
    /// <summary>
    /// 打包自动生成名字.采用全路径的crc为Bundle名字
    /// </summary>
    [LabelText("类名")]
    public bool isGenerateName;
    
    /// <summary>
    /// 所有AB预制体配置
    /// </summary>
    [Header("预制体配置")]
    public List<ABPrefabConfig> m_AllPrefabConfigs = new List<ABPrefabConfig>();
    
    /// <summary>
    /// 所有AB文件配置
    /// </summary>
    [Header("资源配置")]
    public List<ABFileConfig> m_AllFileConfigs = new List<ABFileConfig>();
    
    
    /// <summary>
    /// 所有AB预制体配置
    /// </summary>
    [Header("固定预制体配置")]
    public List<ABPrefabConfig> m_FixedPrefabConfigs = new List<ABPrefabConfig>();
    
    /// <summary>
    /// 所有AB文件配置
    /// </summary>
    [Header("固定资源配置")]
    public List<ABFileConfig> m_FixedFileConfigs = new List<ABFileConfig>();
    
    /// <summary>
    /// 所有AB文件配置
    /// </summary>
    [Header("缓存资源配置")]
    public List<ABPrefabConfig> m_CacheFileConfigs = new List<ABPrefabConfig>();

    [Serializable]
    public struct ABFileConfig
    {
        public bool isDir
        {
            get { return Directory.Exists(Path); }
        }
        
        [LabelText("是否将文件夹下内容单独打包")]
        public bool isSingle;

        /// <summary>
        /// AB包名,如果自动生成AB包名此栏可以不填
        /// </summary>
        [LabelText("AB包名")]
        public string ABName;
        
        [LabelText("路径(文件夹或文件)")]
        public string Path;
    }
    
    [Serializable]
    public struct ABPrefabConfig
    {
        public bool isDir
        {
            get { return Directory.Exists(Path); }
        }

        //public bool isSingle;
        [LabelText("路径(文件夹或文件)")]
        public string Path;
    }
}