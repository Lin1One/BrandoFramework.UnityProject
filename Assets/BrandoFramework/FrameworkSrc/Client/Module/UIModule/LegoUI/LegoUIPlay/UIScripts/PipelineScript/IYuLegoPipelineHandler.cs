#region Head

// Author:            Yu
// CreateDate:        2018/8/15 16:35:12
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 处理乐高UI生命周期的业务逻辑。
    /// </summary>
    public interface IYuLegoUIPipelineHandler
    {
        /// <summary>
        /// 生命周期事件处理器所对应的UI唯一Id。
        /// </summary>
        string UiId { get; }

        /// <summary>
        /// 生命周期事件处理器所对应的UI生命周期事件类型。
        /// </summary>
        UIPipelineType PipelineType { get; }

        /// <summary>
        /// 处理目标UI的生命周期事件。
        /// </summary>
        /// <param name="legoUI"></param>
        void Execute(ILegoUI legoUI);
    }
}