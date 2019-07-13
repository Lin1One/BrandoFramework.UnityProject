#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public interface ILegoComponentPool
    {
        ILegoUI Take(ILegoView locView, string componentId);

        ILegoUI Take(ILegoUI locUi, string componentId);

        void Restore(ILegoUI ui);
    }
}