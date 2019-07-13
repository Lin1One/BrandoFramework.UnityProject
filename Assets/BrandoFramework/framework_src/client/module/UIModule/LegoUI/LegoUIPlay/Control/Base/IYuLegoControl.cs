#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件接口。
    /// </summary>
    public interface ILegoControl
    {
        #region 基础属性

        /// <summary>
        /// 乐高视图控件所在组件。
        /// </summary>
        ILegoUI LocUI { get; }

        /// <summary>
        /// 乐高视图控件的RectTransform组件。
        /// </summary>
        RectTransform RectTransform { get; }

        GameObject GameObject { get; }

        /// <summary>
        /// 控件名。
        /// </summary>
        string Name { get; }

        LegoRectTransformMeta RectMeta { get; }

        #endregion

        #region 变形

        /// <summary>
        /// 使用元数据进行变形重用。
        /// </summary>
        /// <param name="uiMeta">当前的控件集元数据。</param>
        /// <returns>该次变形所完成的变形工作量。</returns>
        void Metamorphose(LegoUIMeta uiMeta);

        /// <summary>
        /// 变形阶段。
        /// </summary>
        LegoMetamorphoseStage MetamorphoseStage { get; }

        #endregion

        #region 构造

        void Construct(ILegoUI locUI, object obj = null);

        #endregion
    }

    public interface IYuLegoInteractableControl
    {
        void RegisterHandler(LegoInteractableType interactableType, IYuLegoActionHandler handler);

        /// <summary>
        /// 可交互控件的可交互开关。
        /// </summary>
        bool Interactable { get; set; }
    }
}