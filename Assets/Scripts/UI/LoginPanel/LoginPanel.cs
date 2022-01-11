using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XLuaDemo
{
    public class LoginPanel : MonoSingleton<LoginPanel>
    {

        public Text m_Tips;
        public Text m_ProgressText;

        public Slider m_Percent;

        public Button m_LoginButton;

        private float _refreshTime = 1;

        private float _currentTime;

        private float _progress;
        
        public void SetPercent(float percent)
        {
            m_Percent.value = percent;
        }

        private double _currentByte;

        public void OnUpdateDownload(object obj)
        {
            _currentByte += (double)obj;
        }

        private bool _changeProgress;

        public void OnUpdateProgress(float progress)
        {
            _progress = progress;
            _changeProgress = true;
        }

        public void StartUpdateRes()
        {
            UpdateRes = true;
        }
        
        public void EndUpdateRes()
        {
            UpdateRes = false;
            _changeProgress = false;
        }
        
        public bool UpdateRes { get; set; }

        private void Update()
        {

            if (UpdateRes)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime >= _refreshTime)
                {
                    _currentTime = 0;
                    SetTips($"下载资源中 {_currentByte:0.00} kb/s");
                }
            }

            if (_changeProgress)
            {
                m_ProgressText.text =  $" {_progress:00.00%}";
                m_Percent.value = _progress;
                _changeProgress = false;
            }
        }
        
        

        public void SetTips(string tips)
        {
            m_Tips.text = tips;
        }

        public void LoadFinish()
        {
            m_LoginButton.gameObject.SetActive(true);
            m_Tips.gameObject.SetActive(false);
            m_Percent.gameObject.SetActive(false);
        }
    }

}
