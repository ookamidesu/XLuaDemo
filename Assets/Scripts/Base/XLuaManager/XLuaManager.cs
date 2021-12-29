using System;
using System.Collections.Generic;
using System.IO;
using XLuaDemo;
using UnityEngine;
using XLua;


[Hotfix]
[LuaCallCSharp]
public class XLuaManager : MonoSingleton<XLuaManager>
{
    Action<float, float> luaUpdate = null;
    Action luaLateUpdate = null;
    Action<float> luaFixedUpdate = null;
    
    LuaEnv luaEnv = null;

    protected  void Start()
    {
        InitLuaEnv();
    }

    /// <summary>
    /// 初始化lua
    /// </summary>
    private void InitLuaEnv()
    {
        luaEnv = new LuaEnv();
        if (luaEnv != null)
        {
            //添加自定义的加载器
            luaEnv.AddLoader(CustomLoader);
            InitExternal();
            LoadScript("LuaMain");
          
           
           

            luaEnv.Global.Get("Update",out luaUpdate);
            luaEnv.Global.Get("LateUpdate",out luaLateUpdate);
            luaEnv.Global.Get("FixedUpdate",out luaFixedUpdate);
            
            SafeDoString("LuaStart()");
            /*if (NetworkManager.GetInstance())
            {
                SafeDoString("require 'NetDispatcher'");
                NetworkManager.GetInstance().OnConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnConnectServer");
                NetworkManager.GetInstance().OnDisConnectCallBack += luaEnv.Global.Get<Action<byte[]>>("OnDisConnectFromServer");
                NetworkManager.GetInstance().OnReceiveLineCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveLineFromServer");
                NetworkManager.GetInstance().OnReceiveMsgCallBack += luaEnv.Global.Get<Action<byte[]>>("OnReceiveMsgFromServer");
            }
            else
                Debug.LogError("must init network manager before init xlua manager!");*/
        }
        /*else
        {
            Debug.LogError("InitLuaEnv failed!");
        }*/
    }

    private void InitExternal()
    {
        
    }
    
    public static byte[] CustomLoader(ref string filepath)
    {
        string scriptPath = string.Empty;
        filepath = filepath.Replace(".", "/") + ".lua";
        scriptPath = Path.Combine(AppConfig.LuaAssetsDir, filepath);
        return Util.GetFileBytes(scriptPath);
    }
    
    void LoadScript(string scriptName)
    {
        //直接引入对应lua文件
        SafeDoString(string.Format("require('{0}')", scriptName));
    }
    
    public void SafeDoString(string scriptContent, string chunkName="chunk")
    {
        if (luaEnv != null)
        {
            try
            {
                Debug.Log("****************"+scriptContent);
                luaEnv.DoString(scriptContent, chunkName);
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg, null);
            }
        }
    }

    /// <summary>
    /// 在update中调用lua的update方法
    /// </summary>
    private void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
            if (luaUpdate != null)
            {
                try
                {
                    luaUpdate(Time.deltaTime, Time.unscaledDeltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogError("luaUpdate err : " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            // if (Time.frameCount % 6000 == 0)
            // {
            //     luaEnv.FullGc();
            // }
        }
    }

    private void LateUpdate() {
        if (luaLateUpdate != null)
        {
            try
            {
                luaLateUpdate();
            }
            catch (Exception ex)
            {
                Debug.LogError("luaLateUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
    
    private void FixedUpdate() {
        if (luaFixedUpdate != null)
        {
            try
            {
                luaFixedUpdate(Time.fixedDeltaTime);
            }
            catch (Exception ex)
            {
                Debug.LogError("luaFixedUpdate err : " + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}

    
#if UNITY_EDITOR
/// <summary>
/// update 相关编译列表
/// </summary>
public static class LuaUpdaterExporter
{
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>()
    {
        typeof(Action),
        typeof(Action<byte[]>),
        typeof(Action<float>),
        typeof(Action<float, float>),
    };
}
#endif
