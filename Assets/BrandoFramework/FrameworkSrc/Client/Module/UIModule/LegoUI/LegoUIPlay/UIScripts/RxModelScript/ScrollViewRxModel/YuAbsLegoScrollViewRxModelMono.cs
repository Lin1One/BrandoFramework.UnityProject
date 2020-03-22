//#if DEBUG

//using Sirenix.OdinInspector;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using YuPlay;


//#region Head

//// Author:            Yu
//// CreateDate:        2018/10/5 8:42:54
//// Email:             836045613@qq.com

///*
// * 修改日期  ：
// * 修改人    ：
// * 修改内容  ：
//*/

//#endregion

//namespace YuLegoUIPlay
//{
//    public class YuAbsLegoScrollViewRxModelMono<TInterface, TMono> : MonoBehaviour, IYuLegoScrollViewRxModel
//        where TMono : MonoBehaviour, TInterface
//        where TInterface : IYuLegoUIRxModel
//    {
//        #region GUI Inspector    

//        [HorizontalGroup("顶部工具栏")]
//        [Button("添加一个数据模型")]
//        [PropertyOrder(1)]
//        private void Test_AddModel()
//        {
//            var model = GetNewMonoModel();
//            uiRxModels.Add(model);
//            onAdd?.Invoke(model);
//        }

//        private TMono GetNewMonoModel()
//        {
//            var index = uiRxModels.Count;
//            var tmpGO = new GameObject(typeof(TMono).Name + "_" + index);
//            tmpGO.transform.SetParent(transform);
//            var model = tmpGO.AddComponent<TMono>();
//            model.InitRxModel();
//            return model;
//        }

//        [HorizontalGroup("顶部工具栏")]
//        [PropertyOrder(1)]
//        [Button("删除一个数据模型")]
//        private void Test_DeleteModel()
//        {
//            if (deleteIndex == -1)
//            {
//                var lastGo = transform.GetChild(transform.childCount - 1).gameObject;
//                var model = uiRxModels.Last();
//                uiRxModels.RemoveAt(uiRxModels.Count - 1);
//                if (Application.isPlaying)
//                {
//                    Destroy(lastGo);
//                }
//                else
//                {
//                    DestroyImmediate(lastGo);
//                }

//                onRemove?.Invoke(deleteIndex, model);
//            }
//            else
//            {
//                if (deleteIndex >= transform.childCount)
//                {
//                    Debug.LogError("删除索引超出当前数据模型子项的最大数量！");
//                    return;
//                }

//                var targetGo = transform.GetChild(deleteIndex).gameObject;
//                var model = uiRxModels[deleteIndex];
//                uiRxModels.RemoveAt(deleteIndex);
//                if (Application.isPlaying)
//                {
//                    Destroy(targetGo);
//                }
//                else
//                {
//                    DestroyImmediate(targetGo);
//                }

//                onRemove?.Invoke(deleteIndex, model);
//            }
//        }

//        [HorizontalGroup("顶部工具栏")]
//        [PropertyOrder(1)]
//        [Button("删除所有数据模型")]
//        private void Test_DeleteAll()
//        {
//            transform.DeleteAllChild();
//            uiRxModels.Clear();
//        }

//        [HorizontalGroup("删除索引")]
//        [LabelText("删除索引")]
//        [ShowInInspector]
//        private int deleteIndex = -1;

//        [LabelText("滚动视图子项数据模型列表")]
//        [ShowInInspector]
//        [PropertyOrder(2)]
//        [HorizontalGroup("滚动视图子项数据模型列表")]
//        private readonly List<TMono> uiRxModels
//            = new List<TMono>();

//        #endregion

//        #region 字段

//        #endregion

//        #region 属性

//        public void CopyRxModel(int index, IYuLegoUIRxModel targetRxModel)
//        {
//            if (uiRxModels.Count <= index)
//            {
//#if DEBUG
//                Debug.Log("没有足够的数据模型可以执行拷贝操作，索引已超出当前数据模型最大数量！");
//#endif
//                return;
//            }

//            var sourceRxModel = uiRxModels[index];
//            sourceRxModel.Copy(targetRxModel);
//        }

//        public int ModelCount => uiRxModels.Count;

//        #endregion

//        #region 数据模型事件绑定

//        private Action<IYuLegoUIRxModel> onAdd;

//        public void BindingAdd(Action<IYuLegoUIRxModel> callback)
//        {
//            onAdd = callback;
//        }

//        private Action<int, IYuLegoUIRxModel> onRemove;

//        public void BindingRemove(Action<int, IYuLegoUIRxModel> callback)
//        {
//            onRemove = callback;
//        }

//        private Action<int, IYuLegoUIRxModel> onInsert;

//        public void BindingInsert(Action<int, IYuLegoUIRxModel> callback)
//        {
//            onInsert = callback;
//        }

//        #endregion

//        #region 数据模型操作

//        public IYuLegoUIRxModel AddRxModel(IYuLegoUIRxModel rxModel)
//        {
//#if DEBUG
//            if (uiRxModels.Contains(rxModel))
//            {
//                throw new Exception("尝试添加一个已存在的数据模型实例！");
//            }
//#endif

//            IYuLegoUIRxModel finalModel;

//            if (rxModel is TMono mono)
//            {
//                finalModel = rxModel;
//                uiRxModels.Add(mono);
//            }
//            else
//            {
//                var monoModel = GetNewMonoModel();
//                monoModel.Copy(rxModel);
//                uiRxModels.Add(monoModel);
//                finalModel = monoModel;
//            }

//            onAdd?.Invoke(rxModel);
//            return finalModel;
//        }

//        public void RemoveRxModel(int modelIndex)
//        {
//#if DEBUG
//            if (ModelCount <= modelIndex)
//            {
//                Debug.LogError($"滚动视图移除操作超出范围，当前最大索引为{ModelCount - 1}！");
//                return;
//            }
//#endif

//            var model = uiRxModels[modelIndex];
//            uiRxModels.RemoveAt(modelIndex);
//            onRemove?.Invoke(modelIndex, model);
//        }

//        public void InsertModel(int index, IYuLegoUIRxModel rxModel)
//        {
//#if DEBUG
//            if (index < 0 || index == uiRxModels.Count - 1)
//            {
//                Debug.LogError($"滚动视图的数据模型插入索引超出范围，期望索引是{index}，" +
//                               $"允许范围为0=>{uiRxModels.Count - 1}！");
//                return;
//            }
//#endif

//            if (rxModel is TMono mono)
//            {
//                uiRxModels.Insert(index, mono);
//            }
//            else
//            {
//                var monoModel = GetNewMonoModel();
//                monoModel.Copy(rxModel);
//                uiRxModels.Insert(index, monoModel);
//            }

//            onInsert?.Invoke(index, rxModel);
//        }

//        public IYuLegoUIRxModel GetRxModel(int index)
//        {
//            return uiRxModels.Count <= index ? null : uiRxModels[index];
//        }

//        [LabelText("组件默认数据模型")]
//        [SerializeField]
//        [ShowInInspector]
//        [ReadOnly]
//        private TMono defaultRxModel;

//        public IYuLegoUIRxModel ComponentDefaultRxModel
//        {
//            get => defaultRxModel;
//            set => defaultRxModel.Copy(value);
//        }

//        #endregion

//        #region Mono事件

//        private void OnEnable()
//        {
//            if (transform.childCount > 1)
//            {
//                defaultRxModel = transform.GetChild(0).GetComponent<TMono>();
//            }

//            for (int index = 1; index < transform.childCount; index++)
//            {
//                var child = transform.GetChild(index);
//                var rxModel = child.GetComponent<TMono>();
//                uiRxModels.Add(rxModel);
//            }
//        }

//        #endregion

//        #region 释放

//        public void Release()
//        {
//            foreach (var rxModel in uiRxModels)
//            {
//                rxModel.Release();
//            }
//        }

//        public void Replace(List<IYuLegoUIRxModel> models)
//        {
//            if (uiRxModels.Count < models.Count)
//            {
//                for (int i = 0; i < uiRxModels.Count; i++)
//                {
//                    var source = uiRxModels[i];
//                    var target = models[i];
//                    source.Copy(target);
//                }

//                var length = models.Count - uiRxModels.Count;
//                for (int i = 0; i < length; i++)
//                {
//                    var monoModel = GetNewMonoModel();
//                    var targetIndex = uiRxModels.Count + i;
//                    var targetModel = models[targetIndex];
//                    monoModel.Copy(targetModel);
//                    uiRxModels.Add(monoModel);
//                }

//                onReplace?.Invoke();
//            }
//            else
//            {
//                for (int i = 0; i < models.Count; i++)
//                {
//                    var source = uiRxModels[i];
//                    var target = models[i];
//                    source.Copy(target);
//                }

//                var length = uiRxModels.Count - models.Count;
//                for (int i = 0; i < length; i++)
//                {
//                    var removeIndex = models.Count + i;
//                    RemoveRxModel(removeIndex);
//                }

//                onReplace?.Invoke();
//            }
//        }

//        private Action onReplace;

//        public void BindingReplace(Action callback)
//        {
//            onReplace = callback;
//        }

//        public void Replace(List<IYuLegoUIRxModel> models, int startIndex = 0)
//        {
//            uiRxModels.Clear();
//            for (int index = 1; index < transform.childCount; index++)
//            {
//                var child = transform.GetChild(index);
//            }
//        }

//        #endregion

//        #region 使用Json还原数据模型




//        #endregion
//    }
//}


//#endif