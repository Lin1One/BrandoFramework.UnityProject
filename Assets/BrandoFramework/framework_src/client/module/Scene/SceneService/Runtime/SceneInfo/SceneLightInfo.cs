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
    //lightmapInfo
    [Serializable]
    public class SceneLightInfo
    {
        public enum ELightmapType
        {
            color,
            dir,
            shadowmask
        }

        public LightmapsMode lightmapMode;
        public int[] dataIndexes;           //lightmapTexs对应的data数组编号
        public ELightmapType[] types;       //lightmapTexs对应的data图片类型
        public string[] lightmapTexNames;
        public string[] lightmapTexPath;
    
        public int DataCount
        {
            get
            {
                return lightmapTexNames != null ? lightmapTexNames.Length : 0;
            }
        }
    }
}