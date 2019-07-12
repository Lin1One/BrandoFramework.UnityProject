#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface ILegoComponentPool
    {
        IYuLegoUI Take(IYuLegoView locView, string componentId);

        IYuLegoUI Take(IYuLegoUI locUi, string componentId);

        void Restore(IYuLegoUI ui);
    }
}