#region Head

// Author:            Yu
// CreateDate:        2018/8/25 19:46:25
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoGlobalMeta
    {
        [LabelText("文本样式全局元数据")] public List<LegoTextStyleMeta> TextStyleMetas;
    }
}