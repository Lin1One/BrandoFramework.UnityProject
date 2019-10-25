#region Head

// Author:            Yu
// CreateDate:        2019/1/12 10:29:13
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.Config;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Client.Assets.Editor
{
    /// <summary>
    /// 资源数据构建器
    /// </summary>
    public static class YuAssetInfoBuilder
    {
        private static readonly HashSet<string> _sameIds
            = new HashSet<string>();

        [MenuItem("Yu//自动化工作流/重建/资源数据 &r")]
        public static void BuildAssetInfos()
        {
            //var currentApp = YuU3dAppSettingDati.CurrentActual;
            var assetInfos = new Dictionary<char, Dictionary<string, AssetInfo>>();
            var hotDir = ProjectInfoDati.GetActualInstance().CurrentProjectAssetDatabaseDirPath;
            var hotPaths = IOUtility.GetPathsContainSonDir(hotDir, SelectAssetPath);

            foreach (var hotPath in hotPaths)
            {
                var assetId = Path.GetFileNameWithoutExtension(hotPath).ToLower();
                var suffix = Path.GetExtension(hotPath);

                if (_sameIds.Contains(assetId))
                {
                    Debug.LogError($"发现命名重复的资源{assetId}!");
                    continue;
                }

                try
                {
                    _sameIds.Add(assetId);
                    var dirPath = hotPath.Replace(hotDir, "");
                    var lastIndex = dirPath.LastIndexOf('/');
                    if (lastIndex != -1)
                    {
                        dirPath = dirPath.Substring(0, lastIndex).EnsureDirEnd();
                    }

                    var info = new AssetInfo(assetId, dirPath, AssetLocation.HotUpdate, suffix);

                    var cIndex = assetId[0];
                    if (!assetInfos.ContainsKey(cIndex))
                    {
                        assetInfos.Add(cIndex, new Dictionary<string, AssetInfo>());
                    }

                    assetInfos[cIndex].Add(assetId, info);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            var writePath = $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}/Config/AssetInfo.byte";
            SerializeUtility.SerializeAndWriteTo(assetInfos, writePath);
            Debug.Log("资源数据创建完毕！");
            AssetDatabase.Refresh();
        }

        private static bool SelectAssetPath(string p)
        {
            var result = !p.EndsWith(".meta") && !p.EndsWith(".tpsheet")
                && !p.EndsWith(".DS_Store");
            return result;
        }

        //[MenuItem("Yu/自动化工作流/重建/资源Id脚本")]
        //private static void RefreshAssetIdScript()
        //{
        //    //YuAssetIdScriptCreater.CreateAssetIdScript(CurrentU3DApp);
        //}

        //[MenuItem("Yu/自动化工作流/重建/重命名同名的资源")]
        //private static void RenameAssetIdScript()
        //{
        //    //YuAssetIdScriptCreater.RenameFile(CurrentU3DApp);
        //}
    }
}