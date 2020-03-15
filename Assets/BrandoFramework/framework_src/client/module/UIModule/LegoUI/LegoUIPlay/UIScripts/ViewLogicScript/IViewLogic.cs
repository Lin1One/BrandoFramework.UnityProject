namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI逻辑处理核心。
    /// 每个实例处理对应UI的核心业务逻辑。
    /// </summary>
    public interface IViewLogic
    {
        void Init(IViewLogicContext context);
    }
}