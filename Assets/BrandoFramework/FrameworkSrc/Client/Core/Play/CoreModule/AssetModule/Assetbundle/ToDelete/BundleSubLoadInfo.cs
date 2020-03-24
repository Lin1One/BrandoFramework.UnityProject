 

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

 

#region Head

// Author:                LinYuzhou
// CreateDate:            5/14/2019 12:13:46 PM
// Email:                 836045613@qq.com

#endregion


namespace YuU3dPlay
{
#if UNITY_EDITOR

    [Serializable]
    public class BundleRelativePathInfo
    {
        [LabelText("Bundle名")] public string BundleId;

        [LabelText("相对加载路径")] public string RelativePath;

        public BundleRelativePathInfo(string bundleId, string relativePath)
        {
            BundleId = bundleId;
            RelativePath = relativePath;
        }
    }

    [Serializable]
    public class AssetBundleMapInfo
    {
        [HorizontalGroup("First")] [LabelText("资源名")][LabelWidth(100)]
        public string AssetId;

        [HorizontalGroup("First")] [LabelText("所属Bundle名")][LabelWidth(100)]
        public string BundleId;

        public AssetBundleMapInfo(string assetId, string bundleId)
        {
            AssetId = assetId;
            BundleId = bundleId;
        }
    }

#endif

    [Serializable]
    public class BundleSubLoadInfo
    {
        /// <summary>
        /// 子包ID。
        /// </summary>
        public string SubPackageId { get; }

        /// <summary>
        /// 资源和Bundle文件之间的映射字典。
        /// </summary>
        public Dictionary<string, string> AssetBundleMap { get; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Bundle和路径之间的映射字典。
        /// </summary>
        public Dictionary<string, string> BundleRelativePathMap { get; }
            = new Dictionary<string, string>();

        #region 构造

        public BundleSubLoadInfo()
        {
        }

        public BundleSubLoadInfo(string subId) => SubPackageId = subId;

        #endregion

        #region 可视化支持

#if UNITY_EDITOR

        public void InitVisualSupport()
        {
            bundleRelativePathInfos = new List<BundleRelativePathInfo>();

            foreach (var kv in BundleRelativePathMap)
            {
                var info = new BundleRelativePathInfo(kv.Key, kv.Value);
                bundleRelativePathInfos.Add(info);
            }

            assetBundleMapInfos = new List<AssetBundleMapInfo>();

            foreach (var kv in AssetBundleMap)
            {
                var info = new AssetBundleMapInfo(kv.Key, kv.Value);
                assetBundleMapInfos.Add(info);
            }
        }

        [LabelText("Bundle路径映射数据")] public List<BundleRelativePathInfo> bundleRelativePathInfos;

        [LabelText("资源所属Bundle映射数据")] public List<AssetBundleMapInfo> assetBundleMapInfos;


#endif

        #endregion

        #region 子包bundle名集合

        private HashSet<string> _subBundles;

        private HashSet<string> SubBundles
        {
            get
            {
                if (_subBundles != null && _subBundles.Count > 0)
                {
                    return _subBundles;
                }

                _subBundles = new HashSet<string>();
                var allBundleIDs = AssetBundleMap.Values.ToList();

                foreach (var bundleID in allBundleIDs)
                {
                    if (!_subBundles.Contains(bundleID))
                    {
                        _subBundles.Add(bundleID);
                    }
                }

                return _subBundles;
            }
        }

        #endregion

        #region 映射添加

        public void AddAssetBundleMap(string assetId, string bundleId)
        {
            if (AssetBundleMap.ContainsKey(assetId))
            {
                AssetBundleMap[assetId] = bundleId;
                Debug.Log($"资源{assetId}的映射关系已修改，现在的BundleID是{AssetBundleMap[assetId]}！");
                return;
            }

            AssetBundleMap.Add(assetId, bundleId);
        }

        public void AddBundleRelativePathMap(string bundleId, string relativePath)
        {
            if (BundleRelativePathMap.ContainsKey(bundleId))
            {
                Debug.LogError($"Bundle{bundleId}已存在相对路径映射，" +
                               $"映射为{BundleRelativePathMap[bundleId]}！");
                return;
            }

            BundleRelativePathMap.Add(bundleId, relativePath);
        }

        #endregion

        #region 查询API

        public bool ContaineBundle(string bundleId) => SubBundles.Contains(bundleId);

        public bool ContainAsset(string assetId) => AssetBundleMap.ContainsKey(assetId);

        public string GetBundleId(string assetId) => AssetBundleMap[assetId];

        #endregion
    }
}
