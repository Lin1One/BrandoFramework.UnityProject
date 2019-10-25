#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/2 20:44:51
// Email:             836045613@qq.com

#endregion

using Common.Config;
using Common;
using Common.Editor;
using Common.PrefsData;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Client.Assets.Editor
{
    public class YuU3dAssetBundleSpanner
    {
        #region 菜单项

        #region 设置

        #region 选择目录单一

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/单独打包")]
        private static void SetTargetDir_AtEvery()
        {
            SetTargetDirAtildType(AssetBundleBuildType.EveryBuild);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/目录打包")]
        private static void SetTargetDir_BuildAtDiry()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtDirSelf);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/尺寸打包")]
        private static void SetTargetDir_BuildAtSize()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtSize);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/目录树打包")]
        private static void SetTargetDir_BuildAtDirTree()
        {
            SetTargetDirAtildType(AssetBundleBuildType.BuildAtDirTree);
        }

        private static void SetTargetDirAtildType(AssetBundleBuildType buildType)
        {
            //locAppId = Unity3DEditorUtility.GetLocAppIdAtSelectDir();

            //if (string.IsNullOrEmpty(locAppId))
            //{
            //    Unity3DEditorUtility.DisplayError("所选择的目录不是一个应用下的有效目录！");
            //    return;
            //}

            //locU3DApp = YuU3dAppSettingDati.TryGetApp(locAppId);
            //appHelper = locU3DApp.Helper;

            var currentAssetbundleSetting
                = AssetBundleEditorDati.GetActualInstance();

            var projectInfo
                = ProjectInfoDati.GetActualInstance();

            if (currentAssetbundleSetting == null)
            {
                Debug.LogError($"所选文件夹 AssetBundleSetting 实例未创建 ！");
                return;
            }

            var dirs = Unity3DEditorUtility.GetSelectDirs();
            foreach (var dir in dirs)
            {
                if (!dir.StartsWith(projectInfo.CurrentProjectAssetDatabaseDirPath))
                {
                    Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
                    return;
                }

                currentAssetbundleSetting.SetBuildAtTargetBuildType(dir, buildType);
                if (buildType == AssetBundleBuildType.BuildAtDirTree) // 清理所有子目录的打包配置
                {
                    var sonDirs = IOUtility.FirstSonDirs(dir);
                    foreach (var sonDir in sonDirs)
                    {
                        currentAssetbundleSetting.CleanBuildSettingAtDir(sonDir);
                    }

                    Debug.Log($"目录{dir}已设置为目录树打包，其所有子目录的打包配置都已被清空！");
                }
            }

            AssetBundleEditorDati.GetSingleDati().Save();
        }

        #endregion

        #region 设置所有子目录递归

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/所有子目录/单独打包")]
        private static void SetAllSonDir_AtEvery()
        {
            SetSonDirAtildType(AssetBundleBuildType.EveryBuild, AllSonDirs);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/所有子目录/尺寸打包")]
        private static void SetAllSonDir_AtSize()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtSize, AllSonDirs);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/所有子目录/目录打包")]
        private static void SetAllSonDir_AtDir()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirSelf, AllSonDirs);
        }

        #endregion

        #region 设置一级子目录

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/一级子目录/单独打包")]
        private static void SetFirstSonDir_AtEvery()
        {
            SetSonDirAtildType(AssetBundleBuildType.EveryBuild, FirstSonDirs);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/一级子目录/尺寸打包")]
        private static void SetFirtSonDir_AtSize()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtSize, FirstSonDirs);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/一级子目录/目录打包")]
        private static void SetFirstSonDir_AtDir()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirSelf, FirstSonDirs);
        }

        [MenuItem("Assets/Yu/AssetBundle/修改打包配置/一级子目录/目录树打包")]
        private static void SetFirstSonDir_AtDirTree()
        {
            SetSonDirAtildType(AssetBundleBuildType.BuildAtDirTree, FirstSonDirs);
        }

        #endregion

        #endregion

        #region 清理设置

        [MenuItem("Assets/Yu/AssetBundle/清理打包配置/所有子目录")]
        private static void CleanAllSonDir()
        {
            //locAppId = Unity3DEditorUtility.GetLocAppIdAtSelectDir();

            //if (string.IsNullOrEmpty(locAppId))
            //{
            //    Unity3DEditorUtility.DisplayError("所选择的目录不是一个应用下的有效目录！");
            //    return;
            //}

            //locU3DApp = YuU3dAppSettingDati.TryGetApp(locAppId);
            //appHelper = locU3DApp.Helper;

            //foreach (var dir in AllSonDirs)
            //{
            //    if (!dir.StartsWith(appHelper.AssetDatabaseDir)
            //        && dir.StartsWith(appHelper.StreamingAssetsDir))
            //    {
            //        Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
            //        continue;
            //    }

            //    var appBuildSetting = YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(locAppId);
            //    if (appBuildSetting == null)
            //    {
            //        Unity3DEditorUtility.DisplayError($"应用{locAppId}没有AssetBundle打包配置！");
            //        throw new Exception($"应用{locAppId}没有AssetBundle打包配置！");
            //    }

            //    appBuildSetting.CleanBuildSettingAtDir(dir);
            //}

            //YuU3dAppAssetBundleSettingDati.GetMultiAtId(locAppId).Save();
        }

        [MenuItem("Assets/Yu/AssetBundle/清理打包配置/所选目录可多选")]
        private static void CleanSelectDir()
        {
            //var dirs = Unity3DEditorUtility.GetSelectDirs();
            //var targetApp = Unity3DEditorUtility.TryGetLocAppAtDir(dirs.First());
            //if (targetApp == null)
            //{
            //    Unity3DEditorUtility.DisplayError($"第一个目录必须是一个应用的允许打包目录！");
            //    return;
            //}

            //var appAssetBundleSetting = YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(targetApp.LocAppId);
            //foreach (var dir in dirs)
            //{
            //    var dirSetting = appAssetBundleSetting.GetDirSetting(dir);
            //    List<String> paths = null;
            //    paths = dirSetting?.GetFullPathsAtDirTree();
            //    appAssetBundleSetting.CleanBuildSettingAtDir(dir, paths);
            //}

            //YuU3dAppAssetBundleSettingDati.GetMultiAtId(targetApp.LocAppId).Save();
        }

        #endregion

        #region 设置AssetBundle名

        /// <summary>
        /// 使用目标目录及所有子目录下的所有AssetBundle打包设置修改AssetBundle包Id。
        /// </summary>
        [MenuItem("Assets/Yu/AssetBundle/设置BundleId/所有子目录及自身")]
        private static void SetSelectDirBundleIdAtAllSonDirSetting()
        {
            SetTargetDirsBundleIdAndSelectBuild(false, false);
        }

        [MenuItem("Assets/Yu/AssetBundle/设置BundleId并打包/所有子目录及自身")]
        private static void SetDirsBundleIdAndBuild()
        {
            SetTargetDirsBundleIdAndSelectBuild(true, false);
        }

        [MenuItem("Assets/Yu/AssetBundle/设置选定目录下所有Mesh文件的材质引用/Just Do It")]
        private static void SetDirsMeshImporterMat()
        {
            var path = EditorUtility.OpenFilePanel("选择Importer Material", "Assets/XTwo/AssetDatabase", "mat");
            path = path.Replace(Application.dataPath, "Assets");
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                SetMeshImporterMat(mat);
            }
            else
            {
                Debug.LogError("无法找到相应Material文件 " + path);
            }
        }

        private static void SetMeshImporterMat(Material mat)
        {
            var mainDir = Unity3DEditorUtility.GetSelectDir();
            var dirs = IOUtility.GetAllDir(mainDir);
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    var fullDir = YuUnityIOUtility.GetFullPath(dir);
                    //Debug.LogError(dir);

                    var fullpaths = Directory.GetFiles(fullDir);
                    foreach (var item in fullpaths)
                    {
                        SetMeshImporterMat(item, mat);
                    }

                }
            }
            //var objs =  AssetDatabase.LoadAllAssetsAtPath(dir);
            //Debug.LogError(objs.Length);
            //foreach (var item in objs)
            //{
            //    Debug.LogError(item.name);
            //}
        }

        private static void SetMeshImporterMat(string fullPath, Material mat)
        {
            if (fullPath.EndsWith(".meta"))
            {
                return;
            }
            var path = YuUnityIOUtility.GetAssetsPath(fullPath);

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null)
            {
                return;
            }

            var importer = ModelImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as ModelImporter;
            if (importer != null)
            {
                var renders = obj.GetComponentsInChildren<Renderer>();
                string matName = null;
                foreach (var item in renders)
                {
                    foreach (var matItem in item.sharedMaterials)
                    {
                        if (matItem != null)
                        {
                            matName = matItem.name;
                            break;
                        }
                    }
                    if (matName != null)
                    {
                        break;
                    }
                }
                if(matName == null)
                {
                    return;
                }

                importer.importMaterials = true;
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;

                importer.SaveAndReimport();

                var map = importer.GetExternalObjectMap();
                List<AssetImporter.SourceAssetIdentifier> list =
                    new List<AssetImporter.SourceAssetIdentifier>();
                foreach (var item in map)
                {
                    //Debug.LogError(item.Key.name + " --- " + item.Key.type);
                    if (item.Key.type == mat.GetType())
                    {
                        list.Add(item.Key);

                    }
                }
                foreach (var item in list)
                {
                    importer.RemoveRemap(item);
                    importer.AddRemap(item, mat);
                }

                foreach (var item in renders)
                {
                    foreach (var matItem in item.sharedMaterials)
                    {
                        if (matItem != null)
                        {
                            matName = matItem.name;
                            importer.AddRemap(new AssetImporter.SourceAssetIdentifier(mat.GetType(), matName), mat);
                        }
                    }
                }

                Debug.Log("完成mesh的Importer Materials操作：" + path);

                importer.SaveAndReimport();
            }
        }

        [MenuItem("Assets/Yu/图集/更新当前选择图集的信息")]
        private static void UpdateAtlasInfo()
        {
            // var spriteNames = new List<string>();
            //var selects = YuEditorUtility.GetSelectPaths();
            //var localSavePath = YuU3dAppSettingDati.CurrentActual?.Helper.LocalHttpRootDir
            //                        + YuU3dAppSettingDati.CurrentActual?.LocAppId + "_AtlasInfo.bytes";

            //var bytes = File.Exists(localSavePath) ? File.ReadAllBytes(localSavePath) : null;
            //Client.LegoUI.YuLegoAppAtlasInfo AppAtlasInfo;
            //if (bytes == null)
            //{
            //    AppAtlasInfo = new Client.LegoUI.YuLegoAppAtlasInfo();
            //}
            //else
            //{
            //    AppAtlasInfo =
            //        YuSerializeUtility.DeSerialize<Client.LegoUI.YuLegoAppAtlasInfo>(bytes);
            //    if (AppAtlasInfo == null)
            //    {
            //        AppAtlasInfo = new Client.LegoUI.YuLegoAppAtlasInfo();
            //    }
            //}

            ////foreach (var item in AppAtlasInfo.SpriteIdMap)
            ////{
            ////    Debug.LogError(item.Key + " -- " + item.Value);
            ////}

            //int count = 0;
            //int maxCount = selects.Count;
            //foreach (var item in selects)
            //{
            //    count++;
            //    EditorUtility.DisplayProgressBar("更新精灵图片数据", "正在更新图片所在的图集数据", ((float)count) / maxCount);
            //    var texImporter = AssetImporter.GetAtPath(item) as TextureImporter;
            //    if (texImporter != null)
            //    {
            //        var atlasId = Path.GetFileNameWithoutExtension(item);
            //        foreach (var spritesheet in texImporter.spritesheet)
            //        {
            //            var spriteName = spritesheet.name.ToLower();
            //            if (!AppAtlasInfo.SpriteIdMap.ContainsKey(spriteName))
            //            {
            //                AppAtlasInfo.SpriteIdMap.Add(spriteName, atlasId);
            //                Debug.Log("已添加精灵图集信息：  " + spriteName + "  --  " + atlasId);
            //            }
            //            else
            //            {
            //                AppAtlasInfo.SpriteIdMap[spriteName] = atlasId;
            //                Debug.Log("已添加精灵图集信息：  " + spriteName + "  --  " + atlasId);
            //            }
            //        }
            //    }
            //}
            //bytes = YuSerializeUtility.Serialize(AppAtlasInfo);
            //YuIOUtility.WriteAllBytes(localSavePath, bytes);
            //EditorUtility.ClearProgressBar();
            //AssetDatabase.Refresh();
        }

        private static void SetTargetDirsBundleIdAndSelectBuild(bool isBuild, bool isSaveSetting)
        {
            var selectDir = Unity3DEditorUtility.GetSelectDir();
            //var targetApp = Unity3DEditorUtility.TryGetLocAppAtDir(selectDir);
            //if (targetApp == null)
            //{
            //    Unity3DEditorUtility.DisplayError("所选择的目录是一个无效目录，必须是一个应用允许打包的目录！");
            //    return;
            //}

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
            //YuAssetBundleUtility.currentAssetBundleInfo = 
            //    YuAssetBundleUtility.GetAppAssetBundleInfo(targetApp.LocAppId);
            for (int i = 0; i < dirSettings.Count; i++)
            {
                EditorUtility.DisplayProgressBar("设置 AB 包名", $"正在设置 {dirSettings[i].Dir} 文件夹的 AB 包名", i * 1.0f / dirSettings.Count);
                YuAssetBundleUtility.SetBundleIdAndSelectIsBuild(dirSettings[i],
                    isBuild, true, isSaveSetting);
            }
            EditorUtility.ClearProgressBar();
            //YuAssetBundleUtility.SaveCurrentAppAssetBundleInfo();

            AssetBundleEditorDati.GetSingleDati().Save();
        }

        #endregion

        #region 具体功能方法

        private static string locAppId;
        //private static YuU3dAppSetting locU3DApp;
        //private static YuAppHelper appHelper;

        private static void SetSonDirAtildType(AssetBundleBuildType buildType, List<string> dirs)
        {
            //locAppId = Unity3DEditorUtility.GetLocAppIdAtSelectDir();

            //if (string.IsNullOrEmpty(locAppId))
            //{
            //    Unity3DEditorUtility.DisplayError("所选择的目录不是一个应用下的有效目录！");
            //    return;
            //}

            //locU3DApp = YuU3dAppSettingDati.TryGetApp(locAppId);
            //appHelper = locU3DApp.Helper;

            //foreach (var dir in dirs)
            //{
            //    if (!dir.StartsWith(appHelper.AssetDatabaseDir)
            //        && dir.StartsWith(appHelper.StreamingAssetsDir))
            //    {
            //        Debug.LogError($"目标目录不是一个有效的AssetBundle打包目录！");
            //        continue;
            //    }

            //    var appBuildSetting = YuU3dAppAssetBundleSettingDati.TryGetAssetBundleSetting(locAppId);
            //    if (appBuildSetting == null)
            //    {
            //        Debug.Log($"应用{locAppId}没有AssetBundle打包配置！");
            //        return;
            //    }

            //    appBuildSetting.SetBuildAtTargetBuildType(dir, buildType);
            //    if (buildType == YuAssetBundleAutoBuildType.BuildAtDirTree) // 清理所有子目录的打包配置
            //    {
            //        var sonDirs = IOUtility.FirstSonDirs(dir);
            //        foreach (var sonDir in sonDirs)
            //        {
            //            appBuildSetting.CleanBuildSettingAtDir(sonDir);
            //        }

            //        Debug.Log($"目录{dir}已设置为目录树打包，其所有子目录的打包配置都已被清空！");
            //    }
            //}

            //YuU3dAppAssetBundleSettingDati.GetMultiAtId(locAppId).Save();
        }

        private static List<string> FirstSonDirs => 
            IOUtility.FirstSonDirs(Unity3DEditorUtility.GetSelectDir());

        private static List<string> AllSonDirs
        {
            get
            {
                var selectDir = Unity3DEditorUtility.GetSelectDir();
                var sonDirs = IOUtility.GetAllSonDirNotSelf(selectDir);
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

        #endregion
    }

}