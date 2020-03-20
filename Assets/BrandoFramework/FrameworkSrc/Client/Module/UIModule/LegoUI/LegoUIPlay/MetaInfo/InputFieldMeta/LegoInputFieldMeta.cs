#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/11 22:16:32
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高输入框元数据。
    /// </summary>
    [System.Serializable]
    public class LegoInputFieldMeta
    {
        #region 输入框元数据

        [LabelText("输入框颜色过渡")]
        public LegoColorTintMeta ColorTintMeta;

        [LabelText("输入框过渡类型")]
        public LegoTransition Transition;

        #endregion

        #region 输入框根物体上图片元数据

        [LabelText("输入框背景图片")]
        public LegoImageMeta RootImageMeta;

        #endregion

        #region 输入框占位提示文本元数据

        [LabelText("占位提示符位置")]
        public LegoRectTransformMeta PlaceHolderTextRectMeta;

        [LabelText("占位提示符文本内容")]
        public LegoTextMeta PlaceHolderTextMeta;

        #endregion

        #region 输入框主体内容文本元数据

        [LabelText("主文本位置")]
        public LegoRectTransformMeta ContentTextRectMeta;

        [LabelText("主文本内容")]
        public LegoTextMeta ContentTextMeta;

        #endregion

        #region 输入框点击音效

        [LabelText("输入框音效")]
        public string InputSoundId;

        #endregion

        public static LegoInputFieldMeta Create(YuLegoInputField inputField)
        {
            var rect = inputField.RectTransform;
            var meta = new LegoInputFieldMeta
            {
                Transition = inputField.transition.ToString().AsEnum<LegoTransition>(),
                ColorTintMeta = LegoColorTintMeta.Create(inputField),
                InputSoundId = inputField.SoundEffectId
            };

            var image = inputField.GetComponent<YuLegoImage>();
            meta.RootImageMeta = LegoImageMeta.Create(image);

            var textPlaceHolder = rect.Find("Placeholder");
            meta.PlaceHolderTextRectMeta = LegoRectTransformMeta
                .Create(textPlaceHolder.GetComponent<RectTransform>());
            meta.PlaceHolderTextMeta = LegoTextMeta.Create(textPlaceHolder.GetComponent<YuLegoText>());

            var textContent = rect.Find("Text");
            meta.ContentTextRectMeta = LegoRectTransformMeta.Create(textContent.RectTransform());
            meta.ContentTextMeta = LegoTextMeta.Create(textContent.GetComponent<YuLegoText>());

            return meta;
        }
    }
}