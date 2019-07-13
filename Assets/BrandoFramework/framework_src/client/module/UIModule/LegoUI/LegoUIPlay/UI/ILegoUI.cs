#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图组件。
    /// 视图界面的基础组成元素。
    /// 组件的业务逻辑处理实例使用注入机制自动注入。
    /// </summary>
    public interface ILegoUI
    {
        #region 属性

        /// <summary>
        /// 组件所在游戏对象的RectTransform。
        /// </summary>
        RectTransform UIRect { get; }

        string Id { get; }

        ILegoUI ParentUI { get; }

        void SetParentUi(ILegoUI parent);

        #endregion

        #region 基础控件存取

        /// <summary>
        /// 获得一个目标控件并调用其构造方法。
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetControlAndConstruct<T>(string id) where T : class, ILegoControl;

        /// <summary>
        /// 获得一个目标控件。
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetControl<T>(string id) where T : class, ILegoControl;

        /// <summary>
        /// 获得目标类型的所有控件。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetControls<T>() where T : class, ILegoControl;

        #endregion

        #region 容器

        RectTransform GetContainer(string id);

        #endregion

        #region 构造

        /// <summary>
        /// 构造一个乐高UI。
        /// </summary>
        /// <param name="locRect">UI所持有的游戏对象根物体。</param>
        /// <param name="pipeHandlers">UI所对应的所有UI生命周期事件处理器。</param>
        /// <param name="sons"></param>
        /// <param name="isInBack"></param>
        void Construct
        (
            RectTransform locRect,
            List<IYuLegoUIPipelineHandler> pipeHandlers,
            List<ILegoUI> sons = null,
            bool isInBack = false
        );

        #endregion

        #region 生命周期

        void Close();

        void Hide();

        void ShowDefault();

		void Show();

        #endregion

        #region 数据模型

        IYuLegoUIRxModel RxModel { get; }

        void SetRxModel(IYuLegoUIRxModel rxModel);

        #endregion

        #region 子组件

        Dictionary<string, ILegoUI> SonComponentDict { get; }

        ILegoUI GetSonComponent(string id);

        ILegoUI GetSonComponent(int index);

        T GetSonComponent<T>(int index) where T : ILegoUI;

        #endregion

        /// <summary>
        /// 跳转至目标UI
        /// </summary>
        /// <param name="uiIndex"></param>
        void SkipToUI(int uiIndex);

        CanvasGroup CanvasGroup { get; }
    }
}