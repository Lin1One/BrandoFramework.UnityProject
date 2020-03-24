#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Client.Core.Editor;
using Client.Utility;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Assets.Editor
{
    public static class AssetBundleBuilder
    {
        private static readonly BundlePathHelper BundlePathHelper = new BundlePathHelper();

        private const string IGNORE_TPSHEET = ".tpsheet";
        private const string IGNORE_META = ".meta";
        private const string IGNORE_GITKEEP = ".gitkeep";

        private const string ASSETBUNDLE_SUFFIX = "assetbundle";
        private const string ASSETBUNDLE_SHORT_SUFFIX = "ab";

        #region 检测打包配置中是否有相同的BundleId

        #endregion

        #region 清理AssetBundleId

        /// <summary>
        /// 清理应用的AssetBundle Id。
        /// </summary>
        public static void CleanAllAssetBundleId()
        {
            var assetPaths = new List<string>();

            var databasePaths = IOUtility.GetPathsContainSonDir(
                ProjectInfoDati.GetActualInstance().ProjectAssetDatabaseDir);
            assetPaths.AddRange(databasePaths);

            foreach (var assetPath in assetPaths)
            {
                if (!SelectPath(assetPath))
                {
                    continue;
                }

                var assetsPath = UnityIOUtility.GetAssetsPath(assetPath);
                var importer = AssetImporter.GetAtPath(assetsPath);
                if (importer == null)
                {
                    continue;
                }

                importer.assetBundleName = null;
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            Debug.Log("AssetBundle包Id已清空！");
        }
        #endregion

        #region 路径检测及过滤

        private static bool SelectPath(string path)
        {
            return !path.EndsWith(IGNORE_META)
                   && !path.EndsWith(IGNORE_TPSHEET)
                   && !path.Contains(IGNORE_GITKEEP);
        }

        public static bool EndCheck(AssetBundleBuildIgnoreRule rule, string p)
        {
            if (rule == null)
            {
                return true;
            }

            foreach (var s in rule.IgnoreEndSuffixs)
            {
                if (p.EndsWith(s))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainCheck(AssetBundleBuildIgnoreRule rule, string p)
        {
            if (rule == null)
            {
                return true;
            }

            foreach (var s in rule.IgnoreContains)
            {
                if (p.Contains(s))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 目录检测，资源需位于 Assetdatabase 目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsLegalAssetBundleDir(string dir )
        {
            var projectInfo = ProjectInfoDati.GetActualInstance();
            if (!dir.StartsWith(projectInfo.ProjectAssetDatabaseDir))
            {
                return false;
            }

            if (dir == projectInfo.ProjectAssetDatabaseDir)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 打包

        //[MenuItem("Yu/AssetBundle/打包当前应用的AssetBundle &b")]
        public static void BuildAllAssetBundle()
        {
            var currentApp = ProjectInfoDati.GetActualInstance();
            var assetBundleOutPutDir = currentApp.AssetBundleBuildDir;
            IOUtility.EnsureDirExist(assetBundleOutPutDir);
            BuildPipeline.BuildAssetBundles(assetBundleOutPutDir,
                BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            UnityEditorUtility.DisplayTooptx($"应用{currentApp.DevelopProjectName}的AssertBundle打包已完成！");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 设置给定配置下资源的AssetBundle名并选择是否打包。
        /// </summary>
        /// <param name="dirSetting">打包配置</param>
        /// <param name="isBuild">是否在设置完AssetBundle名后进行打包，默认进行打包。</param>
        /// <param name="isRefresh">是否在操作全部完毕后进行刷新，默认不刷新。</param>
        /// <param name="isSaveBundleSetting">是否在设置完AssetBundle名进行全局AssetBundle配置的保存，默认保存。</param>
        public static void SetBundleIdAndSelectIsBuild(AssetBundleBuildSetting dirSetting, 
            bool isBuild = true, 
            bool isRefresh = true, 
            bool isSaveBundleSetting = true)
        {
            switch (dirSetting.BuildType)
            {
                case AssetBundleBuildType.EveryBuild:
                    SetBundleIdANdBuild_AtEveryFile(dirSetting, isBuild);
                    break;
                case AssetBundleBuildType.BuildAtSize:
                    SetBundleIdANdBuild_AtSize(dirSetting, isBuild);
                    break;
                case AssetBundleBuildType.BuildAtDirSelf:
                    SetBundleIdANdBuild_AtDirSelf(dirSetting, isBuild);
                    break;
                case AssetBundleBuildType.BuildAtDirTree:
                    SetBundleIdANdBuild_AtDirTree(dirSetting, isBuild);
                    break;
                case AssetBundleBuildType.BuildAtList:
                    break;
            }

            if (isSaveBundleSetting)
            {
                AssetBundleEditorDati.GetSingleDati().Save();
            }

            if (isRefresh)
            {
                AssetDatabase.Refresh();
            }
        }

        private static void SetBundleIdANdBuild_AtEveryFile(AssetBundleBuildSetting dirSetting, bool isBuild = true)
        {
            var everyPaths = dirSetting.SelfDirPaths();
            var bundleId = IOUtility.GetSomeDirPath(dirSetting.Dir, 4).ToLower();
            SetAssetBundleId(dirSetting, everyPaths, bundleId, true);
            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }


        private static void SetBundleIdANdBuild_AtDirSelf(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
            var dirSelfPaths = dirSetting.SelfDirPaths();

            var selfBundleId = IOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower();
            SetAssetBundleId(dirSetting, dirSelfPaths, selfBundleId);
            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }

        private static void SetBundleIdANdBuild_AtDirTree(AssetBundleBuildSetting dirSetting,bool isBuild = true)
        {
            var paths = dirSetting.GetFullPathsAtDirTree();
            var bundleId = IOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower();
            SetAssetBundleId(dirSetting, paths, null);
            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }

        private static void SetBundleIdANdBuild_AtSize(AssetBundleBuildSetting dirSetting, bool isBuild = true)
        {
            var sizeInfo = dirSetting.GetSizeInfo();
            var appBundleInfo = currentAssetBundleInfo;
            foreach (var kv in sizeInfo.BundlePaths)
            {
                SetAssetBundleIdAtSizeAssetBundle(dirSetting, kv.Value, appBundleInfo, kv.Key);
            }

            foreach (var overSizeFileAssetIdAndPath in sizeInfo.OutPaths)
            {
                var bundleName = string.Format("{0}_{1}",
                    IOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower(), overSizeFileAssetIdAndPath.Value);
                SetAssetBundleIdAtSizeAssetBundle(
                    dirSetting,
                    new List<string> { overSizeFileAssetIdAndPath.Key },
                    appBundleInfo,
                    bundleName);
            }

            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }

        /// <summary>
        /// 设置 Bundle 包名
        /// 设置 AssetId To Bundle 映射信息
        /// </summary>
        /// <param name="dirSetting"></param>
        /// <param name="paths"></param>
        /// <param name="bundleId"></param>
        /// <param name="plusFileName"></param>
        private static void SetAssetBundleId(AssetBundleBuildSetting dirSetting,
            List<string> paths, string bundleId = null, bool plusFileName = false)
        {
            var appBundleInfo = currentAssetBundleInfo;

            var bundleIds = new List<string>();

            foreach (var path in paths)
            {
                var assetId = Path.GetFileNameWithoutExtension(path);
                if (string.IsNullOrEmpty(assetId))
                {
                    Debug.LogError($"路径{path}获取文件名失败！");
                    continue;
                }

                var assetLowerId = assetId.ToLower();
                var finalBundleId = bundleId ?? assetLowerId;
                if (plusFileName)
                {
                    finalBundleId = string.Format("{0}_{1}", finalBundleId, assetLowerId);
                }
                finalBundleId = finalBundleId.ToLower();
                if (!bundleIds.Contains(finalBundleId))
                {
                    bundleIds.Add(finalBundleId);
                }

                appBundleInfo.AddMap(assetLowerId, finalBundleId);
                var assetsPath = UnityIOUtility.GetAssetsPath(path);
                var importer = AssetImporter.GetAtPath(assetsPath);
                if (importer == null)
                {
                    Debug.LogError($"资源{assetsPath}获取导入器失败！");
                    continue;
                }

                dirSetting.UpdateBundleIds(bundleIds);
                importer.assetBundleName = finalBundleId;
                importer.assetBundleVariant = ASSETBUNDLE_SHORT_SUFFIX;
            }
        }


        private static void SetAssetBundleIdAtSizeAssetBundle(AssetBundleBuildSetting dirSetting,
            List<string> paths,
            ProjectAssetsToBundleMapInfo appBundleInfo,
            string bundleId = null)
        {
            var bundleIds = new List<string>();

            foreach (var path in paths)
            {
                var assetId = Path.GetFileNameWithoutExtension(path);
                if (assetId == null)
                {
                    Debug.LogError($"路径{path}获取文件名失败！");
                    continue;
                }

                var assetLowerId = assetId.ToLower();
                var finalBundleId = bundleId ??
                    ($"({IOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower()}_{assetLowerId}");
                finalBundleId = finalBundleId.ToLower();
                if (bundleIds.Contains(finalBundleId))
                {
                    bundleIds.Add(finalBundleId);
                }

                appBundleInfo.AddMap(assetLowerId, finalBundleId);
                var assetsPath = UnityIOUtility.GetAssetsPath(path);
                var importer = AssetImporter.GetAtPath(assetsPath);
                if (importer == null)
                {
                    Debug.LogError($"资源{assetsPath}获取导入器失败！");
                    continue;
                }

                dirSetting.UpdateBundleIds(bundleIds);
                importer.assetBundleName = finalBundleId;
                importer.assetBundleVariant = ASSETBUNDLE_SHORT_SUFFIX;
            }
        }

        #region 移动打包后的AssetBundle文件进行目录分类

        private static void TryMoveAssetBundle(AssetBundleBuildSetting dirSetting,
            List<string> paths)
        {
            switch (dirSetting.BuildType)
            {
                case AssetBundleBuildType.EveryBuild:
                    MoveAssetBundleAtEveryBuild(paths, dirSetting);
                    break;
                case AssetBundleBuildType.BuildAtSize:
                case AssetBundleBuildType.BuildAtList:
                    break;
                case AssetBundleBuildType.BuildAtDirSelf:
                case AssetBundleBuildType.BuildAtDirTree:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void MoveAssetBundleAtEveryBuild(List<string> paths,
            AssetBundleBuildSetting dirSetting)
        {
            var bundleIds = paths.Select(p => Path.GetFileNameWithoutExtension(p)?.ToLower())
                .ToList();
            var locApp = ProjectInfoDati.GetActualInstance();
            var assetBundleOutPaths = IOUtility.GetPathDictionary(locApp.AssetBundleBuildDir, s => !s.EndsWith(".manifest"));
            var targetDirId = IOUtility.GetLastDir(dirSetting.Dir);
            var moveDir = locApp.AssetBundleBuildDir + targetDirId;
            moveDir = moveDir.EnsureDirEnd();
            foreach (var bundleId in bundleIds)
            {
                var sourcePath = assetBundleOutPaths[bundleId];
                var targetPath = moveDir + bundleId + ASSETBUNDLE_SHORT_SUFFIX;
               IOUtility.Move(sourcePath, targetPath);
            }
        }

        #endregion

        private static double buildTotalSecond;

        private static void BuildAssetBundle(AssetBundleBuildSetting dirSetting)
        {
            var projectInfo = ProjectInfoDati.GetActualInstance();
            var startTime = DateTime.Now;
            Debug.Log($"应用{projectInfo.DevelopProjectName}在{DateTime.Now}开始执行AssetBundle打包！");
            var assetBundleOutPutDir = projectInfo.AssetBundleBuildDir;
            IOUtility.EnsureDirExist(assetBundleOutPutDir);
            try
            {
                BuildPipeline.BuildAssetBundles(assetBundleOutPutDir,
                    BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            }
            catch (Exception e)
            {
                Debug.Log($"应用{projectInfo.DevelopProjectName}的AssetBundle打包操作发生异常，" +
                          $"异常信息为{e.Message}！");
                throw;
            }

            var endTime = DateTime.Now;
            var costTimeStamp = (endTime - startTime);
            var minutes = costTimeStamp.Minutes;
            var seconds = costTimeStamp.Seconds;
            buildTotalSecond += costTimeStamp.TotalSeconds;
            Debug.Log($"应用{projectInfo.DevelopProjectName}在{DateTime.Now}结束执行AssetBundle打包！");
            Debug.Log($"应用{projectInfo.DevelopProjectName}的AssetBundle打包操作已完成," +
                      $"耗时为{minutes}分{seconds}秒！");
            Debug.Log($"应用{projectInfo.DevelopProjectName}的AssetBundle打包操作已完成," +
                      $"当前总耗时为{buildTotalSecond}秒！");
        }

        #endregion

        #region AssetBundle数据

        /// <summary>
        /// Asset To Bundle 映射信息数据
        /// </summary>
        public static ProjectAssetsToBundleMapInfo currentAssetBundleInfo;

        public static ProjectAssetsToBundleMapInfo GetAppAssetBundleInfo(string appId)
        {
            if (string.IsNullOrEmpty(appId))
            {
                UnityEditorUtility.DisplayError("应用Id不能为空！");
                throw new Exception("应用Id不能为空！");
            }

            var projectInfo = ProjectInfoDati.GetActualInstance();
            var localPath = projectInfo.AssetBundleBuildDir
                            + $"{projectInfo.DevelopProjectName}_AssetToBundleMapInfo.bytes";
            if (File.Exists(localPath))
            {
                var bytes = File.ReadAllBytes(localPath);
                var appBundleInfo = SerializeUtility.DeSerialize<ProjectAssetsToBundleMapInfo>(bytes);
                return appBundleInfo;
            }

            var newBundleInfo = new ProjectAssetsToBundleMapInfo();
            newBundleInfo.LocAppId = appId;
            return newBundleInfo;
        }

        public static void SaveAppAssetBundleInfo(ProjectAssetsToBundleMapInfo bundleInfo)
        {
            if (string.IsNullOrEmpty(bundleInfo.LocAppId))
            {
                UnityEditorUtility.DisplayError("应用Id不能为空！");
                throw new Exception("应用Id不能为空！");
            }

            var projectInfo = ProjectInfoDati.GetActualInstance();
            var localPath = projectInfo.AssetBundleBuildDir
                            + $"{projectInfo.DevelopProjectName}_AssetToBundleMapInfo.bytes";
            var bytes = SerializeUtility.Serialize(bundleInfo);
            IOUtility.WriteAllBytes(localPath, bytes);
            AssetDatabase.Refresh();
            Debug.Log($"应用{bundleInfo.LocAppId}的AssetBundle数据已更新！");
        }

        public static void SaveCurrentAppAssetBundleInfo()
        {
            if (string.IsNullOrEmpty(currentAssetBundleInfo.LocAppId))
            {
                UnityEditorUtility.DisplayError("应用Id不能为空！");
                throw new Exception("应用Id不能为空！");
            }

            var projectInfo = ProjectInfoDati.GetActualInstance();
            var localPath = projectInfo.AssetBundleBuildDir
                            + $"{projectInfo.DevelopProjectName}_AssetToBundleMapInfo.bytes";
            var bytes = SerializeUtility.Serialize(currentAssetBundleInfo);
            IOUtility.WriteAllBytes(localPath, bytes);
            AssetDatabase.Refresh();
            Debug.Log($"应用{currentAssetBundleInfo.LocAppId}的AssetBundle数据已更新！");
        }

        //[MenuItem("Yu/AssetBundle/更新当前应用的AssetBundle依赖数据")]
        /// <summary>
        /// Bundle To Bundle 依赖数据
        /// </summary>
        private static void CreateAppDependInfoMenu()
        {
            var currentApp = ProjectInfoDati.GetActualInstance();
            var path = BundlePathHelper.GetAppBundleDependInfoPath();
            var bundlePath = currentApp.AssetBundleBuildDir + "AssetBundle";
            var appDependInfo = ProjectBundleDependInfo.Create(bundlePath);
            SerializeUtility.SerializeAndWriteTo(appDependInfo, path);
            Debug.Log($"应用{currentApp.DevelopProjectName}的AssetBundle依赖数据更新完成！");
        }

        #endregion
    }
}