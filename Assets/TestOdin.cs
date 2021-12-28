﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Examples;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class TestOdin : MonoBehaviour
{
    /// <summary>
    /// 自动绑定表
    /// </summary>
    private static Dictionary<string, string> AutoBindCheck = new Dictionary<string, string>()
    {
        {"Trans", "Transform"},
        {"OldAnim", "Animation"},
        {"NewAnim", "Animator"},

        {"Rect", "RectTransform"},
        {"Canvas", "Canvas"},
        {"Group", "CanvasGroup"},
        {"VGroup", "VerticalLayoutGroup"},
        {"HGroup", "HorizontalLayoutGroup"},
        {"GGroup", "GridLayoutGroup"},
        {"TGroup", "ToggleGroup"},

        {"Btn", "Button"},
        {"Img", "Image"},
        {"RImg", "RawImage"},
        {"Txt", "Text"},
        {"Input", "InputField"},
        {"Slider", "Slider"},
        {"Mask", "Mask"},
        {"Mask2D", "RectMask2D"},
        {"Tog", "Toggle"},
        {"Sbar", "Scrollbar"},
        {"SRect", "ScrollRect"},
        {"Drop", "Dropdown"},
    };

#if UNITY_EDITOR
    [ButtonGroup("_DefaultGroup",-10f)]
    [LabelText("全部删除")]
    private void RemoveAll()
    {
        m_AllComponents.Clear();
    }
    
    [ButtonGroup("_DefaultGroup",-10f)]
    [LabelText("自动绑定")]
    private void AutoBind()
    {
        m_AllComponents.Clear();
        List<Transform> allChildren = new List<Transform>();
        GetAllChildren(allChildren, transform, true);
        foreach (var child in allChildren)
        {
            if (AutoBindCheck.Keys.Any(key =>child.name.StartsWith($"{key}_")))
            {
                Debug.Log($"找到 : {child.name}");
                var names = child.name.Split('_');
                string startKey = names[0];
                
                string uiType = AutoBindCheck[startKey];
                var component = child.GetComponent(uiType);

                string fildName = startKey;
                for (var i = 1; i < names.Length; i++)
                {
                    string str = names[i];
                    if (str.Length > 1)
                        fildName = fildName+ char.ToUpper(str[0]) + str.Substring(1);
                }
               
                var bindUiComponent = new BindUIComponent()
                {
                    m_FildName = fildName,
                    m_Obj =  child.gameObject,
                    m_Component = component,
                    
                };
                
                m_AllComponents.Add(bindUiComponent);
            }
        }
    }

    private void GetAllChildren(List<Transform> allNodes,Transform parent,bool isRoot)
    {
        if (!isRoot)
        {
            allNodes.Add(parent);
        }
        
        if (parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                GetAllChildren(allNodes, child, false);
            }
        }
    }
    
    [ButtonGroup("_DefaultGroup",-10f)]
    [LabelText("生成代码")]
    private void GenerateCode()
    {
        
    }
    
    [InlineButton("GeneratePrefab","生成预制体")]
    [LabelText("预制体路径")]
    [FolderPath]
    public string m_PrefabPath;

    
    private void GeneratePrefab()
    {
        if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            //bool success;
            //PrefabUtility.SaveAsPrefabAsset(gameObject, path,out success);
            Debug.Log($"预制体{path}已存在");
               
        }
        else
        {
            string prefabPath = $"{m_PrefabPath}/{name}.prefab";
            bool success;
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath,out success);
            if(success)       
                Debug.Log($"生成预制体成功{prefabPath}");
        }
    }

    [FolderPath]
    [LabelText("生成代码路径")]
    public string m_GenerateCodePath;
    

    
    [LabelText("文件夹")]
    [HorizontalGroup("Group1",LabelWidth = 50)]
    public string m_Folder;
    
    [LabelText("类名")]
    [HorizontalGroup("Group1",LabelWidth = 50)]
    public string m_ClassName;

#endif
    

    [TableList]
    public List<BindUIComponent> m_AllComponents;
}

[Serializable]
public class BindUIComponent
{
    [TableColumnWidth(30)]
    public string m_FildName;
    
    [TableColumnWidth(30)]
    public GameObject m_Obj;

    [TableColumnWidth(30)]
    public Component m_Component;
}