#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 20:29:52
// Email:             836045613@qq.com


#endregion


using Common;
using System;
using System.Collections.Generic;


namespace Client.LegoUI
{
    /// <summary>
    /// 管理乐高 UI 滚动视图生命周期事件。
    /// </summary>
    [Singleton]
    public class LegoScrollViewPipelineRouter
    {
        //[Inject]
        //protected readonly IYuU3dAppEntity AppEntity;

        /// <summary>
        /// 当前运行环境中所有应用的UI生命周期事件处理器实例字典。
        /// 第一级key为App应用的Id，值为该应用所有UI的生命周期事件处理器字典。
        /// 第二级Key为UI的唯一Id，值为该UI的所有生命周期事件处理器实例列表。
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, List<IYuLegoScrollViewPipelineHandler>>> pipelineHandlerDict =
            new Dictionary<string, Dictionary<string, List<IYuLegoScrollViewPipelineHandler>>>();

        private Dictionary<string, List<IYuLegoScrollViewPipelineHandler>> currentPipelineHandlers;
        private readonly Dictionary<string, string> scrollViewTokenMap
        = new Dictionary<string, string>();


        public List<IYuLegoScrollViewPipelineHandler> GetHandlers(string id)
        {
            var finalId = id;

            if (id.Contains("@"))
            {
                if (scrollViewTokenMap.ContainsKey(id))
                {
                    finalId = scrollViewTokenMap[id];
                }
                else
                {
                    finalId = id.Split('@')[0];
                    scrollViewTokenMap.Add(id, finalId);
                }
            }

            if (currentPipelineHandlers == null || !currentPipelineHandlers.ContainsKey(finalId))
            {
                return null;
            }

            return currentPipelineHandlers[finalId];
        }

        private Dictionary<string, List<Type>> scrollViewPipelineTypeDic;

        private Dictionary<string, List<Type>> ScrollViewPipelineTypeDic
        {
            get
            {
                if (scrollViewPipelineTypeDic != null)
                {
                    return scrollViewPipelineTypeDic;
                }

                scrollViewPipelineTypeDic = new Dictionary<string, List<Type>>();

                ////foreach (var app in AppEntity.RuningApps)
                ////{
                ////    var appAsem = YuUnityIOUtility.GetUnityAssembly(app.PlayAsmId);
                ////    var types = ReflectUtility.GetTypeList<IYuLegoScrollViewPipelineHandler>(false, false, appAsem);
                ////    scrollViewPipelineTypeDic.Add(app.LocAppId, types);
                ////}

                return scrollViewPipelineTypeDic;
            }
        }

        private void AutoRegisterAllUiPipelineHandlers()
        {
            foreach (var kv in ScrollViewPipelineTypeDic)
            {
                foreach (var type in kv.Value)
                {
                    var handler = (IYuLegoScrollViewPipelineHandler)Activator.CreateInstance(type);
                    CachePipelineHandler(kv.Key, handler);
                }
            }

            // 设置当前UI生命周期处理器字典为当前运行应用的对应字典。
            if (pipelineHandlerDict.Count > 0)
            {
                //currentPipelineHandlers = pipelineHandlerDict[AppEntity.CurrentRuningU3DApp.LocAppId];
            }
        }

        private void CachePipelineHandler(string appId, IYuLegoScrollViewPipelineHandler handler)
        {
            if (!pipelineHandlerDict.ContainsKey(appId))
            {
                pipelineHandlerDict.Add(appId, new Dictionary<string, List<IYuLegoScrollViewPipelineHandler>>());
            }

            var appHandlers = pipelineHandlerDict[appId];
            if (!appHandlers.ContainsKey(handler.UiId))
            {
                appHandlers.Add(handler.UiId, new List<IYuLegoScrollViewPipelineHandler>());
            }

            var uiHandlers = appHandlers[handler.UiId];

#if DEBUG
            if (uiHandlers.Contains(handler))
            {
                throw new Exception($"尝试为UI{handler.UiId}添加重复的生命周期事件处理器！");
            }
#endif

            uiHandlers.Add(handler);
        }

        private void AutoInject()
        {
            AutoRegisterAllUiPipelineHandlers();
        }
    }
}
