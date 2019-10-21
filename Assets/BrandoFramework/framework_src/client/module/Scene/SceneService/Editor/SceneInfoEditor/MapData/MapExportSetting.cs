#region Head

// Author:            LinYuzhou
// CreateDate:        2019/9/18 01:42:04
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using UnityEngine;

namespace YuU3dEditor
{
    public class MapExportSetting : MonoBehaviour
    {
        public uint mapId;

        [Range(1, 100)]
        public int xCellCount = 10;
        [Range(1, 100)]
        public int zCellCount = 10;
    }
}

