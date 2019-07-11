#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com


#endregion

using Common.DataStruct;
using System;


namespace Client.LegoUI
{
    [Serializable]
    public class LegoButtonMeta
    {
        #region ButtonMeta

        public bool Interactable;

        public string ButtonSoundId;

        #region Transition

        public LegoTransition TransitionType;

        #region ColorTint

        public LegoColorTintMeta ColorTintMeta;

        #endregion

        #region SpriteSwap

        public LegoSpriteSwapMeta SpriteSwapMeta;

        #endregion


        #endregion

        #endregion

        #region ButtonImageMeta

        public LegoImageMeta ButtonImageMeta;

        public bool IsNonRectangularButtonImage;

        public float ImageAlphaHitTestMinimumThreshold = 0.5f;

        #endregion

        #region TextMeta

        public bool IsTextActive;
        public LegoRectTransformMeta TextRect;
        public LegoTextMeta TextMeta;

        #endregion

        public static LegoButtonMeta Create(YuLegoButton button)
        {
            var meta = new LegoButtonMeta
            {
                Interactable = button.interactable,
                TransitionType = button.transition.ToString().AsEnum<LegoTransition>(),
                ButtonSoundId = button.SoundEffectId,
                IsNonRectangularButtonImage = button.IsNonRectangularButtonImage
            };

            switch (meta.TransitionType)
            {
                case LegoTransition.None:
                    break;
                case LegoTransition.ColorTint:
                    meta.ColorTintMeta = LegoColorTintMeta.Create(button);
                    break;
                case LegoTransition.SpriteSwap:
                    meta.SpriteSwapMeta = LegoSpriteSwapMeta.create(button);
                    break;
                case LegoTransition.Animation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            meta.ButtonImageMeta = LegoImageMeta.Create(button.BgImage.As<YuLegoImage>());

            // 子文本
            meta.IsTextActive = button.SonText.gameObject.activeSelf;
            meta.TextRect = LegoRectTransformMeta.Create(button.SonText.rectTransform);
            meta.TextMeta = LegoTextMeta.Create(button.SonText);

            return meta;
        }
    }
}