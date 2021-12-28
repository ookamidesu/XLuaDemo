using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLuaDemo
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            
            AppConfig.Init();
        }
        
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.AddComponent<LocalLogHandler>();
            this.gameObject.AddComponent<ServerLogHandler>();
            this.gameObject.AddComponent<XLuaManager>();
            
            
        }


      

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.LogWarning(2);
            }
        
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.LogError(3);
            }
        }

  
    }

}


