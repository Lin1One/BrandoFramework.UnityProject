using client_common;
using Common.DataStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common.Utility
{
    /// <summary>
    /// 反射工具。
    /// 1. 程序集缓存。
    /// </summary>
    public static class YuReflectUtility
    {
        /// <summary>
        /// 程序集公开类型缓存字典
        /// 将当前已经查找过的程序集的公开类型列表缓存在该字典中
        /// 后续查找直接使用缓存列表提高反射效率
        /// </summary>
        private static readonly Dictionary<string, List<Type>>
            AssemblyDictionary = new Dictionary<string, List<Type>>();

        /// <summary>
        /// 尝试获取已缓存的程序集的公开类型列表。
        /// 如果目标程序集当前没有缓存则会返回空。
        /// </summary>
        /// <param name="assemblyName">目标程序集的全名。</param>
        /// <returns></returns>
        private static List<Type> TryGetCachedAssemblyTypes(string assemblyName)
        {
            if (!AssemblyDictionary.ContainsKey(assemblyName)) return null;

            var result = AssemblyDictionary[assemblyName];
            return result;
        }

        /// <summary>
        /// 获得目标程序及的类型列表。
        /// </summary>
        /// <param name="assembly">目标程序集。</param>
        /// <returns></returns>
        public static List<Type> GetAllType(Assembly assembly)
        {
            var types = TryGetCachedAssemblyTypes(assembly.FullName);
            if (types != null)
            {
                return types;
            }

            types = assembly.GetTypes().ToList();
            AssemblyDictionary.Add(assembly.FullName, types);
            return types;
        }

        /// <summary>
        /// 在目标程序集中查找实现了指定类型的所有子类并返回以类型名为Key，类型为值的字典。
        /// 目标类型不能是接口或者抽象类。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="assembly">目标程序集，默认为当前执行的程序集。</param>
        /// <param name="whereFunc">类型过滤委托，默认为空。</param>
        /// <returns></returns>
        private static Dictionary<string, Type> GetTypeDictionary<T>(
            Assembly assembly = null, Func<Type, bool> whereFunc = null)
        {
            var targetAssembly = assembly ?? Assembly.GetExecutingAssembly();
            var asmTypes = GetAllType(targetAssembly);

            var result = asmTypes
                .Where(t => typeof(T).IsAssignableFrom(t))
                .Where(t => !t.IsInterface && !t.IsAbstract);
            if (whereFunc == null)
            {
                return result.ToDictionary(t => t.Name);
            }

            var whereResult = result.Where(whereFunc).ToDictionary(t => t.Name);
            return whereResult;
        }

        /// <summary>
        /// 在目标程序集列表中查找指定类型的所有子类并返回以类型名为Key，类型为值的字典。
        /// 目标类型不能是接口或者抽象类。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="assemblies">目标程序集列表，默认为当前执行的程序集。</param>
        /// <param name="whereFunc">类型过滤委托，默认为空。</param>
        /// <returns></returns>
        public static Dictionary<string, Type> GetTypeDictionary<T>(
            List<Assembly> assemblies = null, Func<Type, bool> whereFunc = null)
        {
            assemblies = assemblies ?? new List<Assembly> {Assembly.GetExecutingAssembly()};
            var typeDictionary = new Dictionary<string, Type>();

            foreach (var assembly in assemblies)
            {
                var rightDictionary = GetTypeDictionary<T>(assembly, whereFunc);
                YuCommonExtend.Combin(typeDictionary, rightDictionary);
            }

            return typeDictionary;
        }

        public static Dictionary<string, Type> GetTypeDictionary<T>(
            params Assembly[] assemblies)
        {
            return GetTypeDictionary<T>(assemblies.ToList());
        }

        /// <summary>
        /// 在目标程序集中查找指定类型的所有子类型并返回类型列表。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="assembly">目标程序集。</param>
        /// <param name="isInterface">是否要过滤接口类型。</param>
        /// <param name="isAbstract">是否要过滤抽象类型。</param>
        /// <returns></returns>
        private static List<Type> GetTypeList<T>(Assembly assembly,
            bool isInterface = false, bool isAbstract = false)
        {
            var targetAssembly = assembly ?? Assembly.GetExecutingAssembly();
            var asmTypes = GetAllType(targetAssembly);

            var result = asmTypes
                .Where(t => typeof(T).IsAssignableFrom(t)
                            && t.IsInterface == isInterface
                            && t.IsAbstract == isAbstract)
                .ToList();

            return result;
        }

        /// <summary>
        /// 在目标程序集列表中查找指定类型的所有子类型并返回类型列表。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <param name="assemblies">目标程序集。</param>
        /// <param name="isInterface">是否要过滤接口类型。</param>
        /// <param name="isAbstract">是否要过滤抽象类型。</param>
        /// <returns></returns>
        public static List<Type> GetTypeList<T>(bool isInterface = false, bool isAbstract = false,
            params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                Debug.Log("目标程序集为空！");
                return null;
            }

            var asmList = assemblies.Length != 0
                ? assemblies.ToList()
                : new List<Assembly> {Assembly.GetExecutingAssembly()};
            if (asmList.Count == 0)
            {
                Debug.Log("目标程序集数量为0！");
                return null;
            }

            var asmTypes = new List<Type>();

            foreach (var assembly in asmList)
            {
                var types = GetTypeList<T>(assembly, isInterface, isAbstract);
                asmTypes.AddRange(types);
            }

            return asmTypes;
        }

        /// <summary>
        /// 使用反射调用目标类型的静态函数。
        /// </summary>
        /// <param name="type">目标类型。</param>
        /// <param name="methodName">函数名。</param>
        /// <param name="parames">函数所需的调用参数数组。</param>
        public static object InvokeStaticMethod(Type type, string methodName, object[] parames)
        {
            var methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                Debug.LogError($"无法从目标类型{type}中找到指定的静态函数{methodName}。");
                return null;
            }

            try
            {
                var obj = methodInfo.Invoke(null, parames);
                return obj;
            }
            catch (Exception exception)
            {
                Debug.LogError($"调用类型{type}的静态函数{methodName}时发生异常，" +
                                        $"异常信息为{exception.Message}");
                throw;
            }
        }

        private static Dictionary<Type, List<MethodInfo>> typeMethodInfos;

        private static Dictionary<Type, List<MethodInfo>> TypeMethodInfos
        {
            get
            {
                if (typeMethodInfos != null)
                {
                    return typeMethodInfos;
                }

                typeMethodInfos = new Dictionary<Type, List<MethodInfo>>();
                return typeMethodInfos;
            }
        }

        private static Dictionary<Type, Dictionary<string, MethodInfo>> typeMethodDict;

        private static Dictionary<Type, Dictionary<string, MethodInfo>> TypeMethodDict
        {
            get
            {
                if (typeMethodDict != null)
                {
                    return typeMethodDict;
                }

                typeMethodDict = new Dictionary<Type, Dictionary<string, MethodInfo>>();
                return typeMethodDict;
            }
        }

        public static object InvokeMethod(object obj, string methdoId)
        {
            return InvokeMethod(obj, methdoId, new object[] { });
        }

        public static object InvokeMethod(object obj, string methodName, object[] pars)
        {
            var type = obj.GetType();
            if (TypeMethodDict.ContainsKey(type))
            {
                if (TypeMethodDict[type].ContainsKey(methodName))
                {
                    var methodInfo = TypeMethodDict[type][methodName];
                    return methodInfo.Invoke(obj, pars);
                }

                var methodInfos = GetMethodInfos(type);
                var targetMethod = methodInfos.ToList().Find(m => m.Name == methodName);
                TypeMethodDict[type].Add(methodName, targetMethod);
                return targetMethod.Invoke(obj, pars);
            }
            else
            {
                TypeMethodDict.Add(type, new Dictionary<string, MethodInfo>());
                var methodInfos = GetMethodInfos(type);
                var targetMethod = methodInfos.ToList().Find(m => m.Name == methodName);
                TypeMethodDict[type].Add(methodName, targetMethod);
                return targetMethod.Invoke(obj, pars);
            }
        }

        public static void LoopSetFieldNoCache(Type type, string fieldId, object instance, object value)
        {
            var fieldInfos = type.GetFields(bindingFlags).ToList();
            var targetFieldInfo = fieldInfos.Find(f => f.Name == fieldId);
            if (targetFieldInfo == null)
            {
                if (type.BaseType == typeof(object))
                {
                    return;
                }

                LoopSetFieldNoCache(type.BaseType, fieldId, instance, value);
            }
            else
            {
                targetFieldInfo.SetValue(instance, value);
            }
        }

        private static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic
                                                                         | BindingFlags.Public | BindingFlags.Static;

        public static void SetField(object obj, string fieldName, object value)
        {
            var fieldInfos = obj.GetType().GetFields(bindingFlags).ToList();
            var targetFieldInfo = fieldInfos.Find(f => f.Name == fieldName);
            targetFieldInfo.SetValue(obj, value);
        }

        public static void SetProperty(object obj, string propertyId, object value)
        {
            var propertyInfos = obj.GetType().GetProperties(bindingFlags).ToList();
            var propertyInfo = propertyInfos.Find(f => f.Name == propertyId);
            propertyInfo.SetValue(obj, value);
        }

        private static List<MethodInfo> GetMethodInfos(Type type)
        {
            if (TypeMethodInfos.ContainsKey(type))
            {
                return TypeMethodInfos[type];
            }

            var methodInfos = type.GetMethods(bindingFlags).ToList();
            TypeMethodInfos.Add(type, methodInfos);
            return methodInfos;
        }

        /// <summary>
        /// 获得指定枚举类型的所有字段信息。
        /// </summary>
        /// <param name="type">目标枚举类型。</param>
        /// <returns></returns>
        public static List<FieldInfo> GetEnumAllFieldInfos(Type type)
        {
            if (type == null)
            {
                Debug.LogError("目标类型为空！");
                return null;
            }

            if (!type.IsEnum)
            {
                Debug.LogError($"目标类型{type}不是一个枚举类型！");
                return null;
            }

            var fieldInfos = type.GetFields().ToList();
            fieldInfos.RemoveAt(0);
            return fieldInfos;
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).Any();
        }

        public static List<T> GetAttributes<T>(this Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), true)
                .Select(t => (T) t).ToList();
            return attributes;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), true)
                .Select(t => (T) t).ToList();
            if (attributes.Count > 0)
            {
                return attributes.First();
            }

            return null;
        }
    }
}