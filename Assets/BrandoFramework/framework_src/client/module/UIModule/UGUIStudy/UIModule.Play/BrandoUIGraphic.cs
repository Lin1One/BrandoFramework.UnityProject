using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Client.UI
{
    /// <summary>
    /// Base class for all UI components that should be derived from when creating new Graphic types.
    /// UI 组件基类
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public  class BrandoUIGraphic : BrandoUIBehaviour, ICanvasElement
    {
        protected BrandoUIGraphic()
        {
            useLegacyMeshGeneration = true;
        }

        #region  Transform

        [NonSerialized]
        private RectTransform m_RectTransform;
        /// <summary>
        /// The RectTransform component used by the Graphic. Cached for speed.
        /// </summary>
        public RectTransform rectTransform
        {
            get
            {
                // The RectTransform is a required component that must not be destroyed. Based on this assumption, a
                // null-reference check is sufficient.
                if (ReferenceEquals(m_RectTransform, null))
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }
                return m_RectTransform;
            }
        }

        /// <summary>
        /// Make the Graphic have the native size of its content.
        /// </summary>
        public virtual void SetNativeSize() { }

        #endregion

        #region MonoBehavior 生命周期
        /// <summary>
        /// Mark the Graphic and the canvas as having been changed.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CacheCanvas();
            BrandoUIGraphicRegistry.RegisterGraphicForCanvas(canvas, this); //注册至 Canvas

#if UNITY_EDITOR
            BrandoGraphicRebuildTracker.TrackGraphic(this);
#endif
            if (s_WhiteTexture == null)
            {
                s_WhiteTexture = Texture2D.whiteTexture;
            }
            SetAllDirty();  //设置为脏，等待渲染
        }

        /// <summary>
        /// Clear references.
        /// </summary>
        protected override void OnDisable()
        {
#if UNITY_EDITOR
            BrandoGraphicRebuildTracker.UnTrackGraphic(this);
#endif
            BrandoUIGraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
            BrandoCanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            if (canvasRenderer != null)
                canvasRenderer.Clear();

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        /// <summary>
        /// Trans外观尺寸变化
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            if (gameObject.activeInHierarchy)
            {
                // prevent double dirtying...
                if (BrandoCanvasUpdateRegistry.IsRebuildingLayout())
                {
                    SetVerticesDirty();
                }
                else
                {
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        /// <summary>
        /// 父物体改变前调
        /// </summary>
        protected override void OnBeforeTransformParentChanged()
        {
            BrandoUIGraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        /// <summary>
        /// 父物体改变
        /// </summary>
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            m_Canvas = null;

            if (!IsActive())
            {
                return;
            }
            CacheCanvas();
            BrandoUIGraphicRegistry.RegisterGraphicForCanvas(canvas, this);
            SetAllDirty();
        }

        protected override void Reset()
        {
            SetAllDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetAllDirty();
        }

#endif

        #endregion

        #region Raycast

        [SerializeField]
        private bool m_RaycastTarget = true;
        /// <summary>
        /// 是否接受射线
        /// </summary>
        public virtual bool raycastTarget
        {
            get
            {
                return m_RaycastTarget;
            }
            set
            {
                m_RaycastTarget = value;
            }
        }

        /// <summary>
        /// 当图像射线发射器发射射线至场景中，
        /// 用各 UI 元素其 RectTransform Rect 判断是否有对其进行操作，进行具体的交互操作
        /// </summary>
        /// <param name="sp">Screen point being tested</param>
        /// <param name="eventCamera">Camera that is being used for the testing.</param>
        /// <returns>True if the provided point is a valid location for GraphicRaycaster raycasts. 
        /// 返回传入的位置是否有效位置 
        /// </returns>
        public virtual bool Raycast(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled)
            {
                return false;
            }

            var t = transform;
            var components = ListPool<Component>.Get();

            bool ignoreParentGroups = false;
            //是否继续遍历
            bool continueTraversal = true;

            while (t != null)
            {
                ///遍历该物体的所有组件
                t.GetComponents(components);
                for (var i = 0; i < components.Count; i++)
                {
                    //遇到子 Canvas 停止遍历
                    var canvas = components[i] as Canvas;
                    if (canvas != null && canvas.overrideSorting)
                    {
                        continueTraversal = false;
                    }

                    //射线接收过滤器组件
                    var filter = components[i] as ICanvasRaycastFilter;
                    if (filter == null)
                    {
                        continue;
                    }

                    var raycastValid = true;

                    //Canvas 组
                    var group = components[i] as CanvasGroup;
                    if (group != null)
                    {
                        if (ignoreParentGroups == false && group.ignoreParentGroups)
                        {
                            ignoreParentGroups = true;
                            raycastValid = filter.IsRaycastLocationValid(sp, eventCamera);
                        }
                        else if (!ignoreParentGroups)
                            raycastValid = filter.IsRaycastLocationValid(sp, eventCamera);
                    }
                    else
                    {
                        //射线有效性
                        raycastValid = filter.IsRaycastLocationValid(sp, eventCamera);
                    }

                    
                    if (!raycastValid)
                    {
                        //射线被过滤，阻断，不再穿透至下一物体
                        ListPool<Component>.Release(components);
                        return false;
                    }
                }
                t = continueTraversal ? t.parent : null;
            }
            ListPool<Component>.Release(components);
            return true;
        }

        #endregion

        #region Cull

        /// <summary>
        /// This method must be called when CanvasRenderer.cull
        /// is modified.
        /// 画布渲染器剔除修改回调
        /// </summary>
        /// <remarks>
        /// This can be used to perform operations 
        /// that were previously skipped because the <c>Graphic</c> was culled.
        /// </remarks>
        public virtual void OnCullingChanged()
        {
            if (!canvasRenderer.cull && (m_VertsDirty || m_MaterialDirty))
            {
                /// When we were culled, we potentially skipped calls to <c>Rebuild</c>.
                BrandoCanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
            }
        }

        #endregion

        #region Canvas

        [NonSerialized]
        private Canvas m_Canvas;
        /// <summary>
        /// 该图像渲染到的 Canvas 
        /// </summary>
        /// <remarks>
        /// In the situation where the Graphic is used in a hierarchy with multiple Canvases, 
        /// the Canvas closest to the root will be used.
        /// </remarks>
        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    CacheCanvas();
                }
                return m_Canvas;
            }
        }

        [NonSerialized]
        private CanvasRenderer m_CanvasRenderer;
        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (ReferenceEquals(m_CanvasRenderer, null))
                {
                    m_CanvasRenderer = GetComponent<CanvasRenderer>();
                }
                return m_CanvasRenderer;
            }
        }

        /// <summary>
        /// 缓存自身所在 Canvas
        /// </summary>
        private void CacheCanvas()
        {
            var list = ListPool<Canvas>.Get();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count > 0)
            {
                // Find the first active and enabled canvas.
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].isActiveAndEnabled)
                    {
                        m_Canvas = list[i];
                        break;
                    }
                }
            }
            else
                m_Canvas = null;
            ListPool<Canvas>.Release(list);
        }

        /// <summary>
        /// Absolute depth of the graphic, used by rendering and events -- lowest to highest.
        /// </summary>
        /// <example>
        /// The depth is relative to the first root canvas.
        /// 相对于根 Canvas
        /// Canvas
        ///  Graphic - 1
        ///  Graphic - 2
        ///  Nested Canvas
        ///     Graphic - 3
        ///     Graphic - 4
        ///  Graphic - 5
        ///
        /// This value is used to determine draw and event ordering.
        /// </example>
        public int depth
        {
            get
            {
                return canvasRenderer.absoluteDepth;
            }
        }

        protected override void OnCanvasHierarchyChanged()
        {
            // Use m_Cavas so we dont auto call CacheCanvas
            Canvas currentCanvas = m_Canvas;

            // Clear the cached canvas. Will be fetched below if active.
            m_Canvas = null;

            if (!IsActive())
                return;

            CacheCanvas();

            if (currentCanvas != m_Canvas)
            {
                BrandoUIGraphicRegistry.UnregisterGraphicForCanvas(currentCanvas, this);

                // Only register if we are active and enabled as OnCanvasHierarchyChanged can get called
                // during object destruction and we dont want to register ourself and then become null.
                if (IsActive())
                {

                }
                BrandoUIGraphicRegistry.RegisterGraphicForCanvas(canvas, this);
            }
        }



        #endregion

        #region 几何信息

        #region 网格信息

        [NonSerialized]
        protected static Mesh s_Mesh;

        /// <summary>
        /// 生效网格
        /// </summary>
        protected static Mesh workerMesh
        {
            get
            {
                if (s_Mesh == null)
                {
                    s_Mesh = new Mesh();
                    s_Mesh.name = "Shared UI Mesh";
                    s_Mesh.hideFlags = HideFlags.HideAndDontSave;
                }
                return s_Mesh;
            }
        }

        /// <summary>
        /// 顶点助手
        /// </summary>
        [NonSerialized]
        private static readonly VertexHelper s_VertexHelper = new VertexHelper();

        /// <summary>
        /// 是否使用传统网格生成
        /// </summary>
        protected bool useLegacyMeshGeneration { get; set; }

        [Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.", false)]
        /// <summary>
        /// Callback function when a UI element needs to generate vertices. 
        /// Fills the vertex buffer data.
        /// 当 UI 元素需生成顶点信息时回调，填充顶点 buffer 数据
        /// </summary>
        /// <param name="m">Mesh to populate with UI data.</param>
        /// <remarks>
        /// Used by Text, UI.Image, and RawImage for example to generate vertices specific to their use case.
        /// Text，Image，RawImage
        /// </remarks>
        protected virtual void OnPopulateMesh(Mesh m)
        {
            OnPopulateMesh(s_VertexHelper); //将顶点信息保存至 VertexHelper
            s_VertexHelper.FillMesh(m);
        }

        /// <summary>
        /// Callback function when a UI element needs to generate vertices. 
        /// Fills the vertex buffer data.
        /// 填充网格
        /// </summary>
        protected virtual void OnPopulateMesh(VertexHelper vh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

            Color32 color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(0f, 0f));
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(1f, 1f));
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(1f, 0f));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        #endregion

        #region 材质

        protected static Material s_DefaultUI = null;
        /// <summary>
        /// 静态默认材质
        /// </summary>
        public static Material defaultGraphicMaterial
        {
            get
            {
                if (s_DefaultUI == null)
                {
                    s_DefaultUI = Canvas.GetDefaultCanvasMaterial();
                }
                return s_DefaultUI;
            }
        }

        public virtual Material defaultMaterial
        {
            get { return defaultGraphicMaterial; }
        }

        // Cached and saved values
        [FormerlySerializedAs("当前材质")]
        [SerializeField]
        protected Material m_Material;
        public virtual Material material
        {
            get
            {
                return (m_Material != null) ? m_Material : defaultMaterial;
            }
            set
            {
                if (m_Material == value)
                    return;

                m_Material = value;
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// The material that will be sent for Rendering (Read only).
        /// 实际发送至CanvasRenderer渲染的材质
        /// </summary>
        /// <remarks>
        /// This is the material that actually gets sent to the CanvasRenderer.
        /// By default it's the same as [[Graphic.material]]. 
        /// When extending Graphic you can override this to send
        /// a different material to the CanvasRenderer than the one set by Graphic.material. 
        /// This is useful if you want to modify the user 
        /// set material in a non destructive manner.
        /// </remarks>
        public virtual Material materialForRendering
        {
            get
            {
                //调用物体的材质修改组件对基础材质进行处理
                var components = ListPool<Component>.Get();
                GetComponents(typeof(IMaterialModifier), components);
                var currentMat = material;
                for (var i = 0; i < components.Count; i++)
                {
                    currentMat = (components[i] as IMaterialModifier).
                        GetModifiedMaterial(currentMat);
                }
                ListPool<Component>.Release(components);
                return currentMat;
            }
        }

        #endregion

        #region 颜色处理

        [SerializeField]
        private Color m_Color = Color.white;
        /// <summary>
        /// 图像基础颜色
        /// </summary>
        public virtual Color color
        {
            get
            {
                return m_Color;
            }
            set
            {
                if (SetPropertyUtility.SetColor(ref m_Color, value))
                {
                    SetVerticesDirty();
                }
            }
        }

        ///<summary>
        ///Tweens the CanvasRenderer color associated with this Graphic.
        ///混合颜色
        ///</summary>
        ///<param name="targetColor">Target color.</param>
        ///<param name="duration">Tween duration.</param>
        ///<param name="ignoreTimeScale">Should ignore Time.scale?</param>
        ///<param name="useAlpha">Should also Tween the alpha channel?</param>
        public virtual void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, true);
        }

        ///<summary>
        ///Tweens the CanvasRenderer color associated with this Graphic.
        ///混合颜色
        ///</summary>
        ///<param name="targetColor">Target color.</param>
        ///<param name="duration">Tween duration.</param>
        ///<param name="ignoreTimeScale">Should ignore Time.scale?</param>
        ///<param name="useAlpha">Should also Tween the alpha channel?</param>
        /// <param name="useRGB">Should the color or the alpha be used to tween</param>
        public virtual void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
        {
            if (canvasRenderer == null || (!useRGB && !useAlpha))
                return;

            Color currentColor = canvasRenderer.GetColor();
            if (currentColor.Equals(targetColor))
            {
                ////m_ColorTweenRunner.StopTween();
                return;
            }

            //ColorTween.ColorTweenMode mode = (useRGB && useAlpha ?
            //    ColorTween.ColorTweenMode.All :
            //    (useRGB ? ColorTween.ColorTweenMode.RGB : ColorTween.ColorTweenMode.Alpha));

            //var colorTween = new ColorTween { duration = duration, startColor = canvasRenderer.GetColor(), targetColor = targetColor };
            //colorTween.AddOnChangedCallback(canvasRenderer.SetColor);
            //colorTween.ignoreTimeScale = ignoreTimeScale;
            //colorTween.tweenMode = mode;
            //m_ColorTweenRunner.StartTween(colorTween);
        }

        static private Color CreateColorFromAlpha(float alpha)
        {
            var alphaColor = Color.black;
            alphaColor.a = alpha;
            return alphaColor;
        }

        ///<summary>
        ///Tweens the alpha of the CanvasRenderer color associated with this Graphic.
        ///</summary>
        ///<param name="alpha">Target alpha.</param>
        ///<param name="duration">Duration of the tween in seconds.</param>
        ///<param name="ignoreTimeScale">Should ignore [[Time.scale]]?</param>
        public virtual void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            CrossFadeColor(CreateColorFromAlpha(alpha), duration, ignoreTimeScale, true, false);
        }

        #endregion

        #region Texture

        protected static Texture2D s_WhiteTexture = null;
        /// <summary>
        /// The graphic's texture. (只读).
        /// </summary>
        /// <remarks>
        /// This is the Texture that gets passed to the CanvasRenderer,
        /// Material and then Shader _MainTex.
        /// When implementing your own Graphic 
        /// you can override this to control 
        /// which texture goes through the UI Rendering pipeline.
        /// Bear in mind that Unity tries to batch UI elements together 
        /// to improve performance, 
        /// so its ideal to work with atlas to 
        /// reduce the number of draw calls.
        /// </remarks>
        public virtual Texture mainTexture
        {
            get
            {
                return s_WhiteTexture;
            }
        }

        #endregion

        /// <summary>
        /// Returns a pixel perfect Rect closest to the Graphic RectTransform.
        /// 获取相对于图形 RectTransform 像素精确的矩形
        /// </summary>
        /// <remarks>
        /// 仅当 Canvas is in Screen Space 情况生效
        /// </remarks>
        public Rect GetPixelAdjustedRect()
        {
            if (!canvas ||
                canvas.renderMode == RenderMode.WorldSpace ||
                canvas.scaleFactor == 0.0f ||
                !canvas.pixelPerfect)
                return rectTransform.rect;
            else
                //返回已调整像素的 Rect
                return RectTransformUtility.PixelAdjustRect(rectTransform, canvas);
        }

        ///<summary>
        ///Adjusts the given pixel to be pixel perfect.
        ///调整像素点至精确
        ///</summary>
        ///<param name="point">Local space point.</param>
        ///<returns>Pixel perfect adjusted point.</returns>
        ///<remarks>
        ///Note: This is only accurate if the Graphic root Canvas is in Screen Space.
        ///</remarks>
        public Vector2 PixelAdjustPoint(Vector2 point)
        {
            if (!canvas || 
                canvas.renderMode == RenderMode.WorldSpace || 
                canvas.scaleFactor == 0.0f || 
                !canvas.pixelPerfect)
                return point;
            else
            {
                return RectTransformUtility.PixelAdjustPoint(point, transform, canvas);
            }
        }

        #endregion

        #region 脏渲染

        #region 标记
        /// <summary>
        /// 顶点是否修改
        /// </summary>s
        [NonSerialized]
        private bool m_VertsDirty;

        /// <summary>
        /// 材质是否修改
        /// </summary>
        [NonSerialized]
        private bool m_MaterialDirty;

        /// <summary>
        ///  Layout 修改回调
        /// </summary>
        [NonSerialized]
        protected UnityAction m_OnDirtyLayoutCallback;

        /// <summary>
        /// 顶点修改回调
        /// </summary>
        [NonSerialized]
        protected UnityAction m_OnDirtyVertsCallback;

        /// <summary>
        /// 材质修改回调
        /// </summary>
        [NonSerialized]
        protected UnityAction m_OnDirtyMaterialCallback;

        /// <summary>
        /// Add a listener to receive notification when the graphics layout is dirtied.
        /// </summary>
        public void RegisterDirtyLayoutCallback(UnityAction action)
        {
            m_OnDirtyLayoutCallback += action;
        }

        /// <summary>
        /// Remove a listener from receiving notifications when the graphics layout are dirtied
        /// </summary>
        public void UnregisterDirtyLayoutCallback(UnityAction action)
        {
            m_OnDirtyLayoutCallback -= action;
        }

        /// <summary>
        /// Add a listener to receive notification when the graphics vertices are dirtied.
        /// </summary>
        public void RegisterDirtyVerticesCallback(UnityAction action)
        {
            m_OnDirtyVertsCallback += action;
        }

        /// <summary>
        /// Remove a listener from receiving notifications when the graphics vertices are dirtied
        /// </summary>
        public void UnregisterDirtyVerticesCallback(UnityAction action)
        {
            m_OnDirtyVertsCallback -= action;
        }

        /// <summary>
        /// Add a listener to receive notification when the graphics material is dirtied.
        /// </summary>
        public void RegisterDirtyMaterialCallback(UnityAction action)
        {
            m_OnDirtyMaterialCallback += action;
        }

        /// <summary>
        /// Remove a listener from receiving notifications when the graphics material are dirtied
        /// </summary>
        public void UnregisterDirtyMaterialCallback(UnityAction action)
        {
            m_OnDirtyMaterialCallback -= action;
        }

        /// <summary>
        /// 标记 Layout 为脏，将重建
        /// </summary>
        /// <remarks>
        /// Send a OnDirtyLayoutCallback notification 
        /// if any elements are registered. See RegisterDirtyLayoutCallback
        /// </remarks>
        public virtual void SetLayoutDirty()
        {
            if (IsActive())
            {
                LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
                m_OnDirtyLayoutCallback?.Invoke();
            }
        }

        /// <summary>
        /// 标记顶点为脏，将重建
        /// </summary>
        /// <remarks>
        /// Send a OnDirtyVertsCallback notification 
        /// if any elements are registered. See RegisterDirtyVerticesCallback
        /// </remarks>
        public virtual void SetVerticesDirty()
        {
            if (IsActive())
            {
                m_VertsDirty = true;
                BrandoCanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                m_OnDirtyVertsCallback?.Invoke();
            }
        }

        /// <summary>
        /// Mark the material as dirty and needing rebuilt.
        /// 标记材质为脏，将重建
        /// </summary>
        /// <remarks>
        /// Send a OnDirtyMaterialCallback notification 
        /// if any elements are registered. See RegisterDirtyMaterialCallback
        /// </remarks>
        public virtual void SetMaterialDirty()
        {
            if (IsActive())
            {
                m_MaterialDirty = true;
                BrandoCanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
                m_OnDirtyMaterialCallback?.Invoke();
            }
        }

        public virtual void SetAllDirty()
        {
            SetLayoutDirty();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        #endregion

        #region 重建
        /// <summary>
        /// Rebuilds the graphic geometry and its material on the PreRender cycle.
        /// 重建图形及其材质 （处于 PreRender cycle）
        /// </summary>
        /// <param name="update">The current step of the rendering CanvasUpdate cycle.</param>
        public virtual void Rebuild(CanvasUpdate update)
        {
            if (canvasRenderer.cull)
            {
                return;
            }
            switch (update)
            {
                case CanvasUpdate.PreRender:
                    if (m_VertsDirty)
                    {
                        //更新几何信息（顶点，网格）
                        UpdateGeometry();
                        m_VertsDirty = false;
                    }
                    if (m_MaterialDirty)
                    {
                        UpdateMaterial();
                        m_MaterialDirty = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// Call to update the geometry of the Graphic onto the CanvasRenderer.
        /// 更新图形的几何信息到 CanvasRenderer
        /// </summary>
        protected virtual void UpdateGeometry()
        {
            if (useLegacyMeshGeneration)
                DoLegacyMeshGeneration();
            else
                DoMeshGeneration();
        }

        //遗留网格生成
        private void DoLegacyMeshGeneration()
        {
            if (rectTransform != null &&
                rectTransform.rect.width >= 0 &&
                rectTransform.rect.height >= 0)
            {
#pragma warning disable 618
                OnPopulateMesh(workerMesh);
#pragma warning restore 618
            }
            else
            {
                workerMesh.Clear();
            }
            var components = ListPool<Component>.Get();
            GetComponents(typeof(IMeshModifier), components);
            for (var i = 0; i < components.Count; i++)
            {
#pragma warning disable 618
                ((IMeshModifier)components[i]).ModifyMesh(workerMesh);
#pragma warning restore 618
            }
            ListPool<Component>.Release(components);
            canvasRenderer.SetMesh(workerMesh);
        }

        /// <summary>
        /// 网格生成
        /// </summary>
        private void DoMeshGeneration()
        {
            if (rectTransform != null &&
                rectTransform.rect.width >= 0 && 
                rectTransform.rect.height >= 0)
            {
                OnPopulateMesh(s_VertexHelper);
            } 
            else
            {
                // clear the vertex helper so invalid graphics dont draw.
                s_VertexHelper.Clear(); 
            }

            var components = ListPool<Component>.Get();
            GetComponents(typeof(IMeshModifier), components);

            for (var i = 0; i < components.Count; i++)
                ((IMeshModifier)components[i]).ModifyMesh(s_VertexHelper);

            ListPool<Component>.Release(components);

            s_VertexHelper.FillMesh(workerMesh);
            canvasRenderer.SetMesh(workerMesh);
        }

        /// <summary>
        /// Call to update the Material of the graphic onto the CanvasRenderer.
        /// 更新图形的材质信息到 CanvasRenderer
        /// </summary>
        protected virtual void UpdateMaterial()
        {
            if (!IsActive())
                return;

            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(materialForRendering, 0);
            canvasRenderer.SetTexture(mainTexture);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only callback that is issued by Unity 
        /// if a rebuild of the Graphic is required.
        /// Currently sent when an asset is reimported.
        /// UI 图形重建回调
        /// </summary>
        public virtual void OnRebuildRequested()
        {
            // when rebuild is requested we need to rebuild all the graphics /
            // and associated components... The correct way to do this is by
            // calling OnValidate... Because MB's don't have a common base class
            // we do this via reflection. It's nasty and ugly... Editor only.
            var mbs = gameObject.GetComponents<MonoBehaviour>();
            foreach (var mb in mbs)
            {
                if (mb == null)
                    continue;
                var methodInfo = mb.GetType().
                    GetMethod("OnValidate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo != null)
                    methodInfo.Invoke(mb, null);
            }
        }

        public virtual void LayoutComplete()
        { }

        public virtual void GraphicUpdateComplete()
        { }
#endif
        #endregion

        #endregion

        #region UIAnimation

        /// <summary>
        /// Animation Properties 修改
        /// </summary>
        protected override void OnDidApplyAnimationProperties()
        {
            SetAllDirty();
        }
        #endregion

    }
}
