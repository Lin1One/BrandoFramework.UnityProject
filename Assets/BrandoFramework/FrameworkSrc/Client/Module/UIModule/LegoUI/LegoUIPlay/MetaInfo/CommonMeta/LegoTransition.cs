#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com


#endregion

namespace Client.LegoUI
{
    [System.Serializable]
    public enum LegoTransition : byte
    {
        None,
        ColorTint,
        SpriteSwap,
        Animation,
    }
}