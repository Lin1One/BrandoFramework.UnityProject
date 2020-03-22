#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion



using Client.ScriptCreate;

namespace Client.DataTable.Editor

{
    /// <summary>
    /// Excel字段处理器。
    /// 负责接收指定类型的 excel 字段数据并处理。
    /// 处理：
    /// 1. 生成对应目标语言的脚本代码。
    /// </summary>
    public interface IExcelFieldHandler
    {
        /// <summary>
        /// 字段处理器对应的脚本类型。
        /// </summary>
        ScriptType ScriptType { get; }

        /// <summary>
        /// 可处理的目标数据结构字符串。
        /// </summary>
        FieldTypeEnum FieldType { get; }

        /// <summary>
        /// 获得指定编程语言的代码文本。
        /// </summary>
        /// <param name="languageType"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="u3DAppSetting"></param>
        /// <returns></returns>
        string GetCodeStr
        (
            string languageType,
            ExcelFieldInfo fieldInfo
            //YuU3dAppSetting u3DAppSetting
        );
    }
}