#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using Common.Utility;
using System;
using System.IO;


namespace Common.PrefsData
{
    /// <summary>
    /// 使用 unity3d 自带json序列化的资料库类型。
    /// 注意：为了序列化其字段不可以使用字典实例。
    /// </summary>
    /// <typeparam name="TActual"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    [Serializable]
    [DatiSuffix(".txt")]
    public abstract class GenericDatiInJson<TActual, TImpl> : GenericDati<TActual, TImpl>
        where TActual : class, new()
        where TImpl : class
    {
        protected override void Serialize(string originPath)
        {
            var content = JsonUtility.ToJson(ActualSerializableObject);
            content = JsonUtility.PrettifyJsonString(content);
            IOUtility.WriteAllText(originPath, content);
        }

        protected override TActual DeSerialize(string originPath)
        {
            var content = File.ReadAllText(originPath);
            var actual = (TActual)JsonUtility.FromJson(content, typeof(TActual));
            return actual;
        }
    }
}