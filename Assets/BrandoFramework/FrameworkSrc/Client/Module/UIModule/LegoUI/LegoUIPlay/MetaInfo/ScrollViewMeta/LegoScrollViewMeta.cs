#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using System;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动列表元数据。
    /// </summary>
    [System.Serializable]
    public class LegoScrollViewMeta
    {
        #region ScrollView根对象元数据

        /// <summary>
        /// 滚动视图的游戏对象名。
        /// </summary>
        public string Id;

        /// <summary>
        /// 是否采用多行多列布局。
        /// </summary>
        public bool IsMultiLayout;

        /// <summary>
        /// 每行每列的组件数量。
        /// </summary>
        public int MaxPerLine;

        public int ItemWidth;

        public int ItemHeight;

        /// <summary>
        /// 是否不初始化自己对应的子组件。
        /// </summary>
        public bool IsNotInitSonComponent;

        /// <summary>
        /// 是否使用水平布局。
        /// </summary>
        public bool IsHorizontal;

        public bool HorizontalEnable;

        public bool VerticalEnable;

        public LegoImageMeta ScrollViewImageMeta;

        /// <summary>
        /// 子项布局X轴修正值。
        /// 用于在开启了水平垂直栏杆时修正子项的居中位置。
        /// </summary>
        public int Xoffset;

        /// <summary>
        /// 子项水平间距
        /// </summary>
        public int VerticalPadiding;

        /// <summary>
        /// 子项垂直间距
        /// </summary>
        public int HorizontalPadiding;

        /// <summary>
        /// 布局类型
        /// </summary>
        public YuArrangement Arrangement;

        #endregion

        #region ScrollRect

        public LegoRectTransformMeta ScrollRectRectMeta;
        public LegoImageMeta ScrollRectImageMeta;

        #endregion

        #region Content

        public LegoRectTransformMeta ContentRectMeta;

        #endregion

#if DEBUG

        public static LegoScrollViewMeta Create(YuLegoScrollView legoScrollView)
        {
            var scHelper = legoScrollView.GetComponent<YuLegoScrollViewHelper>();
            if (scHelper == null)
            {
                throw new Exception($"滚动视图{legoScrollView.Name}上没有发现滚动视图开发助手组件！");
            }

            var meta = new LegoScrollViewMeta
            {
                Id = legoScrollView.Name,
                Arrangement = legoScrollView.Arrangement,
                ItemWidth = scHelper.IitemWidth,
                ItemHeight = scHelper.ItemHeight,
                IsHorizontal = scHelper.IsHorizontal,
                HorizontalEnable = scHelper.HorizontalEnable,
                VerticalEnable = scHelper.VerticalEnable,
                IsMultiLayout = scHelper.IsMulti,
                MaxPerLine = scHelper.MaxPerLine,
                Xoffset = scHelper.Xoffset,
                IsNotInitSonComponent = scHelper.IsNotInitSonComponent,
                HorizontalPadiding = scHelper.Horizontalpadiding,
                VerticalPadiding = scHelper.VerticalPadiding,
                ScrollViewImageMeta = LegoImageMeta
                    .Create(legoScrollView.RectTransform.GetComponent<YuLegoImage>())
            };

            var scrollRect = legoScrollView.RectTransform.Find("ScrollRect").RectTransform();
            meta.ScrollRectRectMeta = LegoRectTransformMeta.Create(scrollRect);
            meta.ScrollRectImageMeta = LegoImageMeta.Create(scrollRect.GetComponent<YuLegoImage>());

            var content = scrollRect.Find("Content").RectTransform();
            meta.ContentRectMeta = LegoRectTransformMeta.Create(content);

            return meta;
        }

#endif
    }
}