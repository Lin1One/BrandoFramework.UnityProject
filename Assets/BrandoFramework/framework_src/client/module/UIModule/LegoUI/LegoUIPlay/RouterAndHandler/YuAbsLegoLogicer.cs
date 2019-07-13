using YuU3dPlay;

namespace Client.LegoUI
{
    public abstract class YuAbsLegoLogicer : IYuLegoLogicer
    {
        public IYuLegoLogicContext Context;

        #region 快捷属性

        public IYuLegoUIRxModel RxModel => Context.MapUI.RxModel;
        public ILegoUI MapUI => Context.MapUI;

        #endregion

        public virtual void Init(IYuLegoLogicContext context)
        {
            Context = context;
        }
    }
}