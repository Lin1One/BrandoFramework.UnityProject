#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using YuU3dPlay;

namespace Client.LegoUI
{
    public interface IYuLegoImage : ILegoControl, IYuColor
    {
        /// <summary>
        /// 图片使用使用的精灵资源名。
        /// </summary>
        string SpriteId { get; set; }

        /// <summary>
        /// 设置图片是否可以接受交互。
        /// </summary>
        bool RaycastTarget { get; set; }

        void SetFillAmount(float fill);

        void SetNativeSize();
    }
}