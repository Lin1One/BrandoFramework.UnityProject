#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图控件存储器。
    /// </summary>
    public class LegoControlStorage : IRelease
    {
        private readonly Dictionary<string, ILegoControl> controlDict
            = new Dictionary<string, ILegoControl>();

        private LegoControlStorage()
        {
        }

        #region 自身对象池

        private static IObjectPool<LegoControlStorage> storagePool;

        public static IObjectPool<LegoControlStorage> StoragePool
        {
            get
            {
                if (storagePool != null)
                {
                    return storagePool;
                }

                storagePool = new ObjectPool<LegoControlStorage>(
                    () => new LegoControlStorage(), 10);
                return storagePool;
            }
        }

        #endregion

        #region 获取及缓存控件

        public T GetControl<T>(string id, RectTransform uiRect) where T : class, ILegoControl
        {
            if (!controlDict.ContainsKey(id))
            {
                var control = uiRect.Find(id).GetComponent<T>();
                if (control == null)
                {
#if DEBUG || DEBUG
                    //YuDebugUtility.LogError($"id{id}的目标{typeof(T)}控件不存在！");
#endif
                    return default(T);
                }

                AddControl(control);
                return control;
            }

            return controlDict[id] as T;
        }

        private void AddControl(ILegoControl control)
        {
            if (controlDict.ContainsKey(control.Name))
            {
#if DEBUG || DEBUG
                //YuDebugUtility.LogError($"控件{control.Name}已添加！");
#endif
                return;
            }

            controlDict.Add(control.Name, control);
            Controls.Add(control);
        }

        #endregion

        #region 释放

        public void Release()
        {
            controlDict.Clear();
        }

        public List<ILegoControl> Controls { get; } = new List<ILegoControl>();

        public List<T> GetControls<T>() where T : class, ILegoControl
        {
            return Controls.FindAll(c => c.GetType() == typeof(T)).Cast<T>().ToList();
        }

        #endregion
    }
}