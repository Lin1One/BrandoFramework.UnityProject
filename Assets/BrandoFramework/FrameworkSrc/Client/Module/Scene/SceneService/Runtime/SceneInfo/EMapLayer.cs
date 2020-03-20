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
    public enum EMapLayer
    {
        None,
        MapRoot,
        Terrain,// = YuUnityConstant.LAYER_TERRAIN,
        Building,// = YuUnityConstant.LAYER_BUILDING,
        Other,// = YuUnityConstant.LAYER_OTHER,
        Tree,// = YuUnityConstant.LAYER_TREE,
        AdornEffect,// = YuUnityConstant.LAYER_PARTICLE,
        Shadow,// = YuUnityConstant.LAYER_SHADOW,
        Animation, // YuUnityConstant.LAYER_OTHER,
    }
}