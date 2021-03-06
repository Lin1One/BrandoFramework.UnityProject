#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/10 19:49:15
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// 角色单位基类
    /// </summary>
    public abstract class UnitEntityBase
    {
        #region 基础属性
        protected long id;                                   
        /// <summary>
        /// 唯一ID
        /// </summary>
        public long ID
        {
            get
            {
                return id;
            }
        }

        protected bool m_isReleased;

        internal UnitType m_type;

        public UnitType Type
        {
            get
            {
                return m_type;
            }
        }


        #endregion
        #region 组件管理

        //核心组件
        protected IUnitEntityTransform unitTrans;
        public IUnitEntityTransform UnitTrans
        {
            get
            {
                return unitTrans;
            }
        }

        private readonly Dictionary<Type, UnitComponentBase> m_comDic = new Dictionary<Type, UnitComponentBase>();

        /// <summary>
        /// 添加一个指定类型的角色组件，不允许添加已存在组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : UnitComponentBase, new()
        {
            Type type = typeof(T);
            if (m_comDic.ContainsKey(type))
            {
#if DEBUG
                Debug.LogError("添加角色组件错误，角色重复添加组件" + type.Name);
#endif 
                return null;
            }

            T com = UnitComponentBase.GetComponent<T>(this);
            com.Init();
            m_comDic.Add(type, com);
            return com;
        }

        /// <summary>
        /// 尝试获取一个指定类型的组件，角色不存在此组件则返回null
        /// </summary>
        public T GetComponent<T>() where T : UnitComponentBase, new()
        {
            Type type = typeof(T);
            UnitComponentBase com;
            m_comDic.TryGetValue(type, out com);
            return com as T;
        }

        /// <summary>
        /// 移除一个指定类型的组件
        /// </summary>
        public void RemoveComponent<T>() where T : UnitComponentBase, new()
        {
            Type type = typeof(T);
            if (!m_comDic.ContainsKey(type))
            {
#if DEBUG
                Debug.LogError("移除角色组件错误，角色不含有该组件" + type.Name);
#endif 
                return;
            }
            UnitComponentBase com = m_comDic[type];
            com.Release();
            UnitComponentBase.RecoverComponent(com);
            m_comDic.Remove(type);
        }

        public virtual void Init(long id, 
            string assetId, 
            Action<UnitEntityBase> onCreated = null, 
            bool isSync = false)
        {
            this.id = id;
            m_isReleased = false;

            //初始化UnityObject数据，交给派生类处理
            InitComponent();

            if (onCreated == null) //给回调加上此类刷新位置的函数
            {
                onCreated = (obj) => { unitTrans.RefreshTrans(); };
            }
            else
            {
                onCreated += (obj) => { unitTrans.RefreshTrans(); };
            }

            unitTrans.LoadAsset(assetId, isSync, onCreated);
        }

        public abstract void InitComponent();

        #endregion

        #region 组件快捷引用

        protected IYuUnitAnimator m_animator;
        public IYuUnitAnimator AnimaControl
        {
            get
            {
                return m_animator;
            }
        }


        protected IYuActStateMachine m_actStateMachine;
        /// <summary>
        /// 行为状态机
        /// </summary>
        public IYuActStateMachine ActStateMachine
        { get { return m_actStateMachine; } }

        //protected IYuSkillControl m_skillControl;
        ///// <summary>
        ///// 技能控制器
        ///// </summary>
        //public IYuSkillControl SkillControl
        //{
        //    get { return m_skillControl; }
        //}



        //public IYuUnitMotion Motion
        //{
        //    get { return m_u3dData as IYuUnitMotion; }
        //}

        //protected IYuUnitPendantManager m_pendantManager;
        ///// <summary>
        ///// 挂件功能接口
        ///// </summary>
        //public IYuUnitPendantManager PendantManager
        //{
        //    get
        //    {
        //        return m_pendantManager;
        //    }
        //}

        //protected IYuUnitMount m_mount;
        ///// <summary>
        ///// 坐骑功能
        ///// </summary>
        //public IYuUnitMount Mount
        //{
        //    get
        //    {
        //        return m_mount;
        //    }
        //}

        protected IYuUnitAIControl m_aIControl;
        /// <summary>
        /// AI控制器
        /// </summary>
        public IYuUnitAIControl AIControl
        {
            get
            {
                if(m_aIControl == null)
                {
                    m_aIControl = new XTwoUnitAIControl();
                }
                return m_aIControl;
            }
        }

        #endregion


        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {
            m_isReleased = true;
            id = -1;
            unitTrans = null;

            foreach (var item in m_comDic.Values)
            {
                item.Release();
                UnitComponentBase.RecoverComponent(item);
            }
            m_comDic.Clear();
        }
    }
}

