

using Common;
using System;
using System.Text;


#region Head

// Author:                liuruoyu1981
// CreateDate:            2019/5/11 17:04:18
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Assets
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

    public interface IAssetInfo
    {
        string AssetId { get; }

        string GetEditorPath(string assetsDir);
        //string GetEditorPath(YuU3dAppSetting appSetting);

        AssetLocation AssetLocation { get; }

        string RunResourcesPath { get; }
    }

    [Serializable]
    public class AssetInfo : IAssetInfo
    {
        public string AssetId { get; }
        public string DirPath { get; }

        public string Suffix { get; }

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
            AssetLocation = assetLocation;
            Suffix = suffix;
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

        //public string GetEditorPath(YuU3dAppSetting appSetting)
        //{
        //    string assetsDir;
        //    var appPath = appSetting.Helper;

        //    switch (AssetLocation)
        //    {
        //        case AssetLocation.Resources:
        //            assetsDir = appPath.ResourcesAssetsDir;
        //            break;
        //        case AssetLocation.StreamingAssets:
        //            assetsDir = appPath.FinalStreamingAssetsDir;
        //            break;
        //        case AssetLocation.HotUpdate:
        //            assetsDir = appPath.AssetDatabaseDir;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    var path = GetEditorPath(assetsDir);
        //    return path;
        //}

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

