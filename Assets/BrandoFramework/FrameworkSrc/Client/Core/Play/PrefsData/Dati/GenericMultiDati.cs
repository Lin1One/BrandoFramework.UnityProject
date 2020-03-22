#region Head

// Author:            LinYuzhou
// CreateDate:        2019/1/26 8:45:19
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Utility;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Common.PrefsData
{
    [Serializable]
    public abstract class GenericMultiDati<TActual, TImpl> : GenericDatiInJson<TActual, TImpl>
        where TActual : class, new()
        where TImpl : class
    {
        [SerializeField]
        [ShowIf("CheckIsNotAppDati")]
        [LabelText("资料文件实例ID")]
        [ReadOnly]
        private string multiId;

        protected virtual bool CheckIsNotAppDati() => true;


        #region 多实例获取和保存

        private static Dictionary<Type, Dictionary<string, object>> multis;

        private static Dictionary<Type, Dictionary<string, object>> Multis =>
            multis ?? (multis = new Dictionary<Type, Dictionary<string, object>>());

        //private static string LastMultiId => YuU3dAppUtility.Prefs?.GetStringItem(MultiTypeId)?.Value;

        private static string MultiTypeId
        {
            get
            {
                var type = typeof(TImpl);
                return type.Name;
            }
        }

        private static GenericMultiDati<TActual, TImpl> currentInstance;

        private static GenericMultiDati<TActual, TImpl> CurrentInstance
        {
            get
            {
                if (currentInstance != null)
                {
                    return currentInstance;
                }
                //var lastId = LastMultiId;
                //currentInstance = GetMultiAtId(lastId);

#if UNITY_EDITOR
                if (currentInstance == null)
                {
                    Debug.LogError($"资料文件{typeof(TImpl).Name}" +
                                   $"在Prefs中无法找到当前实例ID，请先设置实例ID。");
                }
#endif

                return currentInstance;
            }
        }

        /// <summary>
        /// 当前的实际数据存储对象。
        /// </summary>
        public static TActual CurrentActual => CurrentInstance?.ActualSerializableObject;

        public static GenericMultiDati<TActual, TImpl> GetMultiAtId(string multiId)
        {
            if (string.IsNullOrEmpty(multiId))
            {
                return null;
            }

            var implType = typeof(TImpl);
            if (Multis.ContainsKey(implType))
            {
                if (Multis[implType].ContainsKey(multiId))
                {
                    var multiObjes = Multis[implType];
                    var multiObj = (GenericMultiDati<TActual, TImpl>)multiObjes[multiId];
                    if (multiObj == null)
                    {
                        Multis[implType].Remove(multiId);
                    }
                    else
                    {
                        return multiObj;
                    }
                }
            }
            else
            {
                Multis.Add(implType, new Dictionary<string, object>());
            }

            GenericMultiDati<TActual, TImpl> multiInstance = null;

#if UNITY_EDITOR

            multiInstance = GetMultiOriginAtEditor(implType, multiId);
#else
            multiInstance = GetMultiOriginAtPlay(implType, multiId);
#endif
            currentInstance = multiInstance;

            Multis[implType].Add(multiId, multiInstance);
#if UNITY_EDITOR

            multiInstance?.LoadDetailHelp();
#endif
            return multiInstance;
        }

        private static GenericMultiDati<TActual, TImpl> GetMultiOriginAtPlay(Type implType,
            string id)
        {
            //var newScriptInstance = (YuAbsU3dGenericMultiDati<TActual, TImpl>)CreateInstance(implType);
            //newScriptInstance.multiId = id;
            ////var datiAssetName = $"{id}_{implType.Name}";

            //var bytes = MultiDatiBundle.LoadAsset<TextAsset>(datiAssetName);
            //var actualObj = (TActual)YuJsonUtility.FromJson(bytes.ToString(), typeof(TActual));
            //newScriptInstance.ActualSerializableObject = actualObj;
            return default;
        }

        public static GenericMultiDati<TActual, TImpl> GetMultiOriginAtPlay(Type implType,
    string id,AssetBundle datiassetbundle)
        {
            var newScriptInstance = (GenericMultiDati<TActual, TImpl>)CreateInstance(implType);
            newScriptInstance.multiId = id;
            var datiAssetName = $"{id}_{implType.Name}";

            var bytes = datiassetbundle.LoadAsset<TextAsset>(datiAssetName);
            var actualObj = (TActual)UnityEngine.JsonUtility.FromJson(bytes.ToString(), typeof(TActual));
            newScriptInstance.ActualSerializableObject = actualObj;
            return newScriptInstance;
        }

#if UNITY_EDITOR
        private static GenericMultiDati<TActual, TImpl> GetMultiOriginAtEditor(Type implType,
            string id)
        {
            var scriptPath = DatiUtility.GetMultiScriptPath(implType, id);
            if (File.Exists(scriptPath))
            {
                //var scriptAsset = YuAssetDatabaseUtility.LoadAssetAtPath(scriptPath, implType);
                //var multiInstance = (YuAbsU3dGenericMultiDati<TActual, TImpl>)scriptAsset;
                //if(multiInstance!=null)
                //{
                //    return multiInstance;
                //}
            }

            var newScriptInstance = (GenericMultiDati<TActual, TImpl>)CreateInstance(implType);
            newScriptInstance.multiId = id;
            var originPath = DatiUtility.GetMultiOriginPath(implType, id);

            if (File.Exists(originPath))
            {
                var actualObj = newScriptInstance.DeSerialize(originPath);
                newScriptInstance.ActualSerializableObject = actualObj;
            }
            else
            {
                ////newScriptInstance.ActualSerializableObject = YuDatiFactory.GetActualDataObject<TActual>();
            }

            ////YuAssetDatabaseUtility.CreateAsset(newScriptInstance, scriptPath);
            return newScriptInstance;    
        }
#endif
        public static void SetMultiDati(string multiId, string datiAssetStr,Type implType)
        {
            var newScriptInstance = (GenericMultiDati<TActual, TImpl>)CreateInstance(implType);
            newScriptInstance.multiId = multiId;
            ////var actualObj = (TActual)YuJsonUtility.FromJson(datiAssetStr, typeof(TActual));
            ////newScriptInstance.ActualSerializableObject = actualObj;
            
            if (Multis.ContainsKey(implType))
            {
                if (Multis[implType].ContainsKey(multiId))
                {
                    Multis[implType][multiId] = newScriptInstance;
                    return;
                }
            }
            else
            {
                Multis.Add(implType, new Dictionary<string, object>());
            }
            Multis[implType].Add(multiId, newScriptInstance);

        }



#region 获取所有实际数据和实例

        private static List<GenericMultiDati<TActual, TImpl>> allInstance;

        public static List<GenericMultiDati<TActual, TImpl>> AllInstance
        {
            get
            {
                if (allInstance != null)
                {
                    return allInstance;
                }

#if UNITY_EDITOR
                allInstance = UnityModeUtility.IsEditorMode
                    ? GetAllInstanceAtEditor()
                    : GetAllInstanceAtPlay();
#else
                    allInstance = GetAllInstanceAtPlay();
#endif

                var implType = typeof(TImpl);
                Dictionary<string, object> targetMultis = null;

                if (Multis.ContainsKey(implType))
                {
                    targetMultis = Multis[implType];
                }
                else
                {
                    targetMultis = new Dictionary<string, object>();
                    Multis.Add(implType, targetMultis);
                }

                foreach (var u3dDati in allInstance)
                {
                    targetMultis.Add(u3dDati.multiId, u3dDati);
                }

                return allInstance;
            }
        }

        private static List<TActual> allActuals;

        public static List<TActual> AllActuals
        {
            get
            {
                if (allActuals != null)
                {
                    return allActuals;
                }

                allActuals = new List<TActual>();
                foreach (var dati in AllInstance)
                {
                    allActuals.Add(dati.ActualSerializableObject);
                }

                return allActuals;
            }
        }


        private static List<GenericMultiDati<TActual, TImpl>> GetAllInstanceAtPlay()
        {
            //var assetBundlePath = YuU3dDatiUtility.GetDatiAssetBundlePath(typeof(TImpl));
            var instances = new List<GenericMultiDati<TActual, TImpl>>();
            //var implType = typeof(TImpl);
            //var bytes = MultiDatiBundle.LoadAllAssets<TextAsset>();






            //foreach (var datistr in bytes)
            //{

            //    var newScriptInstance = (YuAbsU3dGenericMultiDati<TActual, TImpl>)CreateInstance(implType);

            //    var actualObj = (TActual)YuJsonUtility.FromJson(datistr.ToString(), typeof(TActual));
            //    newScriptInstance.ActualSerializableObject = actualObj;

            //    //var instance = (TImpl)YuJsonUtility.FromJson(datistr.ToString(), typeof(TImpl));
            //    instances.Add(newScriptInstance);
            //}
            return instances;
        }

#if UNITY_EDITOR
       

        private static List<GenericMultiDati<TActual, TImpl>> GetAllInstanceAtEditor()
        {
            var instances = new List<GenericMultiDati<TActual, TImpl>>();
            var rootDir = DatiUtility.GetMultiScriptRootPath(typeof(TImpl));
            var paths = IOUtility.GetPaths(rootDir).Where(p => p.EndsWith(".asset"))
                .ToList();

            foreach (var p in paths)
            {
                //var instance = (YuAbsU3dGenericMultiDati<TActual, TImpl>)YuAssetDatabaseUtility.LoadAssetAtPath(p, typeof(TImpl));
                //instances.Add(instance);
            }
            return instances;
        }

#endif

#endregion

#endregion

#if UNITY_EDITOR

        public override void Save()
        {
            var type = GetType();
            var originPath = DatiUtility.GetMultiOriginPath(type, multiId);

            Serialize(originPath);
            Debug.Log($"目标路径{originPath}的资料文件已更新！");
            //YuAssetDatabaseUtility.Refresh();
            //var asset = YuAssetDatabaseUtility.LoadAssetAtPath(originPath,
            //    typeof(TextAsset));
            //YuEditorAPIInvoker.PingObject(asset);
        }

        [HorizontalGroup("底部按钮")]
        [GUIColor(0.6f, 0.6f, 0.8f)]
        [Button("设置为当前实例", ButtonSizes.Medium)]
        [DisableIf("CheckIsMulti")]
        protected virtual void SetCurrent()
        {
            //YuU3dAppUtility.Prefs.SetStringItem(MultiTypeId, multiId, null);
            //YuU3dAppUtility.Prefs.Save();
            //YuEditorAPIInvoker.RefreshAsset();
            currentInstance = GetMultiAtId(multiId);
            Debug.Log($"资料文件{MultiTypeId}的实例{multiId}已设置为当前实例！");
        }

#endif


    }
}

