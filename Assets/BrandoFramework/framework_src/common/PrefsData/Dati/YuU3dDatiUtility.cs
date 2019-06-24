#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/24 21:52:16
// Email:             35490136@qq.com || liuruoyu1981@gmail.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace YuU3dPlay
{
    public static class YuU3dDatiUtility
    {

        public static string GetSuffix(Type type)
        {
            var suffixAttr = type.GetAttribute<YuDatiSuffixAttribute>() ?? type.BaseType.GetAttribute<YuDatiSuffixAttribute>();

            var suffix = suffixAttr == null ? ".bytes" : suffixAttr.Suffix;
            return suffix;
        }

        public static string GetLocDirId(Type type)
        {
            var hasInEditorAttr = type.HasAttribute<YuDatiInEditorAttribute>();
            return hasInEditorAttr ? "Editor/" : "Play/";
        }

        public static string GetSaveDir(Type type) =>
            Application.dataPath + $"/YuDati/{GetLocDirId(type)}/{type.Name}/";

        public static string GetSingleScriptObjectPath(Type type)
        {
            var path = $"Assets/YuDati/{GetLocDirId(type)}{type.Name}" +
                       $"/{type.Name}_ScriptObjectAsset.asset";
            return path;
        }

        public static string GetSingleOriginPath(Type type)
        {
            var suffix = GetSuffix(type);
            var path = Application.dataPath + $"/YuDati/{GetLocDirId(type)}" +
                       $"{type.Name}/{type.Name}_OriginAsset{suffix}";
            return path;
        }

        public static string GetMultiOriginPath(Type implType, string id)
        {
            var suffix = GetSuffix(implType);

            if (YuUnityUtility.IsEditorMode)
            {
                var path = Application.dataPath + $"/YuDati/{GetLocDirId(implType)}" +
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
            var path = $"Assets/YuDati/{GetLocDirId(implType)}{implType.Name}/";
            return path;
        }

        public static string GetMultiScriptPath(Type implType, string multiId)
        {
            var path = $"Assets/YuDati/{GetLocDirId(implType)}{implType.Name}" +
                       $"/{multiId}.asset";
            return path;
        }

        public static string GetDatiAssetBundlePath(Type implType)
        {
            var streamingBundleDir = Application.streamingAssetsPath
                + "/AssetBundle/YuDati/datiplay_"
                + implType.Name.ToLower()
                + ".assetbundle";
            return streamingBundleDir;
        }

        public static string GetDatiAssetPathAtPlay(string datiType,string appId)
        {
            var DatistreamingAssetPath = $"{Application.streamingAssetsPath}/YuDati/Play/{datiType}/{appId}_{datiType}.txt";
            return DatistreamingAssetPath;
        }


        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static void ReflectSetAppId<TActual, TImpl>(YuAbsU3dGenericDati<TActual, TImpl> dati, string appId)
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

