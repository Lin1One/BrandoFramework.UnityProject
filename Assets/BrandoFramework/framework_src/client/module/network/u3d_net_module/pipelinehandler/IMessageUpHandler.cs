
namespace client_module_network
{
    /// <summary>
    /// 通信上行阶段业务处理器。
    /// </summary>
    public interface IMessageUpHandler
    {
        byte[] Handle(byte[] bytes);
    }
}