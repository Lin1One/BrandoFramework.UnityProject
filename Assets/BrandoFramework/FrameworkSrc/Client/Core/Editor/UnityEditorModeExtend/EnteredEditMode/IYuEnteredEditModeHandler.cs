#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

namespace Client.Core.Editor
{ 
    /// <summary>
    /// 编辑器上下文初始化完毕响应器。
    /// 实现该接口的对象将会在编辑器上下文构建完毕时（编译完成）被自动调用。
    /// </summary>
public interface IYuEnteredEditModeHandler : IYuEditorStateChangeHandler
    {
    }
}