#region Head

// Author:            Yu
// CreateDate:        2018/8/14 22:10:45
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    [Serializable]
    public class LegoColorMeta
    {
        [NonSerialized] private static Dictionary<LegoColorMeta, Color>
            colors = new Dictionary<LegoColorMeta, Color>();

        private static Dictionary<LegoColorMeta, Color> ColorDict
        {
            get { return colors ?? (colors = new Dictionary<LegoColorMeta, Color>()); }
        }

        public float R;
        public float G;
        public float B;
        public float A;

        public static LegoColorMeta Create(Color color)
        {
            var meta = new LegoColorMeta
            {
                R = color.r,
                G = color.g,
                B = color.b,
                A = color.a
            };

            return meta;
        }

        public Color ToColor()
        {
            if (ColorDict.ContainsKey(this))
            {
                return ColorDict[this];
            }

            var color = new Color(R, G, B, A);
            ColorDict.Add(this, color);
            return color;
        }
    }
}