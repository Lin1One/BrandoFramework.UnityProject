#region Head

// Author:            Yu
// CreateDate:        2018/11/8 6:19:20
// Email:             836045613@qq.com

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
    public class LegoUILoader
    {
        public LegoUILoader()
        {
            InitAllCanvas();
        }

        #region 字段

        #region 依赖注入引用

        private IU3dInjector injector;

        private IU3dInjector m_Injector =>
            injector ?? (injector = U3dInjector.MonoInjectorInstance.As<IU3dInjector>());

        private LegoBuilder legoBuilder;
        /// <summary>
        /// LegoUI 构建器，负责在帧循环中完成构建任务，并调用回调
        /// </summary>
        private LegoBuilder LegoBuilder =>
            legoBuilder ? legoBuilder : (legoBuilder = m_Injector.GetMono<LegoBuilder>());

        private LegoMetaHelper metaHelper;
        /// <summary>
        /// UI LegoMeta 助手，负责获取特定 UI 的元数据
        /// </summary>
        private LegoMetaHelper MetaHelper =>
            metaHelper ?? (metaHelper = m_Injector.Get<LegoMetaHelper>());

        private LegoUICodeInstanceLoader codeLoader;
        /// <summary>
        /// UI 脚本获取器，负责获取 UI 对应脚本（LegoView，Components，Logicer，反射创建）
        /// </summary>
        private LegoUICodeInstanceLoader CodeLoader =>
            codeLoader ?? (codeLoader = m_Injector.Get<LegoUICodeInstanceLoader>());

        private LegoRxModelLoader modelLoader;
        /// <summary>
        /// UI 数据模型加载器，负责获取 UI 对应的数据模型（反射创建实例/读取 Json）
        /// </summary>
        private LegoRxModelLoader ModelLoader => modelLoader ?? (modelLoader = m_Injector.Get<LegoRxModelLoader>());

        private LegoUIPipelineLoader pipelineLoader;
        /// <summary>
        /// UI 生命周期处理脚本加载器，负责获取 UI 对应的生命周期处理脚本实例（反射创建实例）
        /// </summary>
        private LegoUIPipelineLoader PipelineLoader =>
            pipelineLoader ?? (pipelineLoader = m_Injector.Get<LegoUIPipelineLoader>());

        
        private LegoBinder legoBinder;
        /// <summary>
        /// UI 数据模型绑定器，负责绑定数据模型与 UI 类型实例中的各控件
        /// </summary>
        private LegoBinder LegoBinder =>
            legoBinder ?? (legoBinder = m_Injector.Get<LegoBinder>());

        private LegoUIMounter legoUIMounter;
        /// <summary>
        /// UI 深度管理器，负责UI界面在 Canvas 中的深度控制
        /// </summary>
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

        private readonly Dictionary<string, IViewLogic> uiViewLogicerDic // UI逻辑类实例字典
            = new Dictionary<string, IViewLogic>();

        // UI逻辑类实例(用于同一组件有多个实例)
        private readonly Dictionary<string, List<IViewLogic>> uiComponentLogicerListsDic 
            = new Dictionary<string, List<IViewLogic>>();

        private const string LEGO_VIEW = "legoview";
        private const string LEGO_COMPONENT = "legocomponent";

        #endregion

        #region 异步加载 UI 

        private readonly Queue<LegoBuildTask> waitTasks = new Queue<LegoBuildTask>();

        private readonly List<ILegoUI> tempUis = new List<ILegoUI>();

        public void WaitUi( string id,
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
            ILegoView view = CodeLoader.GetView(uiRect);
            views.Add(lowerId, view);
            var uiMeta = MetaHelper.GetMeta(uiRect);

            //绑定子组件
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

            //数据模型与 UI 类型实例绑定
            var rxModel = (IYuLegoUIRxModel) ModelLoader.LoadModel(bigId);
            SetViewRxModel(rxModel, view);

            //UI 实体Rect，周期管理类型，子组件与 UI 脚本类型实例绑定
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

            //数据模型数据与 UI 脚本类型实例绑定
            LegoBinder.Binding(view, uiMeta, rxModel);

            //构建完成回调
            buildTask.UiBuildCallback?.Invoke(view);

            //触发界面创建完成事件
            U3DEventModule.TriggerEvent(ProjectCoreEventCode.View_Created, view , null);
            view.ShowDefault();
        }

        /// <summary>
        /// 绑定数据模型与脚本
        /// </summary>
        /// <param name="rxModel"></param>
        /// <param name="view"></param>
        private void SetViewRxModel(IYuLegoUIRxModel rxModel, ILegoUI view)
        {
            rxModel.InitRxModel();
            view.SetRxModel(rxModel);
        }

        #endregion

        #region 加载 Component

        private void WaitComponent( string id,
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
            var bigId = buildTask.RootRect.name;
            var lowerId = bigId;//// YuBigAssetIdMap.GetLowerId(bigId);

            if (lowerId.StartsWith(LEGO_VIEW))
            {
                OnViewBuilded(lowerId, bigId, buildTask);
            }
            else if (lowerId.StartsWith(LEGO_COMPONENT))
            {
                if (buildTask.ComponentMountMeta != null)
                {
                    OnSonComponentLoaded(lowerId, buildTask);
                }
                else
                {
                    OnSingleComponentLoaded(lowerId, buildTask);
                }
            }
            else
            {
                SetComponentPosition(buildTask, null);
            }

            TryStartNextTask();
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

        public IViewLogic GetLogicer(string id)
        {
            string pageId = id.ToLower();
            IViewLogic logicer = null;
            if (uiViewLogicerDic.TryGetValue(pageId, out logicer))
            {
                return logicer;
            }

            Debug.LogError($"{id} 逻辑脚本尚未创建实例");
            return null;
        }

        private void TryInvokeViewUILogicer(ILegoUI ui)
        {
            var logicer = CodeLoader.GetLogic(ui.UIRect.name);
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
            var logicer = CodeLoader.GetLogic(ui.UIRect.name);
            if (logicer == null)
            {
                return;
            }

            var ctx = TakeContext();
            ctx.Init(ui);
            logicer.Init(ctx);
            if (!uiComponentLogicerListsDic.ContainsKey(ui.UIRect.name))
            {
                uiComponentLogicerListsDic.Add(ui.UIRect.name, new List<IViewLogic>());
            }

            uiComponentLogicerListsDic[ui.UIRect.name].Add(logicer);
        }

        #region UI业务处理上下文对象池

        private IGenericObjectPool<IViewLogicContext> contextPool;

        private IGenericObjectPool<IViewLogicContext> ContextPool
        {
            get
            {
                if (contextPool != null)
                {
                    return contextPool;
                }

                contextPool = new GenericObjectPool<IViewLogicContext>(
                    () => new ViewLogicContext(), 10);
                return contextPool;
            }
        }

        private IViewLogicContext TakeContext() => ContextPool.Take();

        private void RestoreContext(IViewLogicContext ctx) => ContextPool.Restore(ctx);

        #endregion

        #endregion

        #region 界面深度控制

        private void AddViewToDepthViews(ILegoView view, LegoViewType viewType)
        {
            var layer = LegoUIMounter.uiLayers[viewType];
            layer.PushView(view);
        }

        #region 分层Canvas

        private readonly Dictionary<LegoViewType, RectTransform> mountPointDict
            = new Dictionary<LegoViewType, RectTransform>();

        private const string CANVAS_ROOT_PATH = "GameRoot/UIRoot/";
        private RectTransform componentPoolRect;

        private static readonly List<string> canvasPaths
            = new List<string>
            {
                "UICanvas/StaticBackground",
                "UICanvas/DynamicBackground",
                "UICanvas/StaticMiddle",
                "UICanvas/DynamicMiddle",
                "UICanvas/StaticTop",
                "UICanvas/DynamicTop",
            };

        /// <summary>
        /// 初始化 Canvas
        /// </summary>
        private void InitAllCanvas()
        {
            var uiRoot = GameObject.Find(CANVAS_ROOT_PATH).transform;

            for (var i = 0; i < canvasPaths.Count; i++)
            {
                var mountType = (LegoViewType) i;
                var path = canvasPaths[i];
                var canvas = uiRoot.Find(path);
                var mountPoint = canvas.GetComponent<RectTransform>();
                mountPointDict.Add(mountType, mountPoint);
            }

            CreateMountLayers(0, 5);


            // 隐藏控件池canvas
            var controlPoolCanvas = uiRoot.Find("ControlPoolCanvas").gameObject;
            controlPoolCanvas.SetActive(false);
            // 隐藏开发用canvas
            var developCanvas = uiRoot.Find("DevelopCanvas").gameObject;
            developCanvas.SetActive(false);

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