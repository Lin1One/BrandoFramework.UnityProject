using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    [Singleton]
    public class UnitModule : IUnitModule
    {
        /// <summary>
        /// 已创建的活动的角色<类型,<唯一id，角色基类>>
        /// </summary>
        private Dictionary<UnitType, List<UnitEntityBase>> activeUnits;

        public void RegistUintType<T>() where T : UnitEntityBase, new()
        {
            var type = typeof(T);
            if (m_dicPool.ContainsKey(type))
            {
#if DEBUG
                Debug.LogError("重复注册角色类型：" + type.Name);
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
        public Transform UnitRoot
        {
            get { return unitRoot != null ? unitRoot.transform : null; }
        }
        


        public UnitModule()
        {
            activeUnits = new Dictionary<UnitType, List<UnitEntityBase>>();
        }

        public void Init()
        {
            unitRoot = GameObject.Find(unitRootGameobjectName);
            if (unitRoot == null)
            {
                Debug.LogError("Unit 根物体未创建");
            }
        }
        #region 创建

        public T CreateUnit<T>(long id, UnitType type, string assetid,
            Action<UnitEntityBase> bandAct,
            Action<UnitEntityBase> onCreated,
            bool isSync = false)
            where T : UnitEntityBase, new()
        {
            UnitEntityBase unit = null;

            if (TryGetUnitByGuid(id, type, out unit))
            {
                return unit as T;
            }

            unit = GetUnit<T>();
            bandAct?.Invoke(unit);
            unit.m_type = type;
            if (type == UnitType.MainUnit)
            {
                mainUnit = unit;
            }

            activeUnits[type].Add(unit);
            unit.Init(id, assetid, onCreated, isSync);

            return unit as T;
        }

        public T CreateUnit<T>(UnitType type, string assetid, Action<UnitEntityBase> onCreated,
            Action<UnitEntityBase> bandAction, bool isSync = false) where T : UnitEntityBase, new()
        {
            UnitEntityBase unit;

            unit = GetUnit<T>();
            bandAction?.Invoke(unit);
            unit.m_type = type;

            if (type == UnitType.MainUnit)
            {
                mainUnit = unit;
            }

            activeUnits[type].Add(unit);
            //unit.Init(id, assetid, onCreated, isSync);

            return unit as T;
        }

        #endregion


        #region 移除
        /// <summary>
        /// 删除一个单位
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveUnit(long guid)
        {
            //foreach (var unit in activeUnits.Values)
            //{
            //    if (activeUnits.ContainsKey(guid))
            //    {
            //        var obj = dic[guid];
            //        obj.Release();
            //        dic.Remove(guid);
            //        RecoverUnit(obj);
            //        return;
            //    }
            //}
        }


        public void RemoveUnit(UnitEntityBase obj)
        {
        }
        #endregion

        #region 查找

        public bool TryGetUnitByGuid(long guid, out UnitEntityBase unit)
        {
            //foreach (var item in activeUnits.Values)
            //{
            //    if (item.ContainsKey(guid))
            //    {
            //        unit = item[guid];
            //        return true;
            //    }
            //}
            unit = null;
            return false;
        }

        public bool TryGetUnitByGuid(long guid, UnitType type, out UnitEntityBase unit)
        {
            //var unitDic = m_dicActiveUnit[type];

            //if (unitDic.ContainsKey(guid))
            //{
            //    unit = unitDic[guid];
            //    return true;
            //}

            unit = null;
            return false;
        }
        #endregion

        #region 对象池

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
