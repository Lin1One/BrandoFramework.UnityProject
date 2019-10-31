#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 20:29:52
// Email:             836045613@qq.com

#endregion

using Common;
using System;



namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图UI模块接口。
    /// </summary>
    public interface ILegoViewModule : IModule
    {
        #region 界面

        bool IsViewActive(string id);

        void WaitUi
        (
            string id,
            Action<ILegoUI> callback,
            LegoViewType uiLayeredCanvas = LegoViewType.DynimicTop,
            int buildSpped = -1,
            bool isBindNexRxModelOnBuild = true
        );

        void LoadUiInBackground(params string[] ids);

        void CloseView(string id);

        object GetRxModel(string logicId);

        #endregion

        #region 组件

        void Restore(ILegoComponent component);

        void GetScrollViewComponent(string id, Action<ILegoUI> callback);

        #endregion

        #region 逻辑处理组件

        IYuLegoLogicer GetLogicer(string id);


        #endregion
    }
}