
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
                if(instance == null)
                {
                    instance = new Injector();
                }
                return instance;
            }
        }

        private Dictionary<Type, Type> typeMaps;
        private Dictionary<Type, Type> TypeMappings => typeMaps ?? (typeMaps = new Dictionary<Type, Type>());


        #region 映射关系

        /// <summary>
        /// 添加类型映射
        /// </summary>
        /// <param name="absT">抽象类型。</param>
        /// <param name="implT">具体实现类型。</param>
        /// <exception cref="Exception"></exception>
        private void Mapping(Type absT, Type implT)
        {
            if (TypeMappings.ContainsKey(absT))
            {
                if (TypeMappings[absT] != implT)
                {
                    Debug.LogError($"源类型{absT.Name}当前映射到目标类型{implT.Name}");
                }
                return;
            }
            TypeMappings.Add(absT, implT);
        }

        public void Mapping<TAbs, TImpl>() where TImpl : TAbs
        {
            Mapping(typeof(TAbs), typeof(TImpl));
        }

        public bool HasMapping<TAbs>() => TypeMappings.ContainsKey(typeof(TAbs));

        private bool IsAbsType(Type t) => t.IsInterface || t.IsAbstract;

        #endregion

        #region 获取

        public TAbs Get<TAbs>() where TAbs : class
        {
            var obj = GetInstanceByReflection(typeof(TAbs));
            //var obj = GetAtNotReflectionEnable<TAbs>();
            var instance = (TAbs) obj;
            return instance;
        }

        protected object GetInstanceByReflection(Type absType)
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

            if (!TypeMappings.ContainsKey(absType))
            {
                if (IsAbsType(absType))
                {
                    throw new Exception($"接口或者抽象类{absType}无法创建实例！");
                }
                implType = absType;
            }
            else
            {
                implType = TypeMappings[absType];
            }
            return implType;
        }

        protected object Reslove(Type implType)
        {
            var instance = TryGetFromSingles(implType);
            if (instance != null)
            {
                return instance;
            }

            instance = TryGetFromFunc(implType);
            if (instance != null)
            {
                return instance;
            }

            var fieldInfos = GetFieldInfos(implType);
            var propertyInfos = GetPropertyInfos(implType);
            var obj = Activator.CreateInstance(implType);

            if (implType.IsSingle())
            {
                Singles.Add(implType, obj);
            }

            InjectFields(obj, fieldInfos);
            InjectPropertys(obj, propertyInfos);
            InvokeInitByReflection(obj);

            return obj;
        }

        private Dictionary<Type, object> singles;
        protected Dictionary<Type, object> Singles => singles ?? (singles = new Dictionary<Type, object>());

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

        private Dictionary<Type, Func<object>> singleFuncs;

        private Dictionary<Type, Func<object>> SingleFuncs => 
            singleFuncs ?? (singleFuncs = new Dictionary<Type, Func<object>>());

        private Dictionary<Type, Func<object>> multiFuncs;

        private Dictionary<Type, Func<object>> MultiFuncs =>
            multiFuncs ?? (multiFuncs = new Dictionary<Type, Func<object>>());
        /// <summary>
        /// 通过类型映射的创建方法获取实例
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object TryGetFromFunc(Type targetType)
        {
            if (SingleFuncs.ContainsKey(targetType))
            {
                var singleFunc = SingleFuncs[targetType];
                var singleInstance = singleFunc();
                Singles.Add(targetType, singleInstance);
                return singleInstance;
            }

            if (!MultiFuncs.ContainsKey(targetType))
            {
                return null;
            }

            var multiFunc = MultiFuncs[targetType];
            var multiInstance = multiFunc();
            return multiInstance;
        }

        private Dictionary<Type, FieldInfo[]> cachedFieldInfos;
        private Dictionary<Type, FieldInfo[]> CachedFieldInfos =>
            cachedFieldInfos ?? (cachedFieldInfos = new Dictionary<Type, FieldInfo[]>());

        private List<FieldInfo> tempFieldInfos;
        private List<FieldInfo> TempFieldInfos => 
            tempFieldInfos ?? (tempFieldInfos = new List<FieldInfo>());

        private const BindingFlags BINGDING_FLAGS = 
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static;


        /// <summary>
        /// 获取依赖类型的实例
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected FieldInfo[] GetFieldInfos(Type targetType)
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

        private Dictionary<Type, PropertyInfo[]> cachedPropertyInfos;

        private Dictionary<Type, PropertyInfo[]> CachedPropertyInfos =>
            cachedPropertyInfos ?? (cachedPropertyInfos = new Dictionary<Type, PropertyInfo[]>());

        private List<PropertyInfo> tempPropertyInfos;

        private List<PropertyInfo> TempPropertyInfos =>
            tempPropertyInfos ?? (tempPropertyInfos = new List<PropertyInfo>());

        protected PropertyInfo[] GetPropertyInfos(Type targetType)
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
                var fieldType = GetFieldOrPropertyType(fieldInfo.FieldType);
                var field = Reslove(fieldType);
                fieldInfo.SetValue(instance, field);
            }
        }

        private void InjectPropertys(object instance, PropertyInfo[] propertyInfos)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = GetFieldOrPropertyType(propertyInfo.PropertyType);
                var property = Reslove(propertyType);
                propertyInfo.SetValue(instance, property);
            }
        }

        protected Type GetFieldOrPropertyType(Type sourceType)
        {
            return TypeMappings.ContainsKey(sourceType) ? TypeMappings[sourceType] : sourceType;
        }

        private const string AUTO_INJECT = "AutoInject";

        void InvokeInitByReflection(object instance)
        {
            var autoInject = instance.GetType().GetMethods(
                    BindingFlags.NonPublic | BindingFlags.Instance)
                .ToList().Find(m => m.Name == AUTO_INJECT);
            autoInject?.Invoke(instance, null);
        }

        protected void InvokeInit(object instance)
        {

            InvokeInitAtReflectionEnable();

            //InvokeInitAtReflectionDisble();


            void InvokeInitAtReflectionEnable()
            {
                var autoInject = instance.GetType().GetMethods(
                        BindingFlags.NonPublic | BindingFlags.Instance)
                    .ToList().Find(m => m.Name == AUTO_INJECT);
                autoInject?.Invoke(instance, null);
            }

//#if !ReflectionEnable
//            void InvokeInitAtReflectionDisble()
//            {
//                //                var yuInit = (IYuInit)instance;
//                //                yuInit.Init();
//            }
//#endif
        }


        #endregion

        #region 清理

        public void CleanAllMap()
        {
            TypeMappings.Clear();
            Singles.Clear();
            MultiFuncs.Clear();
            CachedFieldInfos.Clear();
            CachedPropertyInfos.Clear();
            TempFieldInfos.Clear();
            TempPropertyInfos.Clear();
        }

        #endregion 
    }

    #region 注入器扩展类

    public static class InjectExtension
    {
        public static bool HasInjectAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(InjectAttribute), true).Any();
        }

        public static bool IsSingle(this Type type)
        {
            return type.GetCustomAttributes(typeof(SingletonAttribute), true).Any();
        }
    }

    #endregion

    #region 待整理代码
    //public void RegisterSingleBuildFunc<TAbs, TImpl>(Func<object> func) where TImpl : TAbs
    //{
    //    var absType = typeof(TAbs);

    //    if (SingleFuncs.ContainsKey(absType))
    //    {
    //        throw new Exception($"单例源类型{absType.Name}的构建委托已存在，" +
    //                            $"当前的构建委托为{SingleFuncs[absType]}," +
    //                            $"新的构建委托为{func}！");
    //    }

    //    Mapping<TAbs, TImpl>();
    //    var implType = GetImplType(absType);
    //    SingleFuncs.Add(implType, func);
    //}

    //public void RegisterMultiBuildFunc<TAbs, TImpl>(Func<object> func) where TImpl : TAbs
    //{
    //    var absType = typeof(TAbs);

    //    if (MultiFuncs.ContainsKey(absType))
    //    {
    //        throw new Exception($"多实例源类型{absType.Name}的构建委托已存在，" +
    //                            $"当前的构建委托为{SingleFuncs[absType]}," +
    //                            $"新的构建委托为{func}！");
    //    }

    //    Mapping<TAbs, TImpl>();
    //    var implType = GetImplType(absType);
    //    MultiFuncs.Add(implType, func);
    //}

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