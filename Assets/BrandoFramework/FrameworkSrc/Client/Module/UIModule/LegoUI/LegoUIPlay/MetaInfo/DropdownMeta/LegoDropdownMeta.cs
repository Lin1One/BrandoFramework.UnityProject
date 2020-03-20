#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com


#endregion

using Common;
using System.Collections.Generic;
using UnityEngine;


namespace Client.LegoUI
{
    /// <summary>
    /// 下拉菜单控件元数据。
    /// </summary>
    [System.Serializable]
    public class LegoDropdownMeta
    {
        #region 元数据

        #region 下拉框元数据

        public LegoColorTintMeta ColorTintMeta;
        public LegoTransition Transition;
        public List<YuLegoDropdownOptionMeta> OptionMetas;
        public LegoImageMeta DropdownImageMeta;

        #endregion

        #region 下拉框Label元数据

        public LegoRectTransformMeta LabelRectMeta;
        public LegoTextMeta LabelTextMeta;

        #endregion

        #region 下拉框Arrow元数据

        public LegoRectTransformMeta ArrowRectMeta;
        public LegoImageMeta ArrowImageMeta;

        #endregion

        #region 下拉框Template根物体元数据

        public LegoRectTransformMeta TemplateRectMeta;
        public LegoImageMeta TemplateImageMeta;

        #endregion

        #region 下拉框Template/ViewPort元数据

        public LegoRectTransformMeta ViewPortRectMeta;
        public LegoImageMeta ViewPortImageMeta;

        #endregion

        #region Template/Viewport/Content

        public LegoRectTransformMeta ContentRectMeta;

        #endregion

        #region Template/Viewport/Content/Item

        public LegoRectTransformMeta ItemRootRectMeta;
        public LegoTransition ItemTransition;
        public LegoColorTintMeta ItemRootColorTintMeta;
        public string ItemRootTargetGraphicId;

        #endregion

        #region Template/Viewport/Content/Item/Background

        public LegoRectTransformMeta ItemBackgroundRectMeta;
        public LegoImageMeta ItemBackgroundImageMeta;

        #endregion

        #region Template/Viewport/Content/Item/Checkmark

        public LegoRectTransformMeta ItemCheckmarkRectMeta;
        public LegoImageMeta ItemCheckmarkImageMeta;

        #endregion

        #region Template/Viewport/Content/Item/Label

        public LegoRectTransformMeta ItemLabelRectMeta;
        public LegoTextMeta ItemLabelTextMeta;

        #endregion

        #region Scrollbar

        public LegoColorTintMeta ScrollbarColorTintMeta;
        public LegoRectTransformMeta ScrollbarRectMeta;
        public LegoImageMeta ScrollbarImageMeta;

        #endregion

        #region Scrollbar/SlidingArea

        public LegoRectTransformMeta SlidingAreaRectMeta;

        #endregion

        #region Scrollbar/SlidingArea/Handle

        public LegoRectTransformMeta ScrollbarHandleRectMeta;
        public LegoImageMeta ScrollbarHandleImageMeta;

        #endregion

        #endregion

        public static LegoDropdownMeta Create(YuLegoDropdown dropdown)
        {
            var meta = new LegoDropdownMeta
            {
                Transition = dropdown.transition.ToString().AsEnum<LegoTransition>(),
                ColorTintMeta = LegoColorTintMeta.Create(dropdown),
                OptionMetas = new List<YuLegoDropdownOptionMeta>(),
            };

            var dropdownImage = dropdown.RectTransform.GetComponent<YuLegoImage>();
            meta.DropdownImageMeta = LegoImageMeta.Create(dropdownImage);

            foreach (var optionData in dropdown.options)
            {
                var optionMeta = YuLegoDropdownOptionMeta.Create(optionData);
                meta.OptionMetas.Add(optionMeta);
            }

            var label = dropdown.RectTransform.Find("Label")
                .GetComponent<YuLegoText>();
            meta.LabelRectMeta = LegoRectTransformMeta.Create(label.rectTransform);
            meta.LabelTextMeta = LegoTextMeta.Create(label);

            var arrow = dropdown.RectTransform.Find("Arrow")
                .GetComponent<YuLegoImage>();
            meta.ArrowRectMeta = LegoRectTransformMeta.Create(arrow.RectTransform);
            meta.ArrowImageMeta = LegoImageMeta.Create(arrow);

            var template = dropdown.RectTransform.Find("Template")
                .GetComponent<RectTransform>();
            meta.TemplateRectMeta = LegoRectTransformMeta.Create(template);
            meta.TemplateImageMeta = LegoImageMeta.Create(template.GetComponent<YuLegoImage>());

            var viewport = template.Find("Viewport").GetComponent<RectTransform>();
            meta.ViewPortRectMeta = LegoRectTransformMeta.Create(viewport);
            meta.ViewPortImageMeta = LegoImageMeta.Create(viewport.GetComponent<YuLegoImage>());

            var content = viewport.Find("Content").GetComponent<RectTransform>();
            meta.ContentRectMeta = LegoRectTransformMeta.Create(content);

            var item = content.Find("Item").GetComponent<RectTransform>();
            meta.ItemRootRectMeta = LegoRectTransformMeta.Create(item);
            var itemRootToggle = item.GetComponent<YuLegoToggle>();
            meta.ItemTransition = itemRootToggle.transition.ToString().AsEnum<LegoTransition>();
            meta.ItemRootColorTintMeta = LegoColorTintMeta.Create(itemRootToggle);
            meta.ItemRootTargetGraphicId = itemRootToggle.targetGraphic.name;

            var itemBackground = item.Find("Item Background").GetComponent<RectTransform>();
            meta.ItemBackgroundRectMeta = LegoRectTransformMeta.Create(itemBackground);
            meta.ItemBackgroundImageMeta = LegoImageMeta.Create(itemBackground.GetComponent<YuLegoImage>());

            var itemCheckmark = item.Find("Item Checkmark").GetComponent<RectTransform>();
            meta.ItemCheckmarkRectMeta = LegoRectTransformMeta.Create(itemCheckmark);
            meta.ItemCheckmarkImageMeta = LegoImageMeta.Create(itemCheckmark.GetComponent<YuLegoImage>());

            var itemLabel = item.Find("Item Label").GetComponent<RectTransform>();
            meta.ItemLabelRectMeta = LegoRectTransformMeta.Create(itemLabel);
            meta.ItemLabelTextMeta = LegoTextMeta.Create(itemLabel.GetComponent<YuLegoText>());

            var scrollbar = template.Find("Scrollbar").GetComponent<RectTransform>();
            meta.ScrollbarRectMeta = LegoRectTransformMeta.Create(scrollbar);
            meta.ScrollbarImageMeta = LegoImageMeta.Create(scrollbar.GetComponent<YuLegoImage>());

            var slidingArea = scrollbar.Find("Sliding Area").GetComponent<RectTransform>();
            meta.SlidingAreaRectMeta = LegoRectTransformMeta.Create(slidingArea);

            var slidingAreaHandle = slidingArea.Find("Handle").GetComponent<RectTransform>();
            meta.ScrollbarHandleRectMeta = LegoRectTransformMeta.Create(slidingAreaHandle);
            meta.ScrollbarHandleImageMeta = LegoImageMeta.Create(slidingAreaHandle.GetComponent<YuLegoImage>());

            return meta;
        }
    }

    [System.Serializable]
    public class YuLegoDropdownOptionMeta
    {
        public string Text;
        public string SpriteId;

        public static YuLegoDropdownOptionMeta Create(YuLegoDropdown.OptionData optionData)
        {
            var meta = new YuLegoDropdownOptionMeta
            {
                Text = optionData.text,
                SpriteId = optionData.image == null
                    ? null
                    : optionData.image.name
            };

            return meta;
        }
    }
}