using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    [Serializable]
    public class ProjectBundleDependInfo
    {
        #region 可视化支持

#if UNITY_EDITOR

        [LabelText("Bundle依赖关系数据")] [ReadOnly] public List<BundleDependInfoVisual> DependInfoVisuals;

        public void InitVisualSupport()
        {
            DependInfoVisuals = new List<BundleDependInfoVisual>();

            foreach (var kv in AllDependInfos)
            {
                var dependVisual = new BundleDependInfoVisual(kv.Value);
                DependInfoVisuals.Add(dependVisual);
            }
        }

#endif

        #endregion

        #region 基础

        public Dictionary<string, BundleDependInfo> AllDependInfos { get; }
            = new Dictionary<string, BundleDependInfo>();

        public static ProjectBundleDependInfo Create(string manifestPath)
        {
            var bundle = AssetBundle.LoadFromFile(manifestPath);
            var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var appDependInfo = new ProjectBundleDependInfo();
            var allBundleIds = manifest.GetAllAssetBundles();

            foreach (var bundleId in allBundleIds)
            {
                var directDepends = manifest.GetDirectDependencies(bundleId);
                var dependInfo = new BundleDependInfo(bundleId, directDepends);
                appDependInfo.AllDependInfos.Add(bundleId, dependInfo);
            }

            bundle.Unload(true);
            return appDependInfo;
        }

        public BundleDependInfo GetDependInfo(string bundleId)
        {
            if (AllDependInfos.ContainsKey(bundleId))
            {
#if DEBUG
                if(AllDependInfos[bundleId] == null)
                {
                    Debug.LogError($"Bundle:  {bundleId}找不到依赖数据！");
                }
#endif
                return AllDependInfos[bundleId];
            }

#if DEBUG
            Debug.LogError($"Bundle:  {bundleId}找不到依赖数据！");
#endif
            return null;
        }

        #endregion
    }
}