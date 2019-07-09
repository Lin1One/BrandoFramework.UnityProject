#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion


using System;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    public interface IUnitModule: IModule
    {
        /// <summary>
        /// 注册游戏实体类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegistUintType<T>() where T : UnitEntityBase, new();

        Transform UnitRoot{ get; }

        /// <summary>
        /// 创建一个单位
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <param name="type">类型</param>
        /// <param name="assetid">资源id</param>
        /// <param name="onCreated">加载完回调</param>
        /// <param name="isSync">是否是同步加载</param>
        /// <returns></returns>
        T CreateUnit<T>(Action<UnitEntityBase> bandAct, long id, UnitType type, string assetid,
           Action<UnitEntityBase> onCreated, bool isSync = false) where T : UnitEntityBase, new();

        /// <summary>
        /// 删除一个单位
        /// </summary>
        /// <param name="guid"></param>
        void RemoveUnit(long guid);

        void RemoveUnit(UnitEntityBase obj);

        bool TryGetUnitByGuid(long guid, UnitType type, out UnitEntityBase unit);

        T GetUnit<T>() where T : UnitEntityBase, new();


    }
}

