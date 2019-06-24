#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/26 8:58:33
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.PrefsData;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEngine;


namespace YuU3dPlay
{
    /// <summary>
    /// 可持久化对象，同时支持C#原生及unity可序列化脚本机制。
    /// 作为实际序列化数据的容器使用。
    /// </summary>
    /// <typeparam name="TActual"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    [Serializable]
    public abstract class YuAbsU3dGenericDati<TActual, TImpl> : YuAbsU3dDati, IYuU3dDati<TActual>
        where TActual : class, new()
        where TImpl : class
    {
        #region 基础结构和API

        [SerializeField] [HideLabel] private TActual actualSerializableObject;

        public TActual ActualSerializableObject
        {
            get => actualSerializableObject;
            protected set => actualSerializableObject = value;
        }

        #endregion

        #region 编辑下通用保存入口API

#if UNITY_EDITOR

        [HorizontalGroup("底部按钮")]
        [GUIColor(0.6f, 0.6f, 0.8f)]
        [Button("保存资料文件", ButtonSizes.Medium)]
        public abstract void Save();

        private bool CheckIsMulti() =>
            GetType().GetAttribute<YuDatiDescAttribute>()?.DatiSaveType != YuDatiSaveType.Multi;

#endif

        #endregion

        #region 可重载的序列化和反序列化

        protected virtual TActual DeSerialize(string originPath)
        {
            var bytes = File.ReadAllBytes(originPath);
            var actual = SerializeUtility.DeSerialize<TActual>(bytes);
            return actual;
        }

        protected virtual void Serialize(string originPath)
        {
            var bytes = SerializeUtility.Serialize(ActualSerializableObject);
            YuIOUtility.WriteAllBytes(originPath, bytes, true);
        }

        #endregion
    }
}

