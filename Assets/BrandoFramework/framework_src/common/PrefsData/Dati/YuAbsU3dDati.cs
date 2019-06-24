#region Head

// Author:            Yu
// CreateDate:        2019/1/18 22:17:10
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.DataStruct;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace YuU3dPlay
{
    [Serializable]
    public abstract class YuAbsU3dDati : ScriptableObject, IYuU3dDati
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

        private string TypeId => GetType().TypeId();

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
        //    var detailHelperAttr = GetType().GetAttribute<YuDatiDetailHelpDescAttribute>();
        //    if (detailHelperAttr == null)
        //    {
        //        return;
        //    }

        //    DetailHelp = detailHelperAttr.DetailHelperDesc;
        }

        #endregion

        public static YuAbsU3dDati CreateDati(Type type)
        {
            var newScriptInstance = (YuAbsU3dDati)CreateInstance(type);
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