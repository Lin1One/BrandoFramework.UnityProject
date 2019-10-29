#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using System;
using Sirenix.OdinInspector;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel工具全局配置。
    /// </summary>
    [Serializable]
    public class DataTableReadRule
    {
        [LabelText("中文注释行索引默认为0")]
        public int ChineseCommentIndex = 0;

        [LabelText("英文注释行索引默认为2")]
        public int EnglishCommentIndex = 2;

        [LabelText("数据结构定义行索引默认为3")]
        public int StrcutDefineIndex = 3;
    }
}