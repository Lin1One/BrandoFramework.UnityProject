#if DEBUG

#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using System.Linq;
using Client.Extend;
using Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

using YuU3dPlay;

namespace Client.LegoUI
{
    public class YuLegoScrollViewHelper ////: YuDevelopHelper
    {
        #region 基础字段

        private YuLegoScrollView _legoScrollView;
        private RectTransform content;
        private RectTransform scrollRectRectTrans;

        #endregion

        #region 基础配置

        #region 滚动列表布局类型

        [FoldoutGroup("基础配置")] [LabelText("滚动列表布局类型")] [SerializeField]
        private YuLegoScrollViewType type;

        public YuLegoScrollViewType Type => type;

        #endregion

        #region 子组件宽度

        [HorizontalGroup("基础配置/子组件尺寸")] [FoldoutGroup("基础配置")] [ShowInInspector] [SerializeField] [LabelText("子组件宽度")]
        private int itemWidth;

        public int IitemWidth => itemWidth;

        #endregion

        #region 子组件高度

        [HorizontalGroup("基础配置/子组件尺寸")] [FoldoutGroup("基础配置")] [ShowInInspector] [SerializeField] [LabelText("子组件高度")]
        private int itemHeight;

        public int ItemHeight => itemHeight;

        #endregion

        #region X轴修正

        [HorizontalGroup("基础配置/子项位置修正")]
        [FoldoutGroup("基础配置")]
        [ShowInInspector]
        [LabelText("子项X轴修正")]
        [SerializeField]
        [OnValueChanged("UpdateAllItem")]
        private int xOffset;

        public int Xoffset => xOffset;

        #endregion

        #region Y轴修正

        [HorizontalGroup("基础配置/子项位置修正")]
        [FoldoutGroup("基础配置")]
        [ShowInInspector]
        [LabelText("子项Y轴修正")]
        [SerializeField]
        [OnValueChanged("UpdateAllItem")]
        private int yOffset;

        public int Yoffset => yOffset;

        #endregion

        #region 子项水平间距

        [HorizontalGroup("基础配置/子项间距")] [FoldoutGroup("基础配置")] [LabelText("子项水平间距")] [ShowInInspector] [SerializeField]
        private int horizontalPadiding = 5;

        public int Horizontalpadiding => horizontalPadiding;

        #endregion

        #region 子项垂直间距

        [HorizontalGroup("基础配置/子项间距")] [FoldoutGroup("基础配置")] [LabelText("子项垂直间距")] [ShowInInspector] [SerializeField]
        private int verticalPadiding = 5;

        public int VerticalPadiding => verticalPadiding;

        #endregion

        #region 是否允许水平滚动

        [HorizontalGroup("基础配置/滚动配置")] [FoldoutGroup("基础配置")] [LabelText("水平滚动")] [ShowInInspector] [SerializeField]
        [OnValueChanged("OnSetHorizontalEnable")]
        public bool HorizontalEnable;

        private void OnSetHorizontalEnable()
        {
            var sr = scrollRectRectTrans.GetComponent<ScrollRect>();
            sr.horizontal = HorizontalEnable;
        }

        #endregion

        #region 是否允许垂直滚动

        [HorizontalGroup("基础配置/滚动配置")] [FoldoutGroup("基础配置")] [LabelText("垂直滚动")] [ShowInInspector] [SerializeField]
        [OnValueChanged("OnSetVerticalEnable")]
        public bool VerticalEnable;

        private void OnSetVerticalEnable()
        {
            var sr = scrollRectRectTrans.GetComponent<ScrollRect>();
            sr.vertical = VerticalEnable;
        }


        #endregion

        #region IsMulti

        public bool IsMulti => isMulti;

        [FoldoutGroup("基础配置")]
        [LabelText("是否多行多列布局")]
        [SerializeField]
        [ShowInInspector]
        [OnValueChanged("UpdateAllItem")]
        private bool isMulti;

        #endregion

        #region IsHorizontal

        [FoldoutGroup("基础配置")] [LabelText("是否使用水平布局")] [SerializeField] [ShowInInspector]
        private bool isHorizontal = false;

        public bool IsHorizontal => isHorizontal;

        #endregion

        private void UpdateAllItem()
        {
            Calculate();
            var childs = content.GetAllChild();
            var index = 0;
            ReflectUtility.InvokeMethod(_legoScrollView, "SetIsMulti", new object[] {isMulti});
            ReflectUtility.SetField(_legoScrollView, "maxPerLine", maxPerLine);
            ReflectUtility.SetField(_legoScrollView, "horizontalPadiding", Horizontalpadiding);
            ReflectUtility.SetField(_legoScrollView, "verticalPadiding", VerticalPadiding);
            ReflectUtility.SetField(_legoScrollView, "xOffset", xOffset);

            foreach (var child in childs)
            {
                var position = (Vector3)ReflectUtility.InvokeMethod(_legoScrollView, "GetItemPosition",
                    new object[] {index});
                child.transform.localPosition = position;
                index++;
            }
        }

        #endregion


        #region 等尺寸布局配置

        /// <summary>
        /// 是否不初始化自己对应的子组件。
        /// </summary>
        [FoldoutGroup("等尺寸布局配置")] [LabelText("不自动初始化子组件")]
        public bool IsNotInitSonComponent;

        [FoldoutGroup("等尺寸布局配置")] [LabelText("每行子组件数量")] [SerializeField] [ShowInInspector]
        private int maxPerLine = 1;

        public int MaxPerLine => maxPerLine;

        #endregion


        #region 不等尺寸布局配置

        #endregion

        #region 编辑器下辅助方法

        private void OnEnable()
        {
            ////_legoScrollView = GetComponent<YuLegoScrollView>();
            ////scrollRectRectTrans = transform.Find("ScrollRect").RectTransform();
            ////content = scrollRectRectTrans.Find("Content").RectTransform();
            ////Calculate();
            ////var sr = scrollRectRectTrans.GetComponent<ScrollRect>();
            ////HorizontalEnable = sr.horizontal;
            ////VerticalEnable = sr.vertical;
        }

        /// <summary>
        /// 检查滚动视图下的组件名和滚动视图自身的命名是否一致。
        /// </summary>
        [Button("检查组件名一致性")]
        private void CheckComponentId()
        {
            ////var useComponentId = name.Split('@').Last().Split('=').Last();
            ////var contentName = content.GetChild(0).name.Split('_').Last();
            ////if (useComponentId != contentName)
            ////{
            ////    throw new Exception($"滚动视图{name}的命名和其下的组件{contentName}不一致！");
            ////}
        }

        private void Calculate()
        {
            if (content.childCount == 0)
            {
                return;
            }

            ////var component = content.GetChild(0)?.RectTransform();
            ////if (component == null)
            ////{
            ////    Debug.LogError($"滚动视图{name}下没有发现子组件！");
            ////    return;
            ////}

            ////itemWidth = (int) component.sizeDelta.x;
            ////itemHeight = (int) component.sizeDelta.y;
            ////var horizontal = scrollRectRectTrans.GetComponent<ScrollRect>().horizontal;
            ////isHorizontal = horizontal;
        }

        #endregion
    }
}

#endif