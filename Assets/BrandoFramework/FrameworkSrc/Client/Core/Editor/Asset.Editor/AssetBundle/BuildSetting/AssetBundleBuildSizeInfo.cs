#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com


#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Client.Assets.Editor
{
    /// <summary>
    /// AssetBundle基于尺寸打包方式的分析结果。
    /// </summary>
    [Serializable]
    public class AssetBundleBuildSizeInfo
    {
        /// <summary>
        /// 尺寸分包名和对用文件路径列表的映射字典。
        /// </summary>
        public Dictionary<string, List<string>> BundlePaths
            = new Dictionary<string, List<string>>();

        [LabelText("尺寸分包信息列表")] public List<YuAssetBundleSizeNode> SizeNodes
            = new List<YuAssetBundleSizeNode>();

        public Dictionary<string, string> OutPaths
            = new Dictionary<string, string>();

        public void AppOutFile(string assetId, string path)
        {
            OutPaths.Add(assetId, path);
        }

        [Serializable]
        public class YuAssetBundleSizeNode
        {
            [LabelText("AssetBundle包Id")] public string BundleId;

            [LabelText("资源列表")] public List<string> Paths = new List<string>();
        }

        public void AddSubPacakage(string bundleId, List<string> paths)
        {
            if (BundlePaths.ContainsKey(bundleId))
            {
                throw new Exception($"分包{bundleId}已存在！");
            }

            var newPaths = new List<string>();
            newPaths.AddRange(paths);
            BundlePaths.Add(bundleId, newPaths);
            var node = new YuAssetBundleSizeNode {BundleId = bundleId};
            node.Paths.AddRange(paths);
            SizeNodes.Add(node);
        }

        public int GetAssetTotal()
        {
            var total = 0;

            foreach (var sizeNode in SizeNodes)
            {
                total += sizeNode.Paths.Count;
            }

            return total;
        }
    }
}