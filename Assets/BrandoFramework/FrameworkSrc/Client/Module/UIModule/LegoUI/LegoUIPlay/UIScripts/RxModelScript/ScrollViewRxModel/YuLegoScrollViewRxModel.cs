#region Head

// Author:            Yu
// CreateDate:        2018/10/2 11:59:12
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动视图相应数据模型。
    /// </summary>
    public class YuLegoScrollViewRxModel : IYuLegoScrollViewRxModel
    {
        private readonly List<IYuLegoUIRxModel> uiRxModels
            = new List<IYuLegoUIRxModel>();

        public void Release()
        {
            foreach (var rxModel in uiRxModels)
            {
                rxModel.Release();
            }
        }

        #region 数据模型事件绑定

        private Action<IYuLegoUIRxModel> onAdd;

        public void BindingAdd(Action<IYuLegoUIRxModel> callback)
        {
            onAdd = callback;
        }

        private Action<int, IYuLegoUIRxModel> onRemove;

        public void BindingRemove(Action<int, IYuLegoUIRxModel> callback)
        {
            onRemove = callback;
        }

        private Action<int, IYuLegoUIRxModel> onInsert;

        public void BindingInsert(Action<int, IYuLegoUIRxModel> callback)
        {
            onInsert = callback;
        }

        #endregion

        #region 数据模型操作

        public IYuLegoUIRxModel AddRxModel(IYuLegoUIRxModel rxModel)
        {
#if DEBUG
            if (uiRxModels.Contains(rxModel))
            {
                throw new Exception("尝试添加一个已存在的数据模型实例！");
            }
#endif

            uiRxModels.Add(rxModel);
            onAdd?.Invoke(rxModel);
            return rxModel;
        }

        public void RemoveRxModel(int modelIndex)
        {
#if DEBUG
            if (ModelCount <= modelIndex)
            {
                Debug.LogError($"滚动视图移除操作超出范围，当前最大索引为{ModelCount - 1}！");
                return;
            }
#endif

            var model = uiRxModels[modelIndex];
            uiRxModels.RemoveAt(modelIndex);
            onRemove?.Invoke(modelIndex, model);
        }

        public void InsertModel(int index, IYuLegoUIRxModel rxModel)
        {
#if DEBUG
            if (index < 0 || index == uiRxModels.Count - 1)
            {
                Debug.LogError($"滚动视图的数据模型插入索引超出范围，期望索引是{index}，" +
                               $"允许范围为0=>{uiRxModels.Count - 1}！");
                return;
            }
#endif

            uiRxModels.Insert(index, rxModel);
            onInsert?.Invoke(index, rxModel);
        }

        public void CopyRxModel(int index, IYuLegoUIRxModel targetRxModel)
        {
            if (uiRxModels.Count <= index)
            {
#if DEBUG
                Debug.Log("没有足够的数据模型可以执行拷贝操作，索引已超出当前数据模型最大数量！");
#endif
                return;
            }

            var sourceRxModel = uiRxModels[index];
            sourceRxModel.Copy(targetRxModel);
        }

        public IYuLegoUIRxModel GetRxModel(int index)
        {
            return uiRxModels.Count <= index ? null : uiRxModels[index];
        }

        public void Replace(List<IYuLegoUIRxModel> models)
        {
            uiRxModels.Clear();
            uiRxModels.AddRange(models);
            onReplace?.Invoke();
        }

        private Action onReplace;

        public YuLegoScrollViewRxModel()
        {
        }

        public void BindingReplace(Action callback)
        {
            onReplace = callback;
        }

        public void BindingReplace(Action<List<IYuLegoUIRxModel>> callback)
        {
            throw new NotImplementedException();
        }

        public IYuLegoUIRxModel ComponentDefaultRxModel { get; set; }

        #endregion

        public int ModelCount => uiRxModels.Count;

        public bool IsReplacingRxModel => throw new NotImplementedException();

        bool IYuLegoScrollViewRxModel.IsReplacingRxModel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Queue<List<IYuLegoUIRxModel>> ToReplaceRxModelList => throw new NotImplementedException();
    }
}