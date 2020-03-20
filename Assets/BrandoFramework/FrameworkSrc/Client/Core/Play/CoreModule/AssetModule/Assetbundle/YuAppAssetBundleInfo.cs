using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Core
{
    /// <summary>
    /// 应用的AssetBundle数据集合。
    /// </summary>
    [Serializable]
    public class YuAppAssetBundleInfo
    {
        [ReadOnly]
        [LabelText("当前应用ID")] public string LocAppId;

        //资源--资源所在 Ab 映射，以资源首字母作为索引
        public Dictionary<ushort, Dictionary<string, string>> AssetIdToBundleMap
            = new Dictionary<ushort, Dictionary<string, string>>();

        public void AddMap(string assetId, string bundleId)
        {
            var charIndex = (ushort) assetId[0];

            if (!AssetIdToBundleMap.ContainsKey(charIndex))
            {
                AssetIdToBundleMap.Add(charIndex, new Dictionary<string, string>());
            }

            var targetDict = AssetIdToBundleMap[charIndex];
            if (targetDict.ContainsKey(assetId))
            {
                var sourceBundleId = targetDict[assetId];
                if (sourceBundleId == bundleId)
                {
                    return;
                }

                targetDict[assetId] = bundleId;
                Debug.Log($"资源{assetId}所属的AssetBundle发生改变，" +
                          $"原所属AssetBundle为{sourceBundleId}，现所属AssetBundle为{bundleId}！");
            }
            else
            {
                targetDict.Add(assetId, bundleId);
            }
        }

        public string GetBundleId(string assetId)
        {
            var cIndex = (ushort) assetId[0];
            if (!AssetIdToBundleMap.TryGetValue(cIndex, out var targetDict))
            {
                return null;
            }

            if (!targetDict.ContainsKey(assetId))
            {
#if DEBUG
                Debug.LogError($"找不到资源{assetId}所属的BundleID！");
#endif
                return null;
            }
            var bundleId = targetDict[assetId];
            return bundleId;
        }
    }
}