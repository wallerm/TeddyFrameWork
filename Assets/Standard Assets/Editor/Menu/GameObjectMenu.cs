﻿#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    GameObjectMenu.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 15:09:12
=====================================================
*/
#endregion

using System.IO;
using UnityEditor;
using UnityEngine;

public class GameObjectMenu
{
    [MenuItem("GameObject/导出选中对象层级(嵌套层级)", false, 31)]
    static void GO_ExportGameObjectHierarchy_Nested()
    {
        GameObject root = Selection.activeGameObject as GameObject;
        if (root)
        {
            ExportPanelHierarchy.ExportNested(root);
        }

        Debug.Log("Export GameObject Hierarchy Completed");
    }
}