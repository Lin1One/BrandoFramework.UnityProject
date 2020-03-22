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

namespace Client.LegoUI
{
    /// <summary>
    /// 处理乐高视图控件的交互行为。
    /// </summary>
    public interface IYuLegoActionHandler
    {
        void Execute(object legoControl);
    }
}