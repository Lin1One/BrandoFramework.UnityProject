#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图元素类型。
    /// </summary>
    [System.Serializable]
    public enum LegoUIType : byte
    {
        None,
        Text,
        InlineText,
        Image,
        RawImage,
        Button,
        TButton,
        InputField,
        Slider,
        Progressbar,
        Toggle,
        PlaneToggle,
        Tab,
        Dropdown,
        Rocker,
        Grid,
        ScrollView,
        Container,
        Component,
        View
    }
}