/*
* @Author: name
* @LastEditors: name
* @Description:
* @Date: ${YEAR}-${MONTH}-${DAY} ${TIME}
* @Modify:
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using XLua;
using Object = UnityEngine.Object;

namespace XLuaDemo
{
    [LuaCallCSharp]
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        private AssetBundleConfig config;

        /// <summary>
        /// 资源关系依赖表,可以通过crc找到依赖表
        /// </summary>
        private Dictionary<uint, ResourceItem> _allResource = new Dictionary<uint, ResourceItem>();

        /// <summary>
        /// 当前加载过的AB
        /// </summary>
        private Dictionary<string, AssetBundleItem> _allAssetBundleItems = new Dictionary<string, AssetBundleItem>();

        private ClassObjectPool<AssetBundleItem>
            assetPool = ClassPoolFactor.Instance.CreateClassPool<AssetBundleItem>(500);

        private List<ResourceItem> cacheResource = new List<ResourceItem>();


        private AssetBundleManager()
        {
        }

        /// <summary>
        /// 加载AB配置表
        /// </summary>
        private void LoadABConfig()
        {
            string path = AppConfig.ABPath + Crc32.GetCrc32("assetbundleconfig");
           
            //DEMO 不加密
            
            var configBundle = LoadBundle(path);
            //var configBundle = AssetBundle.LoadFromFile(path);
            var textAsset = configBundle.LoadAsset<TextAsset>("AssetBundleConfig");
            if (textAsset == null)
            {
                Debug.LogError("AB 配置文件不存在");
            }

            MemoryStream stream = new MemoryStream(textAsset.bytes);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            config = (AssetBundleConfig) binaryFormatter.Deserialize(stream);

            _allResource = new Dictionary<uint, ResourceItem>(config.ABList.Count);

            foreach (var abBase in config.ABList)
            {
                ResourceItem item = new ResourceItem()
                {
                    Crc = abBase.Crc,
                    AssetName = abBase.AssetName,
                    ABName = abBase.ABName,
                    ABDependce = abBase.ABDependce,
                };

                _allResource.Add(item.Crc, item);
                if (config.CacheAB.Contains(abBase.ABName) && abBase.AssetName.EndsWith(".prefab"))
                {
                    cacheResource.Add(item);
                }
            }


            Debug.Log(_allResource.Count);
        }

        public IEnumerator LoadFixedAB(Action<float> onFinish)
        {
            LoadABConfig();
            for (int i = 0, count = config.FixedAB.Count - 1; i < config.FixedAB.Count; i++)
            {
                AssetBundle bundle = LoadAssetBundle(config.FixedAB[i]);
                bundle.LoadAllAssets();
                onFinish((float) i / count);
                yield return null;
            }
        }

        private AssetBundle LoadBundle(string path)
        {
            //AssetBundle.LoadFromMemory(AESEncrypt.AESFileByteDecryptAB(path));
            return AssetBundle.LoadFromFile(path);
        }

        public ResourceItem LoadAsset(uint crc)
        {
            ResourceItem item = null;
            if (_allResource.TryGetValue(crc, out item))
            {
                //如果bundle不存在
                if (item.AssetBundle == null)
                {
                    //加载bundle
                    item.AssetBundle = LoadAssetBundle(item.ABName);

                    //加载引用
                    item.ABDependce.ForEach(abName => LoadAssetBundle(abName));
                }

                return item;
            }

            Debug.LogError("资源表出错 " + crc);
            return null;
        }

        /// <summary>
        /// 加载bundle,如果已经加载过.会让引用计数增加
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string abName)
        {
            AssetBundleItem item = null;

            if (_allAssetBundleItems.TryGetValue(abName, out item))
            {
                item.Refcount++;
            }
            else
            {
                item = assetPool.Spawn();
                item.AssetBundle = LoadBundle(AppConfig.ABPath + abName);
                //item.AssetBundle = AssetBundle.LoadFromFile(FileConfig.ABPath + abName);
                item.Refcount++;
                _allAssetBundleItems.Add(abName, item);
            }

            return item.AssetBundle;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="item"></param>
        public void ReleaseAsset(ResourceItem item)
        {
            item.ABDependce.ForEach(abName => UnloadAssetBundle(abName));

            if (UnloadAssetBundle(item.ABName))
            {
                item.AssetBundle = null;
            }
        }

        private bool UnloadAssetBundle(string abName)
        {
            AssetBundleItem item;

            if (_allAssetBundleItems.TryGetValue(abName, out item))
            {
                item.Refcount--;
                if (item.Refcount <= 0)
                {
                    item.AssetBundle.Unload(true);
                    assetPool.Recycle(item);
                    return true;
                }
            }

            return false;
        }

        internal ResourceItem FindAsset(uint crc)
        {
            if (_allResource.ContainsKey(crc))
            {
                return _allResource[crc];
            }
            else
            {
                //当没有找到.代表是从editor中加载
                var item = new ResourceItem();
                item.Crc = crc;
                _allResource.Add(crc, item);
                return item;
            }
        }

        internal ResourceItem[] FindAssetByName(string abName)
        {
            return _allResource.Values.Where(item => item.ABName == abName).ToArray();
        }
        
        internal List<ResourceItem> GetCacheConfig()
        {
            return cacheResource;
        }
    }

    public class AssetBundleItem : IPoolItem
    {
        public AssetBundle AssetBundle;

        public int Refcount;

        public void OnInit()
        {
            AssetBundle = null;
            Refcount = 0;
        }

        public void OnRecycle()
        {
        }

        public void Spawn()
        {
        }
    }

    public class ResourceItem
    {
        /// <summary>
        /// 该资源的Crc
        /// </summary>
        public uint Crc;

        /// <summary>
        /// 该资源的资源名称
        /// </summary>
        public string AssetName = String.Empty;

        /// <summary>
        /// 该资源的AB包命
        /// </summary>
        /// <returns></returns>
        public string ABName = String.Empty;

        /// <summary>
        /// 该资源所依赖的AB包名
        /// </summary>
        /// <returns></returns>
        public List<string> ABDependce;

        /// <summary>
        /// 该资源所在的AB包
        /// </summary>
        public AssetBundle AssetBundle;

        /***************************Resource使用********************************/

        /// <summary>
        /// 加载出来的资源对象
        /// </summary>
        public Object Obj = null;

        /// <summary>
        /// 最后使用的时间
        /// </summary>
        public float m_lastUseTime = 0.0f;

        /// <summary>
        /// 引用计数
        /// </summary>
        protected int m_Refcount = 0;

        public Dictionary<string, Sprite> AllSprites;

        public int Refcount;

        public bool Clear { get; set; }
    }
}