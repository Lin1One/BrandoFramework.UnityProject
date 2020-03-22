#region Head

// Author:            Yu
// CreateDate:        2018/10/2 11:59:12
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;
using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动视图响应式数据模型。
    /// </summary>
    public interface IYuLegoScrollViewRxModel : IRelease
    {
        #region 数据模型事件绑定

        void BindingAdd(Action<IYuLegoUIRxModel> callback);

        void BindingRemove(Action<int, IYuLegoUIRxModel> callback);

        void BindingInsert(Action<int, IYuLegoUIRxModel> callback);

        void BindingReplace(Action<List<IYuLegoUIRxModel>> callback);

        #endregion

        #region 数据模型操作

        IYuLegoUIRxModel AddRxModel(IYuLegoUIRxModel rxModel);

        void RemoveRxModel(int modelIndex);

        void InsertModel(int index, IYuLegoUIRxModel rxModel);

        IYuLegoUIRxModel GetRxModel(int index);

        /// <summary>
        /// 替换不定长的滚动视图模型
        /// </summary>
        /// <param name="models"></param>
        void Replace(List<IYuLegoUIRxModel> models);

        bool IsReplacingRxModel { get; set; }

        Queue<List<IYuLegoUIRxModel>> ToReplaceRxModelList { get; }
        #endregion

        #region 组件数据模型默认值

        /// <summary>
        /// 滚动视图子组件数据模型默认值。
        /// 用于支持一些快速的业务逻辑实现。
        /// </summary>
        IYuLegoUIRxModel ComponentDefaultRxModel { get; set; }

        /// <summary>
        /// 将指定位置的数据模型的值拷贝为给定的目标数据模型。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="targetRxModel"></param>
        void CopyRxModel(int index, IYuLegoUIRxModel targetRxModel);

        #endregion

        /// <summary>
        /// 数据模型数量。
        /// </summary>
        int ModelCount { get; }
    }
}