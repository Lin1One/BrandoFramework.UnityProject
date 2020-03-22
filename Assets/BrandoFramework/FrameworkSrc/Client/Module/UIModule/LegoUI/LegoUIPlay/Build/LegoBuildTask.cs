#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

using Common;
using System;
using UnityEngine;

namespace Client.LegoUI
{
    public class LegoBuildTask : IReset
    {
        static LegoBuildTask()
        {
            InitControlPool();
            metaHelper = Injector.Instance.Get<LegoMetaHelper>();

            //设置宽高
            //var legoSetting = YuU3dAppLegoUISettingDati.CurrentActual.DevieceSetting;
            //viewWidth = legoSetting.ViewWidth;
            //viewHeight = legoSetting.ViewHeight;
        }

        #region 静态构造初始化

        private static readonly int viewWidth;
        private static readonly int viewHeight;

        private static YuLegoRectTransformPool rectPool;
        private static YuLegoTextPool textPool;
        private static YuLegoImagePool imagePool;
        private static YuLegoRawImagePool rawImagePool;
        private static YuLegoButtonPool buttonPool;
        private static YuLegoTButtonPool tButtonPool;
        private static YuLegoTogglePool togglePool;
        private static YuLegoPlaneTogglePool planeTogglePool;
        private static YuLegoSliderPool sliderPool;
        private static YuLegoProgressbarPool progressbarPool;
        private static YuLegoInputFieldPool inputFieldPool;
        private static YuLegoDropdownPool dropdownPool;
        private static YuLegoScrollViewPool scrollViewPool;
        private static YuLegoRockerPool rockerPool;

        private static readonly LegoMetaHelper metaHelper;
        private static readonly Color zeroColor = new Color(0f, 0f, 0f, 0f);

        private static void InitControlPool()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            var injector = Injector.Instance.As<IU3dInjector>();

            rectPool = injector.GetMono<YuLegoRectTransformPool>();
            textPool = injector.GetMono<YuLegoTextPool>();
            imagePool = injector.GetMono<YuLegoImagePool>();
            rawImagePool = injector.GetMono<YuLegoRawImagePool>();
            buttonPool = injector.GetMono<YuLegoButtonPool>();
            tButtonPool = injector.GetMono<YuLegoTButtonPool>();
            togglePool = injector.GetMono<YuLegoTogglePool>();
            planeTogglePool = injector.GetMono<YuLegoPlaneTogglePool>();
            sliderPool = injector.GetMono<YuLegoSliderPool>();
            progressbarPool = injector.GetMono<YuLegoProgressbarPool>();
            inputFieldPool = injector.GetMono<YuLegoInputFieldPool>();
            dropdownPool = injector.GetMono<YuLegoDropdownPool>();
            scrollViewPool = injector.GetMono<YuLegoScrollViewPool>();
            rockerPool = injector.GetMono<YuLegoRockerPool>();
        }

        #endregion

        #region 基础字段

        private LegoBuildTaskPool taskPool;

        private LegoBuildTaskPool TaskPool => taskPool ?? (taskPool = Injector.Instance.Get<LegoBuildTaskPool>());

        private Action<LegoBuildTask> m_PushSonTaskDel;

        private LegoBuildTask m_ParentTask;

        /// <summary>
        /// 当前任务组件的挂载对象。
        /// </summary>
        public RectTransform ParentRect { get; set; }

        /// <summary>
        /// 当前组件或视图的元数据。
        /// </summary>
        public LegoUIMeta TaskMeta { get; private set; }

        /// <summary>
        /// 组件挂载元数据。
        /// 如果该元数据为空则说明组件为视图。
        /// </summary>
        public LegoRectTransformMeta ComponentMountMeta { get; private set; }

        /// <summary>
        /// 组件构建完成的外部委托。
        /// </summary>
        private Action<LegoBuildTask> m_OnBuilded;

        /// <summary>
        /// 是否可以构建。
        /// 帧构建开关。
        /// </summary>
        private bool buildAble;

        /// <summary>
        /// 任务的构建速度。
        /// 速度值越大则发生构建卡顿的几率越高。
        /// </summary>
        private int m_BuildSpeed;

        /// <summary>
        /// 当前已完成的累积构建工作量。
        /// </summary>
        public int BuildedCount { get; private set; }

        /// <summary>
        /// 当前帧完成的构建工作量。
        /// </summary>
        public int BuildedFrameCount { get; private set; }

        /// <summary>
        /// 正在构建的可变形控件。
        /// </summary>
        private ILegoControl buildingControl;

        /// <summary>
        /// 正在构建的控件类型。
        /// </summary>
        private LegoUIType buildingType;

        /// <summary>
        /// 是否在后台加载UI。
        /// </summary>
        public bool IsInBackground { get; private set; }

        /// <summary>
        /// 是否在加载完成后于回调中绑定数据模型
        /// </summary>
        public bool IsBindRxModelOnBuild { get; private set; } = true;

        /// <summary>
        /// 正在构建的控件的Rect元数据。
        /// </summary>
        private LegoRectTransformMeta buildingRectMeta;

        /// <summary>
        /// 当前任务组件的根对象。
        /// </summary>
        public RectTransform RootRect { get; private set; }

        /// <summary>
        /// 当前任务组件根对象所挂载的层级 Canvas 
        /// </summary>
        public LegoViewType ParentCanvas { get; private set; }

        /// <summary>
        /// 当前组件需要完成的控件构建数量。
        /// </summary>
        private int willBuildCount;

        /// <summary>
        /// 任务是否已经完成。
        /// </summary>
        public bool IsComplete { get; private set; }

        /// <summary>
        /// 子组件构建完成数量。
        /// </summary>
        private int sonBuildedNum;

        /// <summary>
        /// 子组件数量
        /// </summary>
        private int sonNum;

        /// <summary>
        /// 容器构建完成数量。
        /// </summary>
        private int containerBuildedNum;

        /// <summary>
        /// 容器数量。
        /// </summary>
        private int containerNum;

        /// <summary>
        /// 当前任务的已加载控件数量记录
        /// 作用：当一个组件本帧未加载完成时，下一帧又接受了另一个相同组件的加载任务，
        /// 此时该组件元数据会被重置，会该组件加载任务的进度丢失，需此字段保存每个任务中
        /// 已完成加载控件数量
        /// </summary>
        private int taskCurrentElementIndex = -1;

        public Action<ILegoUI> UiBuildCallback { get; private set; }

        #endregion

        #region 任务初始化

        private const string LEGO_VIEW = "LegoView";
        private const string LEGO_COMPONENT = "LegoComponent";
        private const string LEGO_CONTAINER = "LegoContainer";

        #region 链式赋值

        public LegoBuildTask SetBackLoad(bool isBack)
        {
            IsInBackground = isBack;
            return this;
        }

        public LegoBuildTask SetBuildSpeed(int buildSpeed)
        {
            m_BuildSpeed = buildSpeed;
            return this;
        }

        public LegoBuildTask SetParentTask(LegoBuildTask parentTask)
        {
            m_ParentTask = parentTask;
            return this;
        }

        public LegoBuildTask SetUILoadCallback(Action<ILegoUI> callback)
        {
            UiBuildCallback = callback;
            return this;
        }

        public LegoBuildTask SetParentRect(RectTransform parentRect)
        {
            ParentRect = parentRect;
            return this;
        }

        /// <summary>
        /// 设置UI的挂载位置元数据（仅仅组件需要设置该属性，界面会直接挂载在0，0的位置）。
        /// </summary>
        /// <param name="mountMeta"></param>
        /// <returns></returns>
        public LegoBuildTask SetMountMeta(LegoRectTransformMeta mountMeta)
        {
            ComponentMountMeta = mountMeta;
            return this;
        }

        public LegoBuildTask SetPushTaskDel(Action<LegoBuildTask> pushTaskDel)
        {
            m_PushSonTaskDel = pushTaskDel;
            return this;
        }

        public LegoBuildTask SetBindRxModel(bool isBinding)
        {
            IsBindRxModelOnBuild = isBinding;
            return this;
        }

        public LegoBuildTask SetParentCanvas(LegoViewType parentCanvasType)
        {
            ParentCanvas = parentCanvasType;
            return this;
        }

        #endregion

        public void Init ( string id,
            Action<LegoBuildTask> onBuilded,
            RectTransform parent)
        {
            ParentRect = parent;
            TaskMeta = metaHelper.GetMeta(id);
            TaskMeta.Reset(); // 每次开始构建都重置元数据
            m_OnBuilded = onBuilded;
            sonNum = TaskMeta.ComponentRefs.Count;
            containerNum = TaskMeta.ContainerRefs.Count;
            willBuildCount = TaskMeta.WillBuildCount;

            CreateRootAtOnce(); // 构建组件根对象
            TryMountAtOnce();
            AddRootButton(); // 添加根按钮
#if DEBUG
            AddHelper(); // 编辑器下添加开发助手组件
#endif

            buildAble = true;
        }

        private void CreateRootAtOnce()
        {
#if DEBUG
            if (Application.isPlaying)
            {
                RootRect = rectPool.Take();
            }
            else
            {
                var tmpGo = new GameObject();
                RootRect = tmpGo.AddComponent<RectTransform>();
            }
#else
            RootRect = rectPool.Take(); 
#endif
            // 组件由于复用可能会同时存在多个，因此组件跟对象的最终命名会在组件构建完毕回调中处理。
            RootRect.name = TaskMeta.RootMeta.Name;
        }

        private void TryMountAtOnce()
        {
            if (ParentRect == null)
            {
                return;
            }

            if (!RootRect.name.StartsWith(LEGO_VIEW))
            {
                RootRect.gameObject.SetActive(TaskMeta.RootMeta.IsDefaultActive);
            }

            if (ParentRect != null)
            {
                RootRect.SetParent(ParentRect);
                RootRect.gameObject.layer = ParentRect.gameObject.layer;

                if (IsInBackground)
                {
                    RootRect.gameObject.SetActive(false);
                }
            }

            if (RootRect.name.StartsWith(LEGO_VIEW))
            {
                RootRect.localPosition = Vector3.zero;
                RootRect.localScale = Vector3.one;
            }
            else
            {
                var rootMeta = TaskMeta.RootMeta;

                RootRect.localPosition = new Vector3(
                    rootMeta.X,
                    rootMeta.Y,
                    rootMeta.Z
                );

                RootRect.localScale = new Vector3(
                    rootMeta.ScaleX,
                    rootMeta.ScaleY,
                    rootMeta.ScaleZ
                );
            }

            var height = TaskMeta.RootMeta.Height;
            var width = TaskMeta.RootMeta.Width;
            RootRect.sizeDelta = new Vector2(width, height);
        }

        void AddRootButton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (RootRect.name.StartsWith(LEGO_CONTAINER))
            {
                return;
            }

            var button = buttonPool.Take();
            button.SonText.gameObject.SetActive(false);
            button.RectTransform.SetParent(RootRect);
            button.RectTransform.name = "Button_Root";
            button.RectTransform.sizeDelta = new Vector2(viewWidth * 2, viewHeight * 2);
            button.RectTransform.localPosition = Vector3.zero;
            button.RectTransform.localScale = Vector3.one;
            ////button.BgImage.Color = zeroColor;
        }

#if DEBUG

        void AddHelper()
        {
            if (RootRect.name.StartsWith("LegoView"))
            {
                ////RootRect.gameObject.AddComponent<YuLegoViewHelper>();
            }
            else
            {
                ////RootRect.gameObject.AddComponent<YuLegoComponentHelper>();
            }
        }
#endif

        #endregion

        /// <summary>
        /// 帧循环构建
        /// </summary>
        public void BuildAtUpdate()
        {
            BuildedFrameCount = 0;

            for (var i = 0; i < m_BuildSpeed; i++)
            {
                if (!buildAble)
                {
                    return;
                }

                if (CheckIsCompleted())
                {
                    return;
                }

                //                try
                //                {
                if (buildingControl == null)
                {
                    if (taskCurrentElementIndex == TaskMeta.ElementIndex)
                    {
                        taskCurrentElementIndex++;
                        buildingType = TaskMeta.NextElement;
                        buildingRectMeta = TaskMeta.NextRect;
                    }
                    else
                    {
                        TaskMeta.ElementIndex = taskCurrentElementIndex;
                        TaskMeta.RectIndex = taskCurrentElementIndex;
                        taskCurrentElementIndex++;
                        buildingType = TaskMeta.NextElement;
                        buildingRectMeta = TaskMeta.NextRect;
                    }

                    
                    if (buildingType == LegoUIType.Component || buildingType == LegoUIType.Container)
                    {
                        PushTask();
                    }
                    else
                    {
                        //获取控件，变形
                        GetNextControl();
                        ExecuteOneMetamorphose();
                    }
                }
                else
                {
                    ExecuteOneMetamorphose();
                }
                //                }
                //                catch (Exception ex)
                //                {
                //                    buildAble = false;
                //                    Debug.LogError(ex.StackTrace);
                //                    throw new Exception($"在构建{buildingRectMeta.Name}时发生异常，异常信息为{ex.Message}！");
                //                }
            }
        }

        private bool CheckIsCompleted() // 检查任务是否完成，如果已完成则跳出当前帧。
        {
            if (BuildedCount == willBuildCount && sonBuildedNum == sonNum
                                               && containerBuildedNum == containerNum)
            {
                IsComplete = true;
                buildAble = false;
                TaskMeta.Reset();
                taskCurrentElementIndex = -1;
                m_OnBuilded(this);
                m_ParentTask?.OnSonCompleted(RootRect);
                return true;
            }
            return false;
        }

        private void OnSonCompleted(RectTransform uiRect)
        {
            if (uiRect.name.StartsWith("LegoComponent"))
            {
                sonBuildedNum++;
            }
            else
            {
                containerBuildedNum++;
            }

            buildAble = true;
        }


        #region 控件获取，变形，布局

        private void GetNextControl()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                GetNextControlAtPlay();
            }
            else
            {
                GetNextControlAtEditor();
            }
#else
            GetNextControlAtPlay();
#endif
        }

        private void GetNextControlAtPlay()
        {
            switch (buildingType)
            {
                case LegoUIType.Text:
                    buildingControl = textPool.Take();
                    break;
                case LegoUIType.Image:
                    buildingControl = imagePool.Take();
                    break;
                case LegoUIType.RawImage:
                    buildingControl = rawImagePool.Take();
                    break;
                case LegoUIType.Button:
                    buildingControl = buttonPool.Take();
                    break;
                case LegoUIType.TButton:
                    buildingControl = tButtonPool.Take();
                    break;
                case LegoUIType.Toggle:
                    buildingControl = togglePool.Take();
                    break;
                case LegoUIType.PlaneToggle:
                    buildingControl = planeTogglePool.Take();
                    break;
                case LegoUIType.InputField:
                    buildingControl = inputFieldPool.Take();
                    break;
                case LegoUIType.Slider:
                    buildingControl = sliderPool.Take();
                    break;
                case LegoUIType.Progressbar:
                    buildingControl = progressbarPool.Take();
                    break;
                case LegoUIType.Tab:
                    break;
                case LegoUIType.Dropdown:
                    buildingControl = dropdownPool.Take();
                    break;
                case LegoUIType.Rocker:
                    buildingControl = rockerPool.Take();
                    break;
                case LegoUIType.Grid:
                    break;
                case LegoUIType.ScrollView:
                    buildingControl = scrollViewPool.Take();
                    break;
                case LegoUIType.None:
                    break;
                case LegoUIType.View:
                    break;
                case LegoUIType.InlineText:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR

        private void GetNextControlAtEditor()
        {
            switch (buildingType)
            {
                case LegoUIType.Text:
                    buildingControl = GetControlByDefaultControls<YuLegoText>();
                    break;
                case LegoUIType.Image:
                    buildingControl = GetControlByDefaultControls<YuLegoImage>();
                    break;
                case LegoUIType.RawImage:
                    buildingControl = GetControlByDefaultControls<YuLegoRawImage>();
                    break;
                case LegoUIType.Button:
                    buildingControl = GetControlByDefaultControls<YuLegoButton>();
                    break;
                case LegoUIType.TButton:
                    buildingControl = GetControlByDefaultControls<YuLegoTButton>();
                    break;
                case LegoUIType.Toggle:
                    buildingControl = GetControlByDefaultControls<YuLegoToggle>();
                    break;
                case LegoUIType.PlaneToggle:
                    buildingControl = GetControlByDefaultControls<YuLegoPlaneToggle>();
                    break;
                case LegoUIType.InputField:
                    buildingControl = GetControlByDefaultControls<YuLegoInputField>();
                    break;
                case LegoUIType.Slider:
                    buildingControl = GetControlByDefaultControls<YuLegoSlider>();
                    break;
                case LegoUIType.Progressbar:
                    buildingControl = GetControlByDefaultControls<YuLegoProgressbar>();
                    break;
                case LegoUIType.Tab:
                    break;
                case LegoUIType.Dropdown:
                    buildingControl = GetControlByDefaultControls<YuLegoDropdown>();
                    break;
                case LegoUIType.Rocker:
                    buildingControl = GetControlByDefaultControls<YuLegoRocker>();
                    break;
                case LegoUIType.Grid:
                    break;
                case LegoUIType.ScrollView:
                    buildingControl = GetControlByDefaultControls<YuLegoScrollView>();
                    break;
                case LegoUIType.None:
                    break;
                case LegoUIType.View:
                    break;
                case LegoUIType.InlineText:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private T GetControlByDefaultControls<T>() where T : Component, ILegoControl
        {
            var control = LegoDefaultControls.GetControl<T>()
                .GetComponent<T>();
            return control;
        }
#endif

        private void ExecuteOneMetamorphose()
        {
            buildingControl.Metamorphose(TaskMeta);
            BuildedCount++;
            BuildedFrameCount++;
            if (buildingControl.MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MountControl();
                buildingControl.GameObject.SetActive(buildingControl.RectMeta.IsDefaultActive);
                buildingControl = null;
            }
        }

        private void MountControl()
        {
            var controlRect = buildingControl.RectTransform;
            controlRect.gameObject.layer = RootRect.gameObject.layer;
            controlRect.SetParent(RootRect);

            controlRect.localPosition = new Vector3(
                buildingRectMeta.X,
                buildingRectMeta.Y,
                buildingRectMeta.Z
            );

            controlRect.localScale = new Vector3(
                buildingRectMeta.ScaleX,
                buildingRectMeta.ScaleY,
                buildingRectMeta.ScaleZ
            );
        }

        #endregion

        #region 子任务构建及压入

        private void PushTask()
        {
            switch (buildingType)
            {
                case LegoUIType.Component:
                    PushSonComponentTask();
                    break;
                case LegoUIType.Container:
                    PushContainerTask();
                    break;
            }
        }

        private void PushSonComponentTask()
        {
            buildAble = false; // 暂停任务构建
            var sonRef = TaskMeta.ComponentRefs[sonBuildedNum];
            var mountMeta = sonRef.MountPosition;
            var task = TaskPool.GetTask(sonRef.RefComponent, m_OnBuilded, RootRect);
            task.SetPushTaskDel(m_PushSonTaskDel)
                .SetBuildSpeed(m_BuildSpeed)
                .SetParentRect(RootRect)
                .SetMountMeta(mountMeta)
                .SetParentTask(this)
                .SetBackLoad(IsInBackground)
                .SetBindRxModel(IsBindRxModelOnBuild);

            m_PushSonTaskDel(task);
        }

        private void PushContainerTask()
        {
            buildAble = false;
            var containerRef = TaskMeta.ContainerRefs[containerBuildedNum];
            var mountMeta = containerRef.MountPosition;
            var task = TaskPool.GetTask(containerRef.ContainerName, m_OnBuilded, RootRect);
            task.SetPushTaskDel(m_PushSonTaskDel)
                .SetBuildSpeed(m_BuildSpeed)
                .SetParentRect(RootRect)
                .SetMountMeta(mountMeta)
                .SetParentTask(this)
                .SetBackLoad(IsInBackground)
                .SetBindRxModel(IsInBackground);
            m_PushSonTaskDel(task);
        }

        private void PushSonTask(string taskId, LegoRectTransformMeta mountMeta, bool isSonComponent)
        {
            buildAble = false; // 暂停任务构建
            var task = TaskPool.GetTask(taskId, m_OnBuilded, RootRect);
            task.SetPushTaskDel(m_PushSonTaskDel)
                .SetBuildSpeed(m_BuildSpeed)
                .SetParentRect(RootRect)
                .SetMountMeta(mountMeta)
                .SetParentTask(this)
                .SetBackLoad(IsInBackground)
                .SetBindRxModel(isSonComponent && IsBindRxModelOnBuild);

            m_PushSonTaskDel(task);
        }

        #endregion

        #region 重置状态

        public void Reset()
        {
            buildAble = false;
            IsComplete = false;
            BuildedCount = 0;
            m_BuildSpeed = 0;
            buildingControl = null;
            ComponentMountMeta = null;
            containerBuildedNum = 0;
            sonBuildedNum = 0;
            m_ParentTask = null;
            UiBuildCallback = null;
            IsBindRxModelOnBuild = true;
            IsInBackground = false;
            ParentRect = null;
        }

        #endregion

    }
}