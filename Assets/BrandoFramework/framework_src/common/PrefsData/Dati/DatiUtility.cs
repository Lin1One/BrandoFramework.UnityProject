#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using Common.DataStruct;
using Common.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;


namespace Common.PrefsData
{
    public static class DatiUtility
    {
        /// <summary>
        /// 获取资料文件序列化文件后缀
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSuffix(Type type)
        {
            var suffixAttr = type.GetAttribute<YuDatiSuffixAttribute>() ?? 
                type.BaseType.GetAttribute<YuDatiSuffixAttribute>();
            var suffix = suffixAttr == null ? ".bytes" : suffixAttr.Suffix;
            return suffix;
        }

        /// <summary>
        /// 获取资料文件环境文件类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDatiEnvironmentDirType(Type type)
        {
            var hasInEditorAttr = type.HasAttribute<DatiInEditorAttribute>();
            return hasInEditorAttr ? "Editor" : "Play";
        }

        public static string GetSaveDir(Type type) =>
            Application.dataPath + $"/DatiFile/{GetDatiEnvironmentDirType(type)}/{type.Name}/";

        /// <summary>
        /// 获取单例配置资料的 Asset 文件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSingleScriptObjectPath(Type type)
        {
            var path = $"Assets/DatiFile/{GetDatiEnvironmentDirType(type)}/{type.Name}" +
                       $"/{type.Name}_ScriptObjectAsset.asset";
            return path;
        }

        /// <summary>
        /// 获取单例配置资料的序列化文件（txt，json，byte）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSingleOriginPath(Type type)
        {
            var suffix = GetSuffix(type);
            var path = Application.dataPath + $"/DatiFile/{GetDatiEnvironmentDirType(type)}/" +
                       $"{type.Name}/{type.Name}_OriginAsset{suffix}";
            return path;
        }

        public static string GetMultiOriginPath(Type implType, string id)
        {
            var suffix = GetSuffix(implType);

            if (YuUnityUtility.IsEditorMode)
            {
                var path = Application.dataPath + $"/DatiFile/{GetDatiEnvironmentDirType(implType)}/" +
                           $"{implType.Name}/{id}{suffix}";
                return path;
            }
            else
            {
                var path = $"{id}_{implType.Name}";
                //if (!File.Exists(path))
                //{
                //    path = Application.persistentDataPath + $"/YuDati/{GetLocDirId(implType)}" +
                //           $"{implType.Name}/{id}{suffix}";
                //}

                return path;
            }
        }

        /// <summary>
        /// Dati Assets路径
        /// </summary>
        /// <param name="implType"></param>
        /// <returns></returns>
        public static string GetMultiScriptRootPath(Type implType)
        {
            var path = $"Assets/DatiFile/{GetDatiEnvironmentDirType(implType)}/{implType.Name}/";
            return path;
        }

        public static string GetMultiScriptPath(Type implType, string multiId)
        {
            var path = $"Assets/DatiFile/{GetDatiEnvironmentDirType(implType)}/{implType.Name}" +
                       $"/{multiId}.asset";
            return path;
        }

        public static string GetDatiAssetBundlePath(Type implType)
        {
            var streamingBundleDir = Application.streamingAssetsPath
                + "/AssetBundle/DatiFile/datiplay_"
                + implType.Name.ToLower()
                + ".assetbundle";
            return streamingBundleDir;
        }

        public static string GetDatiAssetPathAtPlay(string datiType,string appId)
        {
            var DatistreamingAssetPath = $"{Application.streamingAssetsPath}/DatiFile/Play/{datiType}/{appId}_{datiType}.txt";
            return DatistreamingAssetPath;
        }


        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void ReflectSetAppId<TActual, TImpl>(GenericDati<TActual, TImpl> dati, string appId)
            where TActual : class, new()
            where TImpl : class
        {
            if (dati.ActualSerializableObject.As<IYuU3dAppId>() == null)
            {
                return;
            }

            var type = dati.ActualSerializableObject.GetType();
            var baseType = type.BaseType;
            const BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var fieldInfo = baseType.GetField("appId", flag);
            fieldInfo.SetValue(dati.ActualSerializableObject, appId);
        }
    }

}

