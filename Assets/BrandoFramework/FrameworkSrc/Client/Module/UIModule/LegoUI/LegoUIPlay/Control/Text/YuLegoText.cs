#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Client.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client.LegoUI
{
    /// <summary>
    /// Labels are graphics that display text.
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Text", 1)]
    public class YuLegoText :
        YuAbsLegoMaskableGraphic,
        ILayoutElement,
        IPointerClickHandler,
        ILegoText
    {
        #region UGUISrc

        [SerializeField] private LegoFontData m_FontData = LegoFontData.defaultFontData;

#if DEBUG
        // needed to track font changes from the inspector
        private Font m_LastTrackedFont;
#endif

        [TextArea(3, 10)] [SerializeField] protected string m_Text = String.Empty;
        private TextGenerator m_TextCache;
        private TextGenerator m_TextCacheForLayout;

        static protected Material s_DefaultText = null;

        // We use this flag instead of Unregistering/Registering the callback to avoid allocation.
        [NonSerialized] protected bool m_DisableFontTextureRebuiltCallback = false;

        protected YuLegoText()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Get or set the material used by this Text.
        /// </summary>

        public TextGenerator cachedTextGenerator
        {
            get
            {
                return m_TextCache ?? (m_TextCache =
                           (m_Text.Length != 0 ? new TextGenerator(m_Text.Length) : new TextGenerator()));
            }
        }

        public TextGenerator cachedTextGeneratorForLayout
        {
            get { return m_TextCacheForLayout ?? (m_TextCacheForLayout = new TextGenerator()); }
        }

        /// <summary>
        /// Text's texture comes from the font.
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

        public void FontTextureChanged()
        {
            // Only invoke if we are not destroyed.
            if (!this)
                return;

            if (m_DisableFontTextureRebuiltCallback)
                return;

            cachedTextGenerator.Invalidate();

            if (!IsActive())
                return;

            // this is a bit hacky, but it is currently the
            // cleanest solution....
            // if we detect the font texture has changed and are in a rebuild loop
            // we just regenerate the verts for the new UV's
            if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
                UpdateGeometry();
            else
                SetAllDirty();
        }

        public Font font
        {
            get { return m_FontData.font; }
            set
            {
                if (m_FontData.font == value)
                    return;

                YuLegoFontUpdateTracker.UntrackText(this);

                m_FontData.font = value;

                YuLegoFontUpdateTracker.TrackText(this);

#if DEBUG
                // needed to track font changes from the inspector
                m_LastTrackedFont = value;
#endif

                SetAllDirty();
            }
        }

        /// <summary>
        /// Text that's being displayed by the Text.
        /// </summary>

        public virtual string text
        {
            get { return m_Text; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    if (String.IsNullOrEmpty(m_Text))
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

        /// <summary>
        /// Whether this Text will support rich text.
        /// </summary>

        public bool supportRichText
        {
            get { return m_FontData.richText; }
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
        /// </summary>

        public bool resizeTextForBestFit
        {
            get { return m_FontData.bestFit; }
            set
            {
                if (m_FontData.bestFit == value)
                    return;
                m_FontData.bestFit = value;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public int resizeTextMinSize
        {
            get { return m_FontData.minSize; }
            set
            {
                if (m_FontData.minSize == value)
                    return;
                m_FontData.minSize = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public int resizeTextMaxSize
        {
            get { return m_FontData.maxSize; }
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
        /// </summary>

        public TextAnchor alignment
        {
            get { return m_FontData.alignment; }
            set
            {
                if (m_FontData.alignment == value)
                    return;
                m_FontData.alignment = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public bool alignByGeometry
        {
            get { return m_FontData.alignByGeometry; }
            set
            {
                if (m_FontData.alignByGeometry == value)
                    return;
                m_FontData.alignByGeometry = value;

                SetVerticesDirty();
            }
        }

        public int fontSize
        {
            get { return m_FontData.fontSize; }
            set
            {
                if (m_FontData.fontSize == value)
                    return;
                m_FontData.fontSize = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public HorizontalWrapMode horizontalOverflow
        {
            get { return m_FontData.horizontalOverflow; }
            set
            {
                if (m_FontData.horizontalOverflow == value)
                    return;
                m_FontData.horizontalOverflow = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public VerticalWrapMode verticalOverflow
        {
            get { return m_FontData.verticalOverflow; }
            set
            {
                if (m_FontData.verticalOverflow == value)
                    return;
                m_FontData.verticalOverflow = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        public float lineSpacing
        {
            get { return m_FontData.lineSpacing; }
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
        /// </summary>
        public FontStyle fontStyle
        {
            get { return m_FontData.fontStyle; }
            set
            {
                if (m_FontData.fontStyle == value)
                    return;
                m_FontData.fontStyle = value;

                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

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
                return font.fontSize / (float) m_FontData.fontSize;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            cachedTextGenerator.Invalidate();
            YuLegoFontUpdateTracker.TrackText(this);
        }

        public void SethHorizontalOverflow(HorizontalWrapMode mode) => m_FontData.horizontalOverflow = mode;

        protected override void OnDisable()
        {
            YuLegoFontUpdateTracker.UntrackText(this);
            base.OnDisable();
        }

        protected override void UpdateGeometry()
        {
            if (font != null)
            {
                base.UpdateGeometry();
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            AssignDefaultFont();
        }

#endif
        internal void AssignDefaultFont()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

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
            settings.alignByGeometry = m_FontData.alignByGeometry;
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
        /// 是否包含超链接文本内容。
        /// </summary>
        private bool _hasHrefText;

        private void NotHasHrefTextAtOnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count - 4;

            // todo Text Bug

            try
            {
                Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
                roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
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
            catch (Exception e)
            {
                // ignored
            }
        }


        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (!_hasHrefText)
            {
                NotHasHrefTextAtOnPopulateMesh(toFill);
            }
            else
            {
                HasHrefTextAtOnPopulateMesh(toFill);
            }
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        public virtual float minWidth
        {
            get { return 0; }
        }

        public virtual float preferredWidth
        {
            get
            {
                var settings = GetGenerationSettings(Vector2.zero);
                return cachedTextGeneratorForLayout.GetPreferredWidth(m_Text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth
        {
            get { return -1; }
        }

        public virtual float minHeight
        {
            get { return 0; }
        }

        public virtual float preferredHeight
        {
            get
            {
                var settings = GetGenerationSettings(new Vector2(GetPixelAdjustedRect().size.x, 0.0f));
                return cachedTextGeneratorForLayout.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight
        {
            get { return -1; }
        }

        public virtual int layoutPriority
        {
            get { return 0; }
        }

#if UNITY_EDITOR
        public override void OnRebuildRequested()
        {
            // After a Font asset gets re-imported the managed side gets deleted and recreated,
            // that means the delegates are not persisted.
            // so we need to properly enforce a consistent state here.
            YuLegoFontUpdateTracker.UntrackText(this);
            YuLegoFontUpdateTracker.TrackText(this);

            // Also the textgenerator is no longer valid.
            cachedTextGenerator.Invalidate();

            base.OnRebuildRequested();
        }

        // The Text inspector editor can change the font, and we need a way to track changes so that we get the appropriate rebuild callbacks
        // We can intercept changes in OnValidate, and keep track of the previous font reference
        protected override void OnValidate()
        {
            if (!IsActive())
            {
                base.OnValidate();
                return;
            }

            if (m_FontData.font != m_LastTrackedFont)
            {
                Font newFont = m_FontData.font;
                m_FontData.font = m_LastTrackedFont;
                YuLegoFontUpdateTracker.UntrackText(this);
                m_FontData.font = newFont;
                YuLegoFontUpdateTracker.TrackText(this);

                m_LastTrackedFont = newFont;
            }

            base.OnValidate();
        }

#endif // if UNITY_EDITOR

        #endregion

        #region Lego

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public virtual string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                {
                    return;
                }

                text = value;
            }
        }


        public void AdaptPreferredSize()
        {
            var r = new Vector2(RectTransform.sizeDelta.x, preferredHeight);
            RectTransform.sizeDelta = r;
        }


        [SerializeField] [Tooltip("文本控件在所属应用中的全局样式Id")]
        protected int m_StyleId;

        public int StyleId
        {
            get { return m_StyleId; }
        }

        #endregion

        #region 元数据变形

        private Vector2 originalSizeDelta;

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            var textMeta = uiMeta.NextText;
            RectMeta = uiMeta.CurrentRect;
            Metamorphose(RectMeta, textMeta);
        }

        #region 字体

        private static readonly Dictionary<string, Font> fontCaches
            = new Dictionary<string, Font>();

        private const string Arial = "arial";

        private static Font GetFont(string fontId)
        {
            if (string.IsNullOrEmpty(fontId))
            {
                return null;
            }

            if (fontId == Arial)
            {
                var arial = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                return arial;
            }

            if (!fontCaches.ContainsKey(fontId))
            {
#if UNITY_EDITOR
                if (UnityModeUtility.IsEditorMode)
                {
                    //AssetModule.Load<Font>(fontId);


                   //var currentApp = YuU3dAppSettingDati.CurrentActual;
                   // //图片字
                   // var path = currentApp.Helper.AssetDatabaseFontDir
                   //            + fontId + ".fontsettings";
                   // var targetFont = AssetDatabaseUtility.LoadAssetAtPath<Font>(path);
                   // if (targetFont == null)
                   // {
                   //     //文字字体
                   //     path = currentApp.Helper.AssetDatabaseFontDir
                   //            + fontId + ".ttf";
                   //     targetFont = AssetDatabaseUtility.LoadAssetAtPath<Font>(path);
                   //     if (targetFont == null)
                   //     {
                   //         Debug.LogError($"目标字体{fontId}不存在！");
                   //         return null;
                   //     }
                   // }
                   // //var targetFont = AssetModule.Load<Font>(fontId);
                   // fontCaches.Add(fontId, targetFont);
                   // return targetFont;
                }
#endif
                return LoadFontAtPlay(fontId);
            }

            var font = fontCaches[fontId];
            return font;
        }

        private static Font LoadFontAtPlay(string fontId)
        {
            var font = AssetModule.Load<Font>(fontId);
            if (font == null)
            {
                Debug.LogError($"目标字体{fontId}不存在！");
                return null;
            }

            fontCaches.Add(fontId, font);
            return font;
        }

        public string FontId
        {
            get { return font.name; }
            set
            {
                var targetFont = GetFont(value.ToLower());
                if (targetFont == null)
                {
                    Debug.LogError($"目标字体{value}不存在！");
                }
                else
                {
                    font = targetFont;
                }
            }
        }

        #endregion

        public void Metamorphose(LegoRectTransformMeta rectMeta, LegoTextMeta textMeta)
        {
            originalSizeDelta = RectTransform.sizeDelta;
            YuLegoUtility.MetamorphoseRect(RectTransform, rectMeta);
            Text = textMeta.Content;
            raycastTarget = textMeta.RaycastTarget;

            font = GetFont(textMeta.FontId.ToLower());
            //color = textMeta.Color.ToColor();
            fontStyle = (FontStyle) (int) textMeta.FontStyle;
            fontSize = textMeta.FontSize;
            lineSpacing = textMeta.LineSpacing;
            supportRichText = textMeta.RichText;
            resizeTextForBestFit = textMeta.BestFit;
            alignByGeometry = textMeta.AlignByGeometry;

            alignment = (TextAnchor) (int) textMeta.Alignment;
            verticalOverflow = (VerticalWrapMode) (int) textMeta.VerticalOverflow;
            horizontalOverflow = (HorizontalWrapMode) (int) textMeta.HorizontalOverflow;
            UpdateAtStyleId();
        }

        private void UpdateAtStyleId()
        {
        }

        private void ApplyLegoTextStyle(int styleIndex)
        {
            YuLegoTextStyleInfo textStyle =
                YuAppTextStyleInfo.GetTextStyleInfoFromCurrentAppTextStyle(styleIndex);
            if (textStyle != null)
            {
                font = textStyle.font;
                fontStyle = textStyle.fontStyle;
                fontSize = textStyle.textSize;
                color = textStyle.color;
                alignment = textStyle.TextAnchor;
                if (textStyle.lineStyle == LineStyle.singleLine)
                {
                    RectTransform.sizeDelta = new Vector2(preferredWidth, originalSizeDelta.y);
                    RectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);
                }
            }
        }

        #endregion

        #region 数据响应

        public void ReceiveTextChange(string newValue)
        {
            Text = newValue;
        }

        public void ReceiveColorChange(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                return;
            }

            Color newColor;
            ColorUtility.TryParseHtmlString(newValue, out newColor);
            Color = newColor;
        }

        #endregion

        #region 超链接扩展

        private struct YuEmojiInfo
        {
            public float X;
            public float Y;
            public float Size;
            public int Len;
        }

        /// <summary>
        /// 解析完最终文本。
        /// </summary>
        private string _outputText;

        private static Dictionary<string, YuEmojiInfo> _emojiInfos;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();

#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
            {
                return;
            }
#endif

            if (!_hasHrefText)
            {
                return;
            }

            _outputText = GetOuputText(text);
        }

        private void HasHrefTextAtOnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
            {
                return;
            }

            TryInitEmojiContext();
        }

        private void TryInitEmojiContext()
        {
            if (_emojiInfos == null)
            {
                // 初始化表情数据
                _emojiInfos = new Dictionary<string, YuEmojiInfo>();
            }
        }

        /// <summary>
        /// 超链接信息列表。
        /// </summary>
        private readonly List<YuHrefInfo> _hrefInfos = new List<YuHrefInfo>();

        /// <summary>
        /// 超链接文本字符串构造器。
        /// </summary>
        private static readonly StringBuilder _hrefTextBuilder = new StringBuilder();

        private Action<string> _onHrefClick;

        /// <summary>
        /// 超链接文本解析正则表达式。
        /// </summary>
        private static readonly Regex _hrefRegex =
            new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);


        protected virtual string GetOuputText(string outputText)
        {
            _hrefTextBuilder.Length = 0;
            _hrefInfos.Clear();
            var textIndex = 0;

            foreach (Match match in _hrefRegex.Matches(_outputText))
            {
                _hrefTextBuilder.Append(outputText.Substring(textIndex, match.Index - textIndex));
                _hrefTextBuilder.Append("<color='#9ed7ff'>"); // 超链接颜色ff6600

                var group = match.Groups[1];
                var hrefInfo = new YuHrefInfo(_hrefTextBuilder.Length * 4,
                    (_hrefTextBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                    group.Value);
                _hrefInfos.Add(hrefInfo);

                _hrefTextBuilder.Append(match.Groups[2].Value);
                _hrefTextBuilder.Append("</color>");
                textIndex = match.Index + match.Length;
            }

            _hrefTextBuilder.Append(outputText.Substring(textIndex, outputText.Length - textIndex));
            return _hrefTextBuilder.ToString();
        }

        public void OnHrefClick(PointerEventData eventData)
        {
            Vector2 lp;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out lp);

            foreach (var hrefInfo in _hrefInfos)
            {
                var boxes = hrefInfo.Boxes;
                for (int i = 0; i < boxes.Count; ++i)
                {
                    if (!boxes[i].Contains(lp))
                    {
                        continue;
                    }

                    _onHrefClick?.Invoke(hrefInfo.Name);

#if UNITY_EDITOR || DEBUG
                    Debug.Log($"点击了：{hrefInfo.Name}!");
#endif
                    return;
                }
            }
        }

        /// <summary>
        /// 超链接信息。
        /// </summary>
        private class YuHrefInfo
        {
            public int StartIndex { get; private set; }
            public int EndIndex { get; private set; }
            public string Name { get; private set; }
            public List<Rect> Boxes { get; private set; } = new List<Rect>();

            public YuHrefInfo(int startIndex, int endIndex, string name)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
                Name = name;
            }
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            //throw new NotImplementedException();
        }
    }
}