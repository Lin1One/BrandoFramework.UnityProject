using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.Utility
{
    /// <summary>
    /// AssetDatabase资源数据库工具。
    /// 1. 自动判断路径类型、归属。
    /// 2. 加载资源。
    /// </summary>
    public static class AssetDatabaseUtility
    {
        #region 字段

        /// <summary>
        /// 加载资源的方法信息。
        /// </summary>
        private static readonly MethodInfo LoadAssetAtPathMethod;

        /// <summary>
        /// 加载所有资源的方法信息。
        /// </summary>
        private static readonly MethodInfo LoadAllAssetsAtPathMethod;

        private static readonly MethodInfo CreateAssetMethod;

        private static readonly MethodInfo RefreshMethod;

        /// <summary>
        /// 静态构造函数。
        /// 静态初始化获取AssetDatabase相关函数的函数信息。
        /// </summary>
        static AssetDatabaseUtility()
        {
            var editorAssembly = Assembly.Load("UnityEditor");
            var assetDatabaseType = editorAssembly.GetType("UnityEditor.AssetDatabase");
            var methods = assetDatabaseType.GetMethods();
            LoadAssetAtPathMethod = methods.ToList().Find(m => m.Name == "LoadAssetAtPath");
            LoadAllAssetsAtPathMethod = methods.ToList().Find(m => m.Name == "LoadAllAssetsAtPath");
            CreateAssetMethod = methods.ToList().Find(m => m.Name == "CreateAsset");
            RefreshMethod = methods.ToList().Find(m => m.Name == "Refresh");
        }

        #endregion

        #region 公开的工具函数

        public static Object LoadAssetAtPath(string path, Type type)
        {
            if (path.Contains(Application.dataPath))
            {
                path = "Assets" + path.Replace(Application.dataPath, "");
            }

            var args = new object[] {path, type};
            var asset = (Object) LoadAssetAtPathMethod.Invoke(null, args);
            return asset;
        }

        public static T LoadAssetAtPath<T>(string path) where T : Object
        {
            if (path.Contains(Application.dataPath))
            {
                path = "Assets" + path.Replace(Application.dataPath, "");
            }


            var args = new object[] {path, typeof(T)};
            var asset = (T) LoadAssetAtPathMethod.Invoke(null, args);
            //var asset2 = AssetDatabase.LoadAssetAtPath<T>(path);
             //asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset;
        }

        public static List<T> LoadAllAssetsAtPath<T>(string path) where T : Object
        {
            var assetsPath = YuUnityIOUtility.GetAssetsPath(path);
            var args = new object[] {assetsPath};
            var objs = (object[]) LoadAllAssetsAtPathMethod.Invoke(null, args);
            var assets = objs.OfType<T>().ToList();
            return assets;
        }

        public static void CreateAsset(Object asset, string path,
            bool isDeletedExist = false)
        {
            var fullPath = YuUnityIOUtility.GetFullPath(path);
            if (File.Exists(fullPath) && isDeletedExist)
            {
                File.Delete(fullPath);
                Refresh();
            }

            IOUtility.EnsureDirExist(fullPath);
            var assetsPath = YuUnityIOUtility.GetAssetsPath(fullPath);
            var arg = new object[] {asset, assetsPath};
            CreateAssetMethod.Invoke(null, arg);
        }

        private static readonly object[] refreshArgs = {null};

        public static void Refresh()
        {
            RefreshMethod.Invoke(null, refreshArgs);
        }

        #endregion
    }
}