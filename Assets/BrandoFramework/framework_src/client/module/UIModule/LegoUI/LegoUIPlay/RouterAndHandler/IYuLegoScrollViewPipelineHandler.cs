#region Head

// Author:            LinYuzhou
// CreateDate:        2018/11/9 16:35:12
// Email:             

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 处理乐高UI生命周期的业务逻辑。
    /// </summary>
    public interface IYuLegoScrollViewPipelineHandler
    {
        /// <summary>
        /// 生命周期事件处理器所对应的UI唯一Id。
        /// </summary>
        string UiId { get; }

        /// <summary>
        /// 生命周期事件处理器所对应的UI生命周期事件类型。
        /// </summary>
        LegoScrollViewPipelineType PipelineType { get; }

        /// <summary>
        /// 处理滚动列表的生命周期事件。
        /// </summary>
        /// <param name="legoUI"></param>
        void Execute(IYuLegoScrollView legoUI);
    }
}