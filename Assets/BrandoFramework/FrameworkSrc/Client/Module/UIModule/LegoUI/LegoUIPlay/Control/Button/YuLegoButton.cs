#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Client.LegoUI;
using Common;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图按钮控件。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Button", 30)]
    public class YuLegoButton
        : YuAbsLegoInteractableControl,
            ILegoButton,
            IPointerClickHandler,
            ISubmitHandler
    {
        #region UGUISrc

        [Serializable]
        public class ButtonClickedEvent : UnityEvent
        {
        }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();


        protected YuLegoButton()
        {
        }

        #region  UI 操作响应

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            var selfId = name;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // 修改精灵图片
            TryUpdateButtonSpriteSwap();
            Press();
            BaseEventData = eventData;
            InvokeConfigurableInteractDisplay(this);
            InvokeInteractableMethod(LegoInteractableType.OnPointerClick);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());

            BaseEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnSubmit);
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }

        #endregion

        #endregion

        #region 元数据构建

        private enum MetamorphoseStatus : byte
        {
            Button,
            ButtonImage,
            SonText
        }

        private MetamorphoseStatus metamorphoseStatus;

        private IYuLegoImage selfImage;

        public IYuLegoImage BgImage
        {
            get
            {
                if (selfImage != null)
                {
                    return selfImage;
                }

                selfImage = GetComponent<IYuLegoImage>();
                selfImage.RaycastTarget = true;
                return selfImage;
            }
        }

        public ILegoText ButtonContent => SonText;

        private YuLegoText sonText;

        public YuLegoText SonText
        {
            get
            {
                if (sonText != null)
                {
                    return sonText;
                }

                sonText = transform.Find("Text").GetComponent<YuLegoText>();
                return sonText;
            }
        }

        public LegoButtonMeta ButtonMeta { get; private set; }

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (ButtonMeta == null)
            {
                ButtonMeta = uiMeta.NextButton;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (metamorphoseStatus)
            {
                // 按钮自身变形
                case MetamorphoseStatus.Button:
                    MetamorphoseRect(RectMeta);
                    if (ButtonMeta.TransitionType == LegoTransition.ColorTint
                        && ButtonMeta.ColorTintMeta != null)
                    {
                        var colorTintMeta = ButtonMeta.ColorTintMeta;

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

                    SoundEffectId = ButtonMeta.ButtonSoundId;

                    metamorphoseStatus = MetamorphoseStatus.ButtonImage;
                    break;
                case MetamorphoseStatus.ButtonImage:
                    // 按钮附带的Image变形
                    BgImage.As<YuLegoImage>().Metamorphose(ButtonMeta.ButtonImageMeta);
                    if (ButtonMeta.IsNonRectangularButtonImage)
                    {
                        IsNonRectangularButtonImage = true;
                        BgImage.As<YuLegoImage>().alphaHitTestMinimumThreshold = 
                            ButtonMeta.ImageAlphaHitTestMinimumThreshold;
                    }
                    metamorphoseStatus = MetamorphoseStatus.SonText;
                    break;
                case MetamorphoseStatus.SonText:
                    if (ButtonMeta.IsTextActive)
                    {
                        SonText.gameObject.SetActive(true);
                        SonText.Metamorphose(ButtonMeta.TextRect, ButtonMeta.TextMeta);
                    }
                    else
                    {
                        SonText.gameObject.SetActive(false);
                    }

                    metamorphoseStatus = MetamorphoseStatus.Button;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
            }
        }

        #endregion

        #region 数据响应

        public void ReceiveActiveChange(bool newValue)
        {
            GameObject.SetActive(newValue);
        }

        public void ReceiveTextChange(string newValue)
        {
            SonText.Text = newValue;
        }

        public void ReceiveBgChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                BgImage.RectTransform.gameObject.SetActive(false);
                return;
            }

            BgImage.SpriteId = newValue;
            BgImage.RectTransform.gameObject.SetActive(true);
        }

        #endregion

        #region 按钮精灵过渡

        private static YuLegoButton lastButton;

        private void TryUpdateButtonSpriteSwap()
        {
            if (lastButton != null && lastButton.ButtonMeta?.SpriteSwapMeta != null)
            {
                var lastNormalSpriteId = lastButton.ButtonMeta.SpriteSwapMeta.TargetGraphic;
                BgImage.SpriteId = lastNormalSpriteId;
            }

            lastButton = this;
            if (ButtonMeta?.SpriteSwapMeta == null)
            {
                return;
            }

            BgImage.SpriteId = ButtonMeta.SpriteSwapMeta.PressedSprite;
        }

        #endregion
    }
}