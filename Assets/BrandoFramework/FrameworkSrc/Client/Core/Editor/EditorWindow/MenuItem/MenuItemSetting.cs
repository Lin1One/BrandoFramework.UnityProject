#region Head

// Author:            LinYuzhou
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Client.Core.Editor
{
    /// <summary>
    /// YuOdin 菜单窗口中左侧选项的设置类型
    /// 控制包括字体，图标，大小等外观设置，以及右键弹出更多选项等功能设置
    /// </summary>
    public class MenuItemSetting : IMenuItemSetting
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

        private static MenuItemSetting commonMenuItemSetting;

        //常驻功能项设置
        public static MenuItemSetting CommonMenuItemSetting
        {
            get
            {
                if(commonMenuItemSetting == null)
                {
                    commonMenuItemSetting = new MenuItemSetting()
                    {
                        MenuItemStyle = MenuStyle.CommonItemStyle
                    };
                }
                return commonMenuItemSetting;
            }

        }

        #endregion

    }

    public class MenuStyle: OdinMenuStyle
    {
        private static MenuStyle commonItemStyle;

        public static MenuStyle CommonItemStyle
        {
            get
            {
                if (commonItemStyle == null)
                {
                    commonItemStyle = new MenuStyle();
                    commonItemStyle.SetBorderPadding(0);
                    commonItemStyle.SetBorderAlpha(0);
                }
                return commonItemStyle;
            }

            set => commonItemStyle = value;
        }
    }
}

