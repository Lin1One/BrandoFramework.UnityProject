#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using Common.DataStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图聚合体抽象基类。
    /// 1. 界面
    /// 2. 组件
    /// </summary>
    public abstract class AbsLegoUI :
        ILegoUI,
        IRelease,
        IReset
    {
        #region 字段及属性

        public RectTransform UIRect { get; private set; }
        public string Id => UIRect.name;
        private static readonly LegoMetaHelper metaHelper;

        private LegoControlStorage controlStorage;
        private LegoUIMeta selfMeta;

        public ILegoUI ParentUI { get; private set; }

        public void SetParentUi(ILegoUI parent)
        {
            ParentUI = parent;
        }

        #endregion

        #region 基础控件存取

        /// <summary>
        /// 获得一个目标控件并调用其构造方法。
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetControlAndConstruct<T>(string id) where T : class, ILegoControl
        {
            var control = controlStorage.GetControl<T>(id, UIRect);
            control.Construct(this);
            return control;
        }

        /// <summary>
        /// 获得一个目标控件。
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetControl<T>(string id) where T : class, ILegoControl
        {
            var control = controlStorage.GetControl<T>(id, UIRect);
            return control;
        }

        public List<T> GetControls<T>() where T : class, ILegoControl
        {
            return controlStorage.GetControls<T>();
        }

        #endregion

        #region 容器

        private readonly Dictionary<string, RectTransform> containers
            = new Dictionary<string, RectTransform>();

        public RectTransform GetContainer(string id)
        {
            if (containers.ContainsKey(id))
            {
                return containers[id];
            }

            var targetRect = UIRect.Find(id)?.GetComponent<RectTransform>();
            if (targetRect != null)
            {
                containers.Add(id, targetRect);
                return targetRect;
            }

            throw new Exception($"容器{id}在目标UI物体{Id}下无法找到！");
        }

        #endregion

        #region 基础构造

        public CanvasGroup CanvasGroup { get; private set; }

        /// <summary>
        /// 构造一个乐高UI。
        /// </summary>
        /// <param name="locRect">UI所持有的游戏对象根物体。</param>
        /// <param name="pipeHandlers">UI所对应的所有UI生命周期事件处理器。</param>
        /// <param name="sons"></param>
        /// <param name="isInBack"></param>
        public void Construct
        (
            RectTransform locRect,
            List<IYuLegoUIPipelineHandler> pipeHandlers,
            List<ILegoUI> sons = null,
            bool isInBack = false
        )
        {
            UIRect = locRect;
            isInBackground = isInBack;
            selfMeta = metaHelper.GetMeta(UIRect);
            TryAddCanvasGroup();
            TryAddAllSonComponent(sons);
            InitPipelineHandlers(pipeHandlers); // 生命周期处理器
            InitExternalPipeline();             // 外部生命周期
            controlStorage = LegoControlStorage.StoragePool.Take();
            GetOperateControl();
            TryAddRootButton();
            RegisterHandler();
            RegisterHandlerExtend();
            OnCreated();
        }

        private Dictionary<string, ILegoUI> sonComponents;

        public Dictionary<string, ILegoUI> SonComponentDict
        {
            get
            {
                if (sonComponents != null)
                {
                    return sonComponents;
                }

                sonComponents = new Dictionary<string, ILegoUI>();
                return sonComponents;
            }
        }

        private void TryAddAllSonComponent(List<ILegoUI> components)
        {
            foreach (var component in components)
            {
                SonComponentDict.Add(component.Id, component);
            }
        }

        #region 子组件

        public ILegoUI GetSonComponent(string id)
        {
            if (!SonComponentDict.ContainsKey(id))
            {
#if DEBUG
                Debug.LogError($"目标子组件{id}在当前UI{Id}下不存在！");
#endif
                return null;
            }

            var component = SonComponentDict[id];
            return component;
        }

        private List<ILegoUI> sonCompoents;

        private List<ILegoUI> SonComponents
        {
            get
            {
                if (sonCompoents != null)
                {
                    return sonCompoents;
                }

                sonCompoents = SonComponentDict.Values.ToList();
                return sonCompoents;
            }
        }

        public ILegoUI GetSonComponent(int index)
        {
#if UNITY_EDITOR
            if (index >= SonComponents.Count)
            {
                throw new Exception($"组件{Id}下没有索引为{index}的子组件！");
            }
#endif

            return SonComponents[index];
        }

        public T GetSonComponent<T>(int index) where T : ILegoUI
        {
            var targetComponents = SonComponents.FindAll(c => c.GetType() == typeof(T));
            if (targetComponents.Count == 0)
            {
                throw new Exception($"组件{Id}下没有类型为{typeof(T).Name}的子组件！");
            }

            if (index >= targetComponents.Count)
            {
                throw new Exception($"类型{typeof(T).Name}的组件中没有索引为{index}的子组件！");
            }

            var targetSon = (T)targetComponents[index];
            return targetSon;
        }

        #endregion

        private void TryAddCanvasGroup()
        {
            CanvasGroup = UIRect.GetComponent<CanvasGroup>();
            if (CanvasGroup == null)
            {
                CanvasGroup = UIRect.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void InitPipelineHandlers(List<IYuLegoUIPipelineHandler> handlers)
        {
            if (handlers == null)
            {
                return;
            }

            foreach (var handler in handlers)
            {
                if (!pipelineHandlers.ContainsKey(handler.PipelineType))
                {
                    pipelineHandlers.Add(handler.PipelineType, new List<IYuLegoUIPipelineHandler>());
                }

                var mapHandlers = pipelineHandlers[handler.PipelineType];
                mapHandlers.Add(handler);
            }
        }

        public void Show()
        {
            BeforeShow();
            //CanvasGroup.alpha = 1f;
            UIRect.gameObject.SetActive(true);
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            AfterShow();
        }

        public void Close()
        {
            BeforeClose();
            //CanvasGroup.alpha = 0;
            UIRect.gameObject.SetActive(false);
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            AfterClose();
        }

        public void Hide()
        {
            BeforeHide();
            AfterHide();
        }

        private bool isInBackground;

        /// <summary>
        /// 初始化后第一次调用Show。
        /// 判断是否后台加载，如果是则跳出执行。
        /// </summary>
        private void InitShow()
        {
            if (isInBackground)
            {
                return;
            }

            ShowDefault();
        }

        /// <summary>
        /// 组件或View加载完成后设置默认显示
        /// </summary>
        public void ShowDefault()
        {
            if (!selfMeta.RootMeta.IsDefaultActive)
            {
                return;
            }

            BeforeShow();
            CanvasGroup.alpha = 1f;
            UIRect.gameObject.SetActive(true);
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            AfterShow();
        }

        #endregion

        #region 静态构造

        static AbsLegoUI()
        {
            metaHelper = Injector.Instance.Get<LegoMetaHelper>();
        }

        #endregion

        #region 生命周期

        private readonly Dictionary<UIPipelineType, List<IYuLegoUIPipelineHandler>> pipelineHandlers
            = new Dictionary<UIPipelineType, List<IYuLegoUIPipelineHandler>>();

        private void InvokePipeline(UIPipelineType pipeline)
        {
            if (pipelineHandlers.Count == 0)
            {
                return;
            }

            if (!pipelineHandlers.ContainsKey(pipeline))
            {
                return;
            }

            var handlers = pipelineHandlers[pipeline];
            if (handlers.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var handler in handlers)
                {
                    handler.Execute(this);
                }
            }
            catch(Exception  e)
            {
#if DEBUG
                Debug.LogError(e.Message + e.StackTrace);
#endif
            }
        }

        #region 外部生命周期

        private void InitExternalPipeline()
        {

        }


        #endregion

        #region 生命周期函数

        protected virtual void OnCreated()
        {
            InvokePipeline(UIPipelineType.OnCreated);
        }

        protected virtual void BeforeShow()
        {
            InvokePipeline(UIPipelineType.BeforeShow);
        }

        protected virtual void AfterShow()
        {
            InvokePipeline(UIPipelineType.AfterShow);
        }

        protected virtual void BeforeHide()
        {
            InvokePipeline(UIPipelineType.BeforeHide);
        }

        protected virtual void AfterHide()
        {
            InvokePipeline(UIPipelineType.AfterHide);
        }

        protected virtual void BeforeClose()
        {
            InvokePipeline(UIPipelineType.BeforeClose);
        }

        protected virtual void AfterClose()
        {
            InvokePipeline(UIPipelineType.AfterClose);
        }

        #endregion

        #endregion

        #region 释放及清理

        public virtual void Release()
        {
            controlStorage.Release();
            LegoControlStorage.StoragePool.Restore(controlStorage);
        }

        #endregion

        #region 子类重载API

        protected abstract void GetOperateControl();

        protected abstract void RegisterHandler();

        /// <summary>
        /// 该函数提供给业务开发人员扩展事件处理器注册流程。
        /// 可以在该函数中注册一些不常用的交互事件处理器，例如：OnDrag之类。
        /// </summary>
        protected virtual void RegisterHandlerExtend()
        {

        }

        #endregion

        #region 数据模型

        public IYuLegoUIRxModel RxModel { get; private set; }

        public void SetRxModel(IYuLegoUIRxModel rxModel)
        {

            RxModel = rxModel;
            RxModel.MapUI = this;
        }

        #endregion

        #region 重置自身以重用

        public virtual void Reset()
        {
        }

        #endregion

        #region 根按钮

        private YuLegoButton rootButton;
        private static readonly YuLegoUIRootClickHandler defaultRootClickHandler
            = new YuLegoUIRootClickHandler();

        private void TryAddRootButton()
        {
            if (rootButton != null)
            {
                return;
            }

            rootButton = UIRect.Find("Button_Root")?.GetComponent<YuLegoButton>();
            if (rootButton == null)
            {
                return;
            }

            rootButton.gameObject.layer = UIRect.gameObject.layer;

            if (!selfMeta.IsBlankClose)
            {
                rootButton.gameObject.SetActive(false);
                return;
            }

            rootButton.Construct(this);
            //            defaultRootClickHandler.OperateUi = this;
            rootButton.RegisterHandler(LegoInteractableType.OnPointerClick, defaultRootClickHandler);
            rootButton.SonText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置空白区域点击处理器
        /// </summary>
        /// <param name="clickHandler"></param>
        public void SetRootButtonHandler(IYuLegoActionHandler clickHandler)
        {
            if (rootButton == null)
            {
                return;
            }
            rootButton.gameObject.SetActive(true);
            rootButton.RegisterHandler(LegoInteractableType.OnPointerClick, clickHandler);
        }

        private class YuLegoUIRootClickHandler : IYuLegoActionHandler
        {
            //            public IYuLegoUI OperateUi { get; set; }

            public void Execute(object legoControl)
            {
                legoControl.As<ILegoControl>().LocUI.Close();
            }
        }

        #endregion

        public virtual void SkipToUI(int uiIndex)
        {

        }
    }
}