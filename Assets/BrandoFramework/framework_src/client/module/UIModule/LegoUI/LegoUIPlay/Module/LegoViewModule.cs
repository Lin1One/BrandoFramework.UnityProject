#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/9 20:29:52
// Email:             836045613@qq.com


#endregion

using Common;
using System;

namespace Client.LegoUI
{
    [Singleton]
    public class LegoViewModule :IModule,
        ILegoViewModule
    {
        //public override string ServiceType => "View";

        [Inject]
        private readonly YuLegoUILoader uILoader;
        [Inject]
        private readonly YuLegoRxModelLoader modelLoader;

        /// <summary>
        /// 后台加载 UI 
        /// </summary>
        /// <param name="ids"></param>
        public void LoadUiInBackground(params string[] ids)
        {
            uILoader.LoadUiInBackground(ids);
        }

        public void CloseView(string id)
        {
            uILoader.CloseView(id);
        }

        public object GetRxModel(string logicId) => modelLoader.LoadModel(logicId);

        public void GetScrollViewComponent(string id, Action<ILegoUI> callback)
        {
            WaitUi(id, callback, isBindNexRxModelOnBuild: false);
        }

        public IYuLegoLogicer GetLogicer(string id)
        {
            return uILoader.GetLogicer(id);
        }

        public bool IsViewActive(string id) => uILoader.IsViewActive(id);

        public void Restore(ILegoComponent component) => uILoader.Restore(component);

        public void WaitUi
            (
            string id,
            Action<ILegoUI> callback,
            LegoViewType uiLayeredCanvas = LegoViewType.DynamicBackground,
            int buildSpped = -1,
            bool isBindNexRxModelOnBuild = true
            )
            => uILoader.WaitUi(id, callback, uiLayeredCanvas, isBindRxModelOnBuild: isBindNexRxModelOnBuild);

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}