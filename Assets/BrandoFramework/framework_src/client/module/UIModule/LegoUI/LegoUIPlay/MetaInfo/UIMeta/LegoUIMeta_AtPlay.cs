#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Client.LegoUI
{
    [System.Serializable]
    public partial class LegoUIMeta
    {
        #region GUI

        [FoldoutGroup("UI根节点位置元数据")] [HideLabel]
        public LegoRectTransformMeta RootMeta;

        [FoldoutGroup("UI自身元数据")] [LabelText("UI的业务逻辑Id")]
        public string LogicId;

        [FoldoutGroup("UI不规则布局元数据")] [LabelText("左对齐布局距离")]
        public int PaddingLeft;

        [FoldoutGroup("UI不规则布局元数据")] [LabelText("和上一个控件的Y轴间距")]
        public int PaddingLastY;

        [FoldoutGroup("UI子集元数据")] [LabelText("元素类型元数据")]
        public List<YuLegoUIType> ElementTypes;

        [FoldoutGroup("UI子集元数据")] [LabelText("子组件引用元数据")]
        public List<LegoComponentRef> ComponentRefs;

        [FoldoutGroup("UI子集元数据")] [LabelText("容器引用元数据")]
        public List<LegoContainerRef> ContainerRefs;

        [FoldoutGroup("UI子集元数据")] [LabelText("位置元数据")]
        public List<LegoRectTransformMeta> RectMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("文本元数据")]
        public List<LegoTextMeta> TextMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("图片元数据")]
        public List<LegoImageMeta> ImageMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("动态图片元数据")]
        public List<LegoRawImageMeta> RawImageMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("按钮元数据")]
        public List<LegoButtonMeta> ButtonMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("双图片按钮元数据")]
        public List<LegoTButtonMeta> TButtonMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("滑动器元数据")]
        public List<LegoSliderMeta> SliderMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("进度条元数据")]
        public List<LegoProgressbarMeta> ProgressbarMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("开关元数据")]
        public List<LegoToggleMeta> ToggleMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("水平开关元数据")]
        public List<LegoPlaneToggleMeta> PlaneToggleMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("输入框元数据")]
        public List<LegoInputFieldMeta> InputFieldMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("下拉框元数据")]
        public List<LegoDropdownMeta> DropdownMetas;

        [FoldoutGroup("UI子集元数据")] [LabelText("滚动列表元数据")]
        public List<LegoScrollViewMeta> ScrollViewMetas;

        [FoldoutGroup("UI子集元数据")] [FoldoutGroup("UI子集元数据")] [LabelText("摇杆元数据")]
        public List<LegoRockerMeta> RockerMetas;

        #endregion

        #region 动画效果元数据

        //[FoldoutGroup("动画效果元数据")] [LabelText("动画效果元数据列表")]
        //public List<YuUITweenMetaRef> UiTweenRefs;

        //public void AddTweenRef(YuUiTweenMeta tweenMeta)
        //{
        //    if (UiTweenRefs?.Find(m => m.PipelineType == tweenMeta.PipelineType) != null)
        //    {
        //        return;
        //    }

        //    if (UiTweenRefs == null)
        //    {
        //        UiTweenRefs = new List<YuUITweenMetaRef>();
        //    }

        //    var tweenRef = YuUITweenMetaRef.Create(tweenMeta);
        //    UiTweenRefs.Add(tweenRef);
        //}

        //public YuUITweenMetaRef GetTweenRef(YuUIPipelineType pipeline)
        //{
        //    var meta = UiTweenRefs?.Find(m => m.PipelineType == pipeline);
        //    return meta;
        //}

        #endregion

        #region UI配置元数据

        [FoldoutGroup("UI配置元数据")] [LabelText("UI的挂载层级")]
        public LegoViewType ViewType;

        [FoldoutGroup("UI配置元数据")] [LabelText("开启时需要隐藏的视图列表")]
        public List<string> HideTargets;

        [FoldoutGroup("UI配置元数据")] [LabelText("是否需要点击空白关闭自身")]
        public bool IsBlankClose;

        #endregion

        #region 构建时API

        public int WillBuildCount
        {
            get
            {
                var count = 0;
                count += TextMetas.Count * 1;
                count += ImageMetas.Count * 1;
                count += RawImageMetas.Count * 1;
                count += ButtonMetas.Count * 3;
                count += TButtonMetas.Count * 4;
                count += SliderMetas.Count * 6;
                count += ProgressbarMetas.Count * 4;
                count += ToggleMetas.Count * 4;
                count += PlaneToggleMetas.Count * 3;
                count += InputFieldMetas.Count * 4;
                count += DropdownMetas.Count * 13;
                count += ScrollViewMetas.Count * 3;
                count += RockerMetas.Count * 3;
                return count;
            }
        }

        #region 基础的元素类型和位置元数据 

        public LegoRectTransformMeta CurrentRect => RectMetas[RectIndex];
        public int RectIndex { private get; set; } = -1;

        public LegoRectTransformMeta NextRect
        {
            get
            {
                RectIndex++;
                return RectMetas[RectIndex];
            }
        }

        public int ElementIndex { get; set; } = -1;

        public YuLegoUIType NextElement
        {
            get
            {
                ElementIndex++;
                return ElementTypes[ElementIndex];
            }
        }

        #endregion


        private Dictionary<YuLegoUIType, int> indexs;

        private Dictionary<YuLegoUIType, int> Indexs
        {
            get
            {
                if (indexs != null)
                {
                    return indexs;
                }

                indexs = new Dictionary<YuLegoUIType, int>
                {
                    {YuLegoUIType.Text, -1},
                    {YuLegoUIType.Button, -1},
                    {YuLegoUIType.TButton, -1},
                    {YuLegoUIType.Image, -1},
                    {YuLegoUIType.RawImage, -1},
                    {YuLegoUIType.Toggle, -1},
                    {YuLegoUIType.PlaneToggle, -1},
                    {YuLegoUIType.Slider, -1},
                    {YuLegoUIType.Progressbar, -1},
                    {YuLegoUIType.InputField, -1},
                    {YuLegoUIType.Dropdown, -1},
                    {YuLegoUIType.ScrollView, -1},
                    {YuLegoUIType.Rocker, -1}
                };

                return indexs;
            }
        }

        private int GetTargetIndex(YuLegoUIType uiType)
        {
            var index = ++Indexs[uiType];
            return index;
        }

        public LegoTextMeta NextText
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Text);
                var meta = TextMetas[index];
                return meta;
            }
        }

        public LegoButtonMeta NextButton
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Button);
                var meta = ButtonMetas[index];
                return meta;
            }
        }

        public LegoTButtonMeta NextTButton
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.TButton);
                var meta = TButtonMetas[index];
                return meta;
            }
        }

        public LegoImageMeta NextImage
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Image);
                LegoImageMeta meta = null;
                if (index  < ImageMetas.Count)
                {
                    meta = ImageMetas[index];
                }
                return meta;
            }
        }

        public LegoRawImageMeta NextRawImage
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.RawImage);
                var meta = RawImageMetas[index];
                return meta;
            }
        }

        public LegoToggleMeta NextToggle
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Toggle);
                var meta = ToggleMetas[index];
                return meta;
            }
        }

        public LegoPlaneToggleMeta NextPlaneToggle
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.PlaneToggle);
                var meta = PlaneToggleMetas[index];
                return meta;
            }
        }

        public LegoSliderMeta NextSlider
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Slider);
                var meta = SliderMetas[index];
                return meta;
            }
        }

        public LegoProgressbarMeta NextProgressbar
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Progressbar);
                var meta = ProgressbarMetas[index];
                return meta;
            }
        }

        public LegoInputFieldMeta NextInputField
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.InputField);
                var meta = InputFieldMetas[index];
                return meta;
            }
        }

        public LegoDropdownMeta NextDropdown
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Dropdown);
                var meta = DropdownMetas[index];
                return meta;
            }
        }

        public LegoScrollViewMeta NextScrollView
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.ScrollView);
                var meta = ScrollViewMetas[index];
                return meta;
            }
        }

        public LegoScrollViewMeta GetScrollViewMeta(string id) => ScrollViewMetas.Find(m => m.Id == id);

        public LegoRockerMeta NextRocker
        {
            get
            {
                var index = GetTargetIndex(YuLegoUIType.Rocker);
                var meta = RockerMetas[index];
                return meta;
            }
        }

        private static List<YuLegoUIType> uiTypes;

        private static List<YuLegoUIType> UiTypes
        {
            get
            {
                if (uiTypes != null)
                {
                    return uiTypes;
                }

                uiTypes = new List<YuLegoUIType>
                {
                    YuLegoUIType.Text,
                    YuLegoUIType.Button,
                    YuLegoUIType.TButton,
                    YuLegoUIType.Image,
                    YuLegoUIType.RawImage,
                    YuLegoUIType.Toggle,
                    YuLegoUIType.PlaneToggle,
                    YuLegoUIType.Slider,
                    YuLegoUIType.Progressbar,
                    YuLegoUIType.InputField,
                    YuLegoUIType.Dropdown,
                    YuLegoUIType.ScrollView,
                    YuLegoUIType.Rocker
                };

                return uiTypes;
            }
        }

        public void Reset()
        {
            ElementIndex = -1;
            RectIndex = -1;

            foreach (var uiType in UiTypes)
            {
                Indexs[uiType] = -1;
            }
        }

        private int _willUnloadCount = -1;

        public int WillUnloadCount
        {
            get
            {
                if (_willUnloadCount != -1)
                {
                    return _willUnloadCount;
                }

                _willUnloadCount = RectMetas.Count - ComponentRefs.Count
                                                   - ContainerRefs.Count;
                return _willUnloadCount;
            }
        }

        #endregion
    }
}