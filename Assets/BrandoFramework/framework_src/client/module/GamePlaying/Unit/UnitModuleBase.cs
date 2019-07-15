#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    [Singleton]
    public class UnitModuleBase : IUnitModule
    {
        /// <summary>
        /// ���л�Ľ�ɫ<����,<Ψһid����ɫ����>>
        /// </summary>
        private Dictionary<UnitType, Dictionary<long, UnitEntityBase>> activeUnits;

        public void RegistUintType<T>() where T : UnitEntityBase, new()
        {
            var type = typeof(T);
            if (m_dicPool.ContainsKey(type))
            {
#if DEBUG
                Debug.LogError("�ظ�ע���ɫ���ͣ�" + type.Name);
#endif
                return;
            }
            m_dicPool.Add(type, new Stack<UnitEntityBase>());
        }


        private UnitEntityBase mainUnit;
        public UnitEntityBase MainUnit
        {
            get
            {
                return mainUnit;
            }
        }

        private const string unitRootGameobjectName = "GameRoot/UnitRoot";
        private GameObject unitRoot;
        public GameObject UnitRoot
        {
            get { return unitRoot != null ? unitRoot : null; }
        }

        Transform IUnitModule.UnitRoot => throw new NotImplementedException();

        public UnitModuleBase()
        {
            activeUnits = new Dictionary<UnitType, Dictionary<long, UnitEntityBase>>();
        }

        public void Init()
        {
            unitRoot = GameObject.Find(unitRootGameobjectName);
            if (unitRoot == null)
            {
                Debug.LogError("Unit ������δ����");
            }
        }

        /// <summary>
        /// ����һ����λ
        /// </summary>
        /// <param name="id">Ψһid</param>
        /// <param name="type">����</param>
        /// <param name="assetid">��Դid</param>
        /// <param name="onCreated">������ص�</param>
        /// <param name="isSync">�Ƿ���ͬ������</param>
        /// <returns></returns>
        public T CreateUnit<T>(long id, UnitType type, string assetid,
            Action<UnitEntityBase> bandAct,
            Action<UnitEntityBase> onCreated,
            bool isSync = false)
            where T : UnitEntityBase, new()
        {
            UnitEntityBase unit;

            if (TryGetUnitByGuid(id, type, out unit))
            {
#if UNITY_EDITOR
                Debug.LogError("�Ѵ����˴˽�ɫid��");
#endif
                return unit as T;
            }

            unit = GetUnit<T>();
            bandAct?.Invoke(unit);
            unit.m_type = type;
            if (type == UnitType.MainUnit)
            {
                mainUnit = unit;
            }

            activeUnits[type].Add(id, unit);
            unit.Init(id, assetid, onCreated, isSync);

            return unit as T;
        }

        public T CreateUnit<T>(UnitType type, string assetid, Action<UnitEntityBase> onCreated, Action<UnitEntityBase> bandActbool, bool isSync = false) where T : UnitEntityBase, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ɾ��һ����λ
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveUnit(long guid)
        {
            foreach (var dic in activeUnits.Values)
            {
                if (dic.ContainsKey(guid))
                {
                    var obj = dic[guid];
                    obj.Release();
                    dic.Remove(guid);
                    RecoverUnit(obj);
                    return;
                }
            }
        }

        public void RemoveUnit(UnitEntityBase obj)
        {
        }

        public bool TryGetUnitByGuid(long guid, out UnitEntityBase unit)
        {
            foreach (var item in activeUnits.Values)
            {
                if (item.ContainsKey(guid))
                {
                    unit = item[guid];
                    return true;
                }
            }
            unit = null;
            return false;
        }

        public bool TryGetUnitByGuid(long guid, UnitType type, out UnitEntityBase unit)
        {
            var unitDic = activeUnits[type];

            if (unitDic.ContainsKey(guid))
            {
                unit = unitDic[guid];
                return true;
            }

            unit = null;
            return false;
        }


        #region �����

        private readonly Dictionary<Type, Stack<UnitEntityBase>> m_dicPool =
            new Dictionary<Type, Stack<UnitEntityBase>>();

        public T GetUnit<T>() where T : UnitEntityBase, new()
        {
            Type type = typeof(T);

            T unit = null;
            if (m_dicPool[type].Count > 0)
            {
                unit = m_dicPool[type].Pop() as T;
            }
            else
            {
                unit = new T();
            }
            return unit;
        }

        public void RecoverUnit(UnitEntityBase obj)
        {
            Type type = obj.GetType();
            m_dicPool[type].Push(obj);
        }



        #endregion

    }
}

