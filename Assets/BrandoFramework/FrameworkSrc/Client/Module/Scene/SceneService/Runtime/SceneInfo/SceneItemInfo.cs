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

namespace GameWorld
{
    //物体info
    [Serializable]
    public class SceneItemInfo
    {
        public int objNum;
        public string objName;
        public string prefabName;
        public string prefabAssetPath;

        [NonSerialized]
        public int useCount = 0;
        public MyVector3 pos = new MyVector3();
        public MyVector3 rot = new MyVector3();
        public MyVector3 scal = new MyVector3();
        public EMapLayer parentLayer;
        public int[] lightmapIndexes;
        public MyVector4[] lightmapScaleOffsets;
        public int[] realtimeLightmapIndexes;
        public MyVector4[] realtimeLightmapScaleOffsets;
    }
}