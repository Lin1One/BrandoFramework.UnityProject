#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using Common;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YuU3dPlay;

namespace Client.LegoUI
{
    public class YuLegoScrollView : MonoBehaviour, IYuLegoScrollView
    {
        #region 字段

        //布局模式
        public YuArrangement Arrangement { get; set; } = YuArrangement.Vertical;

        private bool isMultiLayout;     //是否多行多列布局
        private int maxPerLine = 1;
        private int horizontalPadiding = 5;
        private int verticalPadiding = 5;
        private int xOffset;            //X 轴方向偏移
        private int itemWidth;
        private int itemHeight;
        private int maxIndexOffset; // 允许移动的最大偏移单位
        private int loopRequireNum;
        private int lastIndex = 0;
        private int minVisibleLine; // 可见的最小行数

        private static LegoScrollViewPipelineRouter pipelineRouter;

        private static LegoScrollViewPipelineRouter PipelineRouter
        {
            get
            {
                if (pipelineRouter != null)
                {
                    return pipelineRouter;
                }

                pipelineRouter = Injector.Instance.Get<LegoScrollViewPipelineRouter>();
                return pipelineRouter;
            }
        }

        private RectTransform content;

        /// <summary>
        /// ScrollView 滚动视图内各子项的容器父物体
        /// </summary>
        public RectTransform Content
        {
            get
            {
                if (content != null)
                {
                    return content;
                }

                content = RectTransform.Find("ScrollRect/Content")
                    .RectTransform();
                return content;
            }
        }

        /// <summary>
        /// 列表循环所需创建的最小组件数量。
        /// </summary>
        private int RequireCount
        {
            get
            {
                var requireCount = loopRequireNum >= ModelCount ? ModelCount : loopRequireNum;
                return requireCount;
            }
        }

        private readonly LinkedList<ILegoComponent> components
            = new LinkedList<ILegoComponent>();

        public IYuLegoScrollViewRxModel ScRxModel { get; private set; }

        /// <summary>
        /// 与乐高视图控件所绑定的数据模型
        /// </summary>
        public IYuLegoControlRxModel ControlRxModel { get; set; }

        private int ModelCount => ScRxModel.ModelCount;

        /// <summary>
        /// 当前向视图模块所请求的组件构建任务数。
        /// 由于时异步模式，因此会出现当前子组件数量为0而所需的组件构建任务都已发出。
        /// 这时候如果只判断当前子组件数量就会导致重复提交组件构建任务
        /// 从而使构建任务发生堆叠错误导致后续出现索引超出范围异常。
        /// </summary>
        private int requestTaskCount;

        private LegoUIMeta componentMeta;

        private bool isInitDone = false;
        private bool isCalculated; // 是否已计算过循环最小子项数量

        #endregion

        #region 快捷及缓存属性

        private static readonly StringBuilder sb
            = new StringBuilder();

        private LegoBinder legoBinder;

        private LegoBinder LegoBinder
        {
            get
            {
                if (legoBinder != null)
                {
                    return legoBinder;
                }

                legoBinder =Injector.Instance.Get<LegoBinder>();
                return legoBinder;
            }
        }

        private LegoMetaHelper metaHelper;

        private LegoMetaHelper MetaHelper
        {
            get
            {
                if (metaHelper != null)
                {
                    return metaHelper;
                }

                metaHelper = Injector.Instance.Get<LegoMetaHelper>();
                return metaHelper;
            }
        }

        private ILegoViewModule viewModule;

        private ILegoViewModule ViewModule
        {
            get
            {
                if (viewModule != null)
                {
                    return viewModule;
                }

                viewModule = Injector.Instance.Get<ILegoViewModule>();
                return viewModule;
            }
        }

        private string componentId;

        private string ComponentId
        {
            get
            {
                if (componentId != null)
                {
                    return componentId;
                }

                sb.Clear();
                sb.Append("LegoComponent_");
                var index = name.LastIndexOf('=');
                sb.Append(name.Substring(index + 1, name.Length - (index + 1)));
                componentId = sb.ToString();
                return componentId;
            }
        }

        #endregion

        #region 滑动回调，循环移动子组件

        /// <summary>
        /// 滑动处理回调
        /// </summary>
        /// <param name="pos"></param>
        private void OnValueChanged(Vector2 pos)
        {
            if (components.Count < RequireCount)
            {
                return;
            }
            OnValueChange();
        }

        public void ResetToZero()
        {
            ScrollRect.StopMovement();
            Content.anchoredPosition = Vector2.zero;
            OnValueChange();
        }

        public void ResetToLast()
        {
            if(ModelCount < loopRequireNum)
            {
                return;
            }
            ScrollRect.StopMovement();
            var itemOffset = Arrangement == YuArrangement.Horizontal ?
              new Vector2(itemWidth + horizontalPadiding, 0) :
              new Vector2(0, itemHeight + verticalPadiding);

            //因 content 锚点位置位于左上，需再加 0.5 的偏移
            Content.anchoredPosition = itemOffset * (maxIndexOffset + 0.5f);
            OnValueChange();
        }

        private void OnValueChange()
        {
            if (content.transform.childCount == 0)
            {
                return;
            }

            var index = GetItemIndexAtCurrentPosition();
            // 位移了超过一个子项的单位（宽度或高度）
            if (lastIndex != index && index > -1 && index < maxIndexOffset)
            {
                if (index > lastIndex)
                {
                    var loopNum = index - lastIndex;

                    for (var i = 0; i < loopNum; i++)
                    {
                        if (components.Last.Value.ScrollViewId < ModelCount - 1)
                        {
                            MoveFirst();
                        }
                    }

                    lastIndex = index;
                    return;
                }

                if (lastIndex > index)
                {
                    var loopNum = lastIndex - index;

                    for (var i = 0; i < loopNum; i++)
                    {
                        if (components.First.Value.ScrollViewId > 0)
                        {
                            MoveLast();
                        }
                    }
                    lastIndex = index;
                }
            }

            //当滑动速度过快，index 直接从为正数滑到小于 -1，或超过 maxIndexOffset
            //在滑动至两侧做特殊判断处理
            //当滑动至 小于 -1 时，等待回弹至 -1 然后进行补齐
            else if (index == -1 && lastIndex != 0)
            {
                var loopNum = lastIndex - index + 1;

                for (var i = 0; i < loopNum; i++)
                {
                    if (components.First.Value.ScrollViewId > 0)
                    {
                        MoveLast();
                    }
                }
                lastIndex = 0;
            }

            //当滑动至 大于 maxIndexOffset 时，等待回弹至 maxIndexOffset 然后进行补齐
            else if (index == maxIndexOffset && lastIndex != maxIndexOffset - 1)
            {
                var loopNum = index - lastIndex;
                for (var i = 0; i < loopNum; i++)
                {
                    if (components.Last.Value.ScrollViewId < ModelCount - 1)
                    {
                        MoveFirst();
                    }
                }
                lastIndex = maxIndexOffset - 1;
            }
        }
        /// <summary>
        /// 将最前面一项作为后排补齐
        /// </summary>
        private void MoveFirst()
        {
            if (isMultiLayout)
            {
                MoveFirstAtMultiLayout();
            }
            else
            {
                MoveFirstAtSingleLayout();
            }
        }

        /// <summary>
        /// 将最后面一项作为前排补齐
        /// </summary>
        private void MoveLast()
        {
            if (isMultiLayout)
            {
                MoveLastAtMultiLayout();
            }
            else
            {
                MoveLastAtSingleLayout();
            }
        }

        private void MoveFirstAtSingleLayout()
        {
            var last = components.Last;
            var first = components.First;
            first.Value.ScrollViewId = last.Value.ScrollViewId + 1;
            components.RemoveFirst();
            components.AddLast(first);
            UpdateComponentEvery(first.Value);
            DrawComponent(first.Value);
        }

        private void MoveFirstAtMultiLayout()
        {
            for (int i = 0; i < maxPerLine; i++)
            {
                //if (components.Last.Value.ScrollViewId == ModelCount - 1)
                //{
                //    return;
                //}

                var first = components.First;
                var remainder = first.Value.ScrollViewId % maxPerLine;
                if (remainder == maxPerLine)
                {
                    return;
                }
                var last = components.Last;
                first.Value.ScrollViewId = last.Value.ScrollViewId + 1;
                components.RemoveFirst();
                components.AddLast(first);

                if (first.Value.ScrollViewId >= ModelCount)
                {
                    first.Value.UIRect.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    UpdateComponentEvery(first.Value);
                    DrawComponent(first.Value);
                }
            }
        }

        private void MoveLastAtSingleLayout()
        {
            var last = components.Last;
            var first = components.First;
            last.Value.ScrollViewId = first.Value.ScrollViewId - 1;
            components.RemoveLast();
            components.AddFirst(last);
            UpdateComponentEvery(last.Value);
            DrawComponent(last.Value);
        }

        private void MoveLastAtMultiLayout()
        {
            for (int i = 0; i < maxPerLine; i++)
            {
                if (components.First.Value.ScrollViewId == 0)
                {
                    return;
                }

                var last = components.Last;
                var remainder = last.Value.ScrollViewId % maxPerLine;
                if (remainder == maxPerLine)
                {
                    return;
                }

                var first = components.First;
                last.Value.ScrollViewId = first.Value.ScrollViewId - 1;
                components.RemoveLast();
                components.AddFirst(last);
                UpdateComponentEvery(last.Value);
                DrawComponent(last.Value);
            }
        }

        /// <summary>
        /// 获得Content当前位置的子项索引。
        /// </summary>
        /// <returns></returns>
        private int GetItemIndexAtCurrentPosition()
        {
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    return Mathf.FloorToInt(Content.anchoredPosition.x / -(itemWidth + horizontalPadiding));
                case YuArrangement.Vertical:
                    return Mathf.FloorToInt(Content.anchoredPosition.y / (itemHeight + verticalPadiding));
            }
            return 0;
        }

        #endregion

        #region 控制子项方法

        private void UpdateComponentAtInit(ILegoComponent component)
        {
            component.UIRect.SetParent(Content);
            component.UIRect.AsLeftTop();
            component.UIRect.pivot = new Vector2(0f, 1f);
            component.UIRect.localScale = Vector3.one;
            component.UIRect.sizeDelta = new Vector2(componentMeta.RootMeta.Width, componentMeta.RootMeta.Height);
            component.UIRect.gameObject.layer = Content.gameObject.layer;
            UpdateComponentEvery(component);
        }

        private void UpdateComponentEvery(ILegoComponent component)
        {
            var componentPosition = GetItemPosition(component.ScrollViewId);
            component.UIRect.localPosition = componentPosition;
            component.UIRect.gameObject.SetActive(true);
#if DEBUG
            component.UIRect.name = componentId + "_" + component.ScrollViewId;
#endif
        }

        private Vector3 GetItemPosition(int index)
        {
            var position = isMultiLayout
                ? GetItemPositionAtMultiLayout(index)
                : GetItemPositionAtSingleLayout(index);
            return position;
        }

        private Vector3 GetItemPositionAtSingleLayout(int index)
        {
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    return new Vector3(index * (itemWidth + horizontalPadiding), 0f, 0f);
                case YuArrangement.Vertical:
                    return new Vector3(xOffset, index * -(itemHeight + verticalPadiding), 0f);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Vector3 GetItemPositionAtMultiLayout(int index)
        {
            Vector3 tmpPosition;

            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    tmpPosition = new Vector3(itemWidth * (index / maxPerLine),
                        -(itemHeight + horizontalPadiding) * (index % maxPerLine), 0f);
                    break;
                case YuArrangement.Vertical:
                    var lineIndex = index / maxPerLine;
                    var rowIndex = index % maxPerLine;
                    tmpPosition = new Vector3(itemWidth * rowIndex + xOffset + rowIndex * horizontalPadiding,
                        -itemHeight * lineIndex - verticalPadiding * lineIndex, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return tmpPosition;
        }

        private void OnComponentLoaded(ILegoUI legoui)
        {
            legoui.SetParentUi(LocUI);
            ILegoComponent component = (ILegoComponent)legoui;
            if (componentMeta == null)
            {
                var bigId = legoui.UIRect.name;
                var lowerId = "";//YuBigAssetIdMap.GetLowerId(bigId);
                var uiMeta = metaHelper.GetMeta(lowerId);
                componentMeta = uiMeta;
            }
            component.ScrollViewId = components.Count; // 设置组件的滚动列表子项索引
            UpdateComponentAtInit(component);
            components.AddLast(component);
            DrawComponent(component);
            UpdateMaxIndexOffset();
            UpadteContentSize();

            // 调用外部委托
            onComponentBuilded?.Invoke(component);

            if (requestTaskCount > 0)
            {
                if (--requestTaskCount == 0)
                {
                    onReplaceComponents?.Invoke(components);
                    RxModelReplaceComplished();
                }
            }
        }

        private void DrawComponent(ILegoComponent component)
        {
            var uiRxModel = ScRxModel.GetRxModel(component.ScrollViewId);
            if(uiRxModel == null)
            {
#if DEBUG
                string name = null;
                if(component.UIRect != null)
                {
                    name = component.UIRect.name;
                }
                Debug.LogError("DrawComponent错误，uiRxModel为null：  " + name
                   + "  " + component.ScrollViewId);
#endif
                return;
            }

            LegoBinder.Binding(component, componentMeta, uiRxModel);
            component.SetRxModel(uiRxModel);
            if (uiRxModel.SonComponentModels.Count != 0)
            {
                foreach (var sonComponent in component.SonComponentDict)
                {
                    var bigId = sonComponent.Key;
                    var lowerId = "";//YuBigAssetIdMap.GetLowerId(bigId);
                    LegoUIMeta sonComponentMeta = metaHelper.GetMeta(lowerId);
                    IYuLegoUIRxModel sonComponentRxModel = uiRxModel.SonComponentModels[bigId];
                    
                    LegoBinder.Binding(sonComponent.Value, sonComponentMeta, sonComponentRxModel);
                    sonComponent.Value.SetRxModel(sonComponentRxModel);
                }
            }
            onDrawComponent?.Invoke(component, uiRxModel);
        }

        private void RestoreComponent()
        {
            var last = components.Last;
            ViewModule.Restore(last.Value);
            components.RemoveLast();
        }

        private void ReDrawAllComponents()
        {
            if (components.First != null && components.First.Value.ScrollViewId != 0)
            {
                ResetComponentsScrollViewId();
            }
            foreach (var component in components)
            {
                DrawComponent(component);
                UpdateComponentAtInit(component);
            }
        }

        private void ResetComponentsScrollViewId()
        {
            var item = components.First;
            for (int i = 0; i < components.Count; i++)
            {
                item.Value.ScrollViewId = i;
                item = item.Next;
            }
        }


        private Action<IYuLegoUIRxModel> onComponentRxModelAdd;
        private Action<ILegoComponent> onComponentBuilded;
        private Action<LinkedList<ILegoComponent>> onReplaceComponents;
        private Action<ILegoComponent, IYuLegoUIRxModel> onDrawComponent;

        #endregion

        #region 控制 ScrollRect 方法

        private void InitScrollRect()
        {
            ScrollRect.onValueChanged.AddListener(OnValueChanged);
            ScrollRect.horizontal = scrollViewMeta.HorizontalEnable;
            ScrollRect.vertical = scrollViewMeta.VerticalEnable;
        }

        /// <summary>
        /// 重新设置滑动范围
        /// </summary>
        private void UpadteContentSize()
        {
            if (isMultiLayout)
            {
                UpdateContentSizeAtMultiLayout();
            }
            else
            {
                UpdateContentSizeAtSingleLayout();
            }
        }

        private void UpdateContentSizeAtSingleLayout()
        {
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    int contentWidth = itemWidth * ModelCount +(horizontalPadiding * (ModelCount - 1));
                    Content.sizeDelta = new Vector2(
                        contentWidth - ScrollRectRect.sizeDelta.x,
                        itemHeight
                        );
                    break;
                case YuArrangement.Vertical:
                    Content.sizeDelta = new Vector2(Content.sizeDelta.x,
                        itemHeight * ModelCount + verticalPadiding * (ModelCount - 1));
                    break;
            }
        }

        private void UpdateContentSizeAtMultiLayout()
        {
            var lineCount = Mathf.CeilToInt((float)ModelCount / maxPerLine);
            var columnCount = Mathf.CeilToInt((float)ModelCount % maxPerLine);
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    Content.sizeDelta = new Vector2(itemWidth * lineCount + horizontalPadiding * (lineCount - 1),
                        Content.sizeDelta.y);
                    break;
                case YuArrangement.Vertical:
                    Content.sizeDelta = new Vector2(Content.sizeDelta.x + (columnCount - 1) * horizontalPadiding,
                        itemHeight * lineCount + verticalPadiding * (lineCount - 1));
                    break;
            }
        }

        private void UpdateMaxIndexOffset()
        {
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    maxIndexOffset = ModelCount - components.Count + 1;
                    break;
                case YuArrangement.Vertical:
                    if (isMultiLayout)
                    {
                        maxIndexOffset = ModelCount % maxPerLine == 0 ?
                           ModelCount / maxPerLine - minVisibleLine :
                           ModelCount / maxPerLine - minVisibleLine + 1;

                    }
                    else
                    {
                        maxIndexOffset = ModelCount - components.Count + 1;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 乐高视图控件接口实现

        public ILegoUI LocUI { get; private set; }
        private RectTransform selfRect;

        public RectTransform RectTransform
        {
            get
            {
                if (selfRect != null)
                {
                    return selfRect;
                }

                selfRect = GetComponent<RectTransform>();
                return selfRect;
            }
        }

        public LegoRectTransformMeta RectMeta { get; private set; }

        private GameObject selfGo;

        public GameObject GameObject
        {
            get
            {
                if (selfGo != null)
                {
                    return selfGo;
                }

                selfGo = RectTransform.gameObject;
                return selfGo;
            }
        }

        public string Name => gameObject.name;

        public void Construct(ILegoUI locUI, object obj = null)
        {
#if DEBUG
            if (Content.childCount > 0)
            {
                Content.DeleteAllChild();
            }
#endif
            LocUI = locUI;

            //生命周期
            var pipelineHandlerList = PipelineRouter.GetHandlers(RectTransform.name.Split('@')[0]);
            InitPipelineHandlerDic(pipelineHandlerList);

            //读取滚动视图元数据初始化控件基本设置
            var locUIMeta = MetaHelper.GetMeta(transform.parent.name);
            scrollViewMeta = locUIMeta.GetScrollViewMeta(name);

            if (scrollViewMeta.IsNotInitSonComponent)
            {
                return;
            }
            
            //初始化滚动视图数据模型相关操作
            InitScrollViewRxModel();
            //初始化滚动视图交互操作
            InitMetaInfo(scrollViewMeta);
            InitScrollRect();

            //添加初始子项
            TryRequestBuildComponent();
            if (RequireCount == 0)
            {
                TryInvokeOnCreated();
            }
        }

        public IYuLegoScrollView OnAddComponentRxmodel(Action<IYuLegoUIRxModel> callback)
        {
            onComponentRxModelAdd = callback;
            return this;
        }

        public IYuLegoScrollView OnAddComponent(Action<ILegoComponent> callback)
        {
            onComponentBuilded = callback;
            return this;
        }

        public IYuLegoScrollView OnReplaceComponents(Action<LinkedList<ILegoComponent>> callback)
        {
            onReplaceComponents = callback;
            return this;
        }

        public IYuLegoScrollView OnDrawComponent(Action<ILegoComponent, IYuLegoUIRxModel> onDraw)
        {
            onDrawComponent = onDraw;
            return this;
        }
        #endregion

        #region ScrollView生命周期管理

        private Dictionary<LegoScrollViewPipelineType, List<IYuLegoScrollViewPipelineHandler>> pipelineHandlerDic
            = new Dictionary<LegoScrollViewPipelineType, List<IYuLegoScrollViewPipelineHandler>>();

        private void InitPipelineHandlerDic(List<IYuLegoScrollViewPipelineHandler> handlerList)
        {
            if (handlerList == null)
            {
                return;
            }

            foreach (var handler in handlerList)
            {
                if (!pipelineHandlerDic.ContainsKey(handler.PipelineType))
                {
                    pipelineHandlerDic.Add(handler.PipelineType, new List<IYuLegoScrollViewPipelineHandler>());
                }

                var mapHandlers = pipelineHandlerDic[handler.PipelineType];
                mapHandlers.Add(handler);
            }
        }

        private void InvokePipelineAction(LegoScrollViewPipelineType pipeline)
        {
            if (pipelineHandlerDic != null && pipelineHandlerDic.ContainsKey(pipeline))
            {
                var actions = pipelineHandlerDic[pipeline];
                foreach (var action in actions)
                {
                    action.Execute(this);
                }
            }
        }

        private void TryInvokeOnCreated()
        {
            if (!isInitDone && components.Count == RequireCount)
            {
                isInitDone = true;
                InvokePipelineAction(LegoScrollViewPipelineType.OnCreated);
            }
        }
        #endregion

        #region 操作数据模型方法

        private void InitScrollViewRxModel()
        {
            ScRxModel = (IYuLegoScrollViewRxModel)ViewModule.GetRxModel(name);
            ScRxModel.BindingAdd(OnRxModelAdd);
            ScRxModel.BindingInsert(OnRxModelInsert);
            ScRxModel.BindingRemove(OnRxModelRemove);
            ScRxModel.BindingReplace(OnRxModelReplace);
        }

        /// <summary>
        /// 添加数据模型
        /// </summary>
        /// <param name="rxModel"></param>
        private void OnRxModelAdd(IYuLegoUIRxModel rxModel)
        {
            TryRequestBuildComponent();
            UpadteContentSize();
            onComponentRxModelAdd?.Invoke(rxModel);
        }

        /// <summary>
        /// 插入数据模型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rxModel"></param>
        private void OnRxModelInsert(int index, IYuLegoUIRxModel rxModel)
        {
            TryRequestBuildComponent();
        }

        /// <summary>
        /// 向 ViewModule 请求构造 UI 组件
        /// </summary>
        private void TryRequestBuildComponent()
        {
            if (RequireCount <= 0)
            {
                return;
            }

            if (components.Count + requestTaskCount < RequireCount)
            {
                requestTaskCount++;
                ////var lowId = YuBigAssetIdMap.GetLowerId(ComponentId);
                ////viewModule.GetScrollViewComponent(lowId, OnComponentLoaded);
            }
            else
            {
                UpdateMaxIndexOffset();
            }
        }

        /// <summary>
        /// 移除数据模型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="rxModel"></param>
        private void OnRxModelRemove(int index, IYuLegoUIRxModel rxModel)
        {
            var last = components.Last;
            ViewModule.Restore(last.Value);
            components.RemoveLast();
            ReDrawAllComponents();
            UpdateMaxIndexOffset();
            UpadteContentSize();
        }

        private Queue<List<IYuLegoUIRxModel>> ToReplaceRxModelList = new Queue<List<IYuLegoUIRxModel>>();
        /// <summary>
        /// 替换数据模型
        /// </summary>
        private void OnRxModelReplace(List<IYuLegoUIRxModel> replaceRxModelList)
        {
            ScrollRect.StopMovement();
            Content.anchoredPosition = Vector2.zero;
            UpadteContentSize();
            int rxModelAmount = ScRxModel.ModelCount;   //数据模型数量
            int componentAmount = components.Count;     //组件数量
            if (componentAmount >= rxModelAmount)
            {
                while (components.Count != rxModelAmount)
                {
                    RestoreComponent();
                }
                ReDrawAllComponents();

                onReplaceComponents?.Invoke(components);
                RxModelReplaceComplished();
            }
            else if (componentAmount >= RequireCount)
            {
                ReDrawAllComponents();
                RxModelReplaceComplished();
            }

            else
            {
                ReDrawAllComponents();
                requestTaskCount = 0;

                for (int i = componentAmount; i < rxModelAmount; i++)
                {
                    OnRxModelAdd(ScRxModel.GetRxModel(i));
                }
            }
            UpdateMaxIndexOffset();
        }

        private void RxModelReplaceComplished()
        {
            ScRxModel.IsReplacingRxModel = false;
            if(ToReplaceRxModelList.Count >0)
            {
                ScRxModel.Replace(ToReplaceRxModelList.Dequeue());
            }
        }

        private void BindSonComponentRxmodl(IYuLegoUIRxModel rxModel, ILegoUI component)
        {
            foreach (var sonComponent in component.SonComponentDict)
            {
                rxModel.SonComponentModels.Add(sonComponent.Key, sonComponent.Value.RxModel);
            }
        }

        #endregion

        #region LegoScrollView 控件初始化
        private void InitMetaInfo(LegoScrollViewMeta svMeta)
        {
            isMultiLayout = svMeta.IsMultiLayout;
            maxPerLine = svMeta.MaxPerLine;

            Arrangement = svMeta.IsHorizontal ?
                YuArrangement.Horizontal :
                YuArrangement.Vertical;

            itemWidth = svMeta.ItemWidth;
            itemHeight = svMeta.ItemHeight;

            horizontalPadiding = svMeta.HorizontalPadiding;
            verticalPadiding = svMeta.VerticalPadiding;

            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    loopRequireNum = isMultiLayout
                        ? ((int)(ScrollRectRect.sizeDelta.x / (itemWidth + horizontalPadiding)) + 2) * maxPerLine
                        : ((int)(ScrollRectRect.sizeDelta.x / (itemWidth + horizontalPadiding)) + 2);
                    break;
                case YuArrangement.Vertical:
                    minVisibleLine = (int)(Content.sizeDelta.y / (itemHeight + verticalPadiding));
                    var minLine = minVisibleLine + 2;
                    loopRequireNum = minLine * maxPerLine;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CalculatOffset(svMeta);

            isCalculated = true;
        }

        private void CalculatOffset(LegoScrollViewMeta svMeta)
        {
            switch (Arrangement)
            {
                case YuArrangement.Horizontal:
                    break;
                case YuArrangement.Vertical:
                    xOffset = svMeta.Xoffset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 元数据变形 

        private enum MetamorphoseStatus : byte
        {
            ScrollView,
            ScrollRect,
            Content
        }

        private YuLegoImage scrollViewImage;

        private YuLegoImage ScrollViewImage
        {
            get
            {
                if (scrollViewImage != null)
                {
                    return scrollViewImage;
                }

                scrollViewImage = GetComponent<YuLegoImage>();
                return scrollViewImage;
            }
        }

        private RectTransform scrollRectRect;

        private RectTransform ScrollRectRect
        {
            get
            {
                if (scrollRectRect != null)
                {
                    return scrollRectRect;
                }

                scrollRectRect = RectTransform.Find("ScrollRect")
                    .RectTransform();
                return scrollRectRect;
            }
        }

        private ScrollRect _scrollRect;

        private ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect != null)
                {
                    return _scrollRect;
                }

                _scrollRect = ScrollRectRect.GetComponent<ScrollRect>();
                return _scrollRect;
            }
        }

        private YuLegoImage scrollRectImage;

        private YuLegoImage ScrollRectImage
        {
            get
            {
                if (scrollRectImage != null)
                {
                    return scrollRectImage;
                }

                scrollRectImage = ScrollRectRect.GetComponent<YuLegoImage>();
                return scrollRectImage;
            }
        }

        private MetamorphoseStatus metamorphoseStatus;
        private LegoScrollViewMeta scrollViewMeta;

        public void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (scrollViewMeta == null)
            {
                scrollViewMeta = uiMeta.NextScrollView;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (metamorphoseStatus)
            {
                case MetamorphoseStatus.ScrollView:
                    YuLegoUtility.MetamorphoseRect(RectTransform, RectMeta);
                    ScrollViewImage.Metamorphose(scrollViewMeta.ScrollViewImageMeta);
                    metamorphoseStatus = MetamorphoseStatus.ScrollRect;
                    break;
                case MetamorphoseStatus.ScrollRect:
                    YuLegoUtility.MetamorphoseRect(ScrollRectRect, scrollViewMeta.ScrollRectRectMeta);
                    ScrollRectImage.Metamorphose(scrollViewMeta.ScrollRectImageMeta);
                    metamorphoseStatus = MetamorphoseStatus.Content;
                    break;
                case MetamorphoseStatus.Content:
                    YuLegoUtility.MetamorphoseRect(Content, scrollViewMeta.ContentRectMeta);
                    scrollViewMeta = null;
                    metamorphoseStatus = MetamorphoseStatus.ScrollView;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public LegoMetamorphoseStage MetamorphoseStage { get; private set; }

        #endregion
    }
}