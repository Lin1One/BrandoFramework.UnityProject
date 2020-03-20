namespace Client.LegoUI
{
    public abstract class ViewLogicBase : IViewLogic
    {
        public IViewLogicContext Context;

        #region 快捷属性

        public IYuLegoUIRxModel RxModel => Context.MapUI.RxModel;
        public ILegoUI MapUI => Context.MapUI;

        #endregion

        public virtual void Init(IViewLogicContext context)
        {
            Context = context;
        }
    }
}