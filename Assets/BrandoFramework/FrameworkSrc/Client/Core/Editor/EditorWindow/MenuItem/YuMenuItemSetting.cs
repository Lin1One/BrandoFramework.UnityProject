#region Head

// Author:            LinYuzhou
// CreateDate:        2019/2/14 20:33:56
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.EditorWindow
{
    /// <summary>
    /// YuOdin 菜单窗口中左侧选项的设置类型
    /// 控制包括字体，图标，大小等外观设置，以及右键弹出更多选项等功能设置
    /// </summary>
    public class YuMenuItemSetting : IYuMenuItemSetting
    {
        #region 选项功能设置

        #region 右键菜单设置
        private Dictionary<string, Action> rightClickOptios;

        private Dictionary<string, Action> RightClickOptios
        {
            get
            {
                if (rightClickOptios == null)
                {
                    rightClickOptios = new Dictionary<string, Action>();
                }
                return rightClickOptios;
            }
            set => rightClickOptios = value;
        }

        public void AddRightClickOption(string optionName, Action rightClickAction)
        {
            if (!RightClickOptios.ContainsKey(optionName))
            {
                RightClickOptios.Add(optionName, rightClickAction);
            }
        }

        public void OnRightClick(OdinMenuItem menuItem)
        {
            var menu = new GenericMenu();
            foreach (var action in RightClickOptios)
            {
                menu.AddItem(new GUIContent(action.Key), false, new GenericMenu.MenuFunction(action.Value));
            }

            menu.ShowAsContext();
        }

        #endregion    

        #endregion    

        #region 选项外观设置

        public EditorIcon MenuItemIcon { get; set; }

        public OdinMenuStyle MenuItemStyle { get; set; }

        public void SetMenuItemRect(float x, float y)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region 常用 Item 预设 

        private static YuMenuItemSetting commonMenuItemSetting;

        //常驻功能项设置
        public static YuMenuItemSetting CommonMenuItemSetting
        {
            get
            {
                if(commonMenuItemSetting == null)
                {
                    commonMenuItemSetting = new YuMenuItemSetting()
                    {
                        MenuItemStyle = YuMenuStyle.CommonItemStyle
                    };
                }
                return commonMenuItemSetting;
            }

        }

        #endregion

    }

    public class YuMenuStyle: OdinMenuStyle
    {
        private static YuMenuStyle commonItemStyle;

        public static YuMenuStyle CommonItemStyle
        {
            get
            {
                if (commonItemStyle == null)
                {
                    commonItemStyle = new YuMenuStyle();
                    commonItemStyle.SetBorderPadding(0);
                    commonItemStyle.SetBorderAlpha(0);
                }
                return commonItemStyle;
            }

            set => commonItemStyle = value;
        }
    }
}

