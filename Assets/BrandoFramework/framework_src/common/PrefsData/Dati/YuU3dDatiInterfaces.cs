#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/26 8:45:19
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion


namespace YuU3dPlay
{
    public interface IYuOnEnable
    {
        void OnEnable();
    }

    public interface IYuOnActive
    {
        void OnActive();
    }

    public interface IYuOnClose
    {
        void OnClose();
    }

    public interface IYuU3dDati : IYuOnEnable, IYuOnClose, IYuOnActive
    {
    }

    public interface IYuU3dDati<out T> : IYuDati<T>, IYuU3dDati
        where T : class, new()
    {
    }
}

