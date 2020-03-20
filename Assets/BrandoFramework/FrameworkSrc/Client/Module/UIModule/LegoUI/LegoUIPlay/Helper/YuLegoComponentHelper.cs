#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图组件开发助手。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego ComponentHelper", 101)]
#if DEBUG
    [ExecuteInEditMode]
#endif
    public class YuLegoComponentHelper : YuDevelopHelper
    {
        [LabelText("组件Id")] public string Id;
        [LabelText("组件的业务逻辑Id")] public string LogicId;

        [LabelText("左对齐布局距离")] [OnValueChanged("OnValueChanged")]
        public int PaddingLeft = 10;

        [LabelText("和上一个控件的Y轴间距")] [OnValueChanged("OnValueChanged")]
        public int PaddingLastY = 5;

#if DEBUG

        public void SetPaddingLastY(int value) => PaddingLastY = value;

        private void OnEnable()
        {
            var componentStr = name.Split('@').First();
            Id = componentStr.Split('_').Last();

            if (name.Contains("Logic="))
            {
                var logicStr = name.Split('@').Last();
                LogicId = logicStr.Split('=').Last();
            }
        }

        private List<RectTransform> AllSameComponentRect
        {
            get
            {
                var allSameComponent = new List<RectTransform>();
                var allHelper = FindObjectsOfType<YuLegoComponentHelper>().ToList();
                var targetHelpers = allHelper.FindAll(h => h.Id == Id);
                foreach (var helper in targetHelpers)
                {
                    allSameComponent.Add(helper.GetComponent<RectTransform>());
                    helper.SetPaddingLastY(PaddingLastY);
                }

                allSameComponent.Reverse();
                return allSameComponent;
            }
        }

        private RectTransform _rectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform != null)
                {
                    return _rectTransform;
                }

                _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private void OnValueChanged()
        {
//            foreach (var rect in AllSameComponentRect)
//            {
//                var siblingIndex = rect.GetSiblingIndex();
//                if (siblingIndex == 0 && rect.parent.childCount == 1)
//                {
//                    continue;
//                }
//
//                var prevRect = rect.parent.GetChild(siblingIndex - 1).GetComponent<RectTransform>();
//                var y = prevRect.localPosition.y + (int) (prevRect.sizeDelta.y / 2) + PaddingLastY;
//                var position = new Vector3(PaddingLeft, y, 0);
//                RectTransform.localPosition = position;
//            }
        }
#endif
    }
}