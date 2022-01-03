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
using UnityEngine;
using XLua;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace XLuaDemo
{
    [LuaCallCSharp]
    public class ResourceManager : Singleton<ResourceManager>
    {
        private static bool isAB =  AppConfig.IsBundle;
        WaitForSeconds localWait = new WaitForSeconds(0.05f);


        static ResourceManager()
        {

        }

        private Dictionary<uint, ResourceItem> allCacheItems = new Dictionary<uint, ResourceItem>();
        
        private Dictionary<string,Sprite> allSprite = new Dictionary<string, Sprite>();

        private ResourceManager()
        {
        }

        public T Load<T>(string path) where T : Object
        {
            var crc = Crc32.GetCrc32(path);
            
            ResourceItem item = GetCacheResourceItem(crc);
            
            if (item != null)
            {
                return item.Obj as T;
            }
            
            T obj = null;
            
#if UNITY_EDITOR
            if (!isAB)
            {
                item = AssetBundleManager.Instance.FindAsset(crc);
                obj = LoadByEditor<T>(path);
                if (!obj)
                {
                    Debug.LogErrorFormat(path +"不存在,请检查路径和后缀是否正确");
                }
            }
#endif

            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadAsset(crc);
                obj = item.AssetBundle.LoadAsset<T>(item.AssetName);
            }

            CacheResource(ref item,obj,crc);
            return obj;
        }
        
        public Sprite LoadSprite(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            Sprite sprite;
            if (!allSprite.TryGetValue(fullPath,out sprite))
            {
                var abPath = fullPath.Substring(0, fullPath.LastIndexOf('/'));
                string spriteName = fullPath.Substring(fullPath.LastIndexOf('/')+1);
                if (!fullPath.Contains("."))
                {
                    abPath = abPath + ".png";
                }
                try
                {
                    sprite = LoadSprite(abPath,spriteName);
                }
                catch (Exception e)
                {
                   Debug.LogErrorFormat(fullPath +"不存在,请检查路径和后缀是否正确");
                }
                
            }
            return sprite;
        }
        
        private Sprite LoadSprite(string bundlePath,string name)
        {
            Sprite sprite;
            var crc = Crc32.GetCrc32(bundlePath);
            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null)
            {
                sprite = item.AllSprites[name];
            }
            else
            {
                Object[] objs = null;
#if UNITY_EDITOR
                if (!isAB)
                {
                    item = AssetBundleManager.Instance.FindAsset(crc);
                    objs = LoadAllByEditor(bundlePath);
                }
#endif

                if (objs == null)
                {
                    item = AssetBundleManager.Instance.LoadAsset(crc);
                    objs = item.AssetBundle.LoadAssetWithSubAssets(item.AssetName);
                }
                Dictionary<string,Sprite> sprites = new Dictionary<string, Sprite>();
                foreach (var obj in objs)
                {
                    var oneSprite = obj as Sprite;
                    if (oneSprite)
                    {
                        sprites.Add(obj.name,oneSprite);
                    }
                }
                sprite = sprites[name];
                CacheSpriteResource(ref item,sprites,crc);
            }
            return sprite;
        }

        private void CacheResource(ref ResourceItem item,Object obj,uint crc)
        {
            item.Refcount++;
            item.Obj = obj;
            item.m_lastUseTime = Time.realtimeSinceStartup;
            allCacheItems.Add(crc,item);
        }
        
        private void CacheSpriteResource(ref ResourceItem item,Dictionary<string,Sprite> sprites,uint crc)
        {
            item.Refcount++;
            item.AllSprites = sprites;
            item.m_lastUseTime = Time.realtimeSinceStartup;
            allCacheItems.Add(crc,item);
        }

        private ResourceItem GetCacheResourceItem(uint crc)
        {
            ResourceItem item = null;
            if (allCacheItems.TryGetValue(crc, out item))
            {
                item.Refcount++;
                item.m_lastUseTime = Time.realtimeSinceStartup;
            }

            return item;
        }

#if UNITY_EDITOR
        private T LoadByEditor<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        
        private UnityEngine.Object[] LoadAllByEditor(string path)
        {
            return AssetDatabase.LoadAllAssetsAtPath(path);
        }
        
        
#endif
        internal void LoadResource(string path, ref ResourceObj resourceObj)
        {
            uint crc = resourceObj.Crc == 0 ? Crc32.GetCrc32(path) : resourceObj.Crc;

            ResourceItem item = GetCacheResourceItem(crc);
            if (item != null)
            {
                resourceObj.ResItem = item;
                return;
            }

            Object obj = null;
#if UNITY_EDITOR
            if (!isAB)
            {
                item = AssetBundleManager.Instance.FindAsset(crc);
                if (item != null && item.Obj != null)
                {
                    obj = item.Obj;
                }
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.Crc = crc;
                    }
                    obj = LoadByEditor<Object>(path);
                }
            }
#endif

            if (obj == null)
            {
                item = AssetBundleManager.Instance.LoadAsset(crc);
                if (item != null && item.AssetBundle != null)
                {
                    if (item.Obj != null)
                    {
                        obj = item.Obj;
                    }
                    else
                    {
                        obj = item.AssetBundle.LoadAsset<Object>(item.AssetName);
                    }
                }
            }

            CacheResource(ref item, obj,crc);
            resourceObj.ResItem = item;
            //item.Clear = resourceObj.BClear;
        }

        internal void DecreaseResoucerRef(ResourceObj resObj)
        {
            var count = DecreaseResoucerRef(resObj.Crc);
            if (count == 0)
            {
                Debug.Log("可以回收");
            }
            Debug.Log(count);
        }
        
        private int DecreaseResoucerRef(uint crc)
        {
            ResourceItem item = null;
            if (!allCacheItems.TryGetValue(crc, out item) || item == null)
                return 0;

            item.Refcount--;

            return item.Refcount;
        }

        public void LoadSprite(string path,LuaFunction func = null)
        {
            LoadAsync<Sprite>(path,null, func);
        }

        public void LoadAsync<T>(string path,Action<ResourceItem> action,LuaFunction func = null) where T : Object
        {
            var crc = Crc32.GetCrc32(path);
            
            ResourceItem item = GetCacheResourceItem(crc);
            
            if (item != null)
            {
                var itemObj = item.Obj as T;
                action?.Invoke(item);
                return;
            }
            
            
#if UNITY_EDITOR
            if (!isAB)
            {
                item = AssetBundleManager.Instance.FindAsset(crc);
                GameManager.Instance.StartCoroutine(LoadAssetInLocal<T>(path, obj =>
                {
                    CacheResource(ref item, obj,crc);
                    action?.Invoke(item);
                }, func));
                //obj = LoadByEditor<T>(path);
                /*if (!obj)
                {
                    Debug.LogErrorFormat(path +"不存在,请检查路径和后缀是否正确");
                }*/
                return;

            }
#endif

            item = AssetBundleManager.Instance.LoadAsset(crc);
            GameManager.Instance.StartCoroutine(LoadAsset<T>(item.AssetBundle,path, obj =>
            {
                CacheResource(ref item, obj,crc);
                action?.Invoke(item);
            }, func));
            
        }
        
        IEnumerator LoadAssetInLocal<T>(string path, Action<T> action = null, LuaFunction func = null) where T : Object
        {
            yield return localWait;
#if UNITY_EDITOR
            T res = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (res != null)
            {
                if (func != null)
                {
                    List<T> list = new List<T>();
                    list.Add(res);
                    object[] args = new object[] { list.ToArray() };
                    func.Call(args);
                    func.Dispose();
                    func = null;
                }
                else if (action != null)
                {
                    //List<Object> list = new List<Object>();
                    //list.Add(res);
                    //Object args = list.ToArray();
                    action(res);
                }
                
            }
            else
            {
                Debug.Log("ResourceManager:LoadAsset Error:cannot find file:" + path);
            }
#endif
        }

        /// <summary>
        /// 先异步加载资源,后续会异步加载所有的,包括AB包和资源
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="action"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerator LoadAsset<T>(AssetBundle bundle,string assetName, Action<T> action = null, LuaFunction func = null) where T : Object
        {
            var request = bundle.LoadAssetAsync<T>(assetName);
            yield return request;

            action(request.asset as T);
            func?.Call( request.asset as T);
        }

    }
}