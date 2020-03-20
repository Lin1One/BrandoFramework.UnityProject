#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using System;

namespace Client.LegoUI
{
    /// <summary>
    /// 文本字体样式类型。
    /// </summary>
    [Serializable]
    public enum LegoTextFontStyle : byte
    {
        Normal,
        Bold,
        Italic,
        BoldAndItalic
    }
}