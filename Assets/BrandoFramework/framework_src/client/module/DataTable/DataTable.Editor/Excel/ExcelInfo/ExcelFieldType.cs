#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel配置表字段类型。
    /// </summary>
    [Serializable]
    public enum ExcelFieldType
    {
        Ignore,
        String,
        Bool,
        Byte,
        Short,
        Int,
        Long,
        Float,
        Enum,
        StringArray,
        ByteArray,
        ShortArray,
        IntArray,
        FloatArray,
        LongArray,
        ParamsPropertyClass,
        SimpleObj
    }

    /// <summary>
    /// Excel数据字段（表列）的解析配置。
    /// </summary>
    [System.Serializable]
    public class YuExcelFieldSetting
    {
        [LabelText("字段所对应的工作簿列索引")]
        [SerializeField]
        public int ColumnIndex;

        [LabelText("字段中文名(程序字段注释)")]
        [SerializeField]
        public string ChineseId;

        [LabelText("字段英文名(程序字段名)")]
        [SerializeField]
        public string EnglishId;

        [LabelText("字段类型")]
        [SerializeField]
        public ExcelFieldType FieldType;
    }

    /// <summary>
    /// Excel配置表解析配置。
    /// </summary>
    [Serializable]
    public class YuExcelSetting
    {
        /// <summary>
        /// 配置表名。
        /// </summary>
        [LabelText("配置表名")]
        [SerializeField]
        public string ExcelId;

        [LabelText("要解析的工作簿索引")]
        [SerializeField]
        public int ParseSheetId;

        [LabelText("导出的类名")]
        [SerializeField]
        public string ExportClassId;

        [LabelText("忽略解析的工作簿列索引")]
        [SerializeField]
        public List<int> IgonreColumns;

        [LabelText("字段解析配置")]
        [SerializeField]
        public List<YuExcelFieldSetting> FieldParseSettings;
    }
}