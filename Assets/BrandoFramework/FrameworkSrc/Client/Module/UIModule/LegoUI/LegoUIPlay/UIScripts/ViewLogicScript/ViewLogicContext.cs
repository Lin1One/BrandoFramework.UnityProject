using Common;

using YuU3dPlay;

namespace Client.LegoUI
{
    public class ViewLogicContext : IViewLogicContext, IRelease, IReset
    {
        public ILegoUI MapUI { get; private set; }
        public void Init(ILegoUI ui)
        {
            MapUI = ui;
        }

        public void Release()
        {
        }

        public void Reset()
        {
            MapUI = null;
        }
    }
}