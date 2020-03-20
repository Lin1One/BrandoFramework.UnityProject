#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using UnityEngine;

namespace Client.LegoUI
{
    [AddComponentMenu("Yu/LegoUI/YuLego Progressbar", 55)]
    public class YuLegoProgressbar :
        YuLegoSlider,
        IYuLegoProgressbar
    {
        #region Lego

        public float Progress
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region 元数据变形

        private enum YuProgressbarMetamorphoseStatus : byte
        {
            Progressbar,
            ImageBackground,
            RectTransform_FillArea,
            Image_Fill
        }

        private YuProgressbarMetamorphoseStatus progressbarStatus;
        private LegoProgressbarMeta progressbarMeta;

        public override void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (progressbarMeta == null)
            {
                progressbarMeta = uiMeta.NextProgressbar;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (progressbarStatus)
            {
                case YuProgressbarMetamorphoseStatus.Progressbar:
                    MetamorphoseRect(RectMeta);
                    progressbarStatus = YuProgressbarMetamorphoseStatus.ImageBackground;
                    break;
                case YuProgressbarMetamorphoseStatus.ImageBackground:
                    ImageBackground.Metamorphose(progressbarMeta.BackgroundImageRect,
                        progressbarMeta.BackgroundImageMeta);
                    progressbarStatus = YuProgressbarMetamorphoseStatus.RectTransform_FillArea;
                    break;
                case YuProgressbarMetamorphoseStatus.RectTransform_FillArea:
                    YuLegoUtility.MetamorphoseRect(RectFillArea, progressbarMeta.FillAreaMeta);
                    progressbarStatus = YuProgressbarMetamorphoseStatus.Image_Fill;
                    break;
                case YuProgressbarMetamorphoseStatus.Image_Fill:
                    ImageFill.Metamorphose(progressbarMeta.FillImageRect,
                        progressbarMeta.FillImageMeta);

                    progressbarMeta = null;
                    progressbarStatus = YuProgressbarMetamorphoseStatus.Progressbar;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 数据响应

        public void ReceiveProgressChange(float newValue)
        {
            Progress = newValue;
        }

        #endregion
    }
}