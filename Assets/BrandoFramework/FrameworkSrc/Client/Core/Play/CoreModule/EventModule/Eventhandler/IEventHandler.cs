using System;

namespace Client.Core
{
    #region 非泛型

    /// <summary>
    /// 事件处理器接口。
    /// </summary>
    public interface IEventHandler
    {
        void HandleEvent(object data);

        bool CheckMatch(Delegate action);

        int Id { get; }
    }

    #endregion

    //#region 泛型

    ///// <summary>
    ///// 事件处理器接口（泛型数据）
    ///// </summary>
    ///// <typeparam name="TData"></typeparam>
    //public interface IEventHandler<TData>
    //{
    //    void HandleEvent(TData eventData);

    //    bool CheckMatch(Action<TData> action);

    //    int Id { get; }
    //}

    ///// <summary>
    ///// 事件处理器接口（泛型数据）
    ///// </summary>
    ///// <typeparam name="TData"></typeparam>
    //public interface IEventHandler<T1,T2>
    //{
    //    void HandleEvent(T1 eventData1, T2 eventData2);

    //    bool CheckMatch(Action<T1, T2> action);

    //    int Id { get; }
    //}

    //#endregion
}