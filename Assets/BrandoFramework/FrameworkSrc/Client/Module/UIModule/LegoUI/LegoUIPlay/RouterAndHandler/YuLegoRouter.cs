#region Head

// Author:            Yu
// CreateDate:        2018/8/29 7:24:19
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI路由器。
    /// 负责以字符串Id的形式返回各种业务零件实例。
    /// </summary>
    public class YuLegoRouter
    {
        private readonly AbsRouter<string, Func<ILegoComponent>> ComponentRouter
            = new YuLegoComponentRouter();

        private readonly AbsRouter<string, Func<ILegoView>> ViewRouter
            = new YuLegoViewRouter();

        private readonly AbsRouter<string, Func<IYuLegoUIRxModel>> RxModelRouter
            = new YuLegoUIRxModelRouter();

        private readonly AbsRouter<string, Func<IYuLegoScrollViewRxModel>> ScrollViewRxModelRouter;

        #region 视图、组件、模型获取API

        public ILegoView GetView(string id)
        {
            return null;/// ViewRouter.Get(YuBigAssetIdMap.GetLowerId(id))();
        }

        public ILegoComponent GetComponent(string id)
        {
            return null;//ComponentRouter.Get(YuBigAssetIdMap.GetLowerId(id))();
        }

        public IYuLegoUIRxModel GetViewOrComponentRxModel(string id)
        {
            return RxModelRouter.Get(id)();
        }

        public IYuLegoScrollViewRxModel GetScrollViewRxModel(string id)
        {
            return ScrollViewRxModelRouter.Get(id)();
        }

        #endregion

        #region 具象注册路由API

        public void MapComponentFunc(string id, Func<ILegoComponent> func)
        {
            ComponentRouter.AddMap(id, func);
        }

        public void MapViewFunc(string id, Func<ILegoView> func)
        {
            ViewRouter.AddMap(id, func);
        }

        public void MapRxModelFunc(string id, Func<IYuLegoUIRxModel> func)
        {
            RxModelRouter.AddMap(id, func);
        }

        public void MapScrollViewRxModelFunc(string id, Func<IYuLegoScrollViewRxModel> func)
        {
            ScrollViewRxModelRouter.AddMap(id, func);
        }

        #endregion

        #region 内部路由器实现类

        private class YuLegoComponentRouter : AbsRouter<string, Func<ILegoComponent>>
        {
        }

        private class YuLegoViewRouter : AbsRouter<string, Func<ILegoView>>
        {
        }

        private class YuLegoUIRxModelRouter : AbsRouter<string, Func<IYuLegoUIRxModel>>
        {
        }

        private class YuLegoScrollViewRouter : AbsRouter<string, Func<object>>
        {
        }

        #endregion
    }
}