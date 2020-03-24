#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/2 20:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common.Editor;
using Client.Utility;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Client.Core.Editor;

namespace Client.Assets.Editor
{
    public class AssetBundleMenuItemExtend
    {
        #region 设置打包方式

        #region 设置单个目录

        [MenuItem("Assets/AssetBundle/修改打包配置/单独打包")]
        private static void SetTargetDir_AtEvery()
        {
            SetTargetDirAtildType(AssetBundleBuildType.EveryBuild);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/目录打包")]
        private static void SetTargetDir_BuildAtDiry()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtDirSelf);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/尺寸打包")]
        private static void SetTargetDir_BuildAtSize()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtSize);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/目录树打包")]
        private static void SetTargetDir_BuildAtDirTree()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtDirTree);
        }

        #endregion

        #region 设置所有子目录递归

        [MenuItem("Assets/AssetBundle/修改打包配置/所有子目录/单独打包")]
        private static void SetAllSonDir_AtEvery()
        {
            SetSonDirAtildType(AssetBundleBuildType.EveryBuild, AllSonDirs);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/所有子目录/尺寸打包")]
        private static void SetAllSonDir_AtSize()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtSize, AllSonDirs);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/所有子目录/目录打包")]
        private static void SetAllSonDir_AtDir()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirSelf, AllSonDirs);
        }

        #endregion

        #region 设置一级子目录

        [MenuItem("Assets/AssetBundle/修改打包配置/一级子目录/单独打包")]
        private static void SetFirstSonDir_AtEvery()
        {
            SetSonDirAtildType(AssetBundleBuildType.EveryBuild, FirstSonDirs);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/一级子目录/尺寸打包")]
        private static void SetFirtSonDir_AtSize()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtSize, FirstSonDirs);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/一级子目录/目录打包")]
        private static void SetFirstSonDir_AtDir()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirSelf, FirstSonDirs);
        }

        [MenuItem("Assets/AssetBundle/修改打包配置/一级子目录/目录树打包")]
        private static void SetFirstSonDir_AtDirTree()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirTree, FirstSonDirs);
        }

        #endregion

        #region 清理设置

        [MenuItem("Assets/AssetBundle/清理打包配置/所有子目录")]
        private static void CleanAllSonDir()
        {
            locAppId = UnityEditorUtility.GetLocAppIdAtSelectDir();
            locU3DApp = ProjectInfoDati.GetActualInstance();
            if (string.IsNullOrEmpty(locAppId))
            {
                UnityEditorUtility.DisplayError("所选择的目录不是一个应用下的有效目录！");
                return;
            }
            var assetbundleEditor = AssetBundleEditorDati.GetActualInstance();
            if (assetbundleEditor == null)
            {
                UnityEditorUtility.DisplayError($"应用{locAppId}没有AssetBundle打包配置！");
                throw new Exception($"应用{locAppId}没有AssetBundle打包配置！");
            }
            foreach (var dir in AllSonDirs)
            {
                if (!dir.StartsWith(locU3DApp.ProjectAssetDatabaseDir))
                    // dir.StartsWith(appHelper.StreamingAssetsDir))
                {
                    Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
                    continue;
                }
                assetbundleEditor.CleanBuildSettingAtDir(dir);
            }
            AssetBundleEditorDati.GetSingleDati().Save();
        }

        [MenuItem("Assets/AssetBundle/清理打包配置/所选目录可多选")]
        private static void CleanSelectDir()
        {
            var dirs = UnityEditorUtility.GetSelectDirs();
            var targetApp = UnityEditorUtility.TryGetLocProjectInfoAtDir(dirs.First());
            if (targetApp == null)
            {
                UnityEditorUtility.DisplayError($"第一个目录必须是一个应用的允许打包目录！");
                return;
            }

            var assetbundleEditor = AssetBundleEditorDati.GetActualInstance();
            foreach (var dir in dirs)
            {
                //var dirSetting = appAssetBundleSetting.GetDirSetting(dir);
                //List<String> paths = null;
                //paths = dirSetting?.GetFullPathsAtDirTree();
                //assetbundleEditor.CleanBuildSettingAtDir(dir, paths);
            }
            AssetBundleEditorDati.GetSingleDati().Save();
        }

        #endregion

        #endregion

        #region 设置AssetBundle名

        /// <summary>
        /// 使用目标目录及所有子目录下的所有AssetBundle打包设置修改AssetBundle包Id。
        /// </summary>
        [MenuItem("Assets/AssetBundle/设置BundleId/所有子目录及自身")]
        private static void SetSelectDirBundleIdAtAllSonDirSetting()
        {
            SetTargetDirsBundleIdAndSelectBuild(false, false);
        }

        [MenuItem("Assets/AssetBundle/设置BundleId并打包/所有子目录及自身")]
        private static void SetDirsBundleIdAndBuild()
        {
            SetTargetDirsBundleIdAndSelectBuild(true, false);
        }

        #endregion

        #region 具体功能方法

        private static string locAppId;
        private static ProjectInfo locU3DApp;
        /// <summary>
        /// 设置目标目录的打包类型
        /// </summary>
        /// <param name="buildType"></param>
        private static void SetTargetDirAtildType(AssetBundleBuildType buildType)
        {
            var projectInfo = ProjectInfoDati.GetActualInstance();
            var assetbundleEditor = AssetBundleEditorDati.GetActualInstance();

            locAppId = projectInfo.DevelopProjectName;

            if (string.IsNullOrEmpty(locAppId))
            {
                UnityEditorUtility.DisplayError("未设置项目名称");
                return;
            }

            //appHelper = locU3DApp.Helper;

            if (assetbundleEditor == null)
            {
                Debug.LogError($"项目的 AssetBundleSetting 实例未创建 ！");
                return;
            }

            var dirs = UnityEditorUtility.GetSelectDirs();
            foreach (var dir in dirs)
            {
                if (!dir.StartsWith(projectInfo.ProjectAssetDatabaseDir))
                {
                    Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
                    return;
                }
                assetbundleEditor.SetBuildAtTargetBuildType(dir, buildType);
                if (buildType == AssetBundleBuildType.BuildAtDirTree) // 清理所有子目录的打包配置
                {
                    var sonDirs = IOUtility.GetAllDir(dir, null, true, false);
                    foreach (var sonDir in sonDirs)
                    {
                        assetbundleEditor.CleanBuildSettingAtDir(sonDir);
                    }
                    Debug.Log($"目录{dir}已设置为目录树打包，其所有子目录的打包配置都已被清空！");
                }
            }
            AssetBundleEditorDati.GetSingleDati().Save();
        }

        private static void SetSonDirAtildType(AssetBundleBuildType buildType, List<string> dirs)
        {
            locAppId = UnityEditorUtility.GetLocAppIdAtSelectDir();
            locU3DApp = ProjectInfoDati.GetActualInstance();
            if (string.IsNullOrEmpty(locAppId))
            {
                UnityEditorUtility.DisplayError("所选择的目录不是一个应用下的有效目录！");
                return;
            }

            foreach (var dir in dirs)
            {
                if (!dir.StartsWith(locU3DApp.ProjectAssetDatabaseDir))
                //&& dir.StartsWith(appHelper.StreamingAssetsDir))
                {
                    Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
                    continue;
                }

                var assetbundleEditor = AssetBundleEditorDati.GetActualInstance();
                if (assetbundleEditor == null)
                {
                    Debug.Log($"应用{locAppId}没有AssetBundle打包配置！");
                    return;
                }

                assetbundleEditor.SetBuildAtTargetBuildType(dir, buildType);
                if (buildType == AssetBundleBuildType.BuildAtDirTree) // 清理所有子目录的打包配置
                {
                    var sonDirs = IOUtility.GetAllDir(dir, null, true, false);
                    foreach (var sonDir in sonDirs)
                    {
                        assetbundleEditor.CleanBuildSettingAtDir(sonDir);
                    }
                    Debug.Log($"目录{dir}已设置为目录树打包，其所有子目录的打包配置都已被清空！");
                }
            }
            AssetBundleEditorDati.GetSingleDati().Save();
        }

        private static void SetTargetDirsBundleIdAndSelectBuild(bool isBuild, bool isSaveSetting)
        {
            var selectDir = UnityEditorUtility.GetSelectDir();
            var targetApp = UnityEditorUtility.TryGetLocProjectInfoAtDir(selectDir);
            if (targetApp == null)
            {
                UnityEditorUtility.DisplayError("所选择的目录是一个无效目录，必须是一个应用允许打包的目录！");
                return;
            }

            var dirs = IOUtility.GetAllDir(selectDir);
            var appAssetBundleSetting = AssetBundleEditorDati.GetActualInstance();
            var dirSettings = new List<AssetBundleBuildSetting>();

            foreach (var dir in dirs)
            {
                var setting = appAssetBundleSetting.GetSetting(dir);
                if (setting == null)
                {
                    continue;
                }

                dirSettings.Add(setting);
            }
            AssetBundleBuilder.currentAssetBundleInfo =
                AssetBundleBuilder.GetAppAssetBundleInfo(targetApp.DevelopProjectName);
            for (int i = 0; i < dirSettings.Count; i++)
            {
                EditorUtility.DisplayProgressBar("设置 AB 包名", $"正在设置 {dirSettings[i].Dir} 文件夹的 AB 包名", i * 1.0f / dirSettings.Count);
                AssetBundleBuilder.SetBundleIdAndSelectIsBuild(dirSettings[i],
                    isBuild, true, isSaveSetting);
            }
            EditorUtility.ClearProgressBar();
            AssetBundleBuilder.SaveCurrentAppAssetBundleInfo();

            AssetBundleEditorDati.GetSingleDati().Save();
        }

        private static List<string> FirstSonDirs =>
            IOUtility.GetAllDir(UnityEditorUtility.GetSelectDir(), null, true, false);

        private static List<string> AllSonDirs
        {
            get
            {
                var selectDir = UnityEditorUtility.GetSelectDir();
                var sonDirs = IOUtility.GetAllDir(selectDir, null, false, false);
                sonDirs.RemoveAt(0);
                return sonDirs;
            }
        }

        #endregion

        #region 检测同名目录

        //[MenuItem("Assets/Yu/AssetBundle/检测同名目录")]
        private static void CheckSameIdDirs()
        {
            //var selectDir = Unity3DEditorUtility.GetSelectDir();
            //var targetApp = Unity3DEditorUtility.TryGetLocAppAtDir(selectDir);
            //if (targetApp == null)
            //{
            //    Unity3DEditorUtility.DisplayError($"$目标目录{selectDir}不是一个应用下的目录！");
            //    return;
            //}

            //var targetDirs = new List<string>();
            //targetDirs.Add(targetApp.Helper.AssetDatabaseDir);
            //targetDirs.Add(targetApp.Helper.StreamingAssetsDir);
            //var sameIdDirs = IOUtility.GetSameIdDirsAtDirTree(targetDirs);
            //if (sameIdDirs.Count == 0)
            //{
            //    Unity3DEditorUtility.DisplayTooptx($"应用{targetApp.LocAppId}的AssetBundle打包目录没有发现同名目录");
            //}
            //else
            //{
            //    Debug.LogError($"应用{targetApp.LocAppId}的AssetBundle打包目录发现同名目录!");
            //    foreach (var dir in sameIdDirs)
            //    {
            //        Debug.LogError($"同名目录{dir}！");
            //    }
            //}
        }

        #endregion

    }

}