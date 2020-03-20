#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface IYuLegoProgressbar : ILegoControl, IYuLegoInteractableControl
    {
        /// <summary>
        /// 当前进度值。
        /// </summary>
        float Progress { get; set; }
    }
}