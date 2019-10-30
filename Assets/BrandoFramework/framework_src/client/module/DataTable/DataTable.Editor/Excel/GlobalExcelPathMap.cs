#region Head

// Author:            LinYuzhou
// CreateDate:        2019/10/23 10:03:31
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Core;
using Common.PrefsData;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Client.DataTable.Editor
{
    [Serializable]
    public class GlobalExcelPathMap : YuAbsSingleSetting<GlobalExcelPathMap>
    {
        public List<DataClassToExcelFileMap> AppExcelSourcePathMaps;

        public void AddAppPathMap(ProjectInfo u3DApp, string id, string path)
        {
            if (AppExcelSourcePathMaps == null)
            {
                AppExcelSourcePathMaps = new List<DataClassToExcelFileMap>();
            }

            var appMap = AppExcelSourcePathMaps.Find(map => map.ProjectId == u3DApp.DevelopProjectName);
            if (appMap == null)
            {
                appMap = new DataClassToExcelFileMap();
                appMap.ProjectId = u3DApp.DevelopProjectName;
                AppExcelSourcePathMaps.Add(appMap);
            }

            appMap.AddPathMap(id, path);
        }

        public static string GetFilename(string classId, string appId)
        {
            var filenameMap = Instance.AppExcelSourcePathMaps
                .Find(m => m.ProjectId == appId);
            if (filenameMap == null)
            {
                throw new Exception($"目标应用{appId}的Excel实体类路文件名映射数据不存在！");
            }

            var fileIndex = filenameMap.ClassIds.IndexOf(classId);
            if (fileIndex == -1)
            {
                throw new Exception($"目标Excel数据实体{classId}的文件名映射数据不存在！");
            }

            var filename = filenameMap.ExcelFileIds[fileIndex];
            return filename;
        }
    }
}