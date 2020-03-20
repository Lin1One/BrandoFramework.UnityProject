#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public abstract class AbsLegoComponent :
        AbsLegoUI,
        ILegoComponent
    {
        public int ScrollViewId { get; set; } = -1;
    }
}