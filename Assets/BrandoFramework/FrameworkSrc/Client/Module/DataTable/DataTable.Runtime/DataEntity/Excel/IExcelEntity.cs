using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.DataTable
{
    public interface IExcelEntity
    {
        /// <summary>
        /// 该数据表的数据是否分为多个文件保存读取
        /// </summary>
        bool IsSplitData { get; set; }

        /// <summary>
        /// 分区 ID
        /// </summary>
        int SplitDataIndex { get;}

        List<DataSplitNumRange> GetExcelEnitySplitDataRange();

        /// <summary>
        /// 将excel的一行数据源转换为数据实例。
        /// </summary>
        void InitEntity(List<string> cells);

        /// <summary>
        /// 将excel的数据源转换为数据列表。
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

    public class DataSplitNumRange
    {
        int minID;
        int maxID;

        public DataSplitNumRange(int min,int max)
        {
            if( min > max )
            {
                min = 0;
                max = 0;
            }
            else
            {
                minID = min;
                maxID = max;
            }
        }

        public bool IsIDInThisRange(int id)
        {
            return id >= minID && id < maxID;
        }
    } 
}

