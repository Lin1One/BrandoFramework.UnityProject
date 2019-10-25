#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using Common;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Common.PrefsData
{
    /// <summary>
    /// 持久化数据抽象类
    /// </summary>
    [Serializable]
    public abstract class Dati : ScriptableObject, IDati
    {
        #region 可关闭的详细说明

        [NonSerialized]
        [ShowInInspector]
        [TextArea(5, 10)]
        [HideLabel]
        [ShowIf("IsShowDetailHelp")]
        [ReadOnly]
        [Title("资料文件详细说明")]
        [VerticalGroup("资料文件详细说明")]
        public string DetailHelp;

        private string TypeId => GetType().TypeName();

        private bool IsShowDetailHelp()
        {
            var value = PlayerPrefs.GetInt(TypeId, 1);
            var isShow = Convert.ToBoolean(value);
            return isShow && !string.IsNullOrEmpty(DetailHelp);
        }

        [ShowIf("IsShowDetailHelp")]
        [VerticalGroup("资料文件详细说明")]
        [Button("关闭资料文件详细说明", ButtonSizes.Medium)]
        private void CloseDetailHelp()
        {
            PlayerPrefs.SetInt(TypeId, 0);
        }

        protected void LoadDetailHelp()
        {
            var detailHelperAttr = GetType().GetAttribute<YuDatiDetailHelpDescAttribute>();
            if (detailHelperAttr == null)
            {
                return;
            }

            DetailHelp = detailHelperAttr.DetailHelperDesc;
        }

        #endregion

        public static Dati CreateDati(Type type)
        {
            var newScriptInstance = (Dati)CreateInstance(type);
            return newScriptInstance;
        }

        public virtual void OnActive()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnClose()
        {
        }
    }

}