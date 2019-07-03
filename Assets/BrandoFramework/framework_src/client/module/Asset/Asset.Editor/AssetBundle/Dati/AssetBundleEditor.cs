#region Head

// Author:            Yu
// CreateDate:        2019/1/14 9:46:02
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.Editor;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Assets.Editor
{
    /// <summary>
    /// 应用AssetBundle打包配置。
    /// </summary>
    [Serializable]
    public class AssetBundleEditor
    {
        #region 可视化
        [Title("AssetBundle 编辑器",TitleAlignment = TitleAlignments.Centered)]
        [LabelText("是否打开AssetBundle配置图标")]
        public bool AssetBundleButtonWindowSwitch = false;

        [TabGroup("项目 Assetbundle 信息")]
        [HideLabel]
        public YuU3dAppAssetBundleInfo AssetbundleInfo;

        [TabGroup("打包目录配置")]
        [HideLabel]
        public AssetBundleBuildSetting TargetDirSetting;

        [TabGroup("打包目录配置")]
        [LabelText("配置项列表")]
        public List<AssetBundleBuildSetting> BundleSettings
             = new List<AssetBundleBuildSetting>();
        //Todo：目录打包宏
        #endregion

        #region 方法

        private void AddSetting(string dir)
        {
            var existSetting = BundleSettings.Find(s => s.Dir == dir);
            if (existSetting != null)
            {
                return;
            }

            var setting = new AssetBundleBuildSetting { Dir = dir };
            BundleSettings.Add(setting);
        }

        public AssetBundleBuildSetting GetSetting(string path)
        {
            path = YuUnityIOUtility.GetAssetsPath(path);
            var setting = BundleSettings.Find(s => s.Dir == path);
            return setting;
        }

        private void OrderByDirId()
        {
            BundleSettings = BundleSettings.OrderBy(s => s.Dir).ToList();
        }

        public void SetBuildAtTargetBuildType(string dir, AssetBundleBuildType buildType)
        {
            //var locAppId = YuEditorUtility.GetLocAppIdAtSelectDir();
            //var appSetting = YuU3dAppSettingDati.TryGetApp(locAppId);

            //if (appSetting == null)
            //{
            //    Debug.LogError($"目标目录{dir}不是一个应用下的目录！");
            //    return;
            //}

            //if (!YuAssetBundleUtility.IsLegalAssetBundleDir(dir, appSetting))
            //{
            //    Debug.LogError($"目标目录{dir}不是一个有效的AssetBundle目录！");
            //    return;
            //}

            var existSetting = BundleSettings.Find(s => s.Dir == dir);
            if (existSetting == null)
            {
                existSetting = new AssetBundleBuildSetting
                {
                    BuildType = buildType,
                    Dir = dir,
                    //LocAppId = Unity3DEditorUtility.GetLocAppIdAtSelectDir()
                };
                BundleSettings.Add(existSetting);
                BundleSettings = BundleSettings.OrderBy(s => s.Dir).ToList();
                Debug.Log($"目录{dir}当前不存在配置数据，已新建配置！");
            }
            else
            {
                existSetting.BuildType = buildType;
                Debug.Log($"目录{dir}已设置为基于{buildType}打包方式！");
            }
        }

        public void CleanBuildSettingAtDir(string dir,List<string> paths = null)
        {
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = null;
                        //importer.assetBundleVariant = null;
                    }
                }
            }

            var existSetting = BundleSettings.Find(s => s.Dir == dir);
            if (existSetting == null)
            {
                Debug.LogError($"目录{dir}当前不存在配置数据，如果确定是一个有效目录，" +
                               "请先刷新应用的目录配置！");
                return;
            }

            BundleSettings.Remove(existSetting);
            Debug.Log($"所选目录{dir}的打包配置已移除！");
        }


        #endregion

    }
}