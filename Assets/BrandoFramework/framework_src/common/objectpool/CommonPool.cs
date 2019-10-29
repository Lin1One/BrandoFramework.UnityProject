using System.Text;

namespace Common
{
    /// <summary>
    /// 静态通用工厂。
    /// 1. 持有各种常用的对象池。
    /// </summary>
    public static class CommonPool
    {
        private static GenericObjectPool<StringBuilder> _StringBuilderPool;

        /// <summary>
        /// 字符串构造器对象池，默认容量为50。
        /// </summary>
        public static GenericObjectPool<StringBuilder> StringBuilderPool
        {
            get
            {
                if (_StringBuilderPool != null) return _StringBuilderPool;

                _StringBuilderPool = new GenericObjectPool<StringBuilder>(() => new StringBuilder(), 10);
                return _StringBuilderPool;
            }
        }


    }
}