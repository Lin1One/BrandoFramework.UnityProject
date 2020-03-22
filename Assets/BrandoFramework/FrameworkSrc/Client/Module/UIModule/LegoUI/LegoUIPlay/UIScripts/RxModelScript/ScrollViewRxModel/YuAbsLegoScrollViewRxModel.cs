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


using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 滚动视图数据模型泛型基类。
    /// </summary>
    [System.Serializable]
    public abstract class YuAbsLegoScrollViewRxModel<T> : IYuLegoScrollViewRxModel where T : IYuLegoUIRxModel
    {
        public IYuLegoUIRxModel ComponentDefaultRxModel { get; set; }

        [LabelText("滚动列表子组件数据模型列表")]
        [SerializeField]
        private List<T> UiRxModels = new List<T>();

        public int ModelCount => UiRxModels.Count;

        public IYuLegoUIRxModel AddRxModel(IYuLegoUIRxModel rxModel)
        {
            var t = (T)rxModel;

            if (!UiRxModels.Contains(t))
            {
                IsReplacingRxModel = true;
                UiRxModels.Add(t);
                onAdd(t);
            }

            return t;
        }

        private Action<IYuLegoUIRxModel> onAdd;

        public void BindingAdd(Action<IYuLegoUIRxModel> callback)
        {
            onAdd = callback;
        }

        private Action<int, IYuLegoUIRxModel> onInsert;

        public void BindingInsert(Action<int, IYuLegoUIRxModel> callback)
        {
            onInsert = callback;
        }

        private Action<int, IYuLegoUIRxModel> onRemove;

        public void BindingRemove(Action<int, IYuLegoUIRxModel> callback)
        {
            onRemove = callback;
        }

        private Action<List<IYuLegoUIRxModel>> onReplace;
        public bool IsReplacingRxModel { get;  set; }
        public Queue<List<IYuLegoUIRxModel>> ToReplaceRxModelList { get; private set; }
            = new Queue<List<IYuLegoUIRxModel>>();
        public void BindingReplace(Action<List<IYuLegoUIRxModel>> callback)
        {
            onReplace += callback;
        }


        public void CopyRxModel(int index, IYuLegoUIRxModel targetRxModel)
        {
            if (UiRxModels.Count <= index)
            {
#if DEBUG
                Debug.Log("没有足够的数据模型可以执行拷贝操作，索引已超出当前数据模型最大数量！");
#endif
                return;
            }

            var sourceRxModel = UiRxModels[index];
            sourceRxModel.Copy(targetRxModel);
        }

        public IYuLegoUIRxModel GetRxModel(int index)
        {
            return UiRxModels.Count <= index ? default(T) : UiRxModels[index];
        }

        public void InsertModel(int index, IYuLegoUIRxModel rxModel)
        {
#if DEBUG
            if (index < 0 || index == UiRxModels.Count - 1)
            {
                Debug.LogError($"滚动视图的数据模型插入索引超出范围，期望索引是{index}，" +
                               $"允许范围为0=>{UiRxModels.Count - 1}！");
                return;
            }
#endif

            UiRxModels.Insert(index, (T)rxModel);
            onInsert?.Invoke(index, rxModel);
        }

        public void Release()
        {
            foreach (var rxModel in UiRxModels)
            {
                rxModel.Release();
            }
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

            var model = UiRxModels[modelIndex];
            UiRxModels.RemoveAt(modelIndex);
            onRemove?.Invoke(modelIndex, model);
        }

        public void Replace(List<IYuLegoUIRxModel> models)
        {
            if (!IsReplacingRxModel)
            {
                IsReplacingRxModel = true;
                UiRxModels.Clear();

                foreach (var item in models)
                {
                    var t = (T)item;
                    UiRxModels.Add(t);
                }
                onReplace?.Invoke(models);


            }
            else
            {
                ToReplaceRxModelList.Enqueue(models);
            }
        }

#if UNITY_EDITOR
        
        [Button("保存")]
        public void Save()
        {
            //var currentApp = YuU3dAppSettingDati.CurrentActual;
            //var jsonContent = YuJsonUtility.ToJson(this);
            //var finalId = GetType().Name;
            //var writePath = currentApp.Helper.AssetDatabaseLegoUIRxModelDir +
            //                "ScrollView/" + finalId + ".txt";
            //YuIOUtility.WriteAllText(writePath, jsonContent);
        }
        
#endif
        

    }
}