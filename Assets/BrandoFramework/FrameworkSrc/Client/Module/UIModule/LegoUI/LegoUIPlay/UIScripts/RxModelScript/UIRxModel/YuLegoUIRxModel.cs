#region Head

// Author:            Yu
// CreateDate:        2018/8/28 11:06:27
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using YuU3dPlay;

namespace Client.LegoUI
{
    [System.Serializable]
    public class YuLegoUIRxModel : IYuLegoUIRxModel
    {
        protected virtual void AddControlRxModel<T>(T rxMoel, string id)
            where T : class, IRelease
        {
            if (!BaseModels.ContainsKey(id))
            {
                BaseModels.Add(id, rxMoel);
            }
        }

        public T GetControlRxModel<T>(string id)
            where T : class, IRelease
        {
            if (BaseModels.ContainsKey(id))
            {
                return BaseModels[id] as T;
            }

            return default(T);
        }

        public Dictionary<string, object> BaseModels { get; } = new Dictionary<string, object>();

        public Dictionary<string, IYuLegoUIRxModel> SonComponentModels { get; } =
            new Dictionary<string, IYuLegoUIRxModel>();

        public virtual void InitRxModel()
        {
        }

        public ILegoUI MapUI { get; set; }

        public void Copy(IYuLegoUIRxModel target)
        {
            var leftRxModels = BaseModels.Values.ToList();
            var rightRxModels = target.BaseModels.Values.ToList();
            if (leftRxModels.Count != rightRxModels.Count)
            {
                throw new Exception("两个数据模型实例容量不相等的UI数据模型实例无法进行复制操作！");
            }

            var length = leftRxModels.Count;
            for (int i = 0; i < length; i++)
            {
                var left = (IYuLegoControlRxModel) leftRxModels[i];
                var right = rightRxModels[i];
                left.Copy(right);
            }
        }

        public T AddControlRxModel<T>(string id) where T : class, IRelease
        {
            var type = typeof(T);
            var func = legoControlRxModelFuncs[type];
            var rxModel = (T) func();
            AddControlRxModel(rxModel, id);
            return rxModel;
        }

        #region 具象类型控件数据模型

        private static readonly Dictionary<Type, Func<IRelease>> legoControlRxModelFuncs
            = new Dictionary<Type, Func<IRelease>>();

        private static YuLegoTextRxModel GetTextRxModel() => YuLegoControlRxModelPool.TakeTextModel();
        private static YuLegoImageRxModel GetImageRxModel() => YuLegoControlRxModelPool.TakeImageModel();
        private static YuLegoRawImageRxModel GetRawImageRxModel() => YuLegoControlRxModelPool.TakeRawImageModel();
        private static YuLegoButtonRxModel GetButtonRxModel() => YuLegoControlRxModelPool.TakeButtonModel();
        private static YuLegoTButtonRxModel GetTButtonRxModel() => YuLegoControlRxModelPool.TakeTButtonModel();
        private static YuLegoToggleRxModel GetToggleRxModel() => YuLegoControlRxModelPool.TakeToggleModel();

        private static YuLegoPlaneToggleRxModel GetPlaneToggleRxModel() =>
            YuLegoControlRxModelPool.TakePlaneToggleModel();

        private static YuLegoSliderRxModel GetSliderRxModel() => YuLegoControlRxModelPool.TakeSliderModel();
        private static YuLegoInputFieldRxModel GetInputFieldRxModel() => YuLegoControlRxModelPool.TakeInputFieldModel();

        private static YuLegoProgressbarRxModel GetProgressbarRxModel() =>
            YuLegoControlRxModelPool.TakeProgressbarModel();

        private static IYuLegoRockerRxModel GetRockerRxModel() => YuLegoControlRxModelPool.TakeRockerModel();

        #endregion

        #region 静态构造

        static YuLegoUIRxModel()
        {
            // 数据模型创建函数
            legoControlRxModelFuncs.Add(typeof(YuLegoTextRxModel), GetTextRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoImageRxModel), GetImageRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoRawImageRxModel), GetRawImageRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoButtonRxModel), GetButtonRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoTButtonRxModel), GetTButtonRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoToggleRxModel), GetToggleRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoPlaneToggleRxModel), GetPlaneToggleRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoSliderRxModel), GetSliderRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoInputFieldRxModel), GetInputFieldRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoProgressbarRxModel), GetProgressbarRxModel);
            legoControlRxModelFuncs.Add(typeof(YuLegoRockerRxModel), GetRockerRxModel);

            // 数据模型回收函数
            releaseActions.Add(typeof(YuLegoTextRxModel), ReleaseTextRxModel);
            releaseActions.Add(typeof(YuLegoImageRxModel), ReleaseImageRxModel);
            releaseActions.Add(typeof(YuLegoRawImageRxModel), ReleaseRawImageRxModel);
            releaseActions.Add(typeof(YuLegoButtonRxModel), ReleaseButtonRxModel);
            releaseActions.Add(typeof(YuLegoTButtonRxModel), ReleaseTButtonRxModel);
            releaseActions.Add(typeof(YuLegoToggleRxModel), ReleaseToggleRxModel);
            releaseActions.Add(typeof(YuLegoPlaneToggleRxModel), ReleasePlaneToggleRxModel);
            releaseActions.Add(typeof(YuLegoSliderRxModel), ReleaseSliderRxModel);
            releaseActions.Add(typeof(YuLegoInputFieldRxModel), ReleaseInputFieldRxModel);
            releaseActions.Add(typeof(YuLegoProgressbarRxModel), ReleaseProgressbarRxModel);
            releaseActions.Add(typeof(YuLegoRockerRxModel), ReleaseRockerRxModel);
        }

        #endregion

        public void CopyFromJson()
        {
            foreach (var value in BaseModels.Values)
            {
                var rxModel = value as IYuLegoControlRxModel;
                rxModel.InitFromSerializeField();
            }
        }

        #region 释放及清理

        private static readonly Dictionary<Type, Action<object>> releaseActions
            = new Dictionary<Type, Action<object>>();

        private static void ReleaseTextRxModel(object obj)
        {
            var model = obj as YuLegoTextRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseImageRxModel(object obj)
        {
            var model = obj as YuLegoImageRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseRawImageRxModel(object obj)
        {
            var model = obj as YuLegoRawImageRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseButtonRxModel(object obj)
        {
            var model = obj as YuLegoButtonRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseTButtonRxModel(object obj)
        {
            var model = obj as YuLegoTButtonRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseToggleRxModel(object obj)
        {
            var model = obj as YuLegoToggleRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }


        private static void ReleasePlaneToggleRxModel(object obj)
        {
            var model = obj as YuLegoPlaneToggleRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseSliderRxModel(object obj)
        {
            var model = obj as YuLegoSliderRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseInputFieldRxModel(object obj)
        {
            var model = obj as YuLegoInputFieldRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseProgressbarRxModel(object obj)
        {
            var model = obj as YuLegoProgressbarRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        private static void ReleaseRockerRxModel(object obj)
        {
            var model = obj as YuLegoRockerRxModel;
            YuLegoControlRxModelPool.Restore(model);
        }

        public void Release()
        {
            foreach (var value in BaseModels.Values)
            {
                var type = value.GetType();
                var action = releaseActions[type];
                action(value);
            }
        }

#if UNITY_EDITOR

        [Button("保存")]
        public void Save()
        {
            ////var currentApp = YuU3dAppSettingDati.CurrentActual;
            ////var jsonContent = JsonUtility.ToJson(this);
            ////var finalId = LogicId ?? GetType().Name;
            ////if (finalId.Contains("@"))
            ////{
            ////    finalId = finalId.Replace("@", "_").Replace("=", "_");
            ////}

            ////var typeStr = GetType().Name.Contains("LegoView") ? "LegoView/" : "LegoComponent";
            ////var writePath = currentApp.Helper.AssetDatabaseLegoUIRxModelDir
            ////                + typeStr + "/" + finalId + ".txt";
            ////IOUtility.WriteAllText(writePath, jsonContent);
        }

#endif


        public string LogicId { get; set; }

        #endregion
    }
}