#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    public interface IAssetInfo
    {
        string AssetId { get; }

        string GetEditorPath(string assetsDir);
        //string GetEditorPath(YuU3dAppSetting appSetting);

        AssetLocation AssetLocation { get; }

        string RunResourcesPath { get; }
    }
}

