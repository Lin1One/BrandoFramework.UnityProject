#region Head

// Author:            Chengkefu
// CreateDate:        2018/10/29 11:05:55
// Email:             chengkefu0730@live.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using UnityEngine;

namespace Common.DataStruct
{
    [Serializable]
    public struct Point2
    {
        public int x;
        public int y;

        public Point2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Point2 obj1, Point2 obj2)
        {
            return (obj1.x == obj2.x && obj1.y == obj2.y);
        }

        public static bool operator !=(Point2 obj1, Point2 obj2)
        {
            return (obj1.x != obj2.x || obj1.y != obj2.y);
        }

        /// <summary>
        /// 判断两个点是否相邻或重叠
        /// </summary>
        /// <param name="coord1"></param>
        /// <param name="coord2"></param>
        /// <returns></returns>
        public static bool IsNearby(Point2 coord1, Point2 coord2)
        {
            if (Mathf.Abs(coord1.x - coord2.x) < 2 &&
               Mathf.Abs(coord1.y - coord2.y) < 2)
            {
                return true;
            }
            return false;
        }
    }
}

