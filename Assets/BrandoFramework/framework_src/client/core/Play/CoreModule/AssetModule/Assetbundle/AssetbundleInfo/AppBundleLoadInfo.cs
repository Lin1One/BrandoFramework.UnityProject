 

using System;
using System.Collections.Generic;

 

#region Head

// Author:                liuruoyu1981
// CreateDate:            5/12/2019 3:33:10 PM
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace YuU3dPlay
{
    [Serializable]
    public class AppBundleLoadInfo
    {
        /// <summary>
        /// 所有AssetBundle子包数据列表。
        /// </summary>
        public Dictionary<string, BundleSubLoadInfo> AllSubPackages { get; }
            = new Dictionary<string, BundleSubLoadInfo>();

        #region ID查询

        public string GetBundleId(string assetId)
        {
            foreach (var kv in AllSubPackages)
            {
                var subPackage = kv.Value;
                if (subPackage.ContainAsset(assetId))
                {
                    return subPackage.GetBundleId(assetId);
                }
            }

            return null;
        }

        public string GetAssetSubId(string assetId)
        {
            foreach (var kv in AllSubPackages)
            {
                var subInfo = kv.Value;

                if (subInfo.ContainAsset(assetId))
                {
                    return subInfo.SubPackageId;
                }
            }

            return null;
        }

        public string GetBundleSubId(string bundleId)
        {
            foreach (var kv in AllSubPackages)
            {
                var subInfo = kv.Value;

                if (subInfo.ContaineBundle(bundleId))
                {
                    return subInfo.SubPackageId;
                }
            }

            return null;
        }

        #endregion

        #region 添加映射

        public void AddAssetBundleMap(string subId, string assetId, string bundleId)
        {
            if (!AllSubPackages.ContainsKey(subId))
            {
                AllSubPackages.Add(subId, new BundleSubLoadInfo(subId));
            }

            var subInfo = AllSubPackages[subId];
            subInfo.AddAssetBundleMap(assetId, bundleId);
        }

        public void AddBundleRelativePathMap(string subId, string bundleId, string relativePath)
        {
            if (!AllSubPackages.ContainsKey(subId))
            {
                AllSubPackages.Add(subId, new BundleSubLoadInfo(subId));
            }

            var subInfo = AllSubPackages[subId];
            subInfo.AddBundleRelativePathMap(bundleId, relativePath);
        }

        #endregion
    }
}
