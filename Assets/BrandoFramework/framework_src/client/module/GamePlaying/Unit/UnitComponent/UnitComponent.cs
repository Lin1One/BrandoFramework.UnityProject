#region Head

// Author:            Chengkefu
// CreateDate:        2018/12/6 11:11:11
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 游戏个体组件基类
    /// </summary>
    public abstract class UnitComponent : IUnitComponent
    {
        #region 静态池功能
        private static readonly Dictionary<Type, Stack<UnitComponent>> s_pool 
            = new Dictionary<Type, Stack<UnitComponent>>();

        //从池中获取一个指定类型的对象
        internal static T GetComponent<T>(UnitEntityBase role) where T : UnitComponent, new()
        {
            Type type = typeof(T);
            if(!s_pool.ContainsKey(type))
            {
                s_pool.Add(type, new Stack<UnitComponent>());
            }
            T com;
            if (s_pool[type].Count >0)
            {
                com = s_pool[type].Pop() as T;
            }
            else
            {
                com = new T();
            }
            com.ResetRole(role);
            com.m_isRecover = false;

            return com;
        }

        //回收一个对象到池中
        internal static void RecoverComponent(UnitComponent com)
        {
            if (com.m_isRecover)
            {
#if DEBUG
                UnityEngine.Debug.LogError("错误，多次回收同个UnitComponent：" + com);
#endif
                return;
            }
            com.m_isRecover = true;
            com.ResetRole(null);
            com.m_refCount++;
            Type type = com.GetType();
            s_pool[type].Push(com);
        }

        #endregion

        //因为组件已改成池模式获取，而各个类型组件可能有异步加载等功能，
        //为了在异步回调中区别是否还在当前
        private uint m_refCount;
        public uint RefCount
        {
            get { return m_refCount; }
        }

        private bool m_isRecover;
        public bool IsRecover
        {
            get
            {
                return m_isRecover;
            }
        }

        private UnitEntityBase m_role;
        public UnitEntityBase Role
        {
            get
            {
                return m_role;
            }
        }
        //public YuTransforem

        private void ResetRole(UnitEntityBase role)
        {
            m_role = role;
        }

        internal void Init()
        {
            OnInit();
        }
        internal void Release()
        {
            OnRelease();
        }

        protected abstract void OnRelease();
        protected abstract void OnInit();
    }
}

