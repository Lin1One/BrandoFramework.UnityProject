#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if DEBUG

#endif

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件运行时创建器。
    /// </summary>
    public static class LegoDefaultControls
    {
        #region Create

        private static readonly Vector2 textButtonSize = new Vector2(160, 30);
        private static readonly Sprite UI_Fill_Sky = GetYuSprite("UI_Fill_Sky");
        private static readonly Sprite UI_Panel_Window = GetYuSprite("UI_Panel_Window");
        private static readonly Sprite UI_Checkmark = GetYuSprite("UI_Checkmark");
        private static readonly Sprite UI_Rocker = GetYuSprite("UI_Rocker");

        public static Transform CurrentParent;

        private static void SetParentAanBasePotition(RectTransform rect)
        {
            rect.SetParent(CurrentParent);
            rect.localPosition = Vector3.zero;
            rect.localScale = Vector3.one;
        }

        private static void SetParentAanBasePotition(GameObject go)
            => SetParentAanBasePotition(go.GetComponent<RectTransform>());


        private static GameObject CreateRectTransform()
        {
            var go = CreateUIElementRoot("RectTransform", new Vector2(100, 100));
            return go;
        }

        public static GameObject CreateText()
        {
            var go = CreateUIElementRoot("Text", s_ThickElementSize);
            var lbl = go.AddComponent<YuLegoText>();
            lbl.text = "New Text";
            SetDefaultTextValues(lbl);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateImage()
        {
            var go = CreateUIElementRoot("Image", s_ImageElementSize);
            var image = go.AddComponent<YuLegoImage>();
            image.type = YuLegoImage.Type.Simple;
            image.raycastTarget = false;


#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateRawImage()
        {
            var go = CreateUIElementRoot("RawImage", s_ImageElementSize);
            var rawImage = go.AddComponent<YuLegoRawImage>();
            rawImage.raycastTarget = false;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateButton()
        {
            var go = CreateUIElementRoot("Button", s_ThickElementSize);

            var childText = new GameObject("Text");
            childText.AddComponent<RectTransform>();
            SetParentAndAlign(childText, go);

            var image = go.AddComponent<YuLegoImage>();
            image.sprite = UI_Fill_Sky;
            image.type = YuLegoImage.Type.Simple;
            image.color = s_DefaultSelectableColor;

            var bt = go.AddComponent<YuLegoButton>();
            SetDefaultColorTransitionValues(bt);

            var text = childText.AddComponent<YuLegoText>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(text);

            var textRectTransform = childText.GetComponent<RectTransform>();
            //textRectTransform.anchorMin = Vector2.zero;
            //textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = textButtonSize;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateTButton()
        {
            var go = CreateUIElementRoot("TButton", s_ThickElementSize);

            var iconGo = CreateUIElementRoot("Icon", s_ThickElementSize);
            var icon = iconGo.AddComponent<YuLegoImage>();
            SetParentAndAlign(iconGo, go);

            var childText = new GameObject("Text");
            childText.AddComponent<RectTransform>();
            SetParentAndAlign(childText, go);

            var image = go.AddComponent<YuLegoImage>();
            image.sprite = UI_Fill_Sky;
            image.type = YuLegoImage.Type.Simple;
            image.color = s_DefaultSelectableColor;

            var bt = go.AddComponent<YuLegoTButton>();
            bt.targetGraphic = image;
            SetDefaultColorTransitionValues(bt);

            icon.sprite = UI_Fill_Sky;
            icon.type = YuLegoImage.Type.Simple;
            icon.color = s_DefaultSelectableColor;
            icon.raycastTarget = false;

            var text = childText.AddComponent<YuLegoText>();
            text.text = "TButton";
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            SetDefaultTextValues(text);

            var textRectTransform = childText.GetComponent<RectTransform>();
            //textRectTransform.anchorMin = Vector2.zero;
            //textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = textButtonSize;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateToggle()
        {
            // Set up hierarchy
            var go = CreateUIElementRoot("Toggle", s_ThinElementSize);

            var background = CreateUIObject("Background", go);
            var checkmark = CreateUIObject("Checkmark", background);
            var childLabel = CreateUIObject("Text", go);

            // Set up components
            var toggle = go.AddComponent<YuLegoToggle>();
            toggle.isOn = true;

            var bgImage = background.AddComponent<YuLegoImage>();
            bgImage.sprite = UI_Fill_Sky;
            bgImage.type = YuLegoImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            var checkmarkImage = checkmark.AddComponent<YuLegoImage>();
            checkmarkImage.sprite = UI_Checkmark;

            var label = childLabel.AddComponent<YuLegoText>();
            label.text = "Toggle";
            SetDefaultTextValues(label);

            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            SetDefaultColorTransitionValues(toggle);

            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 1f);
            bgRect.anchorMax = new Vector2(0f, 1f);
            bgRect.anchoredPosition = new Vector2(10f, -10f);
            bgRect.sizeDelta = new Vector2(kThinHeight, kThinHeight);

            var checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            var labelRect = childLabel.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(23f, 1f);
            labelRect.offsetMax = new Vector2(-5f, -2f);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreatePanleToggle()
        {
            // Set up hierarchy
            var go = CreateUIElementRoot("PlaneToggle", s_ThinElementSize);
            var background = CreateUIObject("Background", go);

            // Set up components

            var toggleImage = go.AddComponent<YuLegoImage>();
            toggleImage.sprite = UI_Fill_Sky;
            toggleImage.type = YuLegoImage.Type.Sliced;
            toggleImage.color = s_DefaultSelectableColor;

            var bgImage = background.AddComponent<YuLegoImage>();
            bgImage.sprite = UI_Panel_Window;
            bgImage.type = YuLegoImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            go.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 20);
            var toggle = go.AddComponent<YuLegoPlaneToggle>();
            toggle.IsOn = true;
            toggle.IsOn = false;

            var bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(20, 20);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateSlider()
        {
            // Create GOs Hierarchy
            var go = CreateUIElementRoot("Slider", s_ThinElementSize);

            var background = CreateUIObject("Background", go);
            var fillArea = CreateUIObject("Fill Area", go);
            var fill = CreateUIObject("Fill", fillArea);
            var handleArea = CreateUIObject("Handle Slide Area", go);
            var handle = CreateUIObject("Handle", handleArea);

            // Background
            var backgroundImage = background.AddComponent<YuLegoImage>();
            backgroundImage.sprite = UI_Fill_Sky;
            backgroundImage.type = YuLegoImage.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            var backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            var fillImage = fill.AddComponent<YuLegoImage>();
            fillImage.sprite = UI_Fill_Sky;
            fillImage.type = YuLegoImage.Type.Sliced;
            fillImage.color = Color.red;

            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            var handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            var handleImage = handle.AddComponent<YuLegoImage>();
            handleImage.sprite = UI_Panel_Window;
            handleImage.color = s_DefaultSelectableColor;

            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            var slider = go.AddComponent<YuLegoSlider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = YuLegoSlider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateProgressbar()
        {
            // Create GOs Hierarchy
            var go = CreateUIElementRoot("Progressbar", s_ThinElementSize);
            var background = CreateUIObject("Background", go);
            var fillArea = CreateUIObject("Fill Area", go);
            var fill = CreateUIObject("Fill", fillArea);


            // Background
            var backgroundImage = background.AddComponent<YuLegoImage>();
            backgroundImage.sprite = UI_Fill_Sky;
            backgroundImage.type = YuLegoImage.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.RaycastTarget = false;
            var backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = progressSize;

            // Fill Area
            var fillAreaRect = fillArea.GetComponent<RectTransform>();

            // Fill
            var fillImage = fill.AddComponent<YuLegoImage>();
            fillImage.sprite = UI_Fill_Sky;
            fillImage.type = YuLegoImage.Type.Sliced;
            fillImage.RaycastTarget = false;

            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(0, 0);

            var progressbar = go.AddComponent<YuLegoProgressbar>();
            progressbar.interactable = false;
            progressbar.value = 0f;
            progressbar.transition = Selectable.Transition.None;
            progressbar.fillRect = fill.GetComponent<RectTransform>();
            progressbar.direction = YuLegoSlider.Direction.LeftToRight;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateInputField()
        {
            var go = CreateUIElementRoot("InputField", s_ThickElementSize);

            var childPlaceholder = CreateUIObject("Placeholder", go);
            var childText = CreateUIObject("Text", go);

            var image = go.AddComponent<YuLegoImage>();
            image.sprite = UI_Fill_Sky;
            image.type = YuLegoImage.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            var inputField = go.AddComponent<YuLegoInputField>();
            SetDefaultColorTransitionValues(inputField);

            var text = childText.AddComponent<YuLegoText>();
            text.text = "";
            text.supportRichText = false;
            SetDefaultTextValues(text);

            var placeholder = childPlaceholder.AddComponent<YuLegoText>();
            placeholder.text = "Enter text...";
            placeholder.fontStyle = FontStyle.Italic;
            // Make placeholder color half as opaque as normal text color.
            var placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            text.alignment = TextAnchor.MiddleLeft;
            text.alignment = TextAnchor.MiddleLeft;

            inputField.textComponent = text;
            inputField.placeholder = placeholder;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        private static GameObject CreateScrollbar()
        {
            // Create GOs Hierarchy
            var go = CreateUIElementRoot("Scrollbar", s_ThinElementSize);

            var sliderArea = CreateUIObject("Sliding Area", go);
            var handle = CreateUIObject("Handle", sliderArea);

            var bgImage = go.AddComponent<YuLegoImage>();
            bgImage.sprite = UI_Fill_Sky;
            bgImage.type = YuLegoImage.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            var handleImage = handle.AddComponent<YuLegoImage>();
            handleImage.sprite = UI_Panel_Window;
            handleImage.type = YuLegoImage.Type.Sliced;
            handleImage.color = s_DefaultSelectableColor;

            var sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            var scrollbar = go.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            SetDefaultColorTransitionValues(scrollbar);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateDropdown()
        {
            var go = CreateUIElementRoot("Dropdown", s_ThickElementSize);

            var label = CreateUIObject("Label", go);
            var arrow = CreateUIObject("Arrow", go);
            var template = CreateUIObject("Template", go);
            var viewport = CreateUIObject("Viewport", template);
            var content = CreateUIObject("Content", viewport);
            var item = CreateUIObject("Item", content);
            var itemBackground = CreateUIObject("Item Background", item);
            var itemCheckmark = CreateUIObject("Item Checkmark", item);
            var itemLabel = CreateUIObject("Item Label", item);

            // Sub controls.

            var scrollbar = CreateScrollbar();
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            var scrollbarScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);

            var vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            var itemLabelText = itemLabel.AddComponent<YuLegoText>();
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAnchor.MiddleLeft;

            var itemBackgroundImage = itemBackground.AddComponent<YuLegoImage>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            var itemCheckmarkImage = itemCheckmark.AddComponent<YuLegoImage>();
            itemCheckmarkImage.sprite = UI_Checkmark;

            var itemToggle = item.AddComponent<YuLegoToggle>();
            itemToggle.targetGraphic = itemBackgroundImage;
            itemToggle.graphic = itemCheckmarkImage;
            itemToggle.isOn = true;

            // Setup template UI components.

            var templateImage = template.AddComponent<YuLegoImage>();
            templateImage.sprite = UI_Fill_Sky;
            templateImage.type = YuLegoImage.Type.Sliced;

            var templateScrollRect = template.AddComponent<ScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;

            var scrollRectMask = viewport.AddComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            var viewportImage = viewport.AddComponent<YuLegoImage>();
            viewportImage.sprite = UI_Fill_Sky;
            viewportImage.type = YuLegoImage.Type.Sliced;

            // Setup dropdown UI components.

            var labelText = label.AddComponent<YuLegoText>();
            SetDefaultTextValues(labelText);
            labelText.alignment = TextAnchor.MiddleLeft;

            var arrowImage = arrow.AddComponent<YuLegoImage>();
            arrowImage.sprite = UI_Fill_Sky;

            var backgroundImage = go.AddComponent<YuLegoImage>();
            backgroundImage.sprite = UI_Fill_Sky;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = YuLegoImage.Type.Sliced;

            var dropdown = go.AddComponent<YuLegoDropdown>();
            dropdown.targetGraphic = backgroundImage;
            SetDefaultColorTransitionValues(dropdown);
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new YuLegoDropdown.OptionData { text = "Option A" });
            dropdown.options.Add(new YuLegoDropdown.OptionData { text = "Option B" });
            dropdown.options.Add(new YuLegoDropdown.OptionData { text = "Option C" });
            dropdown.RefreshShownValue();

            // Set up RectTransforms.

            var labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(10, 6);
            labelRT.offsetMax = new Vector2(-25, -7);

            var arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(20, 20);
            arrowRT.anchoredPosition = new Vector2(-15, 0);

            var templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta = new Vector2(0, 150);

            var viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            var contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            var itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 20);

            var itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            var itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            var itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            template.SetActive(false);

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateScrollView()
        {
            var go = CreateUIElementRoot("ScrollView", s_ThickElementSize);
            var scrollRectGO = CreateUIObject("ScrollRect", go);
            var content = CreateUIObject("Content", scrollRectGO);

            var rootImage = go.AddComponent<YuLegoImage>();
            rootImage.type = YuLegoImage.Type.Sliced;
            var rootRect = go.GetComponent<RectTransform>();
            rootImage.sprite = UI_Fill_Sky;
            go.AddComponent<YuLegoScrollView>();
            rootRect.sizeDelta = new Vector2(320, 450);

            scrollRectGO.AddComponent<YuLegoImage>();
            var scrollRect = scrollRectGO.AddComponent<ScrollRect>();
            var scRect = scrollRectGO.GetComponent<RectTransform>();
            var mask = scrollRectGO.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scRect.sizeDelta = new Vector2(320, 430);

            var contentRect = content.GetComponent<RectTransform>();
            scrollRect.content = contentRect;

            contentRect.anchorMin = Vector2.up;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = new Vector2(0, 430);
            contentRect.pivot = Vector2.up;

#if DEBUG
            if (!Application.isPlaying)
            {
                ////go.AddComponent<YuLegoScrollViewHelper>();
            }

            SetParentAanBasePotition(go);
#endif

            return go;
        }

        public static GameObject CreateRocker()
        {
            var go = CreateUIElementRoot("Rocker", RockerSize);
            var imageBgGO = CreateUIObject("Image_Background", go);
            var imageRockerGO = CreateUIObject("Image_Rocker", go);

            var imageBg = imageBgGO.AddComponent<YuLegoImage>();
            imageBg.type = YuLegoImage.Type.Simple;
            imageBg.raycastTarget = false;
            imageBg.sprite = UI_Rocker;
            imageBg.rectTransform.sizeDelta = RockerSize;

            var imageRocker = imageRockerGO.AddComponent<YuLegoImage>();
            imageRocker.type = YuLegoImage.Type.Simple;
            imageRocker.raycastTarget = true;
            imageRocker.sprite = UI_Rocker;
            imageRocker.rectTransform.sizeDelta = new Vector2(50, 50);

            var rokcer = go.AddComponent<YuLegoRocker>();
            rokcer.content = imageRocker.rectTransform;

#if DEBUG
            SetParentAanBasePotition(go);
#endif

            return go;
        }

        private static Sprite GetYuSprite(string spId)
        {
            var sp = Resources.Load<Sprite>("YuSprite/" + spId);
            return sp;
        }

        #endregion

        #region 工具方法

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        private const float kWidth = 160f;
        private const float kThickHeight = 30f;
        private const float kThinHeight = 20f;
        private static readonly Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        private static readonly Vector2 RockerSize = new Vector2(170, 170);
        private static readonly Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        private static readonly Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        private static readonly Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static readonly Vector2 progressSize = new Vector2(160, 10);
        private static readonly Color s_TextColor = Color.white;

        private static void SetDefaultTextValues(YuLegoText lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;

            // Reset() is not called when playing. We still want the default font to be assigned
            lbl.AssignDefaultFont();
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        #endregion

        #region 类型创建方法映射

        private static readonly Dictionary<Type, Func<GameObject>> controlFuncDict
            = new Dictionary<Type, Func<GameObject>>
            {
                {typeof(RectTransform), CreateRectTransform},
                {typeof(YuLegoText), CreateText},
                {typeof(YuLegoImage), CreateImage},
                {typeof(YuLegoRawImage), CreateRawImage},
                {typeof(YuLegoButton), CreateButton},
                {typeof(YuLegoTButton), CreateTButton},
                {typeof(YuLegoToggle), CreateToggle},
                {typeof(YuLegoPlaneToggle), CreatePanleToggle},
                {typeof(YuLegoSlider), CreateSlider},
                {typeof(YuLegoProgressbar), CreateProgressbar},
                {typeof(YuLegoInputField), CreateInputField},
                {typeof(YuLegoDropdown), CreateDropdown},
                {typeof(YuLegoScrollView), CreateScrollView},
                {typeof(YuLegoRocker), CreateRocker }
            };

        public static GameObject GetControl<T>() where T : Component
        {
            var type = typeof(T);
            var func = controlFuncDict[type];
            var go = func();
            return go;
        }

        #endregion
    }
}