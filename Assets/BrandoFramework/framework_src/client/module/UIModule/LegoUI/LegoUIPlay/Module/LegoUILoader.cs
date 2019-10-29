#region Head

// Author:            Yu
// CreateDate:        2018/11/8 6:19:20
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Core;
using Common;
using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

using YuU3dPlay;

#pragma warning disable 649

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI实例加载器。
    /// </summary>
    [Singleton]
    public class YuLegoUILoader
    {
        #region 字段

        #region 依赖注入引用

        private IYuU3dInjector injector;

        private IYuU3dInjector m_Injector =>
            injector ?? (injector = Injector.Instance.As<IYuU3dInjector>());

        private LegoBuilder legoBuilder;

        private LegoBuilder LegoBuilder =>
            legoBuilder ? legoBuilder : (legoBuilder = m_Injector.GetMono<LegoBuilder>());

        private LegoMetaHelper metaHelper;

        private LegoMetaHelper MetaHelper =>
            metaHelper ?? (metaHelper = m_Injector.Get<LegoMetaHelper>());

        private YuLegoUICodeInstanceLoader codeLoader;

        private YuLegoUICodeInstanceLoader CodeLoader =>
            codeLoader ?? (codeLoader = m_Injector.Get<YuLegoUICodeInstanceLoader>());

        //UI 数据模型管理器
        private YuLegoRxModelLoader modelLoader;
        private YuLegoRxModelLoader ModelLoader => modelLoader ?? (modelLoader = m_Injector.Get<YuLegoRxModelLoader>());

        //UI 生命周期管理器
        private YuLegoUIPipelineLoader pipelineLoader;

        private YuLegoUIPipelineLoader PipelineLoader =>
            pipelineLoader ?? (pipelineLoader = m_Injector.Get<YuLegoUIPipelineLoader>());

        //UI 数据模型绑定器
        private LegoBinder legoBinder;

        private LegoBinder LegoBinder =>
            legoBinder ?? (legoBinder = m_Injector.Get<LegoBinder>());

        //UI 位置挂载器
        private LegoUIMounter legoUIMounter;

        private LegoUIMounter LegoUIMounter =>
            legoUIMounter ?? (legoUIMounter = m_Injector.Get<LegoUIMounter>());

        private IU3DEventModule u3dEventModule;

        private IU3DEventModule U3DEventModule =>
            u3dEventModule ?? (u3dEventModule = m_Injector.Get<IU3DEventModule>());

        #endregion

        private readonly Dictionary<string, ILegoView> views
            = new Dictionary<string, ILegoView>();

        private readonly Dictionary<string, ILegoUI> uiLogicMap // UI实例的逻辑Id映射
            = new Dictionary<string, ILegoUI>();

        private readonly Dictionary<string, Queue<ILegoComponent>> componentCache
            = new Dictionary<string, Queue<ILegoComponent>>();

        private readonly Dictionary<string, IYuLegoLogicer> uiViewLogicerDic // UI逻辑类实例字典
            = new Dictionary<string, IYuLegoLogicer>();

        private readonly Dictionary<string, List<IYuLegoLogicer>> uiComponentLogicerListsDic // UI逻辑类实例(用于同一组件有多个实例)
            = new Dictionary<string, List<IYuLegoLogicer>>();

        private const string LEGO_VIEW = "legoview";
        private const string LEGO_COMPONENT = "legocomponent";

        #endregion

        #region 异步加载 UI 

        private readonly Queue<LegoBuildTask> waitTasks = new Queue<LegoBuildTask>();

        private readonly List<ILegoUI> tempUis = new List<ILegoUI>();

        public void WaitUi(
            string id,
            Action<ILegoUI> onloaded,
            LegoViewType uiLayeredCanvas = LegoViewType.DynamicBackground,
            int speed = -1,
            bool isInBack = false,
            bool isBindRxModelOnBuild = true)
        {
            if (id.StartsWith(LEGO_VIEW))
            {
                WaitView(id, onloaded, speed, isInBack);
            }
            else
            {
                WaitComponent(id, onloaded, speed, uiLayeredCanvas, isInBack, isBindRxModelOnBuild);
            }

            TryStartNextTask();
        }

        private void TryStartNextTask()
        {
            if (waitTasks.Count == 0)
            {
                return;
            }

            var task = waitTasks.Dequeue();
            LegoBuilder.PushSonTask(task);
            LegoBuilder.StartBuild();
        }


        #region 加载 View 

        /// <summary>
        /// 加载 View
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        /// <param name="buildSpeed"></param>
        /// <param name="isInback"></param>
        private void WaitView(string id, Action<ILegoUI> callback, int buildSpeed, bool isInback = false)
        {
            var uiMeta = MetaHelper.GetMeta(id);
            TryHideOther(uiMeta);

            if (views.ContainsKey(id))
            {
                var view = views[id];
                callback?.Invoke(view);
                view.Show();
                return;
            }

            var viewParent = GetViewMountPoint(id);
            var task = LegoBuilder.CreateTask(id, OnBuilded, viewParent, buildSpeed)
                .SetParentRect(viewParent)
                .SetUILoadCallback(callback)
                .SetBackLoad(isInback);
            waitTasks.Enqueue(task);
        }

        private void TryHideOther(LegoUIMeta uIMeta)
        {
            if (uIMeta.HideTargets == null || uIMeta.HideTargets.Count == 0)
            {
                return;
            }

            foreach (var hideId in uIMeta.HideTargets)
            {
                ////var lowerId = YuBigAssetIdMap.GetLowerId(hideId);
                ////CloseView(lowerId);
            }
        }

        private RectTransform GetViewMountPoint(string id)
        {
            var uiMeta = MetaHelper.GetMeta(id);
            return mountPointDict[uiMeta.ViewType];
        }

        private void OnViewBuilded(string lowerId, string bigId, LegoBuildTask buildTask)
        {
            var uiRect = buildTask.RootRect;
            //            loadingTaskIds.Remove(lowerId);
            var view = CodeLoader.GetView(uiRect);
            views.Add(lowerId, view);
            var uiMeta = MetaHelper.GetMeta(uiRect);

            tempUis.Clear();
            foreach (var sonRef in uiMeta.ComponentRefs)
            {
                if (!uiLogicMap.ContainsKey(sonRef.LogicId))
                {
                    throw new Exception($"视图{bigId}的子组件{sonRef.LogicId}无法找到！");
                }

                var component = uiLogicMap[sonRef.LogicId];
                component.SetParentUi(view);
                tempUis.Add(component);
            }

            //数据模型设置
            var rxModel = (IYuLegoUIRxModel) ModelLoader.LoadModel(bigId);
            SetViewRxModel(rxModel, view);

            //UI 实体脚本初始化
            var pipelineHandlers = PipelineLoader.GetHandlers(bigId);
            if (buildTask.IsInBackground)
            {
                view.Construct(uiRect, pipelineHandlers, tempUis, true);
            }
            else
            {
                view.Construct(uiRect, pipelineHandlers, tempUis);
            }

            //控制器
            TryInvokeViewUILogicer(view);

            // 修正界面的Z轴深度
            AddViewToDepthViews(view, uiMeta.ViewType); 

            //数据模型数据与 UI 实体绑定
            LegoBinder.Binding(view, uiMeta, rxModel);

            //构建完成回调
            buildTask.UiBuildCallback?.Invoke(view);

            //触发界面创建完成事件
            U3DEventModule.TriggerEvent(ProjectCoreEventCode.View_Created, null, view);
            view.ShowDefault();
        }

        private void SetViewRxModel(IYuLegoUIRxModel rxModel, ILegoUI view)
        {
            rxModel.InitRxModel();
            view.SetRxModel(rxModel);
        }

        #endregion

        #region 加载 Component

        private void WaitComponent(
            string id,
            Action<ILegoUI> uiLoadCallback,
            int buildSpeed,
            LegoViewType uiLayeredCanvas = LegoViewType.DynamicBackground,
            bool isInback = false,
            bool isBindRxModelOnBuild = true)
        {
            if (componentCache.ContainsKey(id) && componentCache[id].Count > 0)
            {
                var components = componentCache[id];
                var component = components.Dequeue();
                uiLoadCallback?.Invoke(component);
                return;
            }

            var task = LegoBuilder.CreateTask(id, OnBuilded, null, buildSpeed)
                .SetUILoadCallback(uiLoadCallback)
                .SetBackLoad(isInback)
                .SetBindRxModel(isBindRxModelOnBuild)
                .SetParentCanvas(uiLayeredCanvas);

            waitTasks.Enqueue(task);
        }

        /// <summary>
        /// 加载 View 或 Component 中子组件内部回调
        /// </summary>
        private void OnSonComponentLoaded(string lowerId, LegoBuildTask buildTask)
        {
            buildTask.RootRect.name = buildTask.ComponentMountMeta.Name;
            var bigId = buildTask.RootRect.name;
            var uiRect = buildTask.RootRect;
            var parentRect = buildTask.ParentRect;
            SetComponentPosition(buildTask, parentRect);
            var component = CodeLoader.GetComponent(uiRect);
            if (uiLogicMap.ContainsKey(uiRect.name))
            {
                uiLogicMap[uiRect.name] = component;
            }
            else
            {
                uiLogicMap.Add(uiRect.name, component);
            }

            var uiMeta = MetaHelper.GetMeta(lowerId);
            tempUis.Clear();

            foreach (var sonRef in uiMeta.ComponentRefs)
            {
                if (!uiLogicMap.ContainsKey(sonRef.LogicId))
                {
                    throw new Exception($"视图{bigId}的子组件{sonRef.LogicId}无法找到！");
                }

                var son = uiLogicMap[sonRef.LogicId];
                son.SetParentUi(component);
                tempUis.Add(son);
            }

            var pipelineHandlers = PipelineLoader.GetHandlers(bigId);
            bool isSonOfComponent = parentRect.name.Contains("LegoComponent");
            var rxModel = isSonOfComponent
                ? (IYuLegoUIRxModel) ModelLoader.CreateModel(bigId)
                : (IYuLegoUIRxModel) ModelLoader.LoadModel(bigId);

            SetViewSonComponentRxModel(rxModel, component);
            component.Construct(uiRect, pipelineHandlers, tempUis);
            LegoBinder.Binding(component, uiMeta, rxModel);
            TryInvokeComponentUILogicer(component);
            component.ShowDefault();
            if (component.SonComponentDict.Count != 0)
            {
                BindSonComponentRxmodel(rxModel, component);
            }
        }

        /// <summary>
        /// 单独加载组件内部回调
        /// </summary>
        private void OnSingleComponentLoaded(string lowerId, LegoBuildTask buildTask)
        {
            var bigId = buildTask.RootRect.name;
            var uiRect = buildTask.RootRect;
            var parentRect = mountPointDict[buildTask.ParentCanvas];
            var uiMeta = MetaHelper.GetMeta(uiRect);
            var rectMeta = uiMeta.RootMeta;
            uiRect.name = rectMeta.Name;
            uiRect.SetParent(parentRect);
            uiRect.localPosition = new Vector3(
                rectMeta.X,
                rectMeta.Y,
                rectMeta.Z
            );

            uiRect.localScale = new Vector3(
                rectMeta.ScaleX,
                rectMeta.ScaleY,
                rectMeta.ScaleZ
            );

            if (!buildTask.IsInBackground)
            {
                uiRect.gameObject.SetActive(rectMeta.IsDefaultActive);
            }

            var component = CodeLoader.GetComponent(uiRect);
            if (uiLogicMap.ContainsKey(uiRect.name))
            {
                uiLogicMap[uiRect.name] = component;
            }
            else
            {
                uiLogicMap.Add(uiRect.name, component);
            }

            tempUis.Clear();
            foreach (var sonRef in uiMeta.ComponentRefs)
            {
                if (!uiLogicMap.ContainsKey(sonRef.LogicId))
                {
                    throw new Exception($"视图{bigId}的子组件{sonRef.LogicId}无法找到！");
                }

                var son = uiLogicMap[sonRef.LogicId];
                son.SetParentUi(component);
                tempUis.Add(son);
            }

            var pipelineHandlers = PipelineLoader.GetHandlers(bigId);
            var rxModel = (IYuLegoUIRxModel) ModelLoader.CreateModel(uiRect.name);

            SetViewSonComponentRxModel(rxModel, component);
            component.Construct(uiRect, pipelineHandlers, tempUis);
            // todo IsBindRxModelOnBuild设计原由说明补全
            TryInvokeComponentUILogicer(component);
            if (buildTask.IsBindRxModelOnBuild)
            {
                LegoBinder.Binding(component, uiMeta, rxModel);
                component.ShowDefault();
                if (component.SonComponentDict.Count != 0)
                {
                    BindSonComponentRxmodel(rxModel, component);
                }
            }

            try
            {
                buildTask.UiBuildCallback?.Invoke(component);
            }
            catch(Exception e)
            {
#if DEBUG
                Debug.LogError(e.Message + e.StackTrace);
#endif
            }
        }

        private void BindSonComponentRxmodel(IYuLegoUIRxModel rxModel, ILegoUI component)
        {
            foreach (var sonComponent in component.SonComponentDict)
            {
                rxModel.SonComponentModels.Add(sonComponent.Key, sonComponent.Value.RxModel);
            }
        }

        private void SetViewSonComponentRxModel(IYuLegoUIRxModel rxModel, ILegoUI component)
        {
            rxModel.InitRxModel();
            component.SetRxModel(rxModel);
        }

        private void SetComponentPosition(LegoBuildTask buildTask, RectTransform customParentRect)
        {
            var uiRect = buildTask.RootRect;
            var parentRect = customParentRect == null ? buildTask.ParentRect : customParentRect;
            var rectMeta = buildTask.ComponentMountMeta;

            uiRect.name = rectMeta.Name;
            uiRect.SetParent(parentRect);
            uiRect.gameObject.layer = parentRect.gameObject.layer;

            uiRect.localPosition = new Vector3(
                rectMeta.X,
                rectMeta.Y,
                rectMeta.Z
            );

            uiRect.localScale = new Vector3(
                rectMeta.ScaleX,
                rectMeta.ScaleY,
                rectMeta.ScaleZ
            );

            if (!buildTask.IsInBackground)
            {
                uiRect.gameObject.SetActive(rectMeta.IsDefaultActive);
            }
        }

        #endregion

        private void OnBuilded(LegoBuildTask buildTask)
        {
            ////var bigId = buildTask.RootRect.name;
            ////var lowerId = YuBigAssetIdMap.GetLowerId(bigId);

            ////if (lowerId.StartsWith(LEGO_VIEW))
            ////{
            ////    OnViewBuilded(lowerId, bigId, buildTask);
            ////}
            ////else if (lowerId.StartsWith(LEGO_COMPONENT))
            ////{
            ////    if (buildTask.ComponentMountMeta != null)
            ////    {
            ////        OnSonComponentLoaded(lowerId, buildTask);
            ////    }
            ////    else
            ////    {
            ////        OnSingleComponentLoaded(lowerId, buildTask);
            ////    }
            ////}
            ////else
            ////{
            ////    SetComponentPosition(buildTask, null);
            ////}

            ////TryStartNextTask();
        }

        #endregion

        #region 后台加载 UI

        public void LoadUiInBackground(params string[] ids)
        {
            foreach (var id in ids)
            {
                var task = LegoBuilder.CreateTask(id, OnBuilded, componentPoolRect, 10)
                    .SetBackLoad(true);
                waitTasks.Enqueue(task);
            }

            TryStartNextTask();
        }

        #endregion

        #region 界面组件 Logicer

        private void TryInvokeViewUILogicer(ILegoUI ui)
        {
            var logicer = CodeLoader.GetLogicer(ui.UIRect.name);
            if (logicer == null)
            {
                return;
            }

            var ctx = TakeContext();
            ctx.Init(ui);
            logicer.Init(ctx);
            uiViewLogicerDic.Add(ui.UIRect.name.ToLower(), logicer);
        }

        private void TryInvokeComponentUILogicer(ILegoUI ui)
        {
            var logicer = CodeLoader.GetLogicer(ui.UIRect.name);
            if (logicer == null)
            {
                return;
            }

            var ctx = TakeContext();
            ctx.Init(ui);
            logicer.Init(ctx);
            if (!uiComponentLogicerListsDic.ContainsKey(ui.UIRect.name))
            {
                uiComponentLogicerListsDic.Add(ui.UIRect.name, new List<IYuLegoLogicer>());
            }

            uiComponentLogicerListsDic[ui.UIRect.name].Add(logicer);
        }

        public IYuLegoLogicer GetLogicer(string id)
        {
            string pageId = id.ToLower();
            IYuLegoLogicer logicer = null;
            if (uiViewLogicerDic.TryGetValue(pageId, out logicer))
            {
                return logicer;
            }

            Debug.LogError($"{id} 逻辑脚本尚未创建实例");
            return null;
        }

        #region UI业务处理上下文对象池

        private IGenericObjectPool<IYuLegoLogicContext> contextPool;

        private IGenericObjectPool<IYuLegoLogicContext> ContextPool
        {
            get
            {
                if (contextPool != null)
                {
                    return contextPool;
                }

                contextPool = new GenericObjectPool<IYuLegoLogicContext>(
                    () => new YuLegoLogicContext(), 10);
                return contextPool;
            }
        }

        private IYuLegoLogicContext TakeContext() => ContextPool.Take();

        private void RestoreContext(IYuLegoLogicContext ctx) => ContextPool.Restore(ctx);

        #endregion

        #endregion

        #region 界面深度控制

        private readonly Dictionary<LegoViewType, LinkedList<ILegoView>> viewDepthDict
            = new Dictionary<LegoViewType, LinkedList<ILegoView>>();

        private const float DEPTH_Z_ADD = 100.0f;

        private float GetLastDepthAtViewType(LegoViewType viewType)
        {
            if (!viewDepthDict.ContainsKey(viewType))
            {
                return 0.0f;
            }

            var depthViews = viewDepthDict[viewType];
            if (depthViews.Count == 0)
            {
                return 0.0f;
            }

            var last = depthViews.Last.Value;
            return last.DepthZ;
        }

        private void AddViewToDepthViews(ILegoView view, LegoViewType viewType)
        {
            var layer = LegoUIMounter.uiLayers[viewType];
            layer.PushView(view);

            //if (!viewDepthDict.ContainsKey(viewType))
            //{
            //    viewDepthDict.Add(viewType, new LinkedList<IYuLegoView>());
            //}

            //var depthViews = viewDepthDict[viewType];
            //var lastDepth = GetLastDepthAtViewType(viewType);
            //view.DepthZ = lastDepth - DEPTH_Z_ADD;
            //depthViews.AddLast(view);
        }

        #region 分层Canvas

        private readonly Dictionary<LegoViewType, RectTransform> mountPointDict
            = new Dictionary<LegoViewType, RectTransform>();

        private const string CANVAS_ROOT_PATH = "Yu/UI/";
        private RectTransform componentPoolRect;

        private static readonly List<string> canvasPaths
            = new List<string>
            {
                "LayeredCanvas/StaticBackground",
                "LayeredCanvas/DynamicBackground",
                "LayeredCanvas/StaticMiddle",
                "LayeredCanvas/DynamicMiddle",
                "LayeredCanvas/StaticTop",
                "LayeredCanvas/DynamicTop",
            };

        public YuLegoUILoader()
        {
            InitAllCanvas();
        }

        private void InitAllCanvas()
        {
            var uiRoot = GameObject.Find(CANVAS_ROOT_PATH).transform;

            for (var i = 0; i < canvasPaths.Count; i++)
            {
                var mountType = (LegoViewType) i;
                var path = canvasPaths[i];
                var canvas = uiRoot.Find(path);
                var mountPoint = canvas.Find("UIRoot").GetComponent<RectTransform>();
                mountPointDict.Add(mountType, mountPoint);
            }

            CreateMountLayers(0, 5);


            // 隐藏控件池canvas
            var controlPoolCanvas = uiRoot.Find("ControlPoolCanvas").gameObject;
            controlPoolCanvas.SetActive(false);
            // 隐藏开发用canvas
            var developCanvas = uiRoot.Find("DevelopCanvas").gameObject;
            if (developCanvas != null)
            {
                developCanvas.SetActive(false);
            }

            componentPoolRect = uiRoot.Find("ComponentCacheCanvas").GetComponent<RectTransform>();
            componentPoolRect.gameObject.SetActive(false);
        }

        private YuLegoUILayer CreateMountLayers(int currentLayer, int topLayer)
        {
            var uiRoot = GameObject.Find(CANVAS_ROOT_PATH).transform;
            YuLegoUILayer layer;

            if (currentLayer != topLayer)
            {
                layer = new YuLegoUILayer(mountPointDict[(LegoViewType) currentLayer], 100,
                    CreateMountLayers(currentLayer + 1, topLayer), 500);
            }
            else
            {
                layer = new YuLegoUILayer(mountPointDict[(LegoViewType) topLayer], -100, null, 500);
            }

            LegoUIMounter.uiLayers.Add((LegoViewType) currentLayer, layer);
            return layer;
        }

        #endregion

        #endregion

        #region 其他 UI 操作

        public void CloseView(string id)
        {
            if (!views.ContainsKey(id))
            {
                Debug.LogError($"指定的目标视图{id}不存在，无法关闭");
                return;
            }

            var view = views[id];
            view.Close();
        }

        public bool IsViewActive(string id)
        {
            if (!views.ContainsKey(id))
            {
                return false;
            }

            var view = views[id];
            return view.UIRect.gameObject.activeInHierarchy;
        }

        public void Restore(ILegoComponent component)
        {
            var id = component.Id;

            if (!componentCache.ContainsKey(id))
            {
                componentCache.Add(id, new Queue<ILegoComponent>());
            }

            componentCache[id].Enqueue(component);
            component.UIRect.SetParent(componentPoolRect);
        }

        #endregion
    }
}