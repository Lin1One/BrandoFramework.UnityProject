#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动列表。
    /// </summary>
    public interface IYuLegoScrollView : ILegoControl
    {
        IYuLegoScrollViewRxModel ScRxModel { get; }

        /// <summary>
        /// 添加数据模型回调
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IYuLegoScrollView OnAddComponentRxmodel(Action<IYuLegoUIRxModel> callback);

        /// <summary>
        /// 注册一个处理滚动列表子组件新增事件的回调委托。
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        IYuLegoScrollView OnAddComponent(Action<ILegoComponent> callback);

        /// <summary>
        /// 组件绘制完毕后回调吗，用于做业务的特殊处理。
        /// </summary>
        /// <param name="onDraw"></param>
        /// <returns></returns>
        IYuLegoScrollView OnDrawComponent(Action<ILegoComponent, IYuLegoUIRxModel> onDraw);

        /// <summary>
        /// 替换滚动列表所有组件后，返回新组件链表
        /// </summary>
        /// <param name="onDraw"></param>
        /// <returns></returns>
        IYuLegoScrollView OnReplaceComponents(Action<LinkedList<ILegoComponent>> callback);

        RectTransform Content { get; }

        /// <summary>
        /// 将滚动视图重置为起始的位置。
        /// </summary>
        void ResetToZero();

        void ResetToLast();
    }
}