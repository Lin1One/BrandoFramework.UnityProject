#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图开关控件。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Toggle", 31)]
    [RequireComponent(typeof(RectTransform))]
    public class YuLegoToggle
        : YuAbsLegoInteractableControl,
            IPointerClickHandler,
            ISubmitHandler,
            ICanvasElement,
            IYuLegoToggle
    {
        #region UGUISrc

        public enum ToggleTransition
        {
            None,
            Fade
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }

        /// <summary>
        /// Transition type.
        /// </summary>
        public ToggleTransition toggleTransition = ToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic graphic;

        // group that this toggle can belong to
        [SerializeField] private YuLegoToggleGroup m_Group;

        public YuLegoToggleGroup group
        {
            get { return m_Group; }
            set
            {
                m_Group = value;
#if DEBUG
                if (Application.isPlaying)
#endif
                {
                    SetToggleGroup(m_Group, true);
                    PlayEffect(true);
                }
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public ToggleEvent onValueChanged = new ToggleEvent();

        // Whether the toggle is on
        [FormerlySerializedAs("m_IsActive")]
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool m_IsOn;

        protected YuLegoToggle()
        {
        }

        //#if DEBUG
        //        protected override void OnValidate()
        //        {
        //            base.OnValidate();

        //            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
        //            if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
        //                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        //        }

        //#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if DEBUG
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(m_IsOn);
#endif
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(m_Group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (graphic != null)
            {
                bool oldValue = !Mathf.Approximately(graphic.canvasRenderer.GetColor().a, 0);
                if (m_IsOn != oldValue)
                {
                    m_IsOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(YuLegoToggleGroup newGroup, bool setMemberValue)
        {
            var oldGroup = m_Group;

            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (m_Group != null)
                m_Group.UnregisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                m_Group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && newGroup != oldGroup && isOn && IsActive())
                newGroup.NotifyToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool isOn
        {
            get { return m_IsOn; }
            set { Set(value); }
        }

        void Set(bool value)
        {
            Set(value, true);
        }

        void Set(bool value, bool sendCallback)
        {
            if (m_IsOn == value)
                return;

            // if we are in a group and set to true, do group logic
            m_IsOn = value;
            if (m_Group != null && IsActive())
            {
                if (m_IsOn || (!m_Group.AnyTogglesOn() && !m_Group.allowSwitchOff))
                {
                    m_IsOn = true;
                    m_Group.NotifyToggleOn(this);
                }
            }

            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            PlayEffect(toggleTransition == ToggleTransition.None);
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                onValueChanged.Invoke(m_IsOn);
            }
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (graphic == null)
                return;

#if DEBUG
            if (!Application.isPlaying)
                graphic.canvasRenderer.SetAlpha(m_IsOn ? 1f : 0f);
            else
#endif
                graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            isOn = !isOn;
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            InternalToggle();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }

        #endregion

        #region Lego

        #region 构造

        public override void Construct(ILegoUI locUI, object obj = null)
        {
            base.Construct(locUI);
            onValueChanged.AddListener(OnValueChanged);
        }

        #endregion

        #region Lego接口

        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; }
        } 

        #endregion

        #region 元数据变形

        private enum YuLegoToggleMetamorphoseStatus : byte
        {
            Toggle,
            ImageBackground,
            ImageCheck,
            SonText
        }

        #region 子控件

        private YuLegoImage backgroundImage;

        public YuLegoImage BackgroundImage
        {
            get
            {
                if (backgroundImage != null)
                {
                    return backgroundImage;
                }

                backgroundImage = RectTransform.Find("Background")
                    .GetComponent<YuLegoImage>();
                return backgroundImage;
            }
        }

        private YuLegoImage checkmarkImage;

        public YuLegoImage CheckmarkImage
        {
            get
            {
                if (checkmarkImage != null)
                {
                    return checkmarkImage;
                }

                checkmarkImage = RectTransform.Find("Background/Checkmark")
                    .GetComponent<YuLegoImage>();
                return checkmarkImage;
            }
        }

        private YuLegoText text;

        public YuLegoText Text
        {
            get
            {
                if (text != null)
                {
                    return text;
                }

                text = RectTransform.Find("Text").GetComponent<YuLegoText>();
                return text;
            }
        }

        #endregion

        private LegoToggleMeta toggleMeta;
        private YuLegoToggleMetamorphoseStatus metamorphoseStatus;

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (toggleMeta == null)
            {
                toggleMeta = uiMeta.NextToggle;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (metamorphoseStatus)
            {
                case YuLegoToggleMetamorphoseStatus.Toggle:
                    YuLegoUtility.MetamorphoseRect(RectTransform, uiMeta.CurrentRect);

                    if (toggleMeta.Transition == LegoTransition.ColorTint)
                    {
                        var colorTintMeta = toggleMeta.ColorTintMeta;

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

                    SoundEffectId = toggleMeta.SoundId;
                    metamorphoseStatus = YuLegoToggleMetamorphoseStatus.ImageBackground;
                    break;
                case YuLegoToggleMetamorphoseStatus.ImageBackground:
                    BackgroundImage.Metamorphose(toggleMeta.BackgroundRectMeta,
                        toggleMeta.BackgroundImageMeta);
                    metamorphoseStatus = YuLegoToggleMetamorphoseStatus.ImageCheck;
                    break;
                case YuLegoToggleMetamorphoseStatus.ImageCheck:
                    CheckmarkImage.Metamorphose(toggleMeta.CheckmarkRectMeta,
                        toggleMeta.CheckMarkImageMeta);
                    metamorphoseStatus = YuLegoToggleMetamorphoseStatus.SonText;
                    break;
                case YuLegoToggleMetamorphoseStatus.SonText:
                    metamorphoseStatus = YuLegoToggleMetamorphoseStatus.Toggle;
                    Text.Metamorphose(toggleMeta.TextRect, toggleMeta.TextMeta);

                    toggleMeta = null;
                    metamorphoseStatus = YuLegoToggleMetamorphoseStatus.Toggle;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 数据响应

        private void OnValueChanged(bool state)
        {
            m_PushAction?.Invoke(isOn);
            InvokeInteractableMethod(LegoInteractableType.OnValueChanged);
        }

        private Action<bool> m_PushAction;
        public void BindingPush(Action<bool> pushAction) => m_PushAction = pushAction;

        public void ReceiveIsOnChange(bool newValue)
        {
            isOn = newValue;
        }

        public void ReveiveBackgroundChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                BackgroundImage.RectTransform.gameObject.SetActive(false);
                return;
            }
            BackgroundImage.SpriteId = newValue;
            BackgroundImage.RectTransform.gameObject.SetActive(true);

        }

        public void ReveiveCheckmarkChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                CheckmarkImage.RectTransform.gameObject.SetActive(false);
                return;
            }
            CheckmarkImage.SpriteId = newValue;
            CheckmarkImage.RectTransform.gameObject.SetActive(true);
        }

        public void ReveiveTextContentChange(string newValue)
        {
            text.Text = newValue;
        }

        #endregion

        #endregion
    }
}