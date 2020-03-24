using Client.Utility;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


#region Head

// Author:                LinYuzhou
// CreateDate:            2019/10/31 19:37:16
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    public interface IBundlePathInfoHelepr
    {
        string GetBundlePath(string assetId);

        string GetBundleId(string assetId);

        void SetBundleInfo(byte[] bytes);
    }

    [Singleton]
    public class BundlePathInfoHelepr : IBundlePathInfoHelepr
    {
        private ProjectAssetsToBundleMapInfo projectAssetToBundleMap;

        private string abSteamingPath => $"{Application.streamingAssetsPath}/AssetBundle/";

        private string abSandboxPath => $"{Application.persistentDataPath}/AssetBundle/";

        private readonly StringBuilder abPathBuilder = new StringBuilder();

        private readonly ProjectInfo projectConfig;

        public BundlePathInfoHelepr()
        {
            string path = null;
            projectConfig = ProjectInfoDati.GetActualInstance();

            if (UnityModeUtility.IsEditorMode)
            {
                path = /*projectConfig.Helper.LocalHttpRootDir +*/ $"{projectConfig.DevelopProjectName}_AssetBundleInfo.bytes";
                var bytes = File.ReadAllBytes(path);
                projectAssetToBundleMap = SerializeUtility.DeSerialize<ProjectAssetsToBundleMapInfo>(bytes);
            }
        }

        public void SetBundleInfo(byte[] bytes)
        {
            try
            {
                var info = SerializeUtility.DeSerialize<ProjectAssetsToBundleMapInfo>(bytes);
                if (info != null)
                {
                    projectAssetToBundleMap = info;
                }
#if DEBUG
                if (projectAssetToBundleMap == null)
                    Debug.LogError("设置Bundle数据失败");
#endif

            }
            catch(Exception e)
            {
#if DEBUG
                Debug.LogError("设置Bundle数据失败：" + e.Message + e.StackTrace);
#endif
            }
        }

        public string GetBundlePath(string assetId)
        {
            abPathBuilder.Clear();
            string bundleName = string.Empty;
            if (projectAssetToBundleMap.AssetIdToBundleMap.ContainsKey(assetId[0]))
            {
                projectAssetToBundleMap.AssetIdToBundleMap[assetId[0]].TryGetValue(assetId, out bundleName);
            }

            if (bundleName != null)
            {
                abPathBuilder.Append(abSandboxPath);
                abPathBuilder.Append(bundleName.Remove(1).ToLower());
                abPathBuilder.Append("/");
                abPathBuilder.Append(bundleName);
                abPathBuilder.Append(".ab");
            }
            else
            {
                bundleName = assetId;
                abPathBuilder.Append(abSandboxPath);
                abPathBuilder.Append(bundleName.Remove(1).ToLower());
                abPathBuilder.Append("/");
                abPathBuilder.Append(bundleName);
            }

            if (UnityModeUtility.IsEditorMode)
            {
                if (!File.Exists((abPathBuilder.ToString())))
                {
                    abPathBuilder.Replace(abSandboxPath, abSteamingPath);
                    //abPathBuilder.Replace(abSandboxPath, _appSetting.Helper.AssetBundleBuildDir);
                }
            }
            else
            {
                if (!File.Exists((abPathBuilder.ToString())))
                {
                    abPathBuilder.Replace(abSandboxPath, abSteamingPath);
                }
            }

            var path = abPathBuilder.ToString();

#if UNITY_EDITOR
            if (!File.Exists(path))
            {
                Debug.LogError($"资源{assetId}的目标AssetBundle路径{path}不存在！");
            }
#endif

            return path;
        }

        public string GetBundleId(string assetId)
        {
           return projectAssetToBundleMap.GetBundleId(assetId) + ".ab";
        }
    }
}