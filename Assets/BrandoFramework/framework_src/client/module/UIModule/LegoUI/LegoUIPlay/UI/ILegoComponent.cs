#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface ILegoComponent : ILegoUI
    {
        /// <summary>
        /// 当组件位于一个滚动视图中时的子项Id。
        /// </summary>
        int ScrollViewId { get; set; }
    }
}