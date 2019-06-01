#region Head

// Author:            Chengkefu
// CreateDate:        2019/04/25 16:20:00
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.DataStruct;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// Unity对象基本控制组件接口
    /// </summary>
    public interface IUnitEntityTransform : IUnitComponent
    {
        /// <summary>
        /// 根物体Transform
        /// </summary>
        Transform Trans{ get; }

        /// <summary>
        /// 二维Unity坐标（XZ轴）
        /// </summary>
        Vector2 Position2D { get; set; }

        /// <summary>
        /// 所在Unity世界坐标
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// 朝向
        /// </summary>
        Vector3 Forward { get; }
        

        Vector2 Forward2D { get; }

        Vector2 TargetPos { get; }

        Vector2 TargetDir { get; }

        /// <summary>
        /// 所有材质
        /// </summary>
        List<Material> Materials { get; }

        float Scale { get; set; }

        /// <summary>
        /// 根据记录的坐标，刷新Unity的GameObject坐标位置
        /// </summary>
        void RefreshTrans();

        /// <summary>
        /// 设置角色朝向
        /// </summary>
        /// <param name="dir"></param>
        void SetDirect(Vector2 dir);

        /// <summary>
        /// 设置欧拉角旋转量
        /// </summary>
        /// <param name="dir"></param>
        void SetEulerAngle(Vector3 angle);

        /// <summary>
        /// 设置到服务器二维坐标位置
        /// </summary>
        /// <param name="pos"></param>
        void SetCoord(Point2 coord);

        /// <summary>
        /// 强行设置位置3D
        /// </summary>
        /// <param name="pos"></param>
        void SetPostion(Vector3 pos);

        /// <summary>
        /// 强行设置位置，如果位置不再可行走区域，则找最近可行走点
        /// </summary>
        /// <param name="pos"></param>
        void SetPostion(Vector2 pos);

        void SetActive(bool active);

        bool Active { get; }
        

        /// <summary>
        /// 设置角色的显示layer
        /// </summary>
        /// <param name="index"></param>
        void SetLayer(int index);

        /// <summary>
        /// 重新加载模型
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="isSync"></param>
        /// <param name="onCreated"></param>
        void ResetModule(string assetId, bool isSync, Action<UnitEntityBase> onCreated);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="isSync"></param>
        /// <param name="onCreated"></param>
        void LoadAsset(string assetId, bool isSync, Action<UnitEntityBase> onCreated);

        void BindGameObject(string GameObjectName);

    }
}

