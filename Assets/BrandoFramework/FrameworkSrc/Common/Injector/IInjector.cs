
using System;

namespace Common
{
    public interface IInjector
    {
        void Map<TSource, TTarget>() where TTarget : TSource;

        //void ReMapping<TSource, TTarget>() where TTarget : TSource;

        void MapWithSingletonCreateFunc<TSource, TTarget>(Func<object> func) where TTarget : TSource;

        void MapWithMultiCreateFunc<TSource, TTarget>(Func<object> func) where TTarget : TSource;

        T Get<T>() where T : class;

        //T Get<T>(string path) where T : class;

        void CleanAllMap();
    }
}