#region Head

// Author:        LinYuzhou
// CreateDate:    2019/10/29 08:13:46 
// Email:         836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Client.DataTable
{
    /// <summary>
    /// 用于unity环境的Excel数据表模块。
    /// </summary>
    public interface IDataTableModule
    {
        /// <summary>
        /// 获得指定索引的数据表。
        /// </summary>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetRecordAtIndex<T>(int index) where T : IExcelEntity<T>, new();

        /// <summary>
        /// 获得所有符合条件的数据表。
        /// </summary>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetRecords<T>(Predicate<T> func = null) where T : IExcelEntity<T>, new();

        /// <summary>
        /// 获取第一个符合条件的数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetRecordFirst<T>(Predicate<T> func) where T : IExcelEntity<T>, new();

        /// <summary>
        /// 异步加载数据表数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onLoaded"></param>
        void AsyncLoadEntitys<T>(Action<List<T>> onLoaded) where T: IExcelEntity<T>, new();
    }
}