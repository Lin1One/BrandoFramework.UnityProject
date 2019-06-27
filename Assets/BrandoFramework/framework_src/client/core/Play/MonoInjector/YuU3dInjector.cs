#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/27 8:21:35
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client;
using Client.Extend;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Common
{
    public interface IYuU3dInjector : IInjector
    {
        TMono GetMono<TMono>(string path = null, GameObject parent = null)
            where TMono : MonoBehaviour;

        TModule GetModule<TModule>() where TModule : class, IModule;
    }

    public class YuU3dInjector : Injector, IYuU3dInjector
    {
        private static YuU3dInjector monoInjectorInstance;
        public static YuU3dInjector MonoInjectorInstance
        {
            get
            {
                if (monoInjectorInstance == null)
                {
                    monoInjectorInstance = new YuU3dInjector();
                }
                return monoInjectorInstance;
            }
        }


        #region 模块实例缓存

        private Dictionary<Type, object> _modules;
        private Dictionary<Type, object> Modules => _modules ?? (_modules = new Dictionary<Type, object>());

//        private bool IsEditor => YuUnityUtility.IsEditorMode;

        public TModule GetModule<TModule>() where TModule : class, IModule
        {
            var moduleType = typeof(TModule);
            if (Modules.ContainsKey(moduleType))
            {
                return Modules[moduleType] as TModule;
            }

            var module = Get<TModule>();
            Modules.Add(moduleType, module);
            return module;
        }

        #endregion


        private static Dictionary<string, object> pathMapObjs;

        protected static Dictionary<string, object> PathMapObjs =>
            pathMapObjs ?? (pathMapObjs = new Dictionary<string, object>());

        public TAbs Get<TAbs>(string path) where TAbs : class
        {
            if (!string.IsNullOrEmpty(path) && PathMapObjs.ContainsKey(path))
            {
                var instance = PathMapObjs[path];
                return (TAbs)instance;
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("实例获取路径不能为空！");
            }

            var absType = typeof(TAbs);
            var implType = GetImplType(absType);
            var newInstance = (TAbs)Activator.CreateInstance(implType);
            PathMapObjs.Add(path, newInstance);
            return newInstance;
        }

        public TMono GetMono<TMono>(string path = null, GameObject parent = null) where TMono : MonoBehaviour
        {
            var implType = GetImplType(typeof(TMono));
            var finalPath = GetFinalPath(implType, path);
            var result = (TMono) ResloveMono(implType, finalPath, parent);
            return result;
        }

        /// <summary>
        /// 解析一个mono实例。
        /// 优先从单例字典中获取。
        /// </summary>
        /// <param name="implType"></param>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private object ResloveMono(Type implType, string path, GameObject parent)
        {
            object targetMono = null;
            var isSingle = implType.IsSingle();

            if (isSingle && Singles.ContainsKey(implType))
            {
#if UNITY_EDITOR
                targetMono = OnSingleMonoAtEditor(implType, path, parent);
#else
                targetMono = OnSingleMonoAtPlay(implType, path, parent);
#endif
            }

            if (targetMono != null)
            {
                return targetMono;
            }

            if (!string.IsNullOrEmpty(path) && PathMapObjs.ContainsKey(path))
            {
                var mono = PathMapObjs[path];
                return mono;
            }

            targetMono = CreateNewMono(path, implType, isSingle, parent);
            if (isSingle)
            {
                CacheToSingles(implType, targetMono);
            }
            else
            {
                CacheToPathMapObjs(path, targetMono);
            }

            var fieldInfos = GetFieldInfos(implType);
            var propertyInfos = GetPropertyInfos(implType);
            InjectMonoFields(targetMono, fieldInfos);
            InjectMonoPropertys(targetMono, propertyInfos);
            InvokeInit(targetMono);

            return targetMono;
        }

        private void InjectMonoFields(object instance, FieldInfo[] fieldInfos)
        {
            foreach (var fieldInfo in fieldInfos)
            {
                var fieldType = GetFieldOrPropertyType(fieldInfo.FieldType);
                if (fieldType.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    var field = ResloveMono(fieldType, null, null);
                    fieldInfo.SetValue(instance, field);
                }
                else
                {
                    var field = Reslove(fieldType);
                    fieldInfo.SetValue(instance, field);
                }
            }
        }

        private void InjectMonoPropertys(object instance, PropertyInfo[] propertyInfos)
        {
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyType = GetFieldOrPropertyType(propertyInfo.PropertyType);
                if (propertyType.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    var property = ResloveMono(propertyType, null, null);
                    propertyInfo.SetValue(instance, property);
                }
                else
                {
                    var property = Reslove(propertyType);
                    propertyInfo.SetValue(instance, property);
                }
            }
        }

        private object CreateNewMono(string path, Type implType, bool isSingle, GameObject parent)
        {
#if UNITY_EDITOR
            var target = GetMono(path, implType, isSingle, parent);
#else
            var target = CreateMonoByPath(path).AddComponent(implType);
#endif
            return target;
        }

        private void CacheToSingles(Type implType, object target)
        {
            Singles.Add(implType, target);
        }

        private void CacheToPathMapObjs(string path, object target)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("不能使用空路径缓存mono实例！");
            }

            PathMapObjs.Add(path, target);
        }

#if UNITY_EDITOR
        private object OnSingleMonoAtEditor(Type implType, string path, GameObject parent)
        {
            if (!Singles.ContainsKey(implType))
            {
                return null;
            }

            if (Singles[implType] == null)
            {
                Singles.Remove(implType);
                return ResloveMono(implType, path, parent);
            }

            var mono = Singles[implType];
            return mono;
        }
#endif

        private object OnSingleMonoAtPlay(Type implType, string path, GameObject parent)
        {
            if (!Singles.ContainsKey(implType))
            {
                return null;
            }

            var mono = Singles[implType];
            return mono;
        }

        #region mono游戏对象创建

        private static string GetFinalPath(Type type, string path = null)
        {
            string finalPath = null;

            if (path == null)
            {
                var pathAtt = type.GetAttribute<YuMonoPathAttribute>();
                if (pathAtt != null)
                {
                    if (pathAtt.SelfName != null)
                    {
                        finalPath = pathAtt.ParentPath + pathAtt.SelfName;
                    }
                    else
                    {
                        finalPath = pathAtt.ParentPath + type.Name;
                    }
                }
                else
                {
                    finalPath = type.Name;
                }
            }
            else
            {
                finalPath = path;
            }

            return finalPath;
        }

        private static object GetMono(string path, Type type, bool isSingle, GameObject parent)
        {
            var go = CreateMonoByPath(path, parent);
            var mono = go.GetComponent(type);
            if (mono == null)
            {
                mono = go.AddComponent(type);
                return mono;
            }

            if (!isSingle)
            {
                mono = go.AddComponent(type);
            }

            return mono;
        }

        private static GameObject CreateMonoByPath(string path = null, GameObject parent = null)
        {
            if (parent == null)
            {
                var go = YuRoot.AddChild(path);
                return go.gameObject;
            }
            else
            {
                var go = parent.transform.AddChild(path);
                return go.gameObject;
            }
        }

        private const string injectRootId = "GameRoot";

        private static Transform yuRoot;

        private static Transform YuRoot
        {
            get
            {
                if (yuRoot != null)
                {
                    return yuRoot;
                }

                yuRoot = GameObject.Find(injectRootId).transform;
                return yuRoot;
            }
        }

        #endregion
    }
}