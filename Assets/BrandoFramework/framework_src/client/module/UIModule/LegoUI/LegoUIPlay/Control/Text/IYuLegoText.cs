#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com
#endregion

using UnityEngine;

namespace Client.LegoUI
{
    public interface ILegoText : ILegoControl
    {
        Color Color { get; set; }

        string Text { get; set; }

        /// <summary>
        /// 获取目标文本控件的字体Id或修改其所使用的字体。
        /// </summary>
        string FontId { get; set; }
    }
}