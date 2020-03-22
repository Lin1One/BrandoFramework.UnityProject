using System;

namespace Client
{
    /// <summary>
    /// 程序实例路由器。
    /// 通过字符串的Key获取对应的脚本实例。
    /// 可以通过修改路由关系重定向程序实例获取。
    /// </summary>
    public interface ICodeInstanceRouter<T> where T : class
    {
        T GetInstance(string key);

        void RedirectInstance(string key, T instance);
    }

    /// <summary>
    /// 通过给定的键调用目标无参委托返回指定类型的实例。
    /// 在真机设备上执行静态预编译自动生成的静态映射代码，去除所有反射。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICodeInstanceRouter_AtPlay<T> : ICodeInstanceRouter<T> where T : class
    {
        void RegisterFunc(string key, Func<T> func);
    }
}