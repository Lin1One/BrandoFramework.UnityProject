using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// Labels are graphics that display text.
    /// </summary>
    public class BrandoUIText : BrandoUIMaskableGraphic, ILayoutElement
    {
        protected BrandoUIText()
        {
            useLegacyMeshGeneration = false;
        }

        #region 字体信息

        [SerializeField]
        private FontData m_FontData = FontData.defaultFontData;

        /// <summary>
        /// Whether this Text will support rich text.
        /// 是否支持富文本
        /// </summary>
        public bool supportRichText
        {
            get
            {
                return m_FontData.richText;
            }
            set
            {
                if (m_FontData.richText == value)
                    return;
                m_FontData.richText = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Wrap mode used by the text.
        /// 换行模式。
        /// </summary>
        public bool resizeTextForBestFit
        {
            get
            {
                return m_FontData.bestFit;
            }
            set
            {
                if (m_FontData.bestFit == value)
                    return;
                m_FontData.bestFit = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 最小尺寸
        /// </summary>
        public int resizeTextMinSize
        {
            get
            {
                return m_FontData.minSize;
            }
            set
            {
                if (m_FontData.minSize == value)
                    return;
                m_FontData.minSize = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 最大尺寸
        /// </summary>
        public int resizeTextMaxSize
        {
            get
            {
                return m_FontData.maxSize;
            }
            set
            {
                if (m_FontData.maxSize == value)
                    return;
                m_FontData.maxSize = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Alignment anchor used by the text.
        /// 对齐方式
        /// </summary>
        public TextAnchor alignment
        {
            get
            {
                return m_FontData.alignment;
            }
            set
            {
                if (m_FontData.alignment == value)
                    return;
                m_FontData.alignment = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int fontSize
        {
            get
            {
                return m_FontData.fontSize;
            }
            set
            {
                if (m_FontData.fontSize == value)
                    return;
                m_FontData.fontSize = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 水平换行方式
        /// </summary>
        public HorizontalWrapMode horizontalOverflow
        {
            get
            {
                return m_FontData.horizontalOverflow;
            }
            set
            {
                if (m_FontData.horizontalOverflow == value)
                    return;
                m_FontData.horizontalOverflow = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 竖直换行方式
        /// </summary>
        public VerticalWrapMode verticalOverflow
        {
            get
            {
                return m_FontData.verticalOverflow;
            }
            set
            {
                if (m_FontData.verticalOverflow == value)
                    return;
                m_FontData.verticalOverflow = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// 行间距
        /// </summary>
        public float lineSpacing
        {
            get
            {
                return m_FontData.lineSpacing;
            }
            set
            {
                if (m_FontData.lineSpacing == value)
                    return;
                m_FontData.lineSpacing = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        /// <summary>
        /// Font style used by the Text's text.
        /// 字体样式，加粗，斜体
        /// </summary>
        public FontStyle fontStyle
        {
            get
            {
                return m_FontData.fontStyle;
            }
            set
            {
                if (m_FontData.fontStyle == value)
                    return;
                m_FontData.fontStyle = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        #endregion

        #region 文本信息

        [TextArea(3, 10)]
        [SerializeField]
        protected string m_Text = String.Empty;

        /// <summary>
        /// Text that's being displayed by the Text.
        /// 文本内容
        /// </summary>
        public virtual string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(m_Text))
                        return;
                    m_Text = "";
                    SetVerticesDirty();
                }
                else if (m_Text != value)
                {
                    m_Text = value;
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }
        #endregion

        #region 生命周期函数

        protected override void OnEnable()
        {
            base.OnEnable();
            cachedTextGenerator.Invalidate();
            FontUpdateTracker.TrackText(this);
        }

        protected override void OnDisable()
        {
            FontUpdateTracker.UntrackText(this);
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            //获取内置资源 Arial.ttf 默认字体
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

#endif
        #endregion

        #region ILayoutElement 接口

        public virtual void CalculateLayoutInputHorizontal() { }
        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth
        {
            get { return 0; }
        }

        public virtual float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                //给定字符串和设置，返回容器的首选高度，会保留这个文字。
                return cachedTextGeneratorForLayout.GetPreferredWidth(m_Text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth { get { return -1; } }

        public virtual float minHeight
        {
            get { return 0; }
        }

        public virtual float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0.0f));
                return cachedTextGeneratorForLayout.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight { get { return -1; } }

        public virtual int layoutPriority { get { return 0; } }

        #endregion

        #region TextGenerator 生成渲染文本顶点，最优宽高

        //文本生成器
        private TextGenerator m_TextCache;
        /// <summary>
        /// Get or set the material used by this Text.
        /// </summary>
        public TextGenerator cachedTextGenerator
        {
            get
            {
                return m_TextCache ?? 
                    (m_TextCache = (m_Text.Length != 0 ? new TextGenerator(m_Text.Length) 
                    : new TextGenerator()));
            }
        }

        private TextGenerator m_TextCacheForLayout;
        public TextGenerator cachedTextGeneratorForLayout
        {
            get
            {
                return m_TextCacheForLayout ?? 
                    (m_TextCacheForLayout = new TextGenerator());
            }
        }

        /// <summary>
        /// 文本生成设置
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        public TextGenerationSettings GetGenerationSettings(Vector2 extents)
        {
            var settings = new TextGenerationSettings();

            settings.generationExtents = extents;
            if (font != null && font.dynamic)
            {
                settings.fontSize = m_FontData.fontSize;
                settings.resizeTextMinSize = m_FontData.minSize;
                settings.resizeTextMaxSize = m_FontData.maxSize;
            }

            // Other settings
            settings.textAnchor = m_FontData.alignment;
            settings.scaleFactor = pixelsPerUnit;
            settings.color = color;
            settings.font = font;
            settings.pivot = rectTransform.pivot;
            settings.richText = m_FontData.richText;
            settings.lineSpacing = m_FontData.lineSpacing;
            settings.fontStyle = m_FontData.fontStyle;
            settings.resizeTextForBestFit = m_FontData.bestFit;
            settings.updateBounds = false;
            settings.horizontalOverflow = m_FontData.horizontalOverflow;
            settings.verticalOverflow = m_FontData.verticalOverflow;

            return settings;
        }

        #endregion

        #region Font 

        //字体资源
        public Font font
        {
            get
            {
                return m_FontData.font;
            }
            set
            {
                if (m_FontData.font == value)
                    return;
                FontUpdateTracker.UntrackText(this);
                m_FontData.font = value;
                FontUpdateTracker.TrackText(this);
                SetAllDirty();
            }
        }

        #endregion

        #region 几何

        // We use this flag instead of Unregistering/Registering the callback to avoid allocation.
        [NonSerialized]
        protected bool m_DisableFontTextureRebuiltCallback = false;

        #region 顶点 网格
        /// <summary>
        /// 单位像素
        /// </summary>
        public float pixelsPerUnit
        {
            get
            {
                var localCanvas = canvas;
                if (!localCanvas)
                    return 1;
                // For dynamic fonts, ensure we use one pixel per pixel on the screen.
                if (!font || font.dynamic)
                    return localCanvas.scaleFactor;
                // For non-dynamic fonts, calculate pixels per unit based on specified font size relative to font object's own font size.
                if (m_FontData.fontSize <= 0 || font.fontSize <= 0)
                    return 1;
                return font.fontSize / (float)m_FontData.fontSize;
            }
        }


        protected override void UpdateGeometry()
        {
            if (font != null)
            {
                base.UpdateGeometry();
            }
        }



        /// <summary>
        /// 根据锚点，获取对应的 vector2 坐标
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        static public Vector2 GetTextAnchorPivot(TextAnchor anchor)
        {
            switch (anchor)
            {
                case TextAnchor.LowerLeft: return new Vector2(0, 0);
                case TextAnchor.LowerCenter: return new Vector2(0.5f, 0);
                case TextAnchor.LowerRight: return new Vector2(1, 0);
                case TextAnchor.MiddleLeft: return new Vector2(0, 0.5f);
                case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
                case TextAnchor.MiddleRight: return new Vector2(1, 0.5f);
                case TextAnchor.UpperLeft: return new Vector2(0, 1);
                case TextAnchor.UpperCenter: return new Vector2(0.5f, 1);
                case TextAnchor.UpperRight: return new Vector2(1, 1);
                default: return Vector2.zero;
            }
        }

        readonly UIVertex[] m_TempVerts = new UIVertex[4];

        /// <summary>
        /// 填充网格
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            //在我们进行更新时，不关心字体纹理是否发生变化。
            // cachedGextGenerator 的最终结果对此实例有效。
            m_DisableFontTextureRebuiltCallback = true;

            //文本框范围
            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);

            //文本生成器填充
            //将使用给定的字符串生成给定字符串的顶点和其他数据设置。
            cachedTextGenerator.Populate(text, settings);

            Rect inputRect = rectTransform.rect;

            //获取本地空间 中文本的文本对齐锚点
            Vector2 textAnchorPivot = GetTextAnchorPivot(m_FontData.alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = (textAnchorPivot.x == 1 ? inputRect.xMax : inputRect.xMin);
            refPoint.y = (textAnchorPivot.y == 0 ? inputRect.yMin : inputRect.yMax);

            // 确定偏移文本网格的像素分数。
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

            //将偏移应用于顶点
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line...
            int vertCount = verts.Count - 4;

            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            m_DisableFontTextureRebuiltCallback = false;
        }
        #endregion

        #region 材质

        static protected Material s_DefaultText = null;
        /// <summary>
        /// Text's texture comes from the font.
        /// 文本纹理
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (font != null && font.material != null && font.material.mainTexture != null)
                    return font.material.mainTexture;

                if (m_Material != null)
                    return m_Material.mainTexture;

                return base.mainTexture;
            }
        }

        /// <summary>
        /// 文本纹理更新回调
        /// </summary>
        public void FontTextureChanged()
        {
            // Only invoke if we are not destroyed.
            if (!this)
            {
                FontUpdateTracker.UntrackText(this);
                return;
            }

            if (m_DisableFontTextureRebuiltCallback)
            {
                return;
            }
            //将文本生成器标记为无效。 这将迫使全文生成在下次调用Populate时
            cachedTextGenerator.Invalidate();

            if (!IsActive())
                return;

            // this is a bit hacky, but it is currently the
            // cleanest solution....
            // if we detect the font texture has changed and are in a rebuild loop
            // we just regenerate the verts for the new UV's
            //这有点hacky，但它目前是
            //最干净的解决方案....
            //如果我们检测到字体纹理已更改并且处于重建循环中
            //我们只是重新生成新UV的顶点
            if (BrandoCanvasUpdateRegistry.IsRebuildingGraphics() || BrandoCanvasUpdateRegistry.IsRebuildingLayout())
            {
                UpdateGeometry();
            }
            else
            {
                SetAllDirty();
            } 
        }

        #endregion

#if UNITY_EDITOR
        public override void OnRebuildRequested()
        {
            // After a Font asset gets re-imported the managed side gets deleted and recreated,
            // that means the delegates are not persisted.
            // so we need to properly enforce a consistent state here.
            FontUpdateTracker.UntrackText(this);
            FontUpdateTracker.TrackText(this);

            // Also the textgenerator is no longer valid.
            cachedTextGenerator.Invalidate();

            base.OnRebuildRequested();
        }

#endif 

        #endregion

    }
}
