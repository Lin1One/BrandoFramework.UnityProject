#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Client.Assets.Editor
{
    /// <summary>
    /// 应用AssetBundle文件数据查看器。
    /// </summary>
    [Serializable]
    public class YuU3dAppAssetBundleInfo
    {
        [BoxGroup("AssetBundle 依赖关系")]
        [LabelText("AssetBundle名")]
        [InlineButton("FindAssetbundleDepensInfo", "查找依赖ab包")]
        public string assetbundleName;

        [BoxGroup("AssetBundle 依赖关系")]
        [LabelText("AssetBundle 依赖")]
        [TableList]
        public List<assetbundleDependInfo> assetbundleDepends = new List<assetbundleDependInfo>();

        [BoxGroup("AssetBundle 依赖关系")]
        [LabelText("AssetBundle依赖总大小")]
        [TableList]
        public long AllDependsABSize;
        [Serializable]
        public class assetbundleDependInfo
        {
            public string assetbundleName;
            public long assetbundleSize;
        }

        private void FindAssetbundleDepensInfo()
        {
            assetbundleDepends.Clear();
            AllDependsABSize = 0;
            var dependsAbNames = Manifest.GetAllDependencies(assetbundleName);
            foreach (var dependAb in dependsAbNames)
            {
                var abInfo = new assetbundleDependInfo()
                {
                    assetbundleName = dependAb,
                    assetbundleSize = GetAssetbundleSize(dependAb)
                };
                assetbundleDepends.Add(abInfo);
                AllDependsABSize += abInfo.assetbundleSize;
            }
            AllDependsABSize = AllDependsABSize / 1024;

        }

        private long GetAssetbundleSize(string abName)
        {
            long size = 0;
            ////FileInfo abFileInfo = new FileInfo(appAssetBundleDirPath + abName);
            ////if (abFileInfo != null)
            ////{
            ////    size = abFileInfo.Length;
            ////}
            return size;
        }

        [BoxGroup("AssetBundle 资源映射查找")]
        [HideLabel]
        [InlineButton("FindAssetIdInBundle", "查找所属BundleId")]
        public string assetId;

        [BoxGroup("AssetBundle 资源映射查找")]
        [HideLabel]
        [LabelText("所属 BundleId")]
        public string bundleId;

        ////private string appId => YuU3dAppAssetBundleSettingDati.CurrentActual.LocAppId;

        ////private string appAssetBundleDirPath => YuU3dAppSettingDati.TryGetApp(appId).Helper.AssetBundleSandboxDir;

        private AssetBundleManifest manifest;
        private AssetBundleManifest Manifest
        {
            get
            {
                if (manifest == null)
                {
                    //manifest = GetAppBundleManifest(appId);
                }
                return manifest;
            }
        }


        private AssetBundleManifest GetAppBundleManifest(string appId)
        {
            AssetBundle.UnloadAllAssetBundles(true);
            var sandboxPath = //appAssetBundleDirPath +
                              "AssetBundle";
            var bundleManifest = AssetBundle.LoadFromFile(sandboxPath)
                .LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            return bundleManifest;
        }

        ////private YuAppAssetBundleInfo bundleInfo;
        ////private YuAppAssetBundleInfo BundleInfo
        ////{
        ////    get
        ////    {
        ////        if(bundleInfo ==null || bundleInfo.AssetIdBundleMap.Count == 0)
        ////        {
        ////            bundleInfo = GetAppAssetBundleInfo(appId);
        ////        }
        ////        return bundleInfo;
        ////    }
        ////}

        /// <summary>
        /// 查找资源所在 AB 包
        /// </summary>
        private void FindAssetIdInBundle()
        {
            ////var lowerAssetId = assetId.ToLower();
            ////if (BundleInfo.AssetIdBundleMap.ContainsKey(assetId[0]))
            ////{
            ////    BundleInfo.AssetIdBundleMap[assetId[0]].TryGetValue(lowerAssetId, out bundleId);
            ////}
            ////else
            ////{
            ////    bundleId = "";
            ////}
        }

        ////private  YuAppAssetBundleInfo GetAppAssetBundleInfo(string appId)
        ////{
        ////    if (string.IsNullOrEmpty(appId))
        ////    {
        ////        YuEditorUtility.DisplayError("应用Id不能为空！");
        ////        throw new Exception("应用Id不能为空！");
        ////    }

        ////    var app = YuU3dAppSettingDati.TryGetApp(appId);
        ////    var appHelper = app.Helper;
        ////    var localPath = appHelper.SandboxHotUpdateDir
        ////                    + $"{app.LocAppId}_AssetBundleInfo.bytes";
        ////    if (File.Exists(localPath))
        ////    {
        ////        var bytes = File.ReadAllBytes(localPath);
        ////        var appBundleInfo = YuSerializeUtility.DeSerialize<YuAppAssetBundleInfo>(bytes);
        ////        return appBundleInfo;
        ////    }

        ////    var newBundleInfo = new YuAppAssetBundleInfo();
        ////    newBundleInfo.LocAppId = appId;
        ////    return newBundleInfo;
        ////}

        private void LoadAssetBundleManifest()
        {

        }

    }

}

