#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Client.Assets;
using Client.Core;
using Common;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 一个应用的乐高元数据。
    /// </summary>
    public class LegoAppViewMeta
    {
        #region 基础字段

        private IAssetModule assetModule;

        private IAssetModule AssetModule
        {
            get
            {
                if (assetModule != null)
                {
                    return assetModule;
                }

                assetModule = Injector.Instance.Get<IAssetModule>();
                return assetModule;
            }
        }

        private readonly Dictionary<string, LegoUIMeta> uiMetaDict
            = new Dictionary<string, LegoUIMeta>();

        #endregion


        //private IAssetConsumer _assetConsumer;
        //private IAssetConsumer AssetConsumer
        //{
        //    get
        //    {
        //        if (_assetConsumer == null)
        //        {
        //            _assetConsumer = new AssetConsumer();
        //        }
        //        return _assetConsumer;
        //    }
        //}


        public LegoUIMeta GetUIMeta(string uiId)
        {
//#if UNITY_EDITOR
//            if (YuUnityUtility.IsEditorMode)
//            {
//                if (uiMetaDict.ContainsKey(uiId))
//                {
//                    var uiMeta = uiMetaDict[uiId];
//                    return uiMeta;
//                }
//                return GetUIMetaAtEditor(uiId);
//            }
//            else
//#endif
                return GetUIMetaAtPlay(uiId);
        }

        public LegoUIMeta GetUIMetaAtPlay(string uiId)
        {

            LegoUIMeta meta = null;

            if (uiMetaDict.ContainsKey(uiId))
            {
                meta = uiMetaDict[uiId];
            }
            else
            {
                try
                {
                    var textAsset = assetModule.Load<TextAsset>(uiId);

                    if (textAsset == null)
                    {
#if DEBUG
                        Debug.LogError("UI配置文件读取失败: " + uiId);
#endif
                        return null;
                    }

                    var text = textAsset.text;
                    meta = JsonUtility.FromJson<LegoUIMeta>(text);
                    uiMetaDict.Add(uiId, meta);
                    AssetModule.ReleaseTarget(uiId);
                }
                catch (System.Exception e)
                {
#if DEBUG
                    Debug.LogError("UI配置文件读取失败: " + uiId);
                    Debug.LogError(e.Message + e.StackTrace);
#endif
                    return null;
                }
            }

            return meta;
        }


#if UNITY_EDITOR

        private Dictionary<string, string> metaPathDict;

        private LegoUIMeta GetUIMetaAtEditor(string uiId)
        {
            if (metaPathDict == null)
            {
                metaPathDict = new Dictionary<string, string>();
                var app = ProjectInfoDati.GetActualInstance();
                string metaDir = string.Empty;
                var pathDict = IOUtility.GetPathDictionary(metaDir, s => s.EndsWith(".txt"));
                foreach (var kv in pathDict)
                {
                    metaPathDict.Add(kv.Key.ToLower(), kv.Value);
                }
            }

            if (!metaPathDict.ContainsKey(uiId))
            {
                Debug.Log($"id为{uiId}的元数据不存在！");
                return null;
            }

            var path = metaPathDict[uiId];
            var content = File.ReadAllText(path);
            var uiMeta = JsonUtility.FromJson<LegoUIMeta>(content);
            uiMeta.Reset();
            var bytes = SerializeUtility.Serialize(uiMeta);
            var newMeta = SerializeUtility.DeSerialize<LegoUIMeta>(bytes);
            uiMetaDict.Add(uiId, newMeta);
            return uiMeta;
        }
#endif

        public void ReleaseAllMetaBundle()
        {
            //assetModule.ReleaseAsset();
        }
    }
}