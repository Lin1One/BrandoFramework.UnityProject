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
        /// 所有活动的角色<类型,<唯一id，角色基类>>
        /// </summary>
        private Dictionary<UnitType, Dictionary<long, UnitEntityBase>> activeUnits;

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
                Debug.LogError("Unit 根物体未创建");
            }
        }

        /// <summary>
        /// 创建一个单位
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <param name="type">类型</param>
        /// <param name="assetid">资源id</param>
        /// <param name="onCreated">加载完回调</param>
        /// <param name="isSync">是否是同步加载</param>
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
                Debug.LogError("已创建了此角色id：");
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
        /// 删除一个单位
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

