#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client.LegoUI
{
    [AddComponentMenu("Yu/LegoUI/YuLego Slider", 33)]
    [RequireComponent(typeof(RectTransform))]
    public class YuLegoSlider :
        YuAbsLegoInteractableControl,
        IDragHandler,
        IInitializePotentialDragHandler,
        ICanvasElement,
        IYuLegoSlider
    {
        #region UGUISrc

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

        [Serializable]
        public class SliderEvent : UnityEvent<float>
        {
        }

        [SerializeField] private RectTransform m_FillRect;

        public RectTransform fillRect
        {
            get { return m_FillRect; }
            set
            {
                if (YuLegoSetPropertyUtility.SetClass(ref m_FillRect, value))
                {
                    UpdateCachedReferences();
                    UpdateVisuals();
                }
            }
        }

        [SerializeField] private RectTransform m_HandleRect;

        public RectTransform handleRect
        {
            get { return m_HandleRect; }
            set
            {
                if (YuLegoSetPropertyUtility.SetClass(ref m_HandleRect, value))
                {
                    UpdateCachedReferences();
                    UpdateVisuals();
                }
            }
        }

        [Space] [SerializeField] private Direction m_Direction = Direction.LeftToRight;

        public Direction direction
        {
            get { return m_Direction; }
            set
            {
                if (YuLegoSetPropertyUtility.SetStruct(ref m_Direction, value)) UpdateVisuals();
            }
        }

        [SerializeField] private float m_MinValue = 0;

        public float minValue
        {
            get { return m_MinValue; }
            set
            {
                if (YuLegoSetPropertyUtility.SetStruct(ref m_MinValue, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField] private float m_MaxValue = 1;

        public float maxValue
        {
            get { return m_MaxValue; }
            set
            {
                if (YuLegoSetPropertyUtility.SetStruct(ref m_MaxValue, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField] private bool m_WholeNumbers = false;

        public bool wholeNumbers
        {
            get { return m_WholeNumbers; }
            set
            {
                if (YuLegoSetPropertyUtility.SetStruct(ref m_WholeNumbers, value))
                {
                    Set(m_Value);
                    UpdateVisuals();
                }
            }
        }

        [SerializeField] protected float m_Value;

        public virtual float value
        {
            get
            {
                if (wholeNumbers)
                    return Mathf.Round(m_Value);
                return m_Value;
            }
            set { Set(value); }
        }

        public float normalizedValue
        {
            get
            {
                if (Mathf.Approximately(minValue, maxValue))
                    return 0;
                return Mathf.InverseLerp(minValue, maxValue, value);
            }
            set { this.value = Mathf.Lerp(minValue, maxValue, value); }
        }

        [Space]

        // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        [SerializeField]
        private SliderEvent m_OnValueChanged = new SliderEvent();

        public SliderEvent onValueChanged
        {
            get { return m_OnValueChanged; }
            set { m_OnValueChanged = value; }
        }

        // Private fields

        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // Size of each step.
        float stepSize
        {
            get { return wholeNumbers ? 1 : (maxValue - minValue) * 0.1f; }
        }

        protected YuLegoSlider()
        {
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(m_Value, false);
                // Update rects since other things might affect them even if value didn't change.
                UpdateVisuals();
            }

            //var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
            //if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
            //    CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if DEBUG
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(value);
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
            UpdateCachedReferences();
            Set(m_Value, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            m_Value = ClampValue(m_Value);
            float oldNormalizedValue = normalizedValue;
            if (m_FillContainerRect != null)
            {
                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                    oldNormalizedValue = m_FillImage.fillAmount;
                else
                    oldNormalizedValue = (reverseValue
                        ? 1 - m_FillRect.anchorMin[(int)axis]
                        : m_FillRect.anchorMax[(int)axis]);
            }
            else if (m_HandleContainerRect != null)
                oldNormalizedValue = (reverseValue
                    ? 1 - m_HandleRect.anchorMin[(int)axis]
                    : m_HandleRect.anchorMin[(int)axis]);

            UpdateVisuals();

            if (oldNormalizedValue != normalizedValue)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                onValueChanged.Invoke(m_Value);
            }
        }

        void UpdateCachedReferences()
        {
            if (m_FillRect && m_FillRect != (RectTransform)transform)
            {
                m_FillTransform = m_FillRect.transform;
                m_FillImage = m_FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_FillRect = null;
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (m_HandleRect && m_HandleRect != (RectTransform)transform)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleRect = null;
                m_HandleContainerRect = null;
            }
        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            if (wholeNumbers)
                newValue = Mathf.Round(newValue);
            return newValue;
        }

        // Set the valueUpdate the visible Image.
        void Set(float input)
        {
            Set(input, true);
        }

        protected virtual void Set(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = ClampValue(input);

            // If the stepped value doesn't match the last one, it's time to update
            if (m_Value == newValue)
                return;

            m_Value = newValue;
            UpdateVisuals();
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Slider.value", this);
                m_OnValueChanged.Invoke(newValue);
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!IsActive())
                return;

            UpdateVisuals();
        }

        enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        Axis axis
        {
            get
            {
                return (m_Direction == Direction.LeftToRight || m_Direction == Direction.RightToLeft)
                    ? Axis.Horizontal
                    : Axis.Vertical;
            }
        }

        bool reverseValue
        {
            get { return m_Direction == Direction.RightToLeft || m_Direction == Direction.TopToBottom; }
        }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if DEBUG
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, m_FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = normalizedValue;
                }
                else
                {
                    if (reverseValue)
                        anchorMin[(int)axis] = 1 - normalizedValue;
                    else
                        anchorMax[(int)axis] = normalizedValue;
                }

                m_FillRect.anchorMin = anchorMin;
                m_FillRect.anchorMax = anchorMax;
            }

            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[(int)axis] =
                    anchorMax[(int)axis] = (reverseValue ? (1 - normalizedValue) : normalizedValue);
                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect ?? m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[(int)axis] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam,
                    out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / clickRect.rect.size[(int)axis]);
                normalizedValue = (reverseValue ? 1f - val : val);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position,
                    eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position,
                    eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                        Set(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        Set(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        Set(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        Set(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void SetDirection(Direction direction, bool includeRectLayouts)
        {
            Axis oldAxis = axis;
            bool oldReverse = reverseValue;
            this.direction = direction;

            if (!includeRectLayouts)
                return;

            if (axis != oldAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != oldReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }

        #endregion

        #region Lego

        public override void Construct(ILegoUI locUI, object obj = null)
        {
            base.Construct(locUI);
            onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float progressValue)
        {
            value = progressValue;
            m_PushAction?.Invoke(value);
            InvokeInteractableMethod(LegoInteractableType.OnValueChanged);
        }

        public float Progress
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        #endregion

        #region 数据响应

        public void ReceiveValueChange(float newValue)
        {
            value = newValue;
        }

        public void ReceiveDirectionChange(byte newDirection)
        {
            direction = (Direction)newDirection;
        }

        public void ReceiveMinValueChange(float newValue)
        {
            minValue = newValue;
        }

        public void ReceiveMaxValueChange(float newValue)
        {
            maxValue = newValue;
        }

        public void ReceiveIsWholeNumbersChange(bool newValue)
        {
            wholeNumbers = newValue;
        }


        private Action<float> m_PushAction;

        public void BindingPush(Action<float> pushAction)
        {
            m_PushAction = pushAction;
        }

        #endregion

        #region 子控件

        private YuLegoImage imageBackground;

        protected YuLegoImage ImageBackground
        {
            get
            {
                if (imageBackground != null)
                {
                    return imageBackground;
                }

                imageBackground = RectTransform.Find("Background")
                    .GetComponent<YuLegoImage>();
                return imageBackground;
            }
        }

        private RectTransform rectFillArea;

        protected RectTransform RectFillArea
        {
            get
            {
                if (rectFillArea != null)
                {
                    return rectFillArea;
                }

                rectFillArea = RectTransform.Find("Fill Area")
                    .GetComponent<RectTransform>();
                return rectFillArea;
            }
        }

        private YuLegoImage imageFill;

        protected YuLegoImage ImageFill
        {
            get
            {
                if (imageFill != null)
                {
                    return imageFill;
                }

                imageFill = RectFillArea.Find("Fill")
                    .GetComponent<YuLegoImage>();
                return imageFill;
            }
        }

        private RectTransform handleSlideArea;

        private RectTransform HandleSlideArea
        {
            get
            {
                if (handleSlideArea != null)
                {
                    return handleSlideArea;
                }

                handleSlideArea = RectTransform.Find("Handle Slide Area")
                    .GetComponent<RectTransform>();
                return handleSlideArea;
            }
        }

        private YuLegoImage handleImage;

        private YuLegoImage HandleImage
        {
            get
            {
                if (handleImage != null)
                {
                    return handleImage;
                }

                handleImage = HandleSlideArea.Find("Handle")
                    .GetComponent<YuLegoImage>();
                return handleImage;
            }
        }

        #endregion

        #region 元数据变形

        private enum YuLegoSliderMetamorphoseStatus : byte
        {
            Slider,
            Image_Background,
            RectTransform_FillArea,
            Image_Fill,
            RectTransform_HandleSlideArea,
            Image_Handle
        }

        private YuLegoSliderMetamorphoseStatus metamorphoseStatus;
        private LegoSliderMeta sliderMeta;

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (sliderMeta == null)
            {
                sliderMeta = uiMeta.NextSlider;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (metamorphoseStatus)
            {
                case YuLegoSliderMetamorphoseStatus.Slider:
                    YuLegoUtility.MetamorphoseRect(RectTransform, uiMeta.CurrentRect);

                    direction = sliderMeta.Direction;
                    minValue = sliderMeta.MinValue;
                    maxValue = sliderMeta.MaxValue == 0?1:sliderMeta.MaxValue;
                    wholeNumbers = sliderMeta.IsWholeNumbers;

                    if (sliderMeta.Transition == LegoTransition.ColorTint)
                    {
                        var colorTintMeta = sliderMeta.ColorTintMeta;

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

                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.Image_Background;
                    break;
                case YuLegoSliderMetamorphoseStatus.Image_Background:
                    ImageBackground.Metamorphose(sliderMeta.BackgroundImageRect,
                        sliderMeta.BackgroundImageMeta);
                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.RectTransform_FillArea;
                    break;
                case YuLegoSliderMetamorphoseStatus.RectTransform_FillArea:
                    YuLegoUtility.MetamorphoseRect(RectFillArea, sliderMeta.FillAreaMeta);
                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.Image_Fill;
                    break;
                case YuLegoSliderMetamorphoseStatus.Image_Fill:
                    ImageFill.Metamorphose(sliderMeta.FillImageRect, sliderMeta.FillImageMeta);
                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.RectTransform_HandleSlideArea;
                    break;
                case YuLegoSliderMetamorphoseStatus.RectTransform_HandleSlideArea:
                    YuLegoUtility.MetamorphoseRect(HandleSlideArea, sliderMeta.HandleSlideAreaRect);
                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.Image_Handle;
                    break;
                case YuLegoSliderMetamorphoseStatus.Image_Handle:
                    HandleImage.Metamorphose(sliderMeta.HandleImageRect, sliderMeta.HandleImageMeta);

                    sliderMeta = null;
                    metamorphoseStatus = YuLegoSliderMetamorphoseStatus.Slider;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}