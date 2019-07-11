#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using Common.DataStruct;
using UnityEngine.Serialization;

namespace Client.LegoUI
{
    [System.Serializable]
    public enum YuToggleTransitonType : byte
    {
        None,
        Fade,
    }

    [System.Serializable]
    public class LegoToggleMeta
    {
        #region ToggleMeta

        public bool IsOn;
        public string Group;
        public bool Interactable;
        public string SoundId;

        #region ColorTint

        public LegoColorTintMeta ColorTintMeta;

        [FormerlySerializedAs("TransitionType")]
        public LegoTransition Transition;

        #endregion

        #endregion

        #region BackgroudImageMeta

        public LegoRectTransformMeta BackgroundRectMeta;
        public LegoImageMeta BackgroundImageMeta;
        public LegoRectTransformMeta CheckmarkRectMeta;
        public LegoImageMeta CheckMarkImageMeta;
        public LegoRectTransformMeta TextRect;
        public LegoTextMeta TextMeta;

        #endregion

        public static LegoToggleMeta Create(YuLegoToggle toggle)
        {
            var rect = toggle.RectTransform;
            var meta = new LegoToggleMeta
            {
                Interactable = toggle.interactable,
                Transition = toggle.transition.ToString().AsEnum<LegoTransition>(),
                ColorTintMeta = LegoColorTintMeta.Create(toggle),
                SoundId = toggle.SoundEffectId
            };

            var background = rect.Find("Background");
            meta.BackgroundRectMeta = LegoRectTransformMeta.Create(background.RectTransform());
            meta.BackgroundImageMeta = LegoImageMeta.Create(background.GetComponent<YuLegoImage>());

            var checkmark = rect.Find("Background/Checkmark");
            meta.CheckmarkRectMeta = LegoRectTransformMeta.Create(checkmark.RectTransform());
            meta.CheckMarkImageMeta = LegoImageMeta.Create(checkmark.GetComponent<YuLegoImage>());

            var text = rect.Find("Text");
            meta.TextRect = LegoRectTransformMeta.Create(text.RectTransform());
            meta.TextMeta = LegoTextMeta.Create(text.GetComponent<YuLegoText>());

            return meta;
        }
    }
}