#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface ILegoView : ILegoUI
    {
        /// <summary>
        /// 界面的Z轴深度值（可读可写）。
        /// </summary>
        float DepthZ { get; set; }
    }
}