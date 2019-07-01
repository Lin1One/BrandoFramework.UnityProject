#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com


#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Client.Assets.Editor
{
    /// <summary>
    /// AssetBundle打包忽略规则。
    /// 用于控制允许被打包的目标文件。
    /// 1. 后缀结尾
    /// 2. 包含
    /// </summary>
    [Serializable]
    public class YuAssetBundleIgnoreRule
    {
        [LabelText("需要忽略的结尾后缀列表")]
        public List<string> IgnoreEndSuffixs = new List<string>();

        [LabelText("需要忽略的包含字符串列表")]
        public List<string> IgnoreContains = new List<string>();

        public YuAssetBundleIgnoreRule()
        {
            IgnoreEndSuffixs.Add(".meta");
            IgnoreEndSuffixs.Add(".tpsheet");
            IgnoreEndSuffixs.Add(".cs");
        }
    }
}