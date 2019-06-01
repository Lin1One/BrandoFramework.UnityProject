using System.Text;

namespace client_common
{
    /// <summary>
    /// 静态通用工厂。
    /// 1. 持有各种常用的对象池。
    /// </summary>
    public static class YuCommonFactory
    {
        private static ObjectPool<StringBuilder> _StringBuilderPool;

        /// <summary>
        /// 字符串构造器对象池，默认容量为50。
        /// </summary>
        public static ObjectPool<StringBuilder> StringBuilderPool
        {
            get
            {
                if (_StringBuilderPool != null) return _StringBuilderPool;

                _StringBuilderPool = new ObjectPool<StringBuilder>(() => new StringBuilder(), 10);
                return _StringBuilderPool;
            }
        }


    }
}