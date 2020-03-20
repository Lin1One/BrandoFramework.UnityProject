#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client
{
    /// <summary>
    /// 释放接口。
    /// 实现该接口的类型需要在该方法中显式释放自身的资源占用。
    /// </summary>
    public interface IRelease
    {
        /// <summary>
        /// 释放自身的内存占用。
        /// </summary>
        void Release();
    }
}
