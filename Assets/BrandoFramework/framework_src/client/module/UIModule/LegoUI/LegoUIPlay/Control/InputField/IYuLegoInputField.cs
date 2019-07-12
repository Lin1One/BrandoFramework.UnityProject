#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface IYuLegoInputField : ILegoControl, IYuLegoInteractableControl
    {
        /// <summary>
        /// 输入框指示文本
        /// </summary>
        string PlaceHolder { get; }

        /// <summary>
        /// 输入框文本
        /// </summary>
        string Text { get; set; }
    }
}