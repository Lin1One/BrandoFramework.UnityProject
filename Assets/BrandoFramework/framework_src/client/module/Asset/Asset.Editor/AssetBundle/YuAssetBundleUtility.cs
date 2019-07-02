#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com

#endregion

using Client.Assets.Editor;
using Common;
using Common.Config;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Assets
{
    public interface IBundlePathHelper
    {
    }

    [Singleton]
    //[DefaultInjecType(typeof(IBundlePathHelper))]
    public class BundlePathHelper : IBundlePathHelper
    {
        public BundlePathHelper(ProjectInfo appSetting) => _appSetting = appSetting;

        public BundlePathHelper()
        {
        }

        private readonly ProjectInfo _appSetting = ProjectInfoDati.GetActualInstance();

        ////public string GetAppBundleLoadInfoPath()
        ////{
        ////    var path = _appSetting.Helper.FinalSandboxDir + $"{typeof(AppBundleLoadInfo).Name}.bytes";
        ////    return path;
        ////}

        ////public string GetAppBundleDependInfoPath()
        ////{
        ////    var path = _appSetting.Helper.LocalHttpRootDir + $"{typeof(AppBundleDependInfo).Name}.bytes";
        ////    return path;
        ////}
    }

    public static class YuAssetBundleUtility
    {
        //private static readonly BundlePathHelper BundlePathHelper = new BundlePathHelper(U3dGlobal.CurrentApp);

        #region 清理AssetBundleId

        /// <summary>
        /// 清理所有应用的AssetBundle包Id。
        /// </summary>
        public static void CleanAllAssetBundleId()
        {
            ////var appSettings = YuU3dAppSettingDati.AllInstance;

            ////foreach (var appSetting in appSettings)
            ////{
            ////    CleanTargetAppAssetBundleId(appSetting.ActualSerializableObject);
            ////}

            AssetDatabase.RemoveUnusedAssetBundleNames();
            Debug.Log("所有应用的AssetBundle包Id已清空！");
        }

        ////private static void CleanTargetAppAssetBundleId(YuU3dAppSetting targetU3DApp)
        ////{
        ////    var assetPaths = new List<string>();
        ////    var streamPaths = YuIOUtility.GetPathsContainSonDir(targetU3DApp.Helper.StreamingAssetsDir);
        ////    assetPaths.AddRange(streamPaths);
        ////    var databasePaths = YuIOUtility.GetPathsContainSonDir(targetU3DApp.Helper.AssetDatabaseDir);
        ////    assetPaths.AddRange(databasePaths);

        ////    foreach (var assetPath in assetPaths)
        ////    {
        ////        if (!SelectPath(assetPath))
        ////        {
        ////            continue;
        ////        }

        ////        var assetsPath = YuUnityIOUtility.GetAssetsPath(assetPath);
        ////        var importer = AssetImporter.GetAtPath(assetsPath);
        ////        if (importer == null)
        ////        {
        ////            continue;
        ////        }

        ////        importer.assetBundleName = null;
        ////    }

        ////    Debug.Log($"应用{targetU3DApp.LocAppId}的AssetBundle名已全部清空！");
        ////}

        #endregion

        #region 检测打包配置中是否有相同的BundleId

        #endregion

        #region 忽略后缀常量

        private const string IGNORE_TPSHEET = ".tpsheet";
        private const string IGNORE_META = ".meta";
        private const string IGNORE_GITKEEP = ".gitkeep";

        #endregion

        #region 其他常量

        private const string ASSETBUNDLE_SUFFIX = "assetbundle";
        private const string ASSETBUNDLE_SHORT_SUFFIX = "ab";

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

        public static bool IsLegalAssetBundleDir(string dir)////, YuU3dAppSetting u3DAppSetting)
        {
            ////var helper = u3DAppSetting.Helper;
            ////if (!dir.StartsWith(helper.AssetDatabaseDir_Assets) &&
            ////    !dir.StartsWith(helper.SubPackageAssetDir_Assets) &&
            ////    !dir.StartsWith(helper.StreamingAssetsDir_Assets))
            ////{
            ////    return false;
            ////}

            ////if (dir == helper.AssetDatabaseDir_Assets ||
            ////    dir == helper.SubPackageAssetDir_Assets ||
            ////    dir == helper.StreamingAssetsDir_Assets)
            ////{
            ////    return false;
            ////}

            return true;
        }

        #endregion

        #region 打包

        [MenuItem("Yu/AssetBundle/打包当前应用的AssetBundle &b")]
        public static void BuildAllAssetBundle()
        {
            ////var currentApp = YuU3dAppSettingDati.CurrentActual;
            ////var assetBundleOutPutDir = currentApp.Helper.AssetBundleBuildDir;
            ////YuIOUtility.EnsureDirExist(assetBundleOutPutDir);
            ////BuildPipeline.BuildAssetBundles(assetBundleOutPutDir,
            ////    BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            ////YuEditorUtility.DisplayTooptx($"应用{currentApp.LocAppId}的AssertBundle打包已完成！");
            ////AssetDatabase.Refresh();
        }

        ////[MenuItem("Yu/AssetBundle/更新当前应用的AssetBundle依赖数据")]
        ////private static void CreateAppDependInfoMenu()
        ////{
        ////    var currentApp = YuU3dAppSettingDati.CurrentActual;
        ////    CreateAppDependInfo(currentApp);
        ////}

        ////private static void CreateAppDependInfo(YuU3dAppSetting appSetting)
        ////{
        ////    var path = BundlePathHelper.GetAppBundleDependInfoPath();
        ////    var bundlePath = appSetting.Helper.AssetBundleBuildDir + "AssetBundle";
        ////    var appDependInfo = AppBundleDependInfo.Create(bundlePath);
        ////    YuSerializeUtility.SerializeAndWriteTo(appDependInfo, path);
        ////    Debug.Log($"应用{appSetting.LocAppId}的AssetBundle依赖数据更新完成！");
        ////}

        /// <summary>
        /// 设置给定配置下资源的AssetBundle名并选择是否打包。
        /// </summary>
        /// <param name="dirSetting">打包配置</param>
        /// <param name="isBuild">是否在设置完AssetBundle名后进行打包，默认进行打包。</param>
        /// <param name="isRefresh">是否在操作全部完毕后进行刷新，默认不刷新。</param>
        /// <param name="isSaveBundleSetting">是否在设置完AssetBundle名进行全局AssetBundle配置的保存，默认保存。</param>
        public static void SetBundleIdAndSelectIsBuild(AssetBundleBuildSetting dirSetting
            , bool isBuild = true, bool isRefresh = true, bool isSaveBundleSetting = true)
        {
            switch (dirSetting.BuildType)
            {
                case AssetBundleBuildType.EveryBuild:
                    SetBundleIdANdBuild_AtEvery(dirSetting, isBuild);
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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (isSaveBundleSetting)
            {
                //YuU3dAppAssetBundleSettingDati.GetMultiAtId(dirSetting.LocAppId).Save();
            }

            if (isRefresh)
            {
                AssetDatabase.Refresh();
            }
        }

        #region 分类设置Id及打包实现

        private static void SetBundleIdANdBuild_AtEvery(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
            var everyPaths = dirSetting.SelfDirPaths();
            var bundleId = IOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower();
            SetAssetBundleId(dirSetting, everyPaths, bundleId, true);
            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }

        private static void SetBundleIdANdBuild_AtSize(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
            var sizeInfo = dirSetting.GetSizeInfo();
            ////var appBundleInfo = currentAssetBundleInfo;
            ////foreach (var kv in sizeInfo.BundlePaths)
            ////{
            ////    SetAssetBundleIdAtSizeAssetBundle(dirSetting, kv.Value, appBundleInfo, kv.Key);
            ////}

            ////foreach (var overSizeFileAssetIdAndPath in sizeInfo.OutPaths)
            ////{
            ////    var bundleName = string.Format("{0}_{1}",
            ////        YuIOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower(), overSizeFileAssetIdAndPath.Value);
            ////    SetAssetBundleIdAtSizeAssetBundle(
            ////        dirSetting, 
            ////        new List<string>{overSizeFileAssetIdAndPath.Key}, 
            ////        appBundleInfo,
            ////        bundleName);
            ////}

            ////if (isBuild)
            ////{
            ////    BuildAssetBundle(dirSetting);
            ////}
        }

        private static void SetBundleIdANdBuild_AtDirSelf(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
            //var dirSelfPaths = dirSetting.SelfDirPaths();

            //var selfBundleId = YuIOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower();
            //SetAssetBundleId(dirSetting, dirSelfPaths, selfBundleId);
            //if (isBuild)
            //{
            //    BuildAssetBundle(dirSetting);
            //}
        }

        private static void SetBundleIdANdBuild_AtDirTree(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
            ////var paths = dirSetting.GetFullPathsAtDirTree();
            //var bundleId = YuIOUtility.GetSomeDirPath(dirSetting.Dir, 3).ToLower();
            //SetAssetBundleId(dirSetting, paths, null);
            if (isBuild)
            {
                BuildAssetBundle(dirSetting);
            }
        }

        private static void SetBundleIdANdBuild_AtList(AssetBundleBuildSetting dirSetting,
            bool isBuild = true)
        {
        }

        #endregion

        private static void SetAssetBundleId(AssetBundleBuildSetting dirSetting,
            List<string> paths, string bundleId = null, bool plusFileName = false)
        {
            ////var appBundleInfo = currentAssetBundleInfo;

            ////var bundleIds = new List<string>();

            ////foreach (var path in paths)
            ////{
            ////    var assetId = Path.GetFileNameWithoutExtension(path);
            ////    if (string.IsNullOrEmpty(assetId))
            ////    {
            ////        Debug.LogError($"路径{path}获取文件名失败！");
            ////        continue;
            ////    }

            ////    var assetLowerId = assetId.ToLower();
            ////    var finalBundleId = bundleId ?? assetLowerId;
            ////    if (plusFileName)
            ////    {
            ////        finalBundleId = string.Format("{0}_{1}", finalBundleId, assetLowerId);
            ////    }
            ////    finalBundleId = finalBundleId.ToLower();
            ////    if (!bundleIds.Contains(finalBundleId))
            ////    {
            ////        bundleIds.Add(finalBundleId);
            ////    }

            ////    appBundleInfo.AddMap(assetLowerId, finalBundleId);
            ////    var assetsPath = YuUnityIOUtility.GetAssetsPath(path);
            ////    var importer = AssetImporter.GetAtPath(assetsPath);
            ////    if (importer == null)
            ////    {
            ////        Debug.LogError($"资源{assetsPath}获取导入器失败！");
            ////        continue;
            ////    }

            ////    dirSetting.UpdateBundleIds(bundleIds);
            ////    importer.assetBundleName = finalBundleId;
            ////    importer.assetBundleVariant = ASSETBUNDLE_SHORT_SUFFIX;
            ////}
        }


        private static void SetAssetBundleIdAtSizeAssetBundle(AssetBundleBuildSetting dirSetting,
            List<string> paths,
           //// YuAppAssetBundleInfo appBundleInfo,
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

                ////appBundleInfo.AddMap(assetLowerId, finalBundleId);
                var assetsPath = YuUnityIOUtility.GetAssetsPath(path);
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
                    //MoveAssetBundleAtEveryBuild(paths, dirSetting);
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
            ////var bundleIds = paths.Select(p => Path.GetFileNameWithoutExtension(p)?.ToLower())
            ////    .ToList();
            ////var locApp = dirSetting.LocU3DApp;
            ////var assetBundleOutPaths = YuIOUtility.GetPathDictionary(locApp.Helper
            ////    .AssetBundleBuildDir, s => !s.EndsWith(".manifest"));
            ////var targetDirId = YuIOUtility.GetLastDir(dirSetting.Dir);
            ////var moveDir = locApp.Helper.AssetBundleBuildDir + targetDirId;
            ////moveDir = moveDir.EnsureDirEnd();
            ////foreach (var bundleId in bundleIds)
            ////{
            ////    var sourcePath = assetBundleOutPaths[bundleId];
            ////    var targetPath = moveDir + bundleId + ASSETBUNDLE_SHORT_SUFFIX;
            ////    YuIOUtility.Move(sourcePath, targetPath);
            ////}
        }

        #endregion

        private static double buildTotalSecond;

        private static void BuildAssetBundle(AssetBundleBuildSetting dirSetting)
        {
            ////var appSetting = dirSetting.LocU3DApp;
            ////var startTime = DateTime.Now;
            ////Debug.Log($"应用{appSetting.LocAppId}在{DateTime.Now}开始执行AssetBundle打包！");
            ////var assetBundleOutPutDir = appSetting.Helper.AssetBundleBuildDir;
            ////YuIOUtility.EnsureDirExist(assetBundleOutPutDir);
            ////try
            ////{
            ////    BuildPipeline.BuildAssetBundles(assetBundleOutPutDir,
            ////        BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            ////}
            ////catch (Exception e)
            ////{
            ////    Debug.Log($"应用{appSetting.LocAppId}的AssetBundle打包操作发生异常，" +
            ////              $"异常信息为{e.Message}！");
            ////    throw;
            ////}

            ////var endTime = DateTime.Now;
            ////var costTimeStamp = (endTime - startTime);
            ////var minutes = costTimeStamp.Minutes;
            ////var seconds = costTimeStamp.Seconds;
            ////buildTotalSecond += costTimeStamp.TotalSeconds;
            ////Debug.Log($"应用{appSetting.LocAppId}在{DateTime.Now}结束执行AssetBundle打包！");
            ////Debug.Log($"应用{appSetting.LocAppId}的AssetBundle打包操作已完成," +
            ////          $"耗时为{minutes}分{seconds}秒！");
            ////Debug.Log($"应用{appSetting.LocAppId}的AssetBundle打包操作已完成," +
            ////          $"当前总耗时为{buildTotalSecond}秒！");
        }

        #endregion

        #region AssetBundle数据

        ////public static YuAppAssetBundleInfo currentAssetBundleInfo;

        ////public static YuAppAssetBundleInfo GetAppAssetBundleInfo(string appId)
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

        ////public static void SaveAppAssetBundleInfo(YuAppAssetBundleInfo bundleInfo)
        ////{
        ////    if (string.IsNullOrEmpty(bundleInfo.LocAppId))
        ////    {
        ////        YuEditorUtility.DisplayError("应用Id不能为空！");
        ////        throw new Exception("应用Id不能为空！");
        ////    }

        ////    var app = YuU3dAppSettingDati.TryGetApp(bundleInfo.LocAppId);
        ////    var appHelper = app.Helper;
        ////    var localPath = appHelper.LocalHttpRootDir
        ////                    + $"{app.LocAppId}_AssetBundleInfo.bytes";
        ////    var bytes = YuSerializeUtility.Serialize(bundleInfo);
        ////    YuIOUtility.WriteAllBytes(localPath, bytes);
        ////    AssetDatabase.Refresh();
        ////    Debug.Log($"应用{bundleInfo.LocAppId}的AssetBundle数据已更新！");
        ////}

        ////public static void SaveCurrentAppAssetBundleInfo()
        ////{
        ////    if (string.IsNullOrEmpty(currentAssetBundleInfo.LocAppId))
        ////    {
        ////        YuEditorUtility.DisplayError("应用Id不能为空！");
        ////        throw new Exception("应用Id不能为空！");
        ////    }

        ////    var app = YuU3dAppSettingDati.TryGetApp(currentAssetBundleInfo.LocAppId);
        ////    var appHelper = app.Helper;
        ////    var localPath = appHelper.LocalHttpRootDir
        ////                    + $"{app.LocAppId}_AssetBundleInfo.bytes";
        ////    var bytes = YuSerializeUtility.Serialize(currentAssetBundleInfo);
        ////    YuIOUtility.WriteAllBytes(localPath, bytes);
        ////    AssetDatabase.Refresh();
        ////    Debug.Log($"应用{currentAssetBundleInfo.LocAppId}的AssetBundle数据已更新！");
        ////}

        #endregion
    }
}