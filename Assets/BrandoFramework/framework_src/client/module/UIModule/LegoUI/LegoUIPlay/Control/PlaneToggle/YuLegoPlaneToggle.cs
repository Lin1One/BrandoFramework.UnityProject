#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Client.LegoUI
{
    /// <summary>
    /// 水平开关。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego PlaneToggle", 51)]
    [ExecuteInEditMode]
    public class YuLegoPlaneToggle :
        YuLegoButton,
        IYuLegoPlaneToggle
    {
        #region 子控件

        private YuLegoImage backgroundImage;

        public IYuLegoImage FrontImage
        {
            get
            {
                if (backgroundImage != null)
                {
                    return backgroundImage;
                }

                backgroundImage = transform.Find("Background")
                    .GetComponent<YuLegoImage>();
                return backgroundImage;
            }
        }

        private YuLegoImage toggleImage;

        public IYuLegoImage ToggleImage
        {
            get
            {
                if (toggleImage != null)
                {
                    return toggleImage;
                }

                toggleImage = transform.GetComponent<YuLegoImage>();
                return toggleImage;
            }
        }

        #endregion

        private float pointLeft;
        private float pointRight;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointLeft = RectTransform.sizeDelta.x / 2 * -1;
            pointRight = RectTransform.sizeDelta.x / 2;
        }

        private Action<bool> m_PushAction;
        public void BindingPush(Action<bool> pushAction) => m_PushAction = pushAction;

        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        public bool IsOn
        {
            get { return m_IsOn; }
            set
            {
                m_IsOn = value;
                FrontImage.RectTransform.localPosition = m_IsOn
                    ? new Vector3(pointRight, 0)
                    : new Vector3(pointLeft, 0);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            IsOn = !IsOn;
            m_PushAction?.Invoke(m_IsOn);
            InvokeInteractableMethod(LegoInteractableType.OnValueChanged);
        }

        public void ReveiveFrontSpriteChange(string newValue)
        {
            FrontImage.SpriteId = newValue;
        }

        public void ReveiveBackgroundSpriteChange(string newValue)
        {
            ToggleImage.SpriteId = newValue;
        }

        public void ReceiveIsOnChange(bool state)
        {
            IsOn = state;
        }

        #region 元数据变形

        private enum MetamorphoseStatus : byte
        {
            Toggle,
            SelfImage,
            Background,
        }

        private MetamorphoseStatus planeMetamorphoseStatus;

        public LegoPlaneToggleMeta PlaneToggleMeta { get; private set; }

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (PlaneToggleMeta == null)
            {
                PlaneToggleMeta = uiMeta.NextPlaneToggle;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (planeMetamorphoseStatus)
            {
                case MetamorphoseStatus.Toggle:
                    MetamorphoseRect(RectMeta);
                    if (PlaneToggleMeta.TransitionType == LegoTransition.ColorTint
                        && PlaneToggleMeta.ColorTintMeta != null)
                    {
                        var colorTintMeta = PlaneToggleMeta.ColorTintMeta;

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

                    IsOn = PlaneToggleMeta.Ison;
                    Interactable = PlaneToggleMeta.Interactable;
                    SoundEffectId = PlaneToggleMeta.SoundId;
                    planeMetamorphoseStatus = MetamorphoseStatus.SelfImage;
                    break;
                case MetamorphoseStatus.SelfImage:
                    ToggleImage.As<YuLegoImage>().Metamorphose(PlaneToggleMeta.ToggleImageMeta);
                    planeMetamorphoseStatus = MetamorphoseStatus.Background;
                    break;
                case MetamorphoseStatus.Background:
                    FrontImage.As<YuLegoImage>().Metamorphose(PlaneToggleMeta
                            .ImageFrontPointRectMeta, PlaneToggleMeta.ImageFrontPointImageMeta);
                    planeMetamorphoseStatus = MetamorphoseStatus.Toggle;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        
        #endregion
    }
}