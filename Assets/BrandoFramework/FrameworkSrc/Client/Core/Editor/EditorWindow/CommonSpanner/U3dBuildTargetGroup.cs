#region Head

// Author:            LinYuzhou
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             836045613@qq.com

#endregion

using System;
using Sirenix.OdinInspector;

namespace Client.Core.Editor
{
    [Serializable]
    [Flags]
    public enum U3dBuildTargetGroup
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
    public class U3dPrecompiledSettingNode
    {
//        [HorizontalGroup("水平显示")]
        [LabelText("预编译指令")] public string PrecompiledDefineId;

//        [HorizontalGroup("水平显示")]
        [LabelText("作用平台")] public U3dBuildTargetGroup BuildTargetGroup;
    }
}