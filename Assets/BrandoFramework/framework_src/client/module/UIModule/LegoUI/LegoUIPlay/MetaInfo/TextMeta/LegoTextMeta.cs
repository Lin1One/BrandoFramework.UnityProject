#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com


#endregion

using Common;
using Sirenix.OdinInspector;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoTextMeta
    {
        /// <summary>
        /// 文本控件的样式Id。
        /// </summary>
        public int StyleId;

        ////public YuColor Color;

        /// <summary>
        /// 文本的字体名。
        /// </summary>
        public string FontId;

        /// <summary>
        /// 文本的字体样式。
        /// </summary>
        public LegoTextFontStyle FontStyle;

        /// <summary>
        /// 文本的字号大小。
        /// </summary>
        public int FontSize;

        /// <summary>
        /// 文本的间隔。
        /// </summary>
        public float LineSpacing;

        /// <summary>
        /// 是否开启富文本功能。
        /// </summary>
        public bool RichText;

        public LegoTextVerticalOverflow VerticalOverflow;

        public LegoTextHorizontalOverflow HorizontalOverflow;

        /// <summary>
        /// 文本的对齐方式。
        /// </summary>
        public LegoTextAlignment Alignment;

        public bool BestFit;

        public bool AlignByGeometry;

        /// <summary>
        /// 文本控件的文字内容。
        /// </summary>
        public string Content;

        public bool RaycastTarget;

        #region 位于按钮中时跟随按钮图片精灵过渡状态

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

        #endregion

        public static LegoTextMeta Create(YuLegoText text)
        {
            var meta = new LegoTextMeta
            {
                StyleId = text.StyleId,
                Content = text.text,
                RaycastTarget = text.raycastTarget,

                FontId = text.font.name,
                FontStyle = text.fontStyle.ToString().AsEnum<LegoTextFontStyle>(),
                FontSize = text.fontSize,
                ////Color = YuColor.Create(text.color),
                RichText = text.supportRichText,
                LineSpacing = text.lineSpacing,

                VerticalOverflow = text.verticalOverflow.ToString().AsEnum<LegoTextVerticalOverflow>(),
                HorizontalOverflow = text.horizontalOverflow
                    .ToString().AsEnum<LegoTextHorizontalOverflow>(),

                BestFit = text.resizeTextForBestFit,
                Alignment = text.alignment.ToString().AsEnum<LegoTextAlignment>(),
                AlignByGeometry = text.alignByGeometry
            };

            return meta;
        }
    }
}