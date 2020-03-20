#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件变形阶段。
    /// </summary>
    public enum LegoMetamorphoseStage : byte
    {
        /// <summary>
        /// 不在变形。
        /// </summary>
        Completed,

        /// <summary>
        /// 变形中。
        /// </summary>
        Metamorphosing,
    }
}