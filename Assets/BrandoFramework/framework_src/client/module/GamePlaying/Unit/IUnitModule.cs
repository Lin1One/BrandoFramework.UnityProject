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
        /// ע����Ϸʵ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegistUintType<T>() where T : UnitEntityBase, new();

        Transform UnitRoot{ get; }

        /// <summary>
        /// ����һ����λ
        /// </summary>
        /// <param name="id">Ψһid</param>
        /// <param name="type">����</param>
        /// <param name="assetid">��Դid</param>
        /// <param name="onCreated">������ص�</param>
        /// <param name="isSync">�Ƿ���ͬ������</param>
        /// <returns></returns>
        T CreateUnit<T>(Action<UnitEntityBase> bandAct, long id, UnitType type, string assetid,
           Action<UnitEntityBase> onCreated, bool isSync = false) where T : UnitEntityBase, new();

        /// <summary>
        /// ɾ��һ����λ
        /// </summary>
        /// <param name="guid"></param>
        void RemoveUnit(long guid);

        void RemoveUnit(UnitEntityBase obj);

        bool TryGetUnitByGuid(long guid, UnitType type, out UnitEntityBase unit);

        T GetUnit<T>() where T : UnitEntityBase, new();


    }
}

