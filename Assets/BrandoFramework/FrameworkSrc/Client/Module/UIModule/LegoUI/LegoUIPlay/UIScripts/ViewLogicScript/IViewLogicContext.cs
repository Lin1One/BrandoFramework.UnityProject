namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI业务逻辑处理上下文。
    /// </summary>
    public interface IViewLogicContext
    {
        ILegoUI MapUI { get; }

        void Init(ILegoUI ui);
    }
}