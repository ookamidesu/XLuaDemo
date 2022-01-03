using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XLuaDemo
{
    public class LoginPanel : MonoSingleton<LoginPanel>
    {

        public Text m_Tips;

        public Slider m_Percent;

        public Button m_LoginButton;

        public void SetPercent(float percent)
        {
            m_Percent.value = percent;
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
