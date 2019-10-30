#region Head

// Author:        LinYuzhou
// CreateDate:    2019/10/29 18:15:56 PM
// Email:         836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Client.DataTable
{
    public class ExcelDataWrapper<T> where T : IExcelEntity<T>
    {
        public static bool isLoaded;
        public static bool isAsyncLoading;
        public static Action<List<T>> onLoaded;

        public static List<T> Entitys{ get;set;}
    }
}