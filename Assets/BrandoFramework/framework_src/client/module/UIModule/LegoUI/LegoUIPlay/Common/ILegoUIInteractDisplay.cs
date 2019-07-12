#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件交互表现行为接口
    /// </summary>
    public interface ILegoUIInteractDisplay 
    {
        void Display(ILegoControl ui);
    }
}