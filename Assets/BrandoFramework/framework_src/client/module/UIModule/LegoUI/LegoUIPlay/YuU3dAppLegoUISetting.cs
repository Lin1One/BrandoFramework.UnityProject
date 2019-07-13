#region Head

// Author:            Yu
// CreateDate:        1/22/2019 1:13:33 AM
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.PrefsData;
using Sirenix.OdinInspector;
using System;

namespace Client.LegoUI
{
    [Serializable]
    [YuDatiDesc(YuDatiSaveType.Multi, typeof(YuU3dAppLegoUISettingDati), "应用配置及资料/乐高UI")]
    public class YuU3dAppLegoUISettingDati : GenericMultiDati<YuU3dAppLegoUISetting,
        YuU3dAppLegoUISettingDati>
    {
    }

    [Serializable]
    public class YuU3dAppLegoUIPoolSetting 
    {
        [LabelText("空物体始化容量默认为200")] public int RectTransformPoolInitCount = 200;

        [LabelText("文本初始化容量默认为100")] public int TextPoolInitCount = 200;

        [LabelText("按钮初始化容量默认为200")] public int ButtonPoolInitCount = 200;

        [LabelText("双图片按钮初始化容量默认为100")] public int TButtonPoolInitCount = 100;

        [LabelText("图片初始化容量默认为200")] public int ImagePoolInitCount = 200;

        [LabelText("动态图片初始化容量默认为20")] public int RawImagePoolInitCount = 20;

        [LabelText("勾选开关初始化容量默认为5")] public int TogglePoolInitCount = 5;

        [LabelText("水平开关初始化容量默认为5")] public int PlaneTogglePoolInitCount = 5;

        [LabelText("输入框初始化容量默认为10")] public int InputFieldPoolInitCount = 10;

        [LabelText("下拉框初始化容量默认为3")] public int DropdownPoolInitCount = 3;

        [LabelText("滑动条初始化容量默认为10")] public int SliderPoolInitCount = 10;

        [LabelText("进度条初始化容量默认为10")] public int ProgressbarPoolInitCount = 10;

        [LabelText("滚动视图初始化容量默认为10")] public int ScrollViewPoolInitCount = 10;

        [LabelText("摇杆初始化容量默认为2")] public int RockerPoolInitCount = 2;
    }

    [Serializable]
    public class YuLegoUIBuildSetting
    {
        /// <summary>
        /// UI任务总加载速度默认为50。
        /// </summary>
        [LabelText("UI任务总加载速度默认为50")] public int TotalBuildSpeed = 50;

        /// <summary>
        /// UI任务加载速度默认为10。
        /// </summary>
        [LabelText("UI任务加载速度默认为10")] public int TaskBuildSpeed = 10;
    }

    [Serializable]
    public class YuLegoUIDevieceSetting
    {
        /// <summary>
        /// 视图宽。
        /// </summary>
        [LabelText("设备屏幕宽度默认为1280")] public int ViewWidth = 1280;

        /// <summary>
        /// 视图高。
        /// </summary>
        [LabelText("设备屏幕高度默认为720")] public int ViewHeight = 720;
    }

    [Serializable]
    public class YuLegoScriptSetting
    {
        [LabelText("应用Id")] public string AppId;

        [LabelText("乐高UI交互行为处理器基类Id")] public string LegoActionScriptId;

        [LabelText("乐高UI核心逻辑器基类Id")] public string LegoLogicerScriptId;
    }

    [Serializable]
    public class YuU3dAppLegoUISetting
    {
        [BoxGroup("控件池配置")] [HideLabel] public YuU3dAppLegoUIPoolSetting PoolSetting;

        [BoxGroup("构建配置")] [HideLabel] public YuLegoUIBuildSetting BuildSetting;

        [BoxGroup("设备配置")] [HideLabel] public YuLegoUIDevieceSetting DevieceSetting;

        [BoxGroup("乐高UI相关脚本创建配置")] [HideLabel] public YuLegoScriptSetting ScriptSetting;
    }
}