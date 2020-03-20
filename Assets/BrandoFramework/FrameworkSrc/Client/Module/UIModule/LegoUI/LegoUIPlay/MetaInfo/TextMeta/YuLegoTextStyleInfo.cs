#region Head

// Author:            Yu
// CreateDate:        2018/10/11 15:23:48
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高文本样式信息。
    /// </summary>
    [Serializable]
    [FoldoutGroup("")]
    public class YuLegoTextStyleInfo
    {

        [HorizontalGroup("字体样式",LabelWidth = 70)]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("样式Id")]
        public int Id;

        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("样式名称")]
        public string styleName = "中号正文深色文字";

        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("文本形式")]
        public LineStyle lineStyle;

        #region 字符样式
        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("字体")]
        [OnValueChanged("SaveFontName")]
        public Font font;

        private void SaveFontName()
        {
            fontName = font.name;
        }
        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("字体文件名称")]
        [ReadOnly]
        public string fontName;
        
        
        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("字号大小")]
        public int textSize = 18;

        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("文本外观")]
        public FontStyle fontStyle = FontStyle.Normal;

        [VerticalGroup("字体样式/左列")]
        [BoxGroup("字体样式/左列/基本")]
        [LabelText("文本颜色")]
        public Color color = Color.black;

        #region 段落样式

        [VerticalGroup("字体样式/中列")]
        [BoxGroup("字体样式/中列/段落样式")]
        [LabelText("文本锚点")]
        public TextAnchor TextAnchor;

        #endregion

        #endregion

        #region 效果样式

        [VerticalGroup("字体样式/中列")]
        [HorizontalGroup("字体样式/中列/效果样式/是否开启效果")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("是否描边")]
        public bool isOutLine = false;

        [VerticalGroup("字体样式/中列")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("描边颜色")]
        public Color outlineEffectColor = Color.black;

        [VerticalGroup("字体样式/中列")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("描边间距")]
        public Vector2 outLineDistance = Vector2.zero;


        [VerticalGroup("字体样式/中列")]
        [HorizontalGroup("字体样式/中列/效果样式/是否开启效果")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("是否阴影")]
        public bool isShadow = false;

        [VerticalGroup("字体样式/中列")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("阴影颜色")]
        public Color shadowEffectColor = Color.black;

        [VerticalGroup("字体样式/中列")]
        [BoxGroup("字体样式/中列/效果样式")]
        [LabelText("阴影间距")]
        public Vector2 shadowDistance = Vector2.zero;

        #endregion

        #region 浏览文本
        [VerticalGroup("字体样式/右列")]
        [BoxGroup("字体样式/右列/预览")]
        [LabelText("预览文本")]
        [InlineButton("PreviewFontStyle","预览")]
        public string preViewTextString;

        [VerticalGroup("字体样式/右列")]
        [BoxGroup("字体样式/右列/预览")]
        [LabelText("文本缩略图")]
        [PreviewField(180, ObjectFieldAlignment.Left)]
        public Texture2D preViewTextTexture;

        private static GameObject previewObject;
        private static GameObject PreviewObject
        {
            get
            {
                if(previewObject != null )
                {
                    return previewObject;
                }
                else
                {
                    GameObject loadObject = Resources.Load("Setting/YuTextStyle/TextPreview")
                        as GameObject;
                    previewObject = GameObject.Instantiate(loadObject);
                    InitPreviewObject();
                    return previewObject;
                }
                 
            }
        }

        private static void InitPreviewObject()
        {
            Transform TextContent = previewObject.transform.Find
               ("Canvas/PreViewUI/Text_Content");
            YuLegoText preViewContent = TextContent.GetComponent<YuLegoText>();
            if (preViewContent == null)
            {
                preViewContent = TextContent.gameObject.AddComponent<YuLegoText>();
                preViewContent.text = "文本内容";
            }

            Transform TextTitle = previewObject.transform.Find
                ("Canvas/PreViewUI/Text_Title");
            YuLegoText preViewTitle = TextTitle.GetComponent<YuLegoText>();
            if (preViewTitle == null)
            {
                preViewTitle = TextTitle.gameObject.AddComponent<YuLegoText>();
                preViewTitle.text = "标题";
            }

            Transform TitleBgImage = previewObject.transform.Find
                ("Canvas/PreViewUI/Image_Title");
            YuLegoImage preViewTitleBg = TitleBgImage.GetComponent<YuLegoImage>();
            if (preViewTitleBg == null)
            {
                preViewTitleBg = TitleBgImage.gameObject.AddComponent<YuLegoImage>();
                preViewTitleBg.Color = new Color(0.9f, 0.9f, 0.7f);
            }

            Transform ContentBgImage = previewObject.transform.Find
                ("Canvas/PreViewUI/Image_Content");
            YuLegoImage preViewContentBg = ContentBgImage.GetComponent<YuLegoImage>();
            if (preViewContentBg == null)
            {
                preViewContentBg = ContentBgImage.gameObject.AddComponent<YuLegoImage>();
                preViewTitleBg.Color = new Color(0.4f, 0.4f, 0.4f);
            }
        }

        public void PreviewFontStyle()
        {

            Transform TextContent = PreviewObject.transform.Find
                ("Canvas/PreViewUI/Text_Content");
            YuLegoText preViewContent = TextContent.GetComponent<YuLegoText>();
            Transform TextTitle = PreviewObject.transform.Find
                ("Canvas/PreViewUI/Text_Title");
            YuLegoText preViewTitle = TextTitle.GetComponent<YuLegoText>();

            ApplyFontStyleToPreviewText(preViewContent);
            ApplyFontStyleToPreviewText(preViewTitle);
            preViewTextTexture = GetTextPreviewScreenshot();
            //GameObject.DestroyImmediate(previewObject);
        }

        private void ApplyFontStyleToPreviewText(YuLegoText previewText)
        {
            previewText.fontSize = textSize;
            previewText.font = font;
            previewText.Color = color;
            previewText.fontStyle = fontStyle;
            previewText.fontStyle = fontStyle;
            previewText.alignment = TextAnchor;
            SetFontEffect(previewText, isOutLine,isShadow);
            if (preViewTextString != "")
            {
                previewText.text = preViewTextString;
            }
        }

        private void SetFontEffect(YuLegoText previewText,bool isOutLine,bool isShadow)
        {
            if(isOutLine)
            {
                Outline outlineComponent = previewText.gameObject.GetComponent<Outline>();
                if (outlineComponent == null)
                {
                    outlineComponent = previewText.gameObject.AddComponent<Outline>();
                }
                outlineComponent.effectColor = outlineEffectColor;
                outlineComponent.effectDistance = outLineDistance;
            }
            if (isShadow)
            {
                Shadow shdowComponent = previewText.gameObject.GetComponent<Shadow>();
                if (shdowComponent == null)
                {
                    shdowComponent = previewText.gameObject.AddComponent<Shadow>();
                }
                shdowComponent.effectColor = shadowEffectColor;
                shdowComponent.effectDistance = shadowDistance;
            }
        }

        /// <summary>
        /// 对预览场景进行截图，以显示在字体样式窗口上
        /// </summary>
        /// <returns></returns>
        private Texture2D GetTextPreviewScreenshot()
        {
            Camera textUICamera = PreviewObject.transform.Find
                ("TextUICamera").GetComponent<Camera>();
            Rect screenShotRect = new Rect(0, 0, Screen.width, Screen.height);
            RenderTexture rt = new RenderTexture(
                (int)screenShotRect.width, (int)screenShotRect.height, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
            textUICamera.targetTexture = rt;
            textUICamera.Render();
            // 激活rt, 并从中中读取像素。  
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(
                (int)screenShotRect.width, (int)screenShotRect.height, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(screenShotRect, 0, 0);
            screenShot.Apply();
            // 重置相关参数，以使用camera继续在屏幕上显示  
            textUICamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.DestroyImmediate(rt);
            return screenShot;
        }

        #endregion
    }

    public enum LineStyle
    {
        singleLine,    //单行不定宽文本
        multiLine      //多行文本
    }
}