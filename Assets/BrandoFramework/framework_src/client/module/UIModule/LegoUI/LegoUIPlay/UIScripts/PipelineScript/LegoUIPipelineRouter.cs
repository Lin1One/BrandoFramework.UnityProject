#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 20:29:52
// Email:             836045613@qq.com


#endregion


using Common;
using Common.Utility;
using System;
using System.Collections.Generic;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI生命周期事件处理器加载器。
    /// 负责以反射或静态映射的方式缓存、构建所有的UI事件处理器实例。
    /// </summary>
    [Singleton]
    public class LegoUIPipelineLoader
    {
        /// <summary>
        /// 当前运行环境中所有应用的UI生命周期事件处理器实例字典。
        /// 第一级key为App应用的Id，值为该应用所有UI的生命周期事件处理器字典。
        /// 第二级Key为UI的唯一Id，值为该UI的所有生命周期事件处理器实例列表。
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, List<IYuLegoUIPipelineHandler>>> pipelineHandlerDict =
            new Dictionary<string, Dictionary<string, List<IYuLegoUIPipelineHandler>>>();

        private Dictionary<string, List<IYuLegoUIPipelineHandler>> currentPipelineHandlers;

        public List<IYuLegoUIPipelineHandler> GetHandlers(string id)
        {
            if (currentPipelineHandlers == null || !currentPipelineHandlers.ContainsKey(id))
            {
                return null;
            }

            return currentPipelineHandlers[id];
        }

        private Dictionary<string, List<Type>> uiPipelineTypeDic;

        private Dictionary<string, List<Type>> UiPipelineTypeDic
        {
            get
            {
                if (uiPipelineTypeDic != null)
                {
                    return uiPipelineTypeDic;
                }

                uiPipelineTypeDic = new Dictionary<string, List<Type>>();

                ////foreach (var app in YuU3dAppUtility.AppEntity.RuningApps)
                ////{
                //var appAsem = YuUnityIOUtility.(app.PlayAsmId);
                //var types = ReflectUtility.GetTypeList<IYuLegoUIPipelineHandler>(false, false, appAsem);
                //uiPipelineTypeDic.Add(app.LocAppId, types);
                ////}

                return uiPipelineTypeDic;
            }
        }

        private void CachePipelineHandler(string appId, IYuLegoUIPipelineHandler handler)
        {
            if (!pipelineHandlerDict.ContainsKey(appId))
            {
                pipelineHandlerDict.Add(appId, new Dictionary<string, List<IYuLegoUIPipelineHandler>>());
            }

            var appHandlers = pipelineHandlerDict[appId];
            if (!appHandlers.ContainsKey(handler.UiId))
            {
                appHandlers.Add(handler.UiId, new List<IYuLegoUIPipelineHandler>());
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

        private void AutoRegisterAllUiPipelineHandlers()
        {
            foreach (var kv in UiPipelineTypeDic)
            {
                foreach (var type in kv.Value)
                {
                    var handler = (IYuLegoUIPipelineHandler)Activator.CreateInstance(type);
                    CachePipelineHandler(kv.Key, handler);
                }
            }

            // 设置当前UI生命周期处理器字典为当前运行应用的对应字典。
            if (pipelineHandlerDict.Count <= 0)
            {
                return;
            }

            ////var appEntity = YuU3dAppUtility.AppEntity;
            ////currentPipelineHandlers = pipelineHandlerDict[appEntity.CurrentRuningU3DApp.LocAppId];
        }

        private void AutoInject()
        {
            AutoRegisterAllUiPipelineHandlers();
        }
    }
}
