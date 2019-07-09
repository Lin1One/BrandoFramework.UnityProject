#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/9 20:29:52
// Email:             836045613@qq.com


#endregion

using Common;
using Common.Config;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Client.LegoUI
{
    public interface ILegoAtlasLoader
    {
        Sprite GetSprite(string spriteId, string appId = null);

        void RestoreSprite(Sprite sp, string appId = null);
    }

    /// <summary>
    /// 图集加载器
    /// </summary>
    [Singleton]
    public class LegoAtlasLoader : ILegoAtlasLoader
    {
        private readonly Dictionary<string, AtlasInfo> appAtlasInfoDict
            = new Dictionary<string, AtlasInfo>();

        private readonly Dictionary<string, Dictionary<string, ILegoAtlas>> appAtlasDict
            = new Dictionary<string, Dictionary<string, ILegoAtlas>>();

        ////[Inject]
        ////private readonly IYuU3dAppEntity appEntity;
        ////[YuInject] private readonly IAssetInfoHelper m_InfoHelper;
        ////[YuInject] private readonly IBundleLoader m_BundleLoader;
        private ProjectInfo projectInfo = ProjectInfoDati.GetActualInstance();

        public Sprite GetSprite(string spriteId, string appId = null)
        {
            var spriteLowerId = spriteId.ToLower();
            var finalAppId = appId ?? projectInfo.DevelopProjectName;
            EnsureLoadAtlasInfo(finalAppId);

            Sprite targetSp = null;
            var appInfo = appAtlasInfoDict[finalAppId];
            var atlasId = appInfo.GetLocAtlasId(spriteLowerId);
            if (atlasId == null)
            {
#if DEBUG
                Debug.LogError(spriteId + "精灵图片加载失败，无法找到对应的图集");
#endif
                return null;
            }

            if (!appAtlasDict.ContainsKey(finalAppId))
            {
                appAtlasDict.Add(finalAppId, new Dictionary<string, ILegoAtlas>());
            }

            var appAtlas = appAtlasDict[finalAppId];
            if (!appAtlas.ContainsKey(atlasId))
            {
                //加载图集AB
                //var bundleRef = m_BundleLoader.Load(atlasId);
                //var atlas = LegoAtlas.Create(bundleRef);
                //appAtlas.Add(atlasId, atlas);
            }

            var targetAtlas = appAtlas[atlasId];
            targetSp = targetAtlas.GetSprite(spriteLowerId);
            return targetSp;
        }

        private void EnsureLoadAtlasInfo(string finalAppId)
        {
            if (!appAtlasInfoDict.ContainsKey(finalAppId))
            {
                //沙盒目录图集
                var sandboxPath = projectInfo.CurrentProjectAssetDatabaseDirPath;
                AtlasInfo atlasInfo = null;
                if (File.Exists(sandboxPath))
                {
                    var atlasInfobytes = File.ReadAllBytes(sandboxPath);
                    atlasInfo = SerializeUtility.DeSerialize<AtlasInfo>(atlasInfobytes);
                    appAtlasInfoDict.Add(finalAppId, atlasInfo);
                }
                else
                {
                    //Streaming 目录图集
                    //#if UNITY_EDITOR
                    //因程序集工程为编译完成的代码，此处无需编译，故新增 UNITY_EDITOR 宏。
                    //#elif UNITY_STANDALONE_WIN
                    var streamPath = projectInfo.CurrentProjectAssetDatabaseDirPath;
                    if (File.Exists(streamPath))
                    {
                        var atlasInfobytes = File.ReadAllBytes(streamPath);
                        atlasInfo = SerializeUtility.DeSerialize<AtlasInfo>(atlasInfobytes);
                        appAtlasInfoDict.Add(finalAppId, atlasInfo);
                    }
                }
            }
        }

        public void SetAppAtlasInfo(byte[] bytes, string appId)
        {
            var atlasInfo = SerializeUtility.DeSerialize<AtlasInfo>(bytes);
            appAtlasInfoDict.Add(appId, atlasInfo);
        }



        public void RestoreSprite(Sprite sp, string appId = null)
        {
            var finalAppId = appId ?? projectInfo.DevelopProjectName;
            EnsureLoadAtlasInfo(finalAppId);

            var appInfo = appAtlasInfoDict[finalAppId];
            var atlasId = appInfo.GetLocAtlasId(sp.name);
            var appAtlas = appAtlasDict[finalAppId];
            if (!appAtlas.ContainsKey(atlasId))
            {
                return;
            }

            var targetAtlas = appAtlas[atlasId];
            targetAtlas.RestoreSprite(sp);
            if (targetAtlas.RefCount == 0)
            {
                appAtlas.Remove(atlasId);
            }
        }
    }
}

