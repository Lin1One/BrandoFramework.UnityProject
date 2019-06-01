#region Head

// Author:                liuruoyu1981
// CreateDate:            5/18/2019 7:13:06 AM
// Email:                 liuruoyu1981@gmail.com

#endregion

using System;

namespace Client.Assets
{
    public interface IAssetLoadTask
    {
        string AssetId { get; set; }
        Type AssetType { get; set; }
    }
}
