using System;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动列表类型。
    /// 1. 规则布局。
    /// 2. 不规则布局。
    /// </summary>
    [Serializable]
    public enum YuLegoScrollViewType : byte
    {
        /// <summary>
        /// 规则布局。
        /// 子组件的尺寸完全一致。
        /// </summary>
        RegularLayout,

        /// <summary>
        /// 不规则布局。
        /// 子组件的尺寸不一致。
        /// </summary>
        IrregularLayout,
    }
}