#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

namespace Client.Core.Editor
{
    /// <summary>
    /// unity编辑器状态改变事件处理器。
    /// </summary>
    public interface IEditorStateChangeHandler
    {
        void HandleStateChange();

        /// <summary>
        /// 自身业务逻辑说明。
        /// 用于在启动时打印一个输出日志。
        /// </summary>
        string LogicDesc { get; }
    }
}