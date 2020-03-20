#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com


#endregion

using Common;
using Common.Editor;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Client.Assets.Editor
{
    /// <summary>
    /// AssetBundle打包配置。
    /// 一个配置对应一个资源目录。
    /// </summary>
    [Serializable]
    public class AssetBundleBuildSetting
    {
        [LabelText("当前目录")]
        [ReadOnly]
        public string Dir;

        [LabelText("打包方式")]
        public AssetBundleBuildType BuildType;

        [BoxGroup("打包忽略规则")]
        [HideLabel]
        public AssetBundleBuildIgnoreRule IgnoreRule;

        #region 尺寸分包自动分析结果可视化

        [HideLabel]
        [ShowIf("CheckIsBuildAtSize")]
        public AssetBundleBuildAtSizeSetting AtSizeSetting;

        #endregion

        #region 列表打包配置

        [LabelText("配置节点")]
        [ShowIf("CheckIsBuildAtList")]
        public List<AssetBundleBuildBaseNode> BundleNodes
            = new List<AssetBundleBuildBaseNode>();

        /// <summary>
        /// 该配置下资源使用的AssetBundle名列表。
        /// </summary>
        [LabelText("配置下资源的AssetBundle名列表")]
        [ShowIf("CheckIsBuildAtList")]
        [ReadOnly]
        public List<string> BundleIds = new List<string>();

        #endregion

        private bool CheckIsBuildAtSize() => BuildType == AssetBundleBuildType.BuildAtSize;
        private bool CheckIsBuildAtList() => BuildType == AssetBundleBuildType.BuildAtList;

        #region 方法

        [Button("分析尺寸打包", ButtonSizes.Medium)]
        [HorizontalGroup("第二排按钮")]
        [ShowIf("CheckIsBuildAtSize")]
        public AssetBundleBuildSizeInfo GetSizeInfo()
        {
            if (BuildType != AssetBundleBuildType.BuildAtSize)
            {
                throw new Exception($"目录{Dir}的打包方式不是尺寸打包！");
            }

            var dirId = IOUtility.GetSomeDirPath(Dir, 3);
            AtSizeSetting.SizeInfo = new AssetBundleBuildSizeInfo();
            var fileInfos = IOUtility.GetFileInfosAtDir(Dir,true)
                .Where(d => YuAssetBundleUtility.EndCheck(IgnoreRule, d.FullName))
                .Where(d => YuAssetBundleUtility.ContainCheck(IgnoreRule, d.FullName)).ToList();
            long maxSize = AtSizeSetting.PackageSize * 1024;
            long currentSize = 0;
            var bundleIndex = 0;
            var paths = new List<string>();

            foreach (var fileInfo in fileInfos)
            {
                var path = fileInfo.FullName;
                path = path.ReplaceDoubleBackslash();
                currentSize += fileInfo.Length;
                if (fileInfo.Length >= maxSize)
                {
                    var assetId = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    AtSizeSetting.SizeInfo.AppOutFile(path,assetId);
                    Debug.Log($"{fileInfo.Name} 文件大小大于尺寸分包的最大尺寸");
                    continue;
                }

                if (currentSize <= maxSize)
                {
                    paths.Add(path);
                }
                else
                {
                    AddSubPackage();
                    paths.Add(path);
                }
            }

            AddSubPackage(); // 添加最后一个子包尺寸小于分包限制的剩余资源
            AtSizeSetting.SetSizeTotal(AtSizeSetting.SizeInfo.GetAssetTotal());
            ////YuU3dAppAssetBundleSettingDati.GetMultiAtId(LocAppId).Save();
            return AtSizeSetting.SizeInfo;

            void AddSubPackage()
            {
                var subBundleId = $"{dirId}_{bundleIndex}".ToLower();
                AtSizeSetting.SizeInfo.AddSubPacakage(subBundleId, paths);
                bundleIndex++;
                currentSize = 0;
                paths.Clear();
            }
        }

        public List<string> GetFileIds()
        {
            var paths = IOUtility.GetPaths(Dir);
            var ignoreRule = IgnoreRule;
            paths = paths.Where(p => YuAssetBundleUtility.EndCheck(IgnoreRule, p))
                .Where(p => YuAssetBundleUtility.ContainCheck(IgnoreRule, p)).ToList();
            var fileIds = paths.Select(Path.GetFileNameWithoutExtension).ToList();

            return fileIds;
        }

        public void UpdateBundleIds(List<string> bundleIds)
        {
            BundleIds.Clear();
            BundleIds.AddRange(bundleIds);
        }

        //private YuU3dAppSetting locU3DApp;

        //public YuU3dAppSetting LocU3DApp
        //{
        //    get
        //    {
        //        if (locU3DApp != null && !string.IsNullOrEmpty(locU3DApp.LocAppId))
        //        {
        //            return locU3DApp;
        //        }

        //        locU3DApp = YuEditorUtility.TryGetLocAppAtDir(Dir);
        //        return locU3DApp;
        //    }
        //}

        /// <summary>
        /// 获得自身目录下所有符合过滤配置的文件路径列表。
        /// </summary>
        /// <returns></returns>
        public List<string> SelfDirPaths()
        {
            var paths = IOUtility.GetPaths(Dir);
            paths = paths.Where(p => YuAssetBundleUtility.EndCheck(IgnoreRule, p))
                .Where(p => YuAssetBundleUtility.ContainCheck(IgnoreRule, p)).ToList();
            return paths;
        }

        public List<string> GetFullPathsAtDirTree()
        {
            ////var paths = YuIOUtility.GetPathDictionaryWithOutMeta(Dir).Values.ToList();
            ////var ignoreRule = YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(LocAppId).IgnoreRule;
            ////paths = paths
            ////    .Where(p => YuAssetBundleUtility.EndCheck(ignoreRule, p))
            ////    .Where(p => YuAssetBundleUtility.ContainCheck(ignoreRule, p)).ToList();
            ////return paths;
            return null;
        }

        [Button("添加一个列表配置", ButtonSizes.Medium)]
        [HorizontalGroup("第二排按钮")]
        [ShowIf("CheckIsBuildAtList")]
        private void AddNode()
        {
            if (BuildType != AssetBundleBuildType.BuildAtList)
            {
                Unity3DEditorUtility.DisplayError("当前目录的打包方式不是BuildAtList,请先修改打包方式！");
                return;
            }

            var fileIds = GetFileIds();

            foreach (var node in BundleNodes)
            {
                foreach (var file in node.FileIds)
                {
                    fileIds.Remove(file);
                }
            }

            AssetBundleBuildBaseNode newBaseNode = null;
            var dirId = IOUtility.GetLastDir(Dir);
            var newBundleId = dirId + "_" + BundleNodes.Count;
            if (BundleNodes.Find(n => n.BundleId == newBundleId) != null)
            {
                Unity3DEditorUtility.DisplayError($"目标Bundle名已存在！");
                newBaseNode = new AssetBundleBuildBaseNode();
            }
            else
            {
                newBaseNode = new AssetBundleBuildBaseNode {BundleId = newBundleId};
            }

            newBaseNode.AddFiles(fileIds);
            BundleNodes.Add(newBaseNode);
        }

        [Button("清理所有BundleId并打包该目录", ButtonSizes.Medium)]
        [HorizontalGroup("第一排按钮")]
        private void CleanAllBundleIdAndBuildThisDir()
        {
            YuAssetBundleUtility.CleanAllAssetBundleId();
            BuildThisDirAndNotClean();
        }

        [Button("打包该目录(不清理)", ButtonSizes.Medium)]
        [HorizontalGroup("第一排按钮")]
        private void BuildThisDirAndNotClean()
        {
            YuAssetBundleUtility.SetBundleIdAndSelectIsBuild(this);
        }

        #endregion
    }
}