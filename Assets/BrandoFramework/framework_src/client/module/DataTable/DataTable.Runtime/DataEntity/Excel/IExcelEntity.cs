using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.DataTable
{
    public interface IExcelEntity
    {
        /// <summary>
        /// 将excel的一行txt数据源转换为数据实例。
        /// </summary>
        void InitEntity(List<string> cells);

        /// <summary>
        /// 将excel的txt数据源转换为数据列表。
        /// </summary>
        List<IExcelEntity> InitEntitys(List<string> rows);

    }

    public interface IExcelEntity<T>: IExcelEntity where T: IExcelEntity
    {
        /// <summary>
        /// 获得本地数据的一份深度复制副本
        /// </summary>
        T DeepCopy();

    }
}

