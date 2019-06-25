using System;
using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 内容尺寸调整器
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class BrandoContentSizeFitter : BrandoUIBehaviour, ILayoutSelfController
    {
        protected BrandoContentSizeFitter()
        { }

        /// <summary>
        /// 调整尺寸模式
        /// </summary>
        public enum FitMode
        {
            Unconstrained,//无约束
            MinSize,
            PreferredSize
        }

        [SerializeField]
        protected FitMode m_HorizontalFit = FitMode.Unconstrained;
        public FitMode horizontalFit
        {
            get
            {
                return m_HorizontalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_HorizontalFit, value))
                    SetDirty();
            }
        }

        [SerializeField]
        protected FitMode m_VerticalFit = FitMode.Unconstrained;
        public FitMode verticalFit
        {
            get
            {
                return m_VerticalFit;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value))
                    SetDirty();
            }
        }

        [NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;


        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        #endregion

        /// <summary>
        /// Rect 尺寸变化
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }


        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
            if (fitting == FitMode.Unconstrained)
                return;

            m_Tracker.Add(this, 
                rectTransform, 
                (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY)
                );

            // Set size to min or preferred size
            if (fitting == FitMode.MinSize)
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_Rect, axis));
            else
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(m_Rect, axis));
        }

        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

    #endif
    }
}
