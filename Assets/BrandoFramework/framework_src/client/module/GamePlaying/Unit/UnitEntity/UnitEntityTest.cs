#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/10 19:49:15
// Email:             chengkefu0730@live.com

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
    public class UnitEntityTest : UnitEntityBase
    {
        public UnitEntityTest()
        {
        }

        public override void InitComponent()
        {
            //初始化UnityObject数据
            m_u3dData = AddComponent<XTwoU3DData>();

            InitAddtionalComponent();
        }

        public void InitAddtionalComponent()
        {
            ////初始化挂件管理
            //m_pendantManager = AddComponent<XTwoUnitPendantManager>();
            //动画组件
            m_animator = AddComponent<XTwoUnitAnimator>();
            ////坐骑组件
            //m_mount = AddComponent<XTwoUnitMount>();
            //初始化状态机
            //m_actStateMachine = AddComponent<XTwoActStateMachine>();
            ////初始化技能
            //m_skillControl = AddComponent<XTwoSkillControl>();
            //初始化AI
            m_aIControl = AddComponent<XTwoUnitAIControl>();
        }

        public override void Init(long id, string assetId, Action<UnitEntityBase> onCreated = null, bool isSync = false)
        {
            this.id = id;
            m_isReleased = false;

            //初始化UnityObject数据，交给派生类处理

            InitComponent();
            //if (onCreated == null) //给回调加上此类刷新位置的函数
            //{
            //    onCreated = (obj) => { m_u3dData.RefreshTrans(); };
            //}
            //else
            //{
            //    onCreated += (obj) => { m_u3dData.RefreshTrans(); };
            //}
            m_u3dData.LoadAsset(assetId, isSync, onCreated);
            //InitAddtionalComponent();
        }
    }
}

