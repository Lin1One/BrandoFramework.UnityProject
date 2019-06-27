#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion


namespace Client.DataTable.Editor
{
    /// <summary>
    /// excel自动化工作流程枚举类型字段信息。
    /// </summary>
    public class ExcelEnumFieldInfo
    {
        /// <summary>
        /// 枚举字段名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 枚举字段注释。
        /// </summary>
        public string Comment { get; }

        public ExcelEnumFieldInfo(string name, string comment)
        {
            Name = name;
            Comment = comment;
        }
    }
}