#if UNITY_EDITOR
#region Head

// Author:            LinYuzhou
// CreateDate:        2019/2/14 16:33:13
// Email:             836045613@qq.com

/*
 * �޸�����  ��
 * �޸���    ��
 * �޸�����  ��
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
        ///// OdinMenuTree Add �����������ͷ���
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
        ///// OdinMenuTree Add �����������ͷ���,��������ͼ����չ����
        ///// ����������Ϊͨ��Ԥ�裬ͼ���赥���޸�
        ///// </summary>
        //public static void Add(this OdinMenuTree tree, string path, object instance, EditorIcon icon, IYuMenuItemSetting menuItemSetting)
        //{
        //    menuItemSetting.MenuItemIcon = icon;
        //    tree.Add(path, instance, menuItemSetting);
        //}



        ///// <summary>
        ///// ���� OdinMenuTree Add ����Ҽ����������������չ
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