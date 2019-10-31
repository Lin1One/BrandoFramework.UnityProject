#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

namespace Common
{
    /// <summary>
    /// 计时器状态。
    /// </summary>
    public enum TimerStatus : byte
    {
        /// <summary>
        /// 未启动，默认状态。
        /// </summary>
        Unstart,

        /// <summary>
        /// 运行中。
        /// </summary>
        Running,

        /// <summary>
        /// 已暂停，可恢复运行。
        /// </summary>
        Pause,

        /// <summary>
        /// 已关闭，不可恢复即将被回收或销毁。 
        /// </summary>
        Close,
    }
}