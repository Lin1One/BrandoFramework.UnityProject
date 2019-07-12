#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion


using System.Collections.Generic;

namespace Client.LegoUI
{
    public static class LegoConstant
    {
        public static readonly HashSet<string> YuSpriteIds
            = new HashSet<string>
            {
                //"UI_Panel_Window",
                //"UI_Fill_Sky",
                //"UI_Checkmark",
            };

        /// <summary>
        /// 不进行同一父级子控件重名检测和进行路径获取操作的控件名集合。
        /// </summary>
        public static readonly HashSet<string> IgnoreControlIds
            = new HashSet<string>
            {
                "Button",
                "Text",
                "Image",
                "RawImage",
                "Toggle",
                "Slider",
                "InputField",
                "Dropdown",
                "LsitView",
                "Grid",
            };
    }
}