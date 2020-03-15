#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using System.Collections.Generic;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI挂载器。
    /// 负责处理所有视图的深度层级关系。
    /// </summary>
    [Singleton]
    public class LegoUIMounter
    {
        public Dictionary<LegoViewType,YuLegoUILayer> uiLayers = 
            new Dictionary<LegoViewType,YuLegoUILayer>();
    }
}