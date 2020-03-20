#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion


namespace Client.LegoUI
{
    public interface ILegoButton : ILegoControl, IYuLegoInteractableControl
    {
        /// <summary>
        /// 按钮自身上附加的背景图片控件。
        /// </summary>
        IYuLegoImage BgImage { get; }

        /// <summary>
        /// 按钮上的文本。
        /// </summary>
        ILegoText ButtonContent { get; }
    }
}