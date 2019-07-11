#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图挂载层级类型。
    /// </summary>
    [System.Serializable]
    public enum LegoViewType : byte
    {
        StaticBackground,
        DynamicBackground,
        StaticMiddle,
        DynimicMiddle,
        StaticTop,
        DynimicTop,
    }
}