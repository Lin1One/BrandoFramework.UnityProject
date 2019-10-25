#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com



#endregion


using Common;

namespace Client.LegoUI
{
    /// <summary>
    /// 文本样式元数据。
    /// </summary>
    [System.Serializable]
    public class LegoTextStyleMeta
    {
        public string StyleId;
        public string FontName;
        public LegoTextFontStyle FontStyle;
        public int FontSize;
        public float LineSpacing;
        public LegoTextAlignment Alignment;
        public bool SupportRichText;
        public bool AlignByGeometry;
        public LegoTextHorizontalOverflow HorizontalOverflow;
        public LegoTextVerticalOverflow VerticalOverflow;
        public bool BestFit;
        public LegoColorMeta TextLegoColor;
        public string Material;
        public bool RaycastTarget;

        public static LegoTextStyleMeta Create(YuLegoText text)
        {
            var meta = new LegoTextStyleMeta
            {
                FontName = text.font == null ? null : text.font.name,
                FontStyle = text.font == null
                    ? LegoTextFontStyle.Normal
                    : text.fontStyle.ToString().AsEnum<LegoTextFontStyle>(),
                FontSize = text.font == null ? 20 : text.fontSize,
                LineSpacing = text.lineSpacing,
                SupportRichText = text.supportRichText,
                Alignment = text.alignment.ToString().AsEnum<LegoTextAlignment>(),
                AlignByGeometry = text.alignByGeometry,
                HorizontalOverflow = text.horizontalOverflow.ToString()
                    .AsEnum<LegoTextHorizontalOverflow>(),
                VerticalOverflow = text.verticalOverflow.ToString()
                    .AsEnum<LegoTextVerticalOverflow>(),
                BestFit = text.resizeTextForBestFit,
                TextLegoColor = LegoColorMeta.Create(text.color),
                Material = text.material == null ? null : text.material.name,
                RaycastTarget = text.raycastTarget
            };

            return meta;
        }
    }
}