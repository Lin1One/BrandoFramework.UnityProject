#region Head
// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion


namespace Common.PrefsData
{
    public interface IDati : IOnEnable, IOnClose, IOnActive
    {

    }

    public interface IDati<out T> : /*IYuDati<T>*/ IDati
        where T : class, new()
    {
    }

    public interface IOnEnable
    {
        void OnEnable();
    }

    public interface IOnActive
    {
        void OnActive();
    }

    public interface IOnClose
    {
        void OnClose();
    }


}

