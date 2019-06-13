using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Client.UI
{
    /// <summary>
    /// A Graphic that is capable of being masked out.
    /// </summary>
    public abstract class BrandoUIMaskableGraphic : BrandoUIGraphic, IClippable, IMaskable, IMaterialModifier
    {
        /// <summary>
        /// 是否需要重新计算模板
        /// </summary>
        [NonSerialized]
        protected bool m_ShouldRecalculateStencil = true;

        /// <summary>
        /// StencilDepth
        /// </summary>
        [NonSerialized]
        protected int m_StencilValue;

        #region IClippable 接口 裁切

        [Serializable]
        public class CullStateChangedEvent : UnityEvent<bool> { }

        // Event delegates triggered on click.
        [SerializeField]
        private CullStateChangedEvent m_OnCullStateChanged = new CullStateChangedEvent();
        /// <summary>
        /// Callback issued when culling changes.
        /// </summary>
        /// <remarks>
        /// Called whene the culling state of this MaskableGraphic either becomes culled or visible. You can use this to control other elements of your UI as culling happens.
        /// </remarks>
        public CullStateChangedEvent onCullStateChanged
        {
            get { return m_OnCullStateChanged; }
            set { m_OnCullStateChanged = value; }
        }

        /// <summary>
        /// Interface for elements that can be clipped if they are under an IClipper
        /// </summary>
        public virtual void Cull(Rect clipRect, bool validRect)
        {
            var cull = !validRect || !clipRect.Overlaps(rootCanvasRect, true);
            UpdateCull(cull);
        }

        private void UpdateCull(bool cull)
        {
            if (canvasRenderer.cull != cull)
            {
                canvasRenderer.cull = cull;
                //UISystemProfilerApi.AddMarker("MaskableGraphic.cullingChanged", this);
                m_OnCullStateChanged.Invoke(cull);
                OnCullingChanged();
            }
        }

        public void RecalculateClipping()
        {
            UpdateClipParent();
        }
        private void UpdateClipParent()
        {
            var newParent = (maskable && IsActive()) ? MaskUtilities.GetRectMaskForClippable(this) : null;

            // if the new parent is different OR is now inactive
            if (m_ParentMask != null && (newParent != m_ParentMask || !newParent.IsActive()))
            {
                m_ParentMask.RemoveClippable(this);
                UpdateCull(false);
            }

            // don't re-add it if the newparent is inactive
            if (newParent != null && newParent.IsActive())
                newParent.AddClippable(this);

            m_ParentMask = newParent;
        }

        public virtual void SetClipRect(Rect clipRect, bool validRect)
        {
            if (validRect)
            {
                canvasRenderer.EnableRectClipping(clipRect);
            }
            else
            {
                canvasRenderer.DisableRectClipping();
            }
                
        }

        #endregion

        #region IMaskable 接口 遮罩

        /// <summary>
        /// 遮罩材质
        /// </summary>
        [NonSerialized]
        protected Material m_MaskMaterial;

        /// <summary>
        /// 2D 遮罩
        /// </summary>
        [NonSerialized]
        private RectMask2D m_ParentMask;

        // m_Maskable is whether this graphic is allowed to be masked or not. 
        // so graphics under a mask are masked out of the box.
        // Things would still work correctly if m_IncludeForMasking was always true when m_Maskable is, but performance would suffer.
        /// <summary>
        /// 可否允许被遮罩
        /// </summary>
        [NonSerialized]
        private bool m_Maskable = true;
        public bool maskable
        {
            get { return m_Maskable; }
            set
            {
                if (value == m_Maskable)
                    return;
                m_Maskable = value;
                m_ShouldRecalculateStencil = true;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// See IMaskable.RecalculateMasking
        /// </summary>
        public virtual void RecalculateMasking()
        {
            // Remove the material reference as either the graphic of the mask has been enable/ disabled.
            // This will cause the material to be repopulated from the original if need be. (case 994413)
            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = null;
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
        }

        #endregion

        #region IMaterialModifier 接口

        /// <summary>
        /// Perform material modification in this function.
        /// 获取修改后的材质
        /// </summary>
        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            var toUse = baseMaterial;

            if (m_ShouldRecalculateStencil)
            {
                var rootCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
                m_StencilValue = maskable ? 
                    MaskUtilities.GetStencilDepth(transform, rootCanvas) : 0;
                m_ShouldRecalculateStencil = false;
            }

            // if we have a enabled Mask component then it will
            // generate the mask material. This is an optimisation
            // it adds some coupling between components though :(
            Mask maskComponent = GetComponent<Mask>();
            if (m_StencilValue > 0 &&
                (maskComponent == null || !maskComponent.IsActive()))
            {
                var maskMat = StencilMaterial.Add(toUse, 
                    (1 << m_StencilValue) - 1, 
                    StencilOp.Keep, 
                    CompareFunction.Equal,
                    ColorWriteMask.All, 
                    (1 << m_StencilValue) - 1,
                    0);
                StencilMaterial.Remove(m_MaskMaterial);
                m_MaskMaterial = maskMat;
                toUse = m_MaskMaterial;
            }
            return toUse;
        }

        #endregion

        #region 生命周期

        protected override void OnDisable()
        {
            base.OnDisable();
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
            UpdateClipParent();
            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = null;

            if (GetComponent<Mask>() != null)
            {
                MaskUtilities.NotifyStencilStateChanged(this);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_ShouldRecalculateStencil = true;
            UpdateClipParent();
            SetMaterialDirty();
        }

#endif
        #endregion

        #region RectTransform

        readonly Vector3[] m_Corners = new Vector3[4];
        private Rect rootCanvasRect
        {
            get
            {
                rectTransform.GetWorldCorners(m_Corners);

                if (canvas)
                {
                    Matrix4x4 mat = canvas.rootCanvas.transform.worldToLocalMatrix;
                    for (int i = 0; i < 4; ++i)
                        m_Corners[i] = mat.MultiplyPoint(m_Corners[i]);
                }

                return new Rect(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x - m_Corners[0].x, m_Corners[2].y - m_Corners[0].y);
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            if (!isActiveAndEnabled)
                return;

            m_ShouldRecalculateStencil = true;
            UpdateClipParent();
            SetMaterialDirty();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();

            if (!isActiveAndEnabled)
                return;

            m_ShouldRecalculateStencil = true;
            UpdateClipParent();
            SetMaterialDirty();
        }
        #endregion
    }
}
