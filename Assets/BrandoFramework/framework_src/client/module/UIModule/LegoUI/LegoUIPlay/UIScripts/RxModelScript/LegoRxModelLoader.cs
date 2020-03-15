using Client.Assets;
using Client.Core;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI数据模型加载器。
    /// 负责加载一个项目中所有的数据模型。
    /// </summary>
    [Singleton]
    public class LegoRxModelLoader
    {
        //private Dictionary<string, YuLegoRxModelStorage> appModels
        //    = new Dictionary<string, YuLegoRxModelStorage>();

        private readonly YuLegoRxModelStorage currentStorage
            = new YuLegoRxModelStorage();

        private readonly Dictionary<string, object> rxModels
            = new Dictionary<string, object>();

        public object LoadModel(string logicId)
            => currentStorage.LoadModel(logicId);

        public object CreateModel(string logicId)
            => currentStorage.CreateModel(logicId);

        private class YuLegoRxModelStorage
        {
            private readonly Dictionary<string, object> rxModels
                = new Dictionary<string, object>();

            private IAssetModule assetModule;

            private IAssetModule AssetModule =>
                assetModule ?? (assetModule = Injector.Instance .Get<IAssetModule>());

            //private IYuU3dAppEntity appEntity;

            //private IYuU3dAppEntity AppEntity =>
            //    appEntity ?? (appEntity = YuU3dAppUtility.Injector.Get<IYuU3dAppEntity>());

#if UNITY_EDITOR
            private static bool? m_isAssetBundleLoad;
            private static bool IsAssetBundleLoad
            {
                get
                {
                    if (m_isAssetBundleLoad == null)
                    {
                        //m_isAssetBundleLoad = U3dGlobal.Get<IYuU3dAppEntity>().RunSetting.IsLoadFromAssetBundle;
                    }
                    return m_isAssetBundleLoad == true;
                }
            }
#endif

            public object LoadModel(string logicId)
            {
#if UNITY_EDITOR
                if(!IsAssetBundleLoad)
                {
                    return LoadModelAtEditor(logicId);
                }
                else
                {
                    return LoadModelAtPlay(logicId);
                }
#else
                 return LoadModelAtPlay(logicId);
#endif
            }

            public object CreateModel(string logicId)
            {
                string appId = ProjectInfoDati.GetActualInstance().DevelopProjectName;
                var type = GetRxModelType(appId, logicId);
                var newModel = Activator.CreateInstance(type);
                return newModel;
            }

#if UNITY_EDITOR

            private object LoadModelAtEditor(string logicId)
            {
                if (rxModels.ContainsKey(logicId))
                {
                    var targetModel = rxModels[logicId];
                    return targetModel;
                }

                var app = ProjectInfoDati.GetActualInstance();
                var type = GetRxModelType(app.DevelopProjectName, logicId);
                //反射构造数据模型类型
                var newModel = Activator.CreateInstance(type);
                rxModels.Add(logicId, newModel);
                return newModel;
            }

#endif

            private object LoadModelAtPlay(string logicId)
            {
                if (rxModels.ContainsKey(logicId))
                {
                    var targetModel = rxModels[logicId];
                    return targetModel;
                }

                var app = ProjectInfoDati.GetActualInstance();
                var type = GetRxModelType(app.DevelopProjectName, logicId);
                var finalId = logicId ?? type.Name;
                finalId += "_RxModel";
                if (finalId.Contains("@"))
                {
                    finalId = finalId.Replace("@", "_")
                        .Replace("=", "_");
                }

                var assetId = app.DevelopProjectName + "_" + finalId;

                TextAsset textAsset = null;
                try
                {
                    textAsset = AssetModule.Load<TextAsset>(assetId);
                }
                catch(Exception e)
                {
#if DEBUG
                    Debug.LogError(e.Message + e.StackTrace);
#endif
                }
                if (textAsset == null)
                {
                    var newModel = Activator.CreateInstance(type);
                    return newModel;
                }

                var instance = JsonUtility.FromJson(textAsset.text, type);
                return instance;
            }


            #region 数据模型类型存储和获取

            private Dictionary<string, Dictionary<string, Type>> rxTypeDict;

            private Dictionary<string, Dictionary<string, Type>> RxTypeDict
            {
                get
                {
                    if (rxTypeDict != null)
                    {
                        return rxTypeDict;
                    }

                    rxTypeDict = new Dictionary<string, Dictionary<string, Type>>();
                    return rxTypeDict;
                }
            }

            private Dictionary<string, Type> GetAppRxModelTypeDic(string appId)
            {
                if (RxTypeDict.ContainsKey(appId))
                {
                    return RxTypeDict[appId];
                }

                var appAsm = YuUnityIOUtility.GetUnityAssembly(appId + "Play");
                var typeDic = ReflectUtility.GetTypeDictionary<IYuLegoUIRxModel>(appAsm);
                var typeDic2 = ReflectUtility.GetTypeDictionary<IYuLegoScrollViewRxModel>(appAsm);
                typeDic.Combin(typeDic2);
                RxTypeDict.Add(appId, typeDic);
                return typeDic;
            }

            private readonly Dictionary<string, string> typeStrMap
                = new Dictionary<string, string>();

            private Type GetRxModelType(string appId, string logicId)
            {
                var appTypeDic = GetAppRxModelTypeDic(appId);
                string typeStr;

                if (!logicId.Contains("@"))
                {
                    typeStr = appId + "_" + logicId + "_RxModel";
                }
                else
                {
                    logicId = logicId.Split('@')[0];
                    if (logicId.Contains("ScrollView"))
                    {
                        logicId = logicId.Insert(0, "Lego");
                    }

                    typeStr = appId + "_" + logicId + "_RxModel";
                }

                if (logicId.Contains(appId))
                {
                    var type = appTypeDic[logicId];
                    return type;
                }

                if (typeStrMap.ContainsKey(logicId))
                {
                    typeStr = typeStrMap[logicId];
                }
                else
                {
                    typeStrMap.Add(logicId, typeStr);
                }

                if (!appTypeDic.ContainsKey(typeStr))
                {
                    throw new Exception($"目标数据模型类型{typeStr}没有找到！");
                }

                return appTypeDic[typeStr];
            }

            #endregion
        }
    }
}