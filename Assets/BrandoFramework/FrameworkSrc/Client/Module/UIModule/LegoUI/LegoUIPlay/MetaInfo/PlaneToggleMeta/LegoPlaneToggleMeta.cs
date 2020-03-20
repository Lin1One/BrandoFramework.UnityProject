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
    public class LegoPlaneToggleMeta
    {
        private const string IMAGE_FRONT_POINT = "Background";

        public bool Ison;
        public bool Interactable;
        public string SoundId;


        public LegoImageMeta ToggleImageMeta;

        public LegoColorTintMeta ColorTintMeta;
        public LegoTransition TransitionType;
        public LegoRectTransformMeta ImageFrontPointRectMeta;
        public LegoImageMeta ImageFrontPointImageMeta;

        public static LegoPlaneToggleMeta Create(YuLegoPlaneToggle toggle)
        {
            var rect = toggle.RectTransform;
            var meta = new LegoPlaneToggleMeta
            {
                Interactable = toggle.interactable,
                TransitionType = toggle.transition.ToString().AsEnum<LegoTransition>(),
                ColorTintMeta = LegoColorTintMeta.Create(toggle),
                SoundId = toggle.SoundEffectId
            };

            // 开关自身图片元数据
            meta.ToggleImageMeta = LegoImageMeta.Create(toggle.ToggleImage.As<YuLegoImage>());

            var imageFrontPoint = rect.Find(IMAGE_FRONT_POINT);
            meta.ImageFrontPointRectMeta = LegoRectTransformMeta
                .Create(imageFrontPoint.RectTransform());
            meta.ImageFrontPointImageMeta = LegoImageMeta
                .Create(imageFrontPoint.GetComponent<YuLegoImage>());

            return meta;
        }
    }
}