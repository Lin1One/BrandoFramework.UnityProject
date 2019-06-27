#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Common.PrefsData
{
    [Serializable]
    public abstract class GenericSingleDati<TActual, TImpl> : GenericDatiInJson<TActual, TImpl>
        where TActual : class, new()
        where TImpl : class
    {

        #region 单例资料加载


        private static Dictionary<Type, object> singles;
        private static Dictionary<Type, object> Singles => 
            singles ?? (singles = new Dictionary<Type, object>());

        /// <summary>
        /// 获取单例配置数据
        /// 编辑器环境下：第一次打开，将保存的 txt 文件反序列化为所需资料数据，
        /// 如没有找到对应的 Txt，将自动创建一份 Asset 文件，以保存持久化数据。
        /// 点击保存资料文件以将Asset序列化为 txt，以更安全地保存持久化数据。
        /// </summary>
        /// <returns></returns>
        public static GenericSingleDati<TActual, TImpl> GetSingleDati()
        {
            var implType = typeof(TImpl);
            if (Singles.ContainsKey(implType))
            {
                var existInstance = Singles[implType];
                if(existInstance != null)
                {
                    return (GenericSingleDati<TActual, TImpl>)existInstance;
                }
                Singles.Remove(implType);
            }

            GenericSingleDati<TActual, TImpl> instance = null;
            var assetFilePath = DatiUtility.GetSingleScriptObjectPath(implType);

#if UNITY_EDITOR
            if (YuUnityUtility.IsEditorMode)
            {
                if (File.Exists(assetFilePath))
                {
                    var scriptAsset = AssetDatabaseUtility.LoadAssetAtPath(assetFilePath, implType);
                    instance = (GenericSingleDati<TActual, TImpl>)scriptAsset;
                }
            }
#endif

            if (instance == null)
            {
                var originPath = DatiUtility.GetSingleOriginPath(implType);
                instance = YuUnityUtility.IsEditorMode
                    ? LoadSingleOriginAtEditor(implType, originPath, assetFilePath)
                    : LoadSingleOriginAtPlay(implType, originPath);
            }

            Singles.Add(implType, instance);
            instance.LoadDetailHelp();
            return instance;
        }

        public static TActual GetActualInstance()
        {
            return GetSingleDati().ActualSerializableObject;
        }


        private static GenericSingleDati<TActual, TImpl> LoadSingleOriginAtPlay(Type implType,
            string originPath)
        {
            if (!File.Exists(originPath))
            {
                Debug.LogError($"具体资源文件{originPath}不存在！");
                return null;
            }

            var newScriptInstance = (GenericSingleDati<TActual, TImpl>)CreateInstance(implType);
            var actualObj = newScriptInstance.DeSerialize(originPath);
            newScriptInstance.ActualSerializableObject = actualObj;
            return newScriptInstance;
        }

        private static GenericSingleDati<TActual, TImpl> LoadSingleOriginAtEditor(Type implType,
            string originPath, string scriptPath)
        {
            var newScriptInstance = (GenericSingleDati<TActual, TImpl>)CreateInstance(implType);

            if (File.Exists(originPath))
            {
                var actualObj = newScriptInstance.DeSerialize(originPath);
                newScriptInstance.ActualSerializableObject = actualObj;
            }
            else
            {
                ////newScriptInstance.ActualSerializableObject = YuDatiFactory.GetActualDataObject<TActual>();
            }

#if UNITY_EDITOR
            //没有序列化文件则创建 Asset 文件
            AssetDatabaseUtility.CreateAsset(newScriptInstance, scriptPath);
            AssetDatabaseUtility.Refresh();
#endif

            return newScriptInstance;
        }

        #endregion

        #region 单例资料保存

#if UNITY_EDITOR

        public override void Save()
        {
            var type = GetType();
            var originPath = DatiUtility.GetSingleOriginPath(type);

            Serialize(originPath);
            Debug.Log($"目标路径{originPath}的资料文件已更新！");
            AssetDatabaseUtility.Refresh();
            //var asset = YuAssetDatabaseUtility.LoadAssetAtPath(originPath,
                //typeof(TextAsset));
            //YuEditorAPIInvoker.PingObject(asset);
        }

#endif

        #endregion
    }
}

