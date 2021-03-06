/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace XLuaDemo
{
    [System.Serializable]
    public class AssetBundleConfig
    {
        [XmlElement("ABList")]
        public List<ABBase> ABList { get; set; }

        [XmlElement("FixedAB")]
        public List<string> FixedAB { get; set; }
        
        [XmlElement("CacheAB")]
        public List<string> CacheAB { get; set; }
    }

    [System.Serializable]
    public class ABBase
    {
        [XmlAttribute("ABName")]
        public string ABName { get; set; }
    
        [XmlAttribute("Path")]
        public string Path{ get; set; }
        [XmlAttribute("Crc")]
        public uint Crc { get; set; }
    
        [XmlAttribute("AssetName")]
        public string AssetName { get; set; }
        [XmlElement("ABDependce")]
        public List<string> ABDependce { get; set; }
    }

    [System.Serializable]
    public class ABRes
    {

        public string Path;

        public uint Crc;


        public int Length;
        
        public void Update(ABRes abRes)
        {
            this.Crc = abRes.Crc;
            this.Path = abRes.Path;
            this.Length = abRes.Length;
        }

    }

    [Serializable]
    public class ResList
    {
        public List<ABRes> AllRes = new List<ABRes>();
    }

    [Serializable]
    public class VerInfo
    {
        public string AppVer;

        public string ResVer;
    }
}