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

using Newtonsoft.Json;
using System;
using System.IO;


namespace Common.Utility
{
    /// <summary>
    /// Json序列化和反序列化工具。
    /// </summary>
    public static class JsonUtility
    {
        public static string ToJson(object obj)
        {
            var jsContent = UnityEngine.JsonUtility.ToJson(obj);
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
            var instance = UnityEngine.JsonUtility.FromJson<T>(jsContent);
            return instance;
        }

        public static object FromJson(string jsContent, Type targetType)
        {
            var instance = UnityEngine.JsonUtility.FromJson(jsContent, targetType);
            return instance;
        }

        /// <summary>
        /// 将Json字符串格式化为对人友好的阅读格式。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PrettifyJsonString(string str)
        {
            var serializer = new JsonSerializer();
            var textReader = new StringReader(str);
            var JsonReader = new JsonTextReader(textReader);
            var obj = serializer.Deserialize(JsonReader);

            if (obj == null)
            {
                return str;
            }

            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };

            serializer.Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
    }
}

