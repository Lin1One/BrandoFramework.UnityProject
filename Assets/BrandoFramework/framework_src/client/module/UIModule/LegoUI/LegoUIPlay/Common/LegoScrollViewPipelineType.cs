#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高滚动视图生命周期
    /// </summary>
    public enum LegoScrollViewPipelineType : byte
    {

        /// <summary>
        /// 创建完毕。
        /// 只执行一次。
        /// </summary>
        OnCreated,

        /// <summary>
        /// 重排列
        /// 多次执行
        /// </summary>
        OnReorganize,

        /// <summary>
        /// 关闭之后。
        /// 只执行一次。
        /// </summary>
        AfterClose
    }
}