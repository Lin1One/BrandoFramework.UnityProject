using Common;
using Common.Config;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YuCommon;


#region Head

// Author:                liuruoyu1981
// CreateDate:            2019/5/11 17:32:08
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Assets
{
    [Singleton]
    [Serializable]
    //[DefaultInjecType(typeof(IAssetInfoHelper))]
#if UNITY_EDITOR
    public class AssetInfoHelper : MonoBehaviour, IAssetInfoHelper
#else
    public class AssetInfoHelper : IAssetInfoHelper
#endif
    {
        //private readonly YuU3dAppSetting _appSetting = U3dGlobal.CurrentApp;

        private Dictionary<char, Dictionary<string, AssetInfo>> _infos;

        private Dictionary<char, Dictionary<string, AssetInfo>> Infos
        {
            get
            {
                if (_infos != null)
                {
                    return _infos;
                }

                InitAssetInfo();
                return _infos;
            }
        }

        private void InitAssetInfo()
        {
            var bytes = File.ReadAllBytes(
                $"{Application.streamingAssetsPath}/{ProjectInfoDati.GetActualInstance().DevelopProjectName}/Config/AssetInfo.byte");

            _infos = SerializeUtility.DeSerialize<Dictionary<char, Dictionary<string, AssetInfo>>>(bytes);
        }

        public IAssetInfo GetAssetInfo(string assetId)
        {
            var lowerAssetId = assetId.ToLower();
            var cIndex = lowerAssetId[0];
            var infos = Infos[cIndex];
            if(!infos.ContainsKey(lowerAssetId))
            {
#if DEBUG
                Debug.LogError("资源加载失败，无法找到资源info： " + assetId);
#endif
                return null;
            }
            var info = infos[lowerAssetId];
            return info;
        }
    }
}