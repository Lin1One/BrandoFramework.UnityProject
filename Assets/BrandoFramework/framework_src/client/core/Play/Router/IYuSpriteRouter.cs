#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using UnityEngine;

namespace YuU3dPlay
{
    public interface IYuSpriteRouter
    {
        Sprite GetSprite(string spriteId);
    }
}
