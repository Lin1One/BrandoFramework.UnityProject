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
    public class SceneLayerInfo
    {
        public string layerName;
        public MyVector3 pos = new MyVector3();
        public MyVector3 rot = new MyVector3();
        public MyVector3 scal = new MyVector3();
        public EMapLayer parentLayer = EMapLayer.None;
    }
}