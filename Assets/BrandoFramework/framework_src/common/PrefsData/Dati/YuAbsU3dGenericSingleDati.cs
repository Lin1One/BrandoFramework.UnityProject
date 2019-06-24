#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/26 8:45:19
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;


namespace YuU3dPlay
{
    [Serializable]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public abstract class YuAbsU3dGenericSingleDati<TActual, TImpl> : YuAbsU3dGenericJsonDati<TActual, TImpl>
        where TActual : class, new()
        where TImpl : class
    {
        #region 单例资料加载

        private static Dictionary<Type, object> singles;
        private static Dictionary<Type, object> Singles => singles ?? (singles = new Dictionary<Type, object>());

        public static YuAbsU3dGenericSingleDati<TActual, TImpl> GetSingleDati()
        {
            var implType = typeof(TImpl);
            if (Singles.ContainsKey(implType))
            {
                var existInstance = Singles[implType];
                if(existInstance != null)
                {
                    return (YuAbsU3dGenericSingleDati<TActual, TImpl>)existInstance;
                }
                Singles.Remove(implType);
            }

            YuAbsU3dGenericSingleDati<TActual, TImpl> instance = null;
            var scriptPath = YuU3dDatiUtility.GetSingleScriptObjectPath(implType);

#if UNITY_EDITOR
            if (YuUnityUtility.IsEditorMode)
            {
                if (File.Exists(scriptPath))
                {
                    var scriptAsset = YuAssetDatabaseUtility.LoadAssetAtPath(scriptPath, implType);
                    instance = (YuAbsU3dGenericSingleDati<TActual, TImpl>)scriptAsset;
                }
            }
#endif

            if (instance == null)
            {
                var originPath = YuU3dDatiUtility.GetSingleOriginPath(implType);
                instance = YuUnityUtility.IsEditorMode
                    ? LoadSingleOriginAtEditor(implType, originPath, scriptPath)
                    : LoadSingleOriginAtPlay(implType, originPath);
            }

            Singles.Add(implType, instance);
            instance.LoadDetailHelp();
            return instance;
        }

        private static YuAbsU3dGenericSingleDati<TActual, TImpl> LoadSingleOriginAtPlay(Type implType,
            string originPath)
        {
            if (!File.Exists(originPath))
            {
                Debug.LogError($"具体资源文件{originPath}不存在！");
                return null;
            }

            var newScriptInstance = (YuAbsU3dGenericSingleDati<TActual, TImpl>)CreateInstance(implType);
            var actualObj = newScriptInstance.DeSerialize(originPath);
            newScriptInstance.ActualSerializableObject = actualObj;
            return newScriptInstance;
        }

        private static YuAbsU3dGenericSingleDati<TActual, TImpl> LoadSingleOriginAtEditor(Type implType,
            string originPath, string scriptPath)
        {
            var newScriptInstance = (YuAbsU3dGenericSingleDati<TActual, TImpl>)CreateInstance(implType);

            if (File.Exists(originPath))
            {
                var actualObj = newScriptInstance.DeSerialize(originPath);
                newScriptInstance.ActualSerializableObject = actualObj;
            }
            else
            {
                newScriptInstance.ActualSerializableObject = YuDatiFactory.GetActualDataObject<TActual>();
            }

#if UNITY_EDITOR

            YuAssetDatabaseUtility.CreateAsset(newScriptInstance, scriptPath);
            YuAssetDatabaseUtility.Refresh();
#endif

            return newScriptInstance;
        }

        #endregion

        #region 单例资料保存

#if UNITY_EDITOR

        public override void Save()
        {
            var type = GetType();
            var originPath = YuU3dDatiUtility.GetSingleOriginPath(type);

            Serialize(originPath);
            Debug.Log($"目标路径{originPath}的资料文件已更新！");
            YuAssetDatabaseUtility.Refresh();
            var asset = YuAssetDatabaseUtility.LoadAssetAtPath(originPath,
                typeof(TextAsset));
            YuEditorAPIInvoker.PingObject(asset);
        }

#endif

        #endregion
    }
}

