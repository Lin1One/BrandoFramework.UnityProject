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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{

    [Serializable]
    public class SceneSizeInfo
    {
        public MyVector2 UnityMin;
        public MyVector2 UnityMax;

        public MyVector2 ServerOriPos;
        public int xCellCountServer;
        public int zCellCountServer;
    }

    [Serializable]
    public class YuAllMapSize
    {
        public List<string> MapNameList;
        public List<SceneSizeInfo> MapSizeList;
    }


}