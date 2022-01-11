using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLuaDemo
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public enum State{
            None,//无
            UpdateResourceFromNet,//热更阶段：从服务器上拿到最新的资源
            InitAssetBundle,//初始化AssetBundle
            StartLogin,//登录流程
            StartGame,//正式进入场景游戏
            Playing,//完成启动流程了，接下来把控制权交给玩法逻辑
        }
        
        public enum SubState{
            Enter,
            Update,
        }

        private State currentState;
        
        private SubState currentSubState;

        private bool isLoadBundle;

        private LoginPanel _loginPanel;

        private XLuaManager xLuaManager;
        
        void Awake()
        {
            
            AppConfig.Init();
        }
        
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            
            
            this.gameObject.AddComponent<LocalLogHandler>();
            this.gameObject.AddComponent<ServerLogHandler>();
            xLuaManager = this.gameObject.AddComponent<XLuaManager>();
            _loginPanel = LoginPanel.Instance;
          
            currentState = State.UpdateResourceFromNet;

        }


      

        // Update is called once per frame
        void Update()
        {
            if (currentState == State.Playing || currentState == State.None)
                return;

            switch (currentState)
            {
                case State.UpdateResourceFromNet:
                    if (currentSubState == SubState.Enter)
                    {
                        _loginPanel.StartUpdateRes();
                        AssetsHotFixManager.Instance.CheckHotFix(_loginPanel.OnUpdateProgress,_loginPanel.OnUpdateDownload, () =>
                        {
                            _loginPanel.EndUpdateRes();
                            currentState = State.InitAssetBundle;
                            currentSubState = SubState.Enter;
                        });
                        currentSubState = SubState.Update;
                    }
                    break;
                case State.InitAssetBundle:
                    if (currentSubState == SubState.Enter)
                    {
                        _loginPanel.SetTips("加载资源中");
                        StartCoroutine(AssetBundleManager.Instance.LoadFixedAB(progress =>
                        {
                            _loginPanel.SetPercent(progress);
                        }, () =>
                        {
                            _loginPanel.LoadFinish();
                            xLuaManager.InitEvent();
                        }));
                        currentSubState = SubState.Update;
                    }
                   
                    break;
                case State.StartLogin:
                    break;
                case State.StartGame:
                    break;
                case State.Playing:
                    break;
                case State.None:
                    break;
            }
        }

  
    }

}


