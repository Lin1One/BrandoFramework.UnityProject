#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEngine;


namespace Common.PrefsData
{
    /// <summary>
    /// 可持久化数据对象，同时支持C#原生及unity可序列化脚本机制。
    /// 作为实际序列化数据的容器使用。
    /// </summary>
    /// <typeparam name="TActual"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    [Serializable]
    public abstract class GenericDati<TActual, TImpl> : Dati, IDati<TActual>
        where TActual : class, new()
        where TImpl : class
    {
        #region 基础结构和API

        [SerializeField]
        [HideLabel]
        private TActual actualSerializableObject;

        public TActual ActualSerializableObject
        {
            get => actualSerializableObject;
            protected set => actualSerializableObject = value;
        }

        #endregion

        #region 编辑器下通用保存入口API

#if UNITY_EDITOR

        [HorizontalGroup("底部按钮")]
        [GUIColor(0.6f, 0.6f, 0.8f)]
        [Button("保存资料文件", ButtonSizes.Medium)]
        public abstract void Save();

        //private bool CheckIsMulti() =>
        //    GetType().GetAttribute<YuDatiDescAttribute>()?.DatiSaveType != YuDatiSaveType.Multi;

#endif

        #endregion

        #region 序列化

        protected virtual TActual DeSerialize(string originPath)
        {
            var bytes = File.ReadAllBytes(originPath);
            var actual = SerializeUtility.DeSerialize<TActual>(bytes);
            return actual;
        }

        protected virtual void Serialize(string originPath)
        {
            var bytes = SerializeUtility.Serialize(ActualSerializableObject);
            IOUtility.WriteAllBytes(originPath, bytes, true);
        }

        #endregion
    }
}

