
using System;

namespace client_common
{
    public interface IInjector
    {
        void Mapping<TSource, TTarget>() where TTarget : TSource;

        //void ReMapping<TSource, TTarget>() where TTarget : TSource;

        //void RegisterSingleBuildFunc<TSource, TTarget>(Func<object> func) where TTarget : TSource;

        //void RegisterMultiBuildFunc<TSource, TTarget>(Func<object> func) where TTarget : TSource;

        T Get<T>() where T : class;

        //T Get<T>(string path) where T : class;

        void CleanAllMap();
    }
}