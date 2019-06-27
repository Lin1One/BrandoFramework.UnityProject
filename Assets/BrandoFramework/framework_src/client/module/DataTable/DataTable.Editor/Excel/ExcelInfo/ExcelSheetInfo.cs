#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace Client.DataTable.Editor
{
    /// <summary>
    /// Excel表格数据。
    /// 1. 元数据。
    /// 2. 表格字段数据。
    /// </summary>
    public class ExcelSheetInfo
    {
        public string SheetId { get; private set; }
        /// <summary>
        /// excel工作簿英文Id。
        /// </summary>
        public string EnglishId { get; private set; }

        public string ChineseId { get; private set; }

        /// <summary>
        /// 表格字段列表。
        /// </summary>
        public List<ExcelFieldInfo> FieldInfos { get; }

        public ISheet SourceSheet { get; private set; }

        public ExcelSheetInfo(ISheet sheet)
        {
            SourceSheet = sheet;
            ParseClassId(SourceSheet.SheetName);
            FieldInfos = new List<ExcelFieldInfo>();
        }

        /// <summary>
        /// 添加字段信息
        /// </summary>
        /// <param name="fieldInfo"></param>
        public void AddFieldInfo(ExcelFieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                return;
            }

            FieldInfos.Add(fieldInfo);
        }

        /// <summary>
        /// 解析类名
        /// </summary>
        /// <param name="sheetname"></param>
        private void ParseClassId(string sheetname)
        {
            if (sheetname.Contains("@"))
            {
                SheetId = sheetname;
                var array = sheetname.Split('@');
                ChineseId = array[0];
                EnglishId = array[1];
            }
            else
            {
                SheetId = EnglishId = ChineseId = sheetname;
            }
        }
    }
}