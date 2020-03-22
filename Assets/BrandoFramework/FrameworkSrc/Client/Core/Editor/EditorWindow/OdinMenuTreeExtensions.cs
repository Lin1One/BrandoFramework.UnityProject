#if UNITY_EDITOR
#region Head

// Author:            LinYuzhou
// CreateDate:        2019/2/14 16:33:13
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Client.Core.Editor
{
    public static class OdinMenuTreeExtensions
    {
        //public static IEnumerable<OdinMenuItem> AddAllDatisAtPath(this OdinMenuTree tree,
        //string saveDir, string menu, Func<string, UnityEngine.Object> createFunc,
        //    Func<object, bool> checkTypeFunc)
        //{
        //    if (!Directory.Exists(saveDir))
        //    {
        //        Directory.CreateDirectory(saveDir);
        //        YuAssetDatabaseUtility.Refresh();
        //    }

        //    var paths = YuIOUtility.GetPaths(saveDir)
        //        .Where(s => !s.EndsWith(".meta") && s.EndsWith(".asset"))
        //        .ToList();

        //    var result = new HashSet<OdinMenuItem>();

        //    foreach (var path in paths)
        //    {
        //        var fileId = Path.GetFileNameWithoutExtension(path);
        //        var dati = createFunc(fileId);
        //        if (!checkTypeFunc(dati))
        //        {
        //            continue;
        //        }

        //        tree.AddMenuItemAtPath(result, menu, new OdinMenuItem(tree, fileId, dati));
        //    }
        //    return result;
        //}


        ///// <summary>
        ///// OdinMenuTree Add 包含设置类型方法
        ///// </summary>
        //public static void Add(this OdinMenuTree tree,string path, object instance,IYuMenuItemSetting menuItemSetting)
        //{
        //    var item = tree.AddObjectAtPath(path, instance).LastOrDefault();
        //    if (item != null)
        //    {
        //        if(menuItemSetting.MenuItemStyle != null)
        //        {
        //            item.Style = menuItemSetting.MenuItemStyle;
        //        }
        //        if(menuItemSetting.MenuItemIcon != null)
        //        {
        //            item.Icon = menuItemSetting.MenuItemIcon.Highlighted;
        //            item.IconSelected = menuItemSetting.MenuItemIcon.Raw;
        //        }
        //        item.OnRightClick += menuItemSetting.OnRightClick;
        //    }
        //}

        ///// <summary>
        ///// OdinMenuTree Add 包含设置类型方法,单独设置图标扩展方法
        ///// 用于设置类为通用预设，图标需单独修改
        ///// </summary>
        //public static void Add(this OdinMenuTree tree, string path, object instance, EditorIcon icon, IYuMenuItemSetting menuItemSetting)
        //{
        //    menuItemSetting.MenuItemIcon = icon;
        //    tree.Add(path, instance, menuItemSetting);
        //}



        ///// <summary>
        ///// 新增 OdinMenuTree Add 添加右键点击方法的所需扩展
        ///// </summary>
        //public static IEnumerable<OdinMenuItem> SetRightClickAction(this IEnumerable<OdinMenuItem> menuItems, Action<OdinMenuItem> rightClickAction)
        //{
        //    var last = menuItems.LastOrDefault();
        //    if (last != null)
        //    {
        //        last.OnRightClick += rightClickAction;
        //    }
        //    return menuItems;
        //}
    }
}
#endif