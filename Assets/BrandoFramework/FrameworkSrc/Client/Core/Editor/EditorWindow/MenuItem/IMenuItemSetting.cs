using System;

namespace Client.Core.Editor
{
    public interface IMenuItemSetting
    {
        void AddRightClickOption(string optionName, Action rightClickAction);

        //void OnRightClick(OdinMenuItem menuItem);

        //void SetMenuItemRect(float x, float y);

        //EditorIcon MenuItemIcon { get; set; }

        //OdinMenuStyle MenuItemStyle { get; set; }
    }
}

