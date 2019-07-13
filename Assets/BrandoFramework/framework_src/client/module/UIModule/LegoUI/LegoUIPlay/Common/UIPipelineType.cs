using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI生命周期桥接器。
    /// 提供目标UI外部生命周期处理委托的获取和存储。
    /// </summary>
    public enum UIPipelineType
    {
        OnCreated,
        BeforeShow,
        AfterShow,
        BeforeHide,
        AfterHide,
        BeforeClose,
        AfterClose
    }
}
    