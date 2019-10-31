#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using Common;
using System;
using System.Text;

namespace Client.Core
{
    [Serializable]
    public class AssetInfoGUI
    {
        public string AssetId;
        public string DirPath;
        public string Suffix;
        public AssetLocation AssetLocation;

        public AssetInfoGUI(AssetInfo assetInfo)
        {
            AssetId = assetInfo.AssetId;
            DirPath = assetInfo.DirPath;
            Suffix = assetInfo.Suffix;
            AssetLocation = assetInfo.AssetLocation;
        }
    }

    [Serializable]
    public class AssetInfo : IAssetInfo
    {
        public string AssetId { get; }  //文件名
        public string DirPath { get; }  //文件夹
        public string Suffix { get; }   //文件后缀

        public AssetLocation AssetLocation { get; }

        private string _runResourcesPath;

        public string RunResourcesPath
        {
            get
            {
                if (_runResourcesPath != null)
                {
                    return _runResourcesPath;
                }

                _runResourcesPath = DirPath + AssetId;
                return _runResourcesPath;
            }
        }

        public AssetInfo(string assetId, string dirPath, AssetLocation assetLocation, string suffix)
        {
            AssetId = assetId;
            DirPath = dirPath.EnsureDirEnd();
            Suffix = suffix;
            AssetLocation = assetLocation;
        }

        private static StringBuilder _sb;
        private static StringBuilder Sb => _sb ?? (_sb = new StringBuilder());

        public string GetEditorPath(string assetsDir)
        {
            Sb.Clear();
            string path = null;

            switch (AssetLocation)
            {
                case AssetLocation.Resources:
                    path = GetResroucesPath(assetsDir);
                    break;
                case AssetLocation.StreamingAssets:
                    path = GetStreamPath(assetsDir);
                    break;
                case AssetLocation.HotUpdate:
                    path = GetHotUpdatePath(assetsDir);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return path;
        }

        private string GetResroucesPath(string assetsDir)
        {
            Sb.Append(assetsDir);
            Sb.Append(DirPath);
            Sb.Append(AssetId);
            return Sb.ToString();
        }

        private string GetStreamPath(string assetsDir)
        {
            Sb.Append(assetsDir);
            Sb.Append(DirPath);
            Sb.Append(AssetId);
            Sb.Append(Suffix);
            return Sb.ToString();
        }

        private string GetHotUpdatePath(string assetsDir)
        {
            Sb.Append(assetsDir);
            Sb.Append(DirPath);
            Sb.Append(AssetId);
            Sb.Append(Suffix);
            return Sb.ToString();
        }
    }
}

