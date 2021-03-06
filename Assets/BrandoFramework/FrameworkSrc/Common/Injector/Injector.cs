
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common
{
    public class Injector : IInjector
    {
        private static Injector instance;
        public static Injector Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Injector();
                }
                return instance;
            }
        }
        protected Injector() { }

        /// 抽象类型或接口To具体实现类型映射
        private Dictionary<Type, Type> typeMaps;
        private Dictionary<Type, Type> TypeMaps => typeMaps ?? (typeMaps = new Dictionary<Type, Type>());

        /// <summary>
        /// 单例类型实例缓存
        /// </summary>
        private Dictionary<Type, object> singles;
        protected Dictionary<Type, object> Singles => singles ?? (singles = new Dictionary<Type, object>());

        /// <summary>
        /// 单例类型构造委托缓存
        /// Key：具体类型
        /// Value：构造委托
        /// </summary>
        private Dictionary<Type, Func<object>> singleFuncs;
        private Dictionary<Type, Func<object>> SingleFuncs => singleFuncs ?? (singleFuncs = new Dictionary<Type, Func<object>>());

        /// <summary>
        /// 类型构造委托缓存
        /// </summary>
        private Dictionary<Type, Func<object>> multiFuncs;
        private Dictionary<Type, Func<object>> MultiFuncs => multiFuncs ?? (multiFuncs = new Dictionary<Type, Func<object>>());

        /// <summary>
        /// 类型内字段信息缓存
        /// </summary>
        private Dictionary<Type, FieldInfo[]> cachedFieldInfos;
        private Dictionary<Type, FieldInfo[]> CachedFieldInfos => cachedFieldInfos ?? (cachedFieldInfos = new Dictionary<Type, FieldInfo[]>());

        private List<FieldInfo> tempFieldInfos;
        private List<FieldInfo> TempFieldInfos => tempFieldInfos ?? (tempFieldInfos = new List<FieldInfo>());

        /// <summary>
        /// 类型内属性信息缓存
        /// </summary>
        private Dictionary<Type, PropertyInfo[]> cachedPropertyInfos;
        private Dictionary<Type, PropertyInfo[]> CachedPropertyInfos => cachedPropertyInfos ?? (cachedPropertyInfos = new Dictionary<Type, PropertyInfo[]>());

        private List<PropertyInfo> tempPropertyInfos;
        private List<PropertyInfo> TempPropertyInfos => tempPropertyInfos ?? (tempPropertyInfos = new List<PropertyInfo>());

        public void CleanAllMap()
        {
            TypeMaps.Clear();
            Singles.Clear();
            MultiFuncs.Clear();
            CachedFieldInfos.Clear();
            CachedPropertyInfos.Clear();
            TempFieldInfos.Clear();
            TempPropertyInfos.Clear();
        }

        private const BindingFlags BINGDING_FLAGS =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static;

        private const string AUTO_INJECT = "AutoInject";

        #region 添加映射关系

        /// <summary>
        /// 添加类型映射
        /// </summary>
        /// <param name="absT">抽象类型或接口。</param>
        /// <param name="implT">具体实现类型。</param>
        /// <exception cref="Exception"></exception>
        private void Map(Type absT, Type implT)
        {
            if (TypeMaps.ContainsKey(absT))
            {
                if (TypeMaps[absT] != implT)
                {
                    Debug.LogError($"源类型{absT.Name}当前映射到目标类型{implT.Name}");
                }
                return;
            }
            TypeMaps.Add(absT, implT);
        }

        public void Map<TAbs, TImpl>() where TImpl : TAbs
        {
            Map(typeof(TAbs), typeof(TImpl));
        }

        public bool HasMapped<TAbs>() => TypeMaps.ContainsKey(typeof(TAbs));

        public void MapWithSingletonCreateFunc<TAbs, TImpl>(Func<object> func) where TImpl : TAbs
        {
            var absType = typeof(TAbs);

            if (SingleFuncs.ContainsKey(absType))
            {
                throw new Exception($"单例源类型{absType.Name}的构建委托已存在，" +
                                    $"当前的构建委托为{SingleFuncs[absType]}," +
                                    $"新的构建委托为{func}！");
            }

            Map<TAbs, TImpl>();
            var implType = GetImplType(absType);
            SingleFuncs.Add(implType, func);
        }

        public void MapWithMultiCreateFunc<TAbs, TImpl>(Func<object> func) where TImpl : TAbs
        {
            var absType = typeof(TAbs);

            if (MultiFuncs.ContainsKey(absType))
            {
                throw new Exception($"多实例源类型{absType.Name}的构建委托已存在，" +
                                    $"当前的构建委托为{SingleFuncs[absType]}," +
                                    $"新的构建委托为{func}！");
            }

            Map<TAbs, TImpl>();
            var implType = GetImplType(absType);
            MultiFuncs.Add(implType, func);
        }

        #endregion

        #region 获取

        public TAbs Get<TAbs>() where TAbs : class
        {

            var obj = GetTypeInstance(typeof(TAbs));
            //var obj = GetAtNotReflectionEnable<TAbs>();

            var instance = (TAbs)obj;
            return instance;
        }

        protected object GetTypeInstance(Type absType)
        {
            var implType = GetImplType(absType);
            var tAbs = Reslove(implType);
            return tAbs;
        }

        /// <summary>
        /// 获得与抽象类型映射的具体类型。
        /// </summary>
        /// <param name="absType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected Type GetImplType(Type absType)
        {
            Type implType;

            if (!TypeMaps.ContainsKey(absType))
            {
                var defaultType = absType.GetAbsTypeDefaultMapImplType();
                if (defaultType != null)
                {
                    implType = defaultType;
                }
                else if (IsAbsType(absType))
                {
                    throw new Exception($"接口或者抽象类{absType},未映射实现类型，无法创建实例！");
                }
                else
                {
                    implType = absType;
                }
            }
            else
            {
                implType = TypeMaps[absType];
            }
            return implType;
        }

        /// <summary>
        /// 分析并构造类型
        /// 1、检测是否为已创建的单例对象
        /// 2、检测是否已注入构建方法，有则用构建方法创建
        /// 3、反射构建，遍历依赖注入的字段和属性，并尝试注入，最终调用类型注入的 Init 方法
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        protected object Reslove(Type implType)
        {
            object instance;
            if (implType.IsSingle())
            {
                //已创建单例获取
                instance = TryGetFromSingles(implType);
                if (instance != null)
                {
                    return instance;
                }
            }

            //通过类型传入的创建方法创建
            instance = TryGetFromTypeCreateFunc(implType);
            if (instance != null)
            {
                return instance;
            }

            //反射构建
            var fieldInfos = GetInjectFieldInfos(implType);
            var propertyInfos = GetInjectPropertyInfos(implType);
            var obj = Activator.CreateInstance(implType);

            if (implType.IsSingle())
            {
                Singles.Add(implType, obj);
            }

            InjectFields(obj, fieldInfos);
            InjectPropertys(obj, propertyInfos);

            //自动注入方法
            InvokeInitByReflection(obj);
            return obj;
        }

        #region 反射构建
        /// <summary>
        /// 获取单例类型
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        private object TryGetFromSingles(Type implType)
        {
            if (!Singles.ContainsKey(implType))
            {
                return null;
            }
            var existSingle = Singles[implType];
            return existSingle;
        }

        /// <summary>
        /// 通过类型映射的创建方法获取实例
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object TryGetFromTypeCreateFunc(Type targetType)
        {
            if (SingleFuncs.ContainsKey(targetType))
            {
                var singleFunc = SingleFuncs[targetType];
                var singleInstance = singleFunc();
                Singles.Add(targetType, singleInstance);
                return singleInstance;
            }

            if (MultiFuncs.ContainsKey(targetType))
            {
                var multiFunc = MultiFuncs[targetType];
                var multiInstance = multiFunc();
                return multiInstance;

            }
            return null;
        }

        /// <summary>
        /// 获取类型中依赖注入的字段
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected FieldInfo[] GetInjectFieldInfos(Type targetType)
        {
            if (CachedFieldInfos.ContainsKey(targetType))
            {
                return CachedFieldInfos[targetType];
            }

            var fieldInfos = targetType.GetFields(BINGDING_FLAGS);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.HasInjectAttribute())
                {
                    TempFieldInfos.Add(fieldInfo);
                }
            }

            var fieldInfoArray = TempFieldInfos.ToArray();
            CachedFieldInfos.Add(targetType, fieldInfoArray);
            TempFieldInfos.Clear();
            return fieldInfoArray;
        }
        /// <summary>
        /// 获取类型中依赖注入的变量
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected PropertyInfo[] GetInjectPropertyInfos(Type targetType)
        {
            if (CachedPropertyInfos.ContainsKey(targetType))
            {
                return CachedPropertyInfos[targetType];
            }

            var propertys = targetType.GetProperties(BINGDING_FLAGS);
            foreach (var propertyInfo in propertys)
            {
                if (propertyInfo.HasInjectAttribute())
                {
                    TempPropertyInfos.Add(propertyInfo);
                }
            }

            var propertyInfoArray = TempPropertyInfos.ToArray();
            CachedPropertyInfos.Add(targetType, propertyInfoArray);
            TempPropertyInfos.Clear();
            return propertyInfoArray;
        }

        private void InjectFields(object instance, FieldInfo[] fieldInfos)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var instanceField = GetTypeInstance(fieldInfo.FieldType);
                fieldInfo.SetValue(instance, instanceField);
            }
        }

        private void InjectPropertys(object instance, PropertyInfo[] propertyInfos)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var instanceProperty = GetTypeInstance(propertyInfo.PropertyType);
                propertyInfo.SetValue(instance, instanceProperty);
            }
        }

        protected Type GetFieldOrPropertyType(Type sourceType)
        {
            return TypeMaps.ContainsKey(sourceType) ? TypeMaps[sourceType] : sourceType;
        }

        /// <summary>
        /// 调用自动注入方法
        /// </summary>
        /// <param name="instance"></param>
        void InvokeInitByReflection(object instance)
        {
            var autoInject = instance.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .ToList().Find(m => m.Name == AUTO_INJECT);
            autoInject?.Invoke(instance, null);
        }

        protected void InvokeInit(object instance)
        {

            InvokeInitAtReflectionEnable();

            //InvokeInitAtReflectionDisble();

            //#if !ReflectionEnable
            //            void InvokeInitAtReflectionDisble()
            //            {
            //                //                var yuInit = (IYuInit)instance;
            //                //                yuInit.Init();
            //            }
            //#endif
        }
        void InvokeInitAtReflectionEnable()
        {
            var autoInject = instance.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .ToList().Find(m => m.Name == AUTO_INJECT);
            autoInject?.Invoke(instance, null);
        }

        #endregion

        #endregion

        #region ToolMethod

        private bool IsAbsType(Type t) => t.IsInterface || t.IsAbstract;

        #endregion
    }

    #region 待整理代码




    //public void ReMapping<TAbs, TImpl>() where TImpl : TAbs
    //{
    //    var absType = typeof(TAbs);
    //    if (!TypeMappings.ContainsKey(absType))
    //    {
    //        Debug.LogError($"源类型{typeof(TAbs).Name}当前没有映射关系无法重新建立映射关系！");
    //        return;
    //    }

    //    var implType = typeof(TImpl);
    //    TypeMappings[absType] = implType;
    //    Debug.Log($"源类型{absType.Name}的映射关系重定向到{implType.Name}！");
    //}

    //private TAbs GetAtNotReflectionEnable<TAbs>()
    //{
    //    var implType = typeof(TAbs);
    //    var sourceType = GetImplType(implType);

    //    var existSingle = TryGetFromSingles(sourceType);
    //    if (existSingle != null)
    //    {
    //        return (TAbs)existSingle;
    //    }

    //    return OnSinglesNotHas();

    //    TAbs OnSinglesNotHas()
    //    {
    //        if (SingleFuncs.ContainsKey(sourceType))
    //        {
    //            var singleFunc = SingleFuncs[sourceType];
    //            var singleInstance = (TAbs)singleFunc();
    //            Singles.Add(implType, singleInstance);
    //            return singleInstance;
    //        }

    //        if (!MultiFuncs.ContainsKey(sourceType))
    //        {
    //            return default;
    //        }

    //        var buildFunc = MultiFuncs[sourceType];
    //        var instance = (TAbs)buildFunc();
    //        return instance;
    //    }
    //}

    //private static Dictionary<string, object> pathMapObjs;

    //protected static Dictionary<string, object> PathMapObjs =>
    //    pathMapObjs ?? (pathMapObjs = new Dictionary<string, object>());

    //public TAbs Get<TAbs>(string path) where TAbs : class
    //{
    //    if (!string.IsNullOrEmpty(path) && PathMapObjs.ContainsKey(path))
    //    {
    //        var instance = PathMapObjs[path];
    //        return (TAbs)instance;
    //    }

    //    if (string.IsNullOrEmpty(path))
    //    {
    //        throw new Exception("实例获取路径不能为空！");
    //    }

    //    var absType = typeof(TAbs);
    //    var implType = GetImplType(absType);
    //    var newInstance = (TAbs)Activator.CreateInstance(implType);
    //    PathMapObjs.Add(path, newInstance);
    //    return newInstance;
    //}
    #endregion
}