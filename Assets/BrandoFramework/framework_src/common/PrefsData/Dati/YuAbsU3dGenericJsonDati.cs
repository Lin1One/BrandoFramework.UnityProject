#region Head

// Author:            Yu
// CreateDate:        2019/1/21 19:52:10
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.PrefsData;
using System;
using System.IO;
using UnityEngine;


namespace YuU3dPlay
{
    /// <summary>
    /// 使用unity3d自带json序列化的资料库类型。
    /// 为了序列化其字段不可以使用字典实例。
    /// </summary>
    /// <typeparam name="TActual"></typeparam>
    /// <typeparam name="TImpl"></typeparam>
    [Serializable]
    [YuDatiSuffix(".txt")]
    public abstract class YuAbsU3dGenericJsonDati<TActual, TImpl> : YuAbsU3dGenericDati<TActual, TImpl>
        where TActual : class, new()
        where TImpl : class
    {
        protected override void Serialize(string originPath)
        {
            var content = JsonUtility.ToJson(ActualSerializableObject);
            //content = EditorAPIInvoker.PrettifyJsonString(content);
            IOUtility.WriteAllText(originPath, content);
        }

        protected override TActual DeSerialize(string originPath)
        {
            var content = File.ReadAllText(originPath);
            var actual = (TActual)YuJsonUtility.FromJson(content, typeof(TActual));
            return actual;
        }
    }
}