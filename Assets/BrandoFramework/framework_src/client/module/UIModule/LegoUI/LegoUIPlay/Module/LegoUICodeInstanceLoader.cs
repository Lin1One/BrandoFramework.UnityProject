#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 20:29:52
// Email:             836045613@qq.com

#endregion

using UnityEngine;

using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI程序实例加载器。
    /// </summary>
    public class YuLegoUICodeInstanceLoader : ILegoUICodeLoader
    {
//#pragma warning disable 649
//        [Inject] private readonly IYuU3dAppEntity appEntity;
//#pragma warning restore 649
//        private YuU3dAppSetting CurrentRunU3DApp => appEntity.CurrentRuningU3DApp;

        #region 实例路由器

        private IYuCodeInstanceRouter<ILegoView> viewRouter;

        private IYuCodeInstanceRouter<ILegoView> ViewRouter
            => viewRouter ?? (viewRouter = new YuLegoViewCodeRouter());

        private IYuCodeInstanceRouter<ILegoComponent> componentRouter;

        private IYuCodeInstanceRouter<ILegoComponent> ComponentRouter
            => componentRouter ?? (componentRouter = new YuLegoComponentCodeRouter());

        private IYuCodeInstanceRouter<IYuLegoLogicer> logicerRouter;

        private IYuCodeInstanceRouter<IYuLegoLogicer> LogicerRouter
            => logicerRouter ?? (logicerRouter = new YuLegoLogicerCodeRouter());

        #endregion

        #region 私有路由器类

        private class YuLegoViewCodeRouter : YuAbsCodeInstanceRouter<ILegoView>
        {
        }

        private class YuLegoComponentCodeRouter : YuAbsCodeInstanceRouter<ILegoComponent>
        {
        }

        private class YuLegoLogicerCodeRouter : YuAbsCodeInstanceRouter<IYuLegoLogicer>
        {
        }

        #endregion

        #region 实例路由方法

        public ILegoView GetView(RectTransform uiRect)
        {
            ////var viewTypeId = CurrentRunU3DApp.LocAppId + "_" + uiRect.name;
            ////var view = ViewRouter.GetInstance(viewTypeId);
            ////return view;
            return null;
        }

        public ILegoComponent GetComponent(RectTransform uiRect)
        {
            ////var componentTypeId = uiRect.name.Contains("@")
            ////    ? CurrentRunU3DApp.LocAppId + "_" + uiRect.name.Split('@')[0]
            ////    : CurrentRunU3DApp.LocAppId + "_" + uiRect.name;
            ////var view = ComponentRouter.GetInstance(componentTypeId);
            ////return view;
            return null;
        }

        public IYuLegoLogicer GetLogicer(string uiRect)
        {
            return null;
            ////var uiId = uiRect.Contains("@")
            ////    ? CurrentRunU3DApp.LocAppId + "_" + uiRect.Split('@')[0]
            ////    : CurrentRunU3DApp.LocAppId + "_" + uiRect;
            ////var logicerId = uiId + "_Logicer";
            ////var logicer = LogicerRouter.GetInstance(logicerId);
            ////return logicer;
        }

        #endregion
    }
}