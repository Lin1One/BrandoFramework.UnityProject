using System;

namespace Common.EditorWindow
{
    public interface IYuMenuItemSetting
    {
        void AddRightClickOption(string optionName, Action rightClickAction);

        //void OnRightClick(OdinMenuItem menuItem);

        //void SetMenuItemRect(float x, float y);

        //EditorIcon MenuItemIcon { get; set; }

        //OdinMenuStyle MenuItemStyle { get; set; }
    }
}

