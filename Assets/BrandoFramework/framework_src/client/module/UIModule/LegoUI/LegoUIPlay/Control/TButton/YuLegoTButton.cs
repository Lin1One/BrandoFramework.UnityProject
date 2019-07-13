#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common.DataStruct;
using System;
using UnityEngine.UI;
using YuCommon;

namespace Client.LegoUI
{
    public interface IYuLegoTButton : ILegoButton, IYuLegoInteractableControl
    {
        IYuLegoImage IconImage { get; }
    }

    /// <summary>
    /// 除了自身游戏对象上附加的Image组件外，还可以在其下附加一个Image组件及其游戏对象的按钮。
    /// </summary>
    public class YuLegoTButton : YuLegoButton, IYuLegoTButton
    {
        private IYuLegoImage iconImage;

        public IYuLegoImage IconImage
        {
            get
            {
                if (iconImage != null)
                {
                    return iconImage;
                }
                iconImage = RectTransform.Find("Icon")
                    .GetComponent<YuLegoImage>();
                return iconImage;
            }
        }

        private enum YuTButtonMetamorphoseStatus : byte
        {
            Button,
            ButtonImage,
            IconImage,
            SonText
        }

        #region 数据响应

        public void ReceiveIconChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                IconImage.LocUI.UIRect.gameObject.SetActive(false);
                return;
            }
            IconImage.SpriteId = newValue;
        }

        #endregion

        #region 元数据变形

        private LegoTButtonMeta tButtonMeta;
        private YuTButtonMetamorphoseStatus tButtonStatus;

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (tButtonMeta == null)
            {
                tButtonMeta = uiMeta.NextTButton;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (tButtonStatus)
            {
                // 按钮自身变形
                case YuTButtonMetamorphoseStatus.Button:
                    MetamorphoseRect(RectMeta);
                    if (tButtonMeta.TransitionType == LegoTransition.ColorTint
                        && tButtonMeta.ColorTintMeta != null)
                    {
                        var colorTintMeta = tButtonMeta.ColorTintMeta;

                        colors = new ColorBlock
                        {
                            normalColor = colorTintMeta.NormalLegoColor.ToColor(),
                            highlightedColor = colorTintMeta.HighlightedLegoColor.ToColor(),
                            pressedColor = colorTintMeta.PressedLegoColor.ToColor(),
                            disabledColor = colorTintMeta.DisabledLegoColor.ToColor(),
                            colorMultiplier = colorTintMeta.ColorMultiplier,
                            fadeDuration = colorTintMeta.FadeDuration
                        };
                    }

                    tButtonStatus = YuTButtonMetamorphoseStatus.ButtonImage;
                    break;
                case YuTButtonMetamorphoseStatus.ButtonImage:
                    // 按钮附带的Image变形
                    BgImage.As<YuLegoImage>().Metamorphose(tButtonMeta.ButtonImageMeta);
                    tButtonStatus = YuTButtonMetamorphoseStatus.IconImage;
                    break;
                case YuTButtonMetamorphoseStatus.IconImage:
                    IconImage.As<YuLegoImage>().Metamorphose(tButtonMeta.IconRectMeta,
                        tButtonMeta.IconImageMeta);
                    tButtonStatus = YuTButtonMetamorphoseStatus.SonText;
                    break;
                case YuTButtonMetamorphoseStatus.SonText:
                    if (tButtonMeta.IsTextActive)
                    {
                        SonText.gameObject.SetActive(true);
                        SonText.Metamorphose(tButtonMeta.TextRect, tButtonMeta.TextMeta);
                    }
                    else
                    {
                        SonText.gameObject.SetActive(false);
                    }

                    tButtonMeta = null;
                    tButtonStatus = YuTButtonMetamorphoseStatus.Button;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}