

namespace Common
{
    /// <summary>
    /// 核心类。
    /// 1. 提供注入器实例的获取。
    /// 2. 提供注入器上下文的构建API。
    /// </summary>
    public static class InjectorFactory
    {
        private static bool isBinding;
        private static IInjector injectorImpl;

        public static void BindingInjector(IInjector injector)
        {
            if (isBinding)
            {
                return;
            }

            injectorImpl = injector;
            isBinding = true;
        }

        public static IInjector GetInjector() => injectorImpl ?? (injectorImpl = new Injector());
    }
}