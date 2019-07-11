#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Common;
using Common.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图元数据助手。
    /// 该类负责初始化运行时所有应用的视图元数据。
    /// 提供获取视图元数据的API。
    /// </summary>
    [Singleton]
    public class LegoMetaHelper
    {
        #region 元数据初始化

        private readonly Dictionary<string, LegoAppViewMeta> appViewMetaDict
            = new Dictionary<string, LegoAppViewMeta>();

        public LegoMetaHelper()
        {
            LoadCurrentAppMeta();
        }

        #endregion

        #region 元数据加载

        private void LoadCurrentAppMeta()
        {
            var appSetting = ProjectInfoDati.GetActualInstance();
                var appMeta = new LegoAppViewMeta();
                appViewMetaDict.Add(appSetting.DevelopProjectName, appMeta);
        }

        #endregion

        #region 获取元数据

        private readonly Dictionary<string, string> metaIdMap
            = new Dictionary<string, string>();

        public LegoUIMeta GetMeta(RectTransform uiRect)
        {
            var appId = ProjectInfoDati.GetActualInstance().DevelopProjectName;
            var appUIMeta = appViewMetaDict[appId];
            string metaId;

            if (metaIdMap.ContainsKey(uiRect.name))
            {
                metaId = metaIdMap[uiRect.name];
            }
            else
            {
                metaId = uiRect.UITypeId().ToLower();
                metaIdMap.Add(uiRect.name, metaId);
            }

            var lowerMetaId = /*YuBigAssetIdMap.GetLowerId*/(metaId);
            var meta = appUIMeta.GetUIMeta(lowerMetaId);
            return meta;
        }

        public LegoUIMeta GetMeta(string id)
        {
            var finalId = id.Contains("@")
                ? id.Split('@')[0]
                : id;
            var appId = ProjectInfoDati.GetActualInstance().DevelopProjectName;
            if (appId == null)
            {
                return null;
            }

            var appUIMeta = appViewMetaDict[appId];
            var lowerMetaId = /*YuBigAssetIdMap.GetLowerId*/(finalId);
            var meta = appUIMeta.GetUIMeta(lowerMetaId);
            return meta;
        }

        #endregion

        #region 刷新元数据（编辑器模式）

        public void ReloadAllMeta()
        {
            appViewMetaDict.Clear();
            LoadCurrentAppMeta();
        }

        #endregion
    }

    public static class YuLegoExtendAtPlay
    {
        public static string UITypeId(this RectTransform uiRect)
        {
            var typeId = uiRect.name.Contains("@")
                ? uiRect.name.Split('@')[0]
                : uiRect.name;
            return typeId;
        }
    }
}