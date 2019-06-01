#region Head

// Author:            liuruoyu1981
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             liuruoyu1981@gmail.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using Sirenix.OdinInspector;
using YuU3dOdinEditor;

namespace Common.EditorWindow
{
    [Serializable]
    [Flags]
    public enum YuU3dBuildTargetGroup
    {
        Standalone = 1 << 1,
        iOS = 1 << 2,
        Android = 1 << 3,
        WebGL = 1 << 4,
        PS4 = 1 << 5,
        XboxOne = 1 << 6,
        Switch = 1 << 7,
        Lumin = 1 << 8,

//        All = Standalone | iOS | Android | WebGL | PS4
//              | XboxOne | Switch | Lumin
    }

    [Serializable]
    public class YuU3dPrecompiledSettingNode
    {
//        [HorizontalGroup("水平显示")]
        [LabelText("预编译指令")] public string PrecompiledDefineId;

//        [HorizontalGroup("水平显示")]
        [LabelText("作用平台")] public YuU3dBuildTargetGroup BuildTargetGroup;
    }
}