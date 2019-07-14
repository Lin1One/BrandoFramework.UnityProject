using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    public static partial class RectUtil
    {
        public static Rect ReSize(this Rect rt, float _Diff_X, float _Diff_Y)
        {
            _Diff_X *= 0.5f;
            _Diff_Y *= 0.5f;
            
            rt.xMin += _Diff_X;
            rt.yMin += _Diff_Y;
            rt.xMax -= _Diff_X;
            rt.yMax -= _Diff_Y;

            return rt;
        }
        public static Rect[] Division(this Rect rt, float[] _xList, float[] _yList)
        {
            int xCount = _xList == null ? 1 : _xList.Length;
            int yCount = _yList == null ? 1 : _yList.Length;
            List<Rect> result = new List<Rect>();
            float yAcc = 0, height;
            for (int y = 0 ; y < yCount; ++y){
                height = rt.height * (_yList == null || y >= _yList.Length ? 1 : _yList[y]);
                float xAcc = 0, width;
                for (int x = 0 ; x < xCount; ++x){
                    width = rt.width * (_xList == null || x >= _xList.Length ? 1 : _xList[x]);
                    result.Add(new Rect(rt.xMin + xAcc, 
                            rt.yMin + yAcc, 
                            width, height)
                    );
                    xAcc += width;
                }
                yAcc += height;
            }
            return result.ToArray();
        }
        public static Rect[] Division(this Rect rt, int cols, int rows)
        {
            cols = cols < 1 ? 1 : cols;
            rows = rows < 1 ? 1 : rows;

            List<Rect> result = new List<Rect>();
            float cell_width = rt.width / cols;
            float cell_height = rt.height / rows;
            for (int y = 0 ; y < rows; ++y){
                for (int x = 0 ; x < cols; ++x){
                    result.Add(new Rect(rt.xMin + x * cell_width, 
                            rt.yMin + y * cell_height, 
                            cell_width, cell_height)
                    );
                }
            }
            return result.ToArray();
        }
        public static Rect Sub_Vertical(ref Rect rt, float sub_y)
        {
            Rect new_rt = new Rect(rt.position, new Vector2(rt.width, sub_y));
            rt.yMin = new_rt.yMax;
            return new_rt;
        }
        public static Rect Sub_Horizontal(ref Rect rt, float sub_x)
        {
            Rect new_rt = new Rect(rt.position, new Vector2(sub_x, rt.height));
            rt.xMin = new_rt.xMax;
            return new_rt;
        }
#if UNITY_EDITOR
        public static Rect Sub_Vertical(ref Rect rt)
        {
            return Sub_Vertical(ref rt, UnityEditor.EditorGUIUtility.singleLineHeight);
        }
#endif
    }
}