#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com


#endregion

using Common.DataStruct;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Client.Assets.Editor
{
    [Serializable]
    public enum YuAssetBundleAutoBuildType
    {
        /// <summary>
        /// 目录下的每个资源单独打包。
        /// </summary>
        EveryBuild,

        /// <summary>
        /// 目录下的资源依据给定的包大小打包。
        /// 解释：将目录下的资源按照默认的文件排序，依次取出放入当前的bunle包中，
        /// 如果包大小未超出则继续去除文件放入，若超出则中止放入该文件。
        /// 将当前包中的文件修改为统一的bundle名。
        /// 进行下一个bundle包设置。
        /// </summary>
        BuildAtSize,

        /// <summary>
        /// 目录下的所有资源打成一个包。
        /// 不包含所有子目录。
        /// </summary>
        BuildAtDirSelf,

        /// <summary>
        /// 目录及所有子目录下的所有资源打成一个包。
        /// 使用该方式打包前会自动清理掉所有子目录可能存在的打包配置。
        /// </summary>
        BuildAtDirTree,

        /// <summary>
        /// 基于包文件配置列表打包。
        /// </summary>
        BuildAtList,
    }

    /// <summary>
    /// AssetBundle基于尺寸打包方式的分析结果。
    /// </summary>
    [Serializable]
    public class YuAssetBundleSizeInfo
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

    /// <summary>
    /// AssetBundle打包配置。
    /// 一个配置对应一个资源目录。
    /// 配置分成自动打包和手动打包两种。
    /// </summary>
    [Serializable]
    public class YuAssetBundleDirSetting
    {
        [HideInInspector]
        public string LocAppId;

        [LabelText("配置对应目录")] [ReadOnly] public string Dir;

        [LabelText("打包方式")] public YuAssetBundleAutoBuildType BuildType;

        #region 尺寸分包自动分析结果可视化

        [HideLabel] [ShowIf("CheckIsBuildAtSize")]
        public YuAssetBundleBuildAtSizeSetting AtSizeSetting;

        #endregion

        private bool CheckIsBuildAtSize() => BuildType == YuAssetBundleAutoBuildType.BuildAtSize;
        private bool CheckIsBuildAtList() => BuildType == YuAssetBundleAutoBuildType.BuildAtList;

        [LabelText("配置节点")] [ShowIf("CheckIsBuildAtList")]
        public List<YuAssetBundleBaseNode> BundleNodes
            = new List<YuAssetBundleBaseNode>();

        /// <summary>
        /// 该配置下资源使用的AssetBundle名列表。
        /// </summary>
        [LabelText("配置下资源的AssetBundle名列表")] [ReadOnly]
        public List<string> BundleIds = new List<string>();

        public void UpdateBundleIds(List<string> bundleIds)
        {
            BundleIds.Clear();
            BundleIds.AddRange(bundleIds);
        }


        #region 方法

        [GUIColor(1f, 0.57f, 0f)]
        [Button("分析尺寸打包", ButtonSizes.Medium)]
        [HorizontalGroup("第一排按钮")]
        public YuAssetBundleSizeInfo GetSizeInfo()
        {
            if (BuildType != YuAssetBundleAutoBuildType.BuildAtSize)
            {
                throw new Exception($"目录{Dir}的打包方式不是尺寸打包！");
            }

            var dirId = YuIOUtility.GetSomeDirPath(Dir, 3);
            AtSizeSetting.SizeInfo = new YuAssetBundleSizeInfo();
            var fileInfos = YuIOUtility.GetAllFileInfosAtDir(Dir)
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

        private YuAssetBundleIgnoreRule IgnoreRule = null;
            ////YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(LocAppId).IgnoreRule;

        public List<string> GetFileIds()
        {
            var paths = YuIOUtility.GetPaths(Dir);
            var ignoreRule = IgnoreRule;
            paths = paths.Where(p => YuAssetBundleUtility.EndCheck(ignoreRule, p))
                .Where(p => YuAssetBundleUtility.ContainCheck(ignoreRule, p)).ToList();
            var fileIds = paths.Select(Path.GetFileNameWithoutExtension).ToList();

            return fileIds;
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
            var paths = YuIOUtility.GetPaths(Dir);
            ////var ignoreRule = YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(LocAppId).IgnoreRule;
            ////paths = paths.Where(p => YuAssetBundleUtility.EndCheck(ignoreRule, p))
            ////    .Where(p => YuAssetBundleUtility.ContainCheck(ignoreRule, p)).ToList();

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

        [GUIColor(1f, 0.57f, 0f)]
        [Button("添加一个列表配置", ButtonSizes.Medium)]
        [HorizontalGroup("第一排按钮")]
        private void AddNode()
        {
            if (BuildType != YuAssetBundleAutoBuildType.BuildAtList)
            {
                //YuEditorUtility.DisplayError("当前目录的打包方式不是BuildAtList,请先修改打包方式！");
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

            YuAssetBundleBaseNode newBaseNode = null;
            var dirId = YuIOUtility.GetLastDir(Dir);
            var newBundleId = dirId + "_" + BundleNodes.Count;
            if (BundleNodes.Find(n => n.BundleId == newBundleId) != null)
            {
                //YuEditorUtility.DisplayError($"目标Bundle名已存在！");
                newBaseNode = new YuAssetBundleBaseNode();
            }
            else
            {
                newBaseNode = new YuAssetBundleBaseNode {BundleId = newBundleId};
            }

            newBaseNode.AddFiles(fileIds);
            BundleNodes.Add(newBaseNode);
        }

        [GUIColor(1f, 0.57f, 0f)]
        [Button("清理所有BundleId并打包该目录", ButtonSizes.Medium)]
        [HorizontalGroup("第二排按钮")]
        private void CleanAllBundleIdAndBuildThisDir()
        {
            YuAssetBundleUtility.CleanAllAssetBundleId();
            BuildThisDirAndNotClean();
        }

        [GUIColor(1f, 0.57f, 0f)]
        [Button("打包该目录(不清理)", ButtonSizes.Medium)]
        [HorizontalGroup("第二排按钮")]
        private void BuildThisDirAndNotClean()
        {
            YuAssetBundleUtility.SetBundleIdAndSelectIsBuild(this);
        }

        #endregion
    }
}