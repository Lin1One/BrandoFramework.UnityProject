
using Client.Core;
using Client.Core.Editor;
using Client.Utility;
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
        public ProjectAssetBundleInfoEditor AssetbundleInfo;

        [TabGroup("打包目录配置")]
        [HideLabel]
        public AssetBundleBuildSetting TargetDirSetting;

        [TabGroup("打包目录配置")]
        [LabelText("配置项列表")]
        public List<AssetBundleBuildSetting> BundleDirSettings
             = new List<AssetBundleBuildSetting>();
        //Todo：目录打包宏
        #endregion

        private void AddSetting(string dir)
        {
            var existSetting = BundleDirSettings.Find(s => s.Dir == dir);
            if (existSetting != null)
            {
                return;
            }
            var setting = new AssetBundleBuildSetting { Dir = dir };
            BundleDirSettings.Add(setting);
        }

        public AssetBundleBuildSetting GetSetting(string path)
        {
            path = UnityIOUtility.GetAssetsPath(path);
            var setting = BundleDirSettings.Find(s => s.Dir == path);
            return setting;
        }

        private void OrderByDirId()
        {
            BundleDirSettings = BundleDirSettings.OrderBy(s => s.Dir).ToList();
        }

        public void SetBuildAtTargetBuildType(string dir, AssetBundleBuildType buildType)
        {
            var locAppId = UnityEditorUtility.GetLocAppIdAtSelectDir();
            var appSetting = ProjectInfoDati.GetActualInstance();

            if (locAppId == null)
            {
                Debug.LogError($"目标目录{dir}不是一个应用下的目录！");
                return;
            }

            if (!AssetBundleBuilder.IsLegalAssetBundleDir(dir))
            {
                Debug.LogError($"目标目录{dir}不是一个有效的AssetBundle目录！");
                return;
            }

            var existSetting = BundleDirSettings.Find(s => s.Dir == dir);
            if (existSetting == null)
            {
                existSetting = new AssetBundleBuildSetting
                {
                    BuildType = buildType,
                    Dir = dir,
                    //LocAppId = Unity3DEditorUtility.GetLocAppIdAtSelectDir()
                };
                BundleDirSettings.Add(existSetting);
                BundleDirSettings = BundleDirSettings.OrderBy(s => s.Dir).ToList();
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
                        importer.assetBundleVariant = null;
                    }
                }
            }

            var existSetting = BundleDirSettings.Find(s => s.Dir == dir);
            if (existSetting == null)
            {
                Debug.LogError($"目录{dir}当前不存在配置数据，如果确定是一个有效目录，" +
                               "请先刷新应用的目录配置！");
                return;
            }

            BundleDirSettings.Remove(existSetting);
            Debug.Log($"所选目录{dir}的打包配置已移除！");
        }
    }
}