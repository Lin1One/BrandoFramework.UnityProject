#region Head

// Author:            LinYuzhou
// CreateDate:        2019/9/18 01:42:04
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using UnityEngine;


namespace Common.Utility
{
    /// <summary>
    /// Json序列化和反序列化工具。
    /// </summary>
    public static class YuJsonUtility
    {
        public static string ToJson(object obj)
        {
            var jsContent = JsonUtility.ToJson(obj);
            //if (YuUnityUtility.IsEditorMode)
            //{
            //    jsContent = YuEditorAPIInvoker.FormatJson(jsContent);
            //}

            return jsContent;
        }

        public static void WriteAsJson(string path, object obj)
        {
            var jsContent = ToJson(obj);
            IOUtility.WriteAllText(path, jsContent);
        }

        public static T FromJson<T>(string jsContent)
        {
            var instance = JsonUtility.FromJson<T>(jsContent);
            return instance;
        }

        public static object FromJson(string jsContent, Type targetType)
        {
            var instance = JsonUtility.FromJson(jsContent, targetType);
            return instance;
        }
    }
}

