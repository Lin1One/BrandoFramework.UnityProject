#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion


using Client.Extend;
using Common.DataStruct;

namespace Client.LegoUI
{
    [System.Serializable]
    public enum LegoSliderDirection : byte
    {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
    }

    [System.Serializable]
    public class LegoSliderMeta
    {

        public LegoTransition Transition;
        public LegoColorTintMeta ColorTintMeta;
        public LegoRectTransformMeta HandleSlideAreaRect;
        public LegoRectTransformMeta HandleImageRect;
        public LegoImageMeta HandleImageMeta;
        public LegoRectTransformMeta BackgroundImageRect;
        public LegoImageMeta BackgroundImageMeta;

        public LegoRectTransformMeta FillAreaMeta;

        public LegoRectTransformMeta FillImageRect;
        public LegoImageMeta FillImageMeta;

        public YuLegoSlider.Direction Direction;
        public float MinValue;
        public float MaxValue;
        public bool IsWholeNumbers;

        public static LegoSliderMeta Create(YuLegoSlider slider)
        {
            var meta = new LegoSliderMeta();
            var rect = slider.RectTransform;

            meta.Transition = slider.transition.ToString()
                .AsEnum<LegoTransition>();
            meta.ColorTintMeta = LegoColorTintMeta.Create(slider);

            var background = rect.Find("Background");
            meta.BackgroundImageRect = LegoRectTransformMeta.Create(background.RectTransform());
            meta.BackgroundImageMeta = LegoImageMeta.Create(background.GetComponent<YuLegoImage>());

            var fillArea = rect.Find("Fill Area");
            meta.FillAreaMeta = LegoRectTransformMeta.Create(fillArea.RectTransform());

            var fill = rect.Find("Fill Area/Fill");
            meta.FillImageRect = LegoRectTransformMeta.Create(fill.RectTransform());
            meta.FillImageMeta = LegoImageMeta.Create(fill.GetComponent<YuLegoImage>());

            var handleSlideArea = rect.Find("Handle Slide Area");
            meta.HandleSlideAreaRect = LegoRectTransformMeta.Create(handleSlideArea.RectTransform());

            var handle = rect.Find("Handle Slide Area/Handle");
            meta.HandleImageRect = LegoRectTransformMeta.Create(handle.RectTransform());
            meta.HandleImageMeta = LegoImageMeta.Create(handle.GetComponent<YuLegoImage>());

            meta.Direction = slider.direction;
            meta.MinValue = slider.minValue;
            meta.MaxValue = slider.maxValue;
            meta.IsWholeNumbers = slider.wholeNumbers;

            return meta;
        }
    }
}