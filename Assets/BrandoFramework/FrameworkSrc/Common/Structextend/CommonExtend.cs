using System;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 常用扩展功能函数集合类。
    /// </summary>
    public static class CommonExtend
    {
        #region 类型转换关键字函数

        /// <summary>
        /// 尝试将一个对象转型为指定类型。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型。</typeparam>
        /// <param name="t">实例对象。</param>
        /// <returns></returns>
        public static T As<T>(this object t) where T : class
        {
            var instance = t as T;
            return instance;
        }

        #endregion

        #region 基础语法扩展

        public static void IfElse(bool condition, Action trueAction, Action falseAction)
        {
            if (condition)
            {
                trueAction();
            }
            else
            {
                falseAction();
            }
        }

        public static void IfElse<T>(bool condition, T data, Action<T> trueAction, Action<T> falseAction)
        {
            if (condition)
            {
                trueAction(data);
            }
            else
            {
                falseAction(data);
            }
        }

        public static void IfElse<T1, T2>(bool condition, T1 data1,
            T2 data2, Action<T1> trueAction, Action<T2> falseAction)
        {
            if (condition)
            {
                trueAction(data1);
            }
            else
            {
                falseAction(data2);
            }
        }

        #endregion

        #region 浮点值

        /// <summary>
        /// 比较给定的两个浮点数是否近似相等。
        /// 默认的误差值在0.001。
        /// </summary>
        /// <param name="lf"></param>
        /// <param name="rf"></param>
        /// <returns></returns>
        public static bool Equal(this float lf, float rf)
        {
            var result = Math.Abs(lf - rf) < 0.001;
            return result;
        }

        public static bool Equal(this double lf, double rf)
        {
            var result = Math.Abs(lf - rf) < 0.001;
            return result;
        }

        /// <summary>
        /// 获得一个颜色分量的整型表示（255整数颜色值）
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int ColorInt(this float v)
        {
            var fv = v * 255;
            var result = (int) fv;
            return result;
        }

        #endregion

        #region 网络通信

        private static readonly byte[] bigPointBytes = new byte[4];


        /// <summary>
        /// Protobuf 为小端字节序
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetHeadBytes(this byte[] bytes)
        {
            try
            {
                for (var i = 3; i >= 0; i--)
                {
                    bigPointBytes[i] = bytes[3 - i];
                }

                return bigPointBytes;
            }
            catch (Exception e)
            {
                Debug.LogError("GetBigBytes Error:" + e.Message);
                return null;
            }
        }

        #endregion

        #region Type

        public static string TypeName(this object obj) => obj.GetType().Name;

        #endregion
    }
}