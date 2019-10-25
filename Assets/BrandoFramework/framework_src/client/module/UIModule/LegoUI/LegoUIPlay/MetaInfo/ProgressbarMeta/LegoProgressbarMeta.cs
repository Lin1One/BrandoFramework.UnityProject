#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com


#endregion

using Client.Extend;
using Common;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoProgressbarMeta
    {
        public LegoColorTintMeta ColorTintMeta;
        public string HighlightedSprite;
        public string PressedSprite;
        public string DisabledSprite;

        public float MinValue;
        public float MaxValue;
        public bool WholeNumbers;
        public LegoSliderDirection Direction;
        public LegoTransition Transition;

        public LegoRectTransformMeta BackgroundImageRect;
        public LegoImageMeta BackgroundImageMeta;

        public LegoRectTransformMeta FillAreaMeta;

        public LegoRectTransformMeta FillImageRect;
        public LegoImageMeta FillImageMeta;

        public static LegoProgressbarMeta Create(YuLegoProgressbar legoProgressbar)
        {
            var meta = new LegoProgressbarMeta();
            var rect = legoProgressbar.RectTransform;

            meta.Transition = legoProgressbar.transition.ToString()
                .AsEnum<LegoTransition>();
            meta.ColorTintMeta = LegoColorTintMeta.Create(legoProgressbar);

            var imageBackground = rect.Find("Background");
            meta.BackgroundImageRect = LegoRectTransformMeta.Create(imageBackground.RectTransform());
            meta.BackgroundImageMeta = LegoImageMeta.Create(imageBackground.GetComponent<YuLegoImage>());

            var rectFillArea = rect.Find("Fill Area");
            meta.FillAreaMeta = LegoRectTransformMeta.Create(rectFillArea.RectTransform());

            var iamgeFill = rect.Find("Fill Area/Fill");
            meta.FillImageRect = LegoRectTransformMeta.Create(iamgeFill.RectTransform());
            meta.FillImageMeta = LegoImageMeta.Create(iamgeFill.GetComponent<YuLegoImage>());

            return meta;
        }
    }
}