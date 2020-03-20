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
    public class IsoUnitEntity : UnitEntityBase
    {
        /// <summary>
        /// 初始化 Todo 需要改为读取配置文件的方式
        /// </summary>
        /// <param name="animator"></param>
        public override void Init(long id, string assetId, Action<UnitEntityBase> onCreated = null, bool isSync = false)
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

        public override void InitComponent()
        {

        }

    }
}

