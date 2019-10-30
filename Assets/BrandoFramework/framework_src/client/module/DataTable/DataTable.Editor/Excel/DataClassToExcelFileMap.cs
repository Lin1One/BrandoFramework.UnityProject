#region Head

// Author:            LinYuzhou
// CreateDate:        2019/10/23 11:56:19
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// 应用Excel源文件路径和生成的脚本之间的映射数据。
    /// </summary>
    [Serializable]
    public class DataClassToExcelFileMap
    {
        public string ProjectId;
        public List<string> ClassIds;
        public List<string> ExcelFileIds;

        public void AddPathMap(string classId, string path)
        {
            if (ClassIds == null)
            {
                ClassIds = new List<string>();
            }

            if (ExcelFileIds == null)
            {
                ExcelFileIds = new List<string>();
            }

            if (ClassIds.Contains(classId))
            {
                Debug.Log($"Excel数据实体{classId}的路径映射已存在，将更新为新的数据！");
                var index = ClassIds.IndexOf(classId);
                ClassIds.Remove(classId);
                ExcelFileIds.RemoveAt(index);
            }

            ClassIds.Add(classId);
            ExcelFileIds.Add(path);
        }

        public string GetExcelPath(string id)
        {
            if (ClassIds.Contains(id))
            {
                var index = ClassIds.IndexOf(id);
                return ExcelFileIds[index];
            }

            Debug.LogError($"所请求的Excel数据实体{id}没有对应的Excel文件路径存在！");
            return null;
        }
    }
}