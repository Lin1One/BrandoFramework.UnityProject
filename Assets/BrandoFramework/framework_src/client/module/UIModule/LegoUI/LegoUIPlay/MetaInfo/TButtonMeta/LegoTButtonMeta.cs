#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using System;
using Sirenix.OdinInspector;
using Client.LegoUI;
using Common.DataStruct;

namespace Client.LegoUI
{
    [Serializable]
    public class LegoTButtonMeta : LegoButtonMeta
    {
        #region Image_Icon

        [LabelText("图标位置大小元数据")] public LegoRectTransformMeta IconRectMeta;

        [LabelText("图标图片元数据")] public LegoImageMeta IconImageMeta;

        #endregion

        public static LegoTButtonMeta Create(YuLegoTButton button)
        {
            var meta = new LegoTButtonMeta
            {
                Interactable = button.interactable,
                TransitionType = button.transition.ToString().AsEnum<LegoTransition>()
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

            // 按钮上的图片
            meta.ButtonImageMeta = LegoImageMeta.Create(button.BgImage.As<YuLegoImage>());

            // 子文本
            meta.IsTextActive = button.SonText.gameObject.activeSelf;
            meta.TextRect = LegoRectTransformMeta.Create(button.SonText.rectTransform);
            meta.TextMeta = LegoTextMeta.Create(button.SonText);

            // 图标图片
            meta.IconRectMeta = LegoRectTransformMeta.Create(button.IconImage.RectTransform);
            meta.IconImageMeta = LegoImageMeta.Create(button.IconImage.As<YuLegoImage>());

            return meta;
        }
    }
}