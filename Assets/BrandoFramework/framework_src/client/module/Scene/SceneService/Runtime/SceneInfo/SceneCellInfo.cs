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

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace GameWorld
{
    //格子info
    [Serializable]
    public class SceneCellInfo
    {
        [LabelText("场景格ID")]
        [LabelWidth(70)]
        public int cellId;

        [LabelText("场景物体ID列表")]
        public List<int> itemsNum;
        [NonSerialized]
        public bool isOpen = false;
    }
}