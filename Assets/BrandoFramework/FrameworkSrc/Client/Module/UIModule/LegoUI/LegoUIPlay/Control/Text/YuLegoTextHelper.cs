using Sirenix.OdinInspector;
using UnityEngine;
using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图按钮控件开发助手。
    /// </summary>
#if DEBUG
    [ExecuteInEditMode]
#endif
    public class YuLegoTextHelper : DevelopHelper
    {
        [BoxGroup("精灵过渡状态样式Id")]
        [LabelText("普通样式Id")]
        public string NormalId;

        [BoxGroup("精灵过渡状态样式Id")]
        [LabelText("悬停时样式Id")]
        public string HighlightedId;

        [BoxGroup("精灵过渡状态样式Id")]
        [LabelText("点击时样式Id")]
        public string PressedId;

        [BoxGroup("精灵过渡状态样式Id")]
        [LabelText("禁用时样式Id")]
        public string DisabledId;

    }
}
