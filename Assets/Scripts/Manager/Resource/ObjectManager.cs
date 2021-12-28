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

namespace XLuaDemo
{
    public class ResourceObj : IPoolItem
    {
        //路径对应CRC
        public uint Crc = 0;
        //存ResourceItem
        public ResourceItem ResItem = null;
        //实例化出来的GameObject
        public GameObject CloneObj = null;
        //是否跳场景清除
        public bool BClear = true;
        //储存GUID
        public int Guid = 0;
        //是否已经放回对象池
        public bool Already = false;
        //--------------------------------
        //是否放到场景节点下面
        public bool SetSceneParent = false;
        public void OnInit()
        {
            Already = true;
        }

        public void OnRecycle()
        {
            Crc = 0;
            CloneObj = null;
            BClear = true;
            Guid = 0;
            ResItem = null;
            SetSceneParent = false;
        }

        public void Spawn()
        {
            Already = false;
        }
    }
    
    public class ObjectManager: MonoSingleton<ObjectManager>
    {
        /// <summary>
        /// 缓存的对象池
        /// </summary>
        private Dictionary<uint, List<ResourceObj>> _objectPoolDic = new Dictionary<uint, List<ResourceObj>>();
        
        private ClassObjectPool<PoolItemList<IResourceItem>>
            listPool = ClassPoolFactor.Instance.CreateClassPool<PoolItemList<IResourceItem>>(10);
        
        /// <summary>
        /// 所有的对象
        /// </summary>
        private Dictionary<int,ResourceObj> _allObjects = new Dictionary<int, ResourceObj>();
        
        /*/// <summary>
        /// 暂时缓存,后续缓存对象管理
        /// </summary>
        private Dictionary<int,bool> allObjBeClear = new Dictionary<int, bool>();*/
        
        /// <summary>
        /// 类对象池
        /// </summary>
        private ClassObjectPool<ResourceObj> _pool = ClassPoolFactor.Instance.CreateClassPool<ResourceObj>(100);


        protected override void OnCreate()
        {
            gameObject.SetActive(false);
        }

        public void InitCache()
        {
            var items = AssetBundleManager.Instance.GetCacheConfig();
            foreach (var resourceItem in items)
            {
                ResourceObj resourceObj = _pool.Spawn();
                uint crc = resourceItem.Crc;
                resourceObj.Crc = crc;
                resourceObj.BClear = false;
                
                ResourceManager.Instance.LoadResource(resourceItem.AssetName,ref resourceObj);
                if (resourceObj.ResItem.Obj != null)
                {
                    resourceObj.CloneObj = Instantiate(resourceObj.ResItem.Obj, transform) as GameObject;
                    OnLoadObj(resourceObj.CloneObj);
                }
                
                resourceObj.Already = false;
                resourceObj.Guid = resourceObj.CloneObj.GetInstanceID();
                if (!_allObjects.ContainsKey(resourceObj.Guid))
                {
                    _allObjects.Add(resourceObj.Guid,resourceObj);
                }
                //resourceObj.CloneObj.transform.Reset();
                RecycleObject(resourceObj.CloneObj);
            }
        }

        public GameObject InstantiateObject(string path,Transform parent = null, bool bClear = true)
        {
            uint crc = Crc32.GetCrc32(path);
            ResourceObj resourceObj = GetObjectFromPool(crc);
            if (resourceObj == null)
            {
                resourceObj = _pool.Spawn();
                resourceObj.Crc = crc;
                resourceObj.BClear = bClear;

                ResourceManager.Instance.LoadResource(path,ref resourceObj);
                if (resourceObj.ResItem.Obj != null)
                {
                    resourceObj.CloneObj = Instantiate(resourceObj.ResItem.Obj,parent, transform) as GameObject;
                    OnLoadObj(resourceObj.CloneObj);
                }
            }

            resourceObj.Already = false;
            resourceObj.Guid = resourceObj.CloneObj.GetInstanceID();
            resourceObj.BClear = bClear;
            if (!_allObjects.ContainsKey(resourceObj.Guid))
            {
                _allObjects.Add(resourceObj.Guid,resourceObj);
            }
            //resourceObj.CloneObj.transform.Reset();
            
            //allObjBeClear[resourceObj.CloneObj.GetInstanceID()] = bClear;
            resourceObj.CloneObj.SetActive(true);
            return resourceObj.CloneObj;
        }

        private ResourceObj GetObjectFromPool(uint crc)
        {
            List<ResourceObj> st = null;
            if (_objectPoolDic.TryGetValue(crc, out st) && st != null && st.Count > 0)
            {
                ResourceObj resObj = st[0];
                st.RemoveAt(0);
                GameObject obj = resObj.CloneObj;
                if (!System.Object.ReferenceEquals(obj, null))
                {
                    /*if (!System.Object.ReferenceEquals(resObj.m_OfflineData, null))
                    {
                        resObj.m_OfflineData.ResetProp();
                    }*/
                    //resObj.m_Already = false;
#if UNITY_EDITOR
                    if (obj.name.EndsWith("(Recycle)"))
                    {
                        obj.name = obj.name.Replace("(Recycle)", "");
                    }
#endif
                }
                return resObj;
            }
            return null;
        }

        public void RecycleObject(GameObject obj)
        {
            ResourceObj resObj = null;
            int tempID = obj.GetInstanceID();
            if (!_allObjects.TryGetValue(tempID, out resObj))
            {
                Debug.Log(obj.name + "对象不是ObjectManager创建的！");
                return;
            }

            if (resObj == null)
            {
                Debug.Log("缓存的ResouceObj为空！");
                return;
            }

            if (resObj.Already)
            {
                Debug.Log("该对象已经放回对象池了，检测自己是否情况引用!");
                return;
            }

#if UNITY_EDITOR
            obj.name += "(Recycle)";
#endif

            List<ResourceObj> st = null;
            if (!_objectPoolDic.TryGetValue(resObj.Crc, out st) || st == null)
            {
                st = new List<ResourceObj>();
                _objectPoolDic.Add(resObj.Crc, st);
            }
            if (resObj.CloneObj)
            {
                resObj.CloneObj.transform.SetParent(transform); 
            }
            
            st.Add(resObj);
            resObj.Already = true;
            //_pool.Recycle(resObj);
           
        }

        public void DestroyObject(Object obj)
        {
            //todo 待完成销毁功能
            //ResourceManager做一个引用计数
            //ResourceManager.Instance.DecreaseResoucerRef(resObj);
        }

        public void OnSceneChange()
        {
            foreach (var resourceObj in _allObjects.Values)
            {
                if (!resourceObj.Already && resourceObj.BClear)
                {
                    RecycleObject(resourceObj.CloneObj);
                }
                
            }
        }

        public void OnLoadObj(GameObject obj)
        {
            var poolItemList = listPool.Spawn();
            obj.GetComponents(poolItemList);
            foreach (var item in poolItemList)
            {
                item.OnLoad();
            }
        }

        public void OnRecyclObj(GameObject obj)
        {
            var poolItemList = listPool.Spawn();
            obj.GetComponents(poolItemList);
            foreach (var item in poolItemList)
            {
                item.OnRecycle();
            }
        }

        public void InstantiateObjectAsync(string path,LuaFunction func,Transform parent = null,bool bClear = false)
        {
            XLuaManager.Instance.StartCoroutine(InstantiateAsync(path, parent,obj =>
            {
                
                func.Call(obj);
            }, bClear));

        }


        IEnumerator InstantiateAsync(string path,Transform parent,Action<GameObject> action,bool bClear = false)
        {
            Debug.Log($"加载{path}");
            yield return null;
            uint crc = Crc32.GetCrc32(path);
            ResourceObj resourceObj = GetObjectFromPool(crc);
            if (resourceObj == null)
            {
                resourceObj = _pool.Spawn();
                resourceObj.Crc = crc;
                resourceObj.BClear = bClear;
                
                ResourceManager.Instance.LoadAsync<GameObject>(path,item =>
                {
                    resourceObj.ResItem = item;
                    if (resourceObj.ResItem.Obj != null)
                    {
                        resourceObj.CloneObj = Instantiate(resourceObj.ResItem.Obj,parent, transform) as GameObject;
                    }
                    
                    resourceObj.Already = false;
                    resourceObj.Guid = resourceObj.CloneObj.GetInstanceID();
                    resourceObj.BClear = bClear;
                    if (!_allObjects.ContainsKey(resourceObj.Guid))
                    {
                        _allObjects.Add(resourceObj.Guid,resourceObj);
                    }
                    resourceObj.CloneObj.SetActive(true);

                    action(resourceObj.CloneObj);

                },null);
                yield break;

            }
            
            

            resourceObj.Already = false;
            resourceObj.Guid = resourceObj.CloneObj.GetInstanceID();
            resourceObj.BClear = bClear;
            if (!_allObjects.ContainsKey(resourceObj.Guid))
            {
                _allObjects.Add(resourceObj.Guid,resourceObj);
            }
            resourceObj.CloneObj.SetActive(true);
            action(resourceObj.CloneObj);

        }
        
    }
}