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
    public class UnitModule : IUnitModule
    {
        /// <summary>
        /// 所有活动的角色<类型,<唯一id，角色基类>>
        /// </summary>
        private Dictionary<UnitType, Dictionary<long, UnitEntityBase>> m_dicActiveUnit;

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

        //客户端本地角色
        private Dictionary<int, UnitEntityBase> m_dicLocalUnit;
        private int m_localUnitCount;

        private UnitEntityBase m_leadPlayer;
        public UnitEntityBase LeadPlayer
        { get { return m_leadPlayer; } }

        public Transform UnitRoot
        {
            get { return m_roleRoot != null ? m_roleRoot.transform : null; }
        }
        private GameObject m_roleRoot;
        public Transform ItemRoot
        {
            get { return m_itemRoot != null ? m_itemRoot.transform : null; }
        }
        private GameObject m_itemRoot;

        public UnitModule()
        {
            m_dicActiveUnit = new Dictionary<UnitType, Dictionary<long, UnitEntityBase>>();
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                m_dicActiveUnit.Add(type, new Dictionary<long, UnitEntityBase>());
            }

            m_dicLocalUnit = new Dictionary<int, UnitEntityBase>();
            m_localUnitCount = 10001;
        }

        public void Init()
        {
            m_roleRoot = GameObject.Find("Yu/Scene/RoleRoot");
            if (m_roleRoot == null)
            {
                m_roleRoot = new GameObject("RoleRoot");
                m_roleRoot.transform.SetParent(GameObject.Find("Yu/Scene").transform);
            }

            m_itemRoot = GameObject.Find("Yu/Scene/ItemRoot");
            if (m_itemRoot == null)
            {
                m_itemRoot = new GameObject("ItemRoot");
                m_itemRoot.transform.SetParent(GameObject.Find("Yu/Scene").transform);
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
        public T CreateUnit<T>(Action<UnitEntityBase> bandAct, long id, UnitType type, string assetid,
            Action<UnitEntityBase> onCreated, bool isSync = false) where T : UnitEntityBase, new()
        {
            UnitEntityBase unit = null;

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
            if (type == UnitType.LeadPlayer)
            {
                m_leadPlayer = unit;
            }

            m_dicActiveUnit[type].Add(id, unit);
            unit.Init(id, assetid, onCreated, isSync);

            return unit as T;
        }

        /// <summary>
        /// 创建一个客户端本地单位
        /// </summary>
        public T CreateLocalUnit<T>(Action<UnitEntityBase> bandAct, UnitType type, string assetid,
            Action<UnitEntityBase> onCreated, bool isSync = false) where T : UnitEntityBase, new()
        {
            UnitEntityBase unit = GetUnit<T>();
            bandAct(unit);

            unit.m_type = type;
            int id = m_localUnitCount++;
            m_dicLocalUnit.Add(id, unit);
            unit.Init(id, assetid, onCreated, isSync);
            return unit as T;
        }

        /// <summary>
        /// 删除一个单位
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveUnit(long guid)
        {
            foreach (var dic in m_dicActiveUnit.Values)
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

        /// <summary>
        /// 删除一个本地单位
        /// </summary>
        /// <param name="id"></param>
        public void RemoveLocalUnit(long id)
        {
            int intId = (int)id;
            if (m_dicLocalUnit.ContainsKey(intId))
            {
                var obj = m_dicLocalUnit[intId];
                obj.Release();
                m_dicLocalUnit.Remove(intId);
                RecoverUnit(obj);
            }
        }

        public void RemoveUnit(UnitEntityBase obj)
        {
        }

        public bool TryGetUnitByGuid(long guid, out UnitEntityBase unit)
        {
            foreach (var item in m_dicActiveUnit.Values)
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
            var unitDic = m_dicActiveUnit[type];

            if (unitDic.ContainsKey(guid))
            {
                unit = unitDic[guid];
                return true;
            }

            unit = null;
            return false;
        }

        /// <summary>
        /// 尝试获取一个已有本地NPC的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UnitEntityBase GetLoaclUnit(int id)
        {
            if (m_dicLocalUnit.ContainsKey(id))
            {
                return m_dicLocalUnit[id];
            }
            return null;
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

