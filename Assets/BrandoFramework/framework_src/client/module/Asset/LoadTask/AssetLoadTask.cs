#region Head

// Author:                liuruoyu1981
// CreateDate:            5/18/2019 7:13:06 AM
// Email:                 liuruoyu1981@gmail.com

#endregion

using System;


namespace Client.Assets
{
    internal class AssetLoadTask : IAssetLoadTask
    {
        public string AssetId { get; set; }
        public Type AssetType { get; set; }

        public void Reset()
        {
            AssetId = null;
            AssetType = null;
        }
    }
}
