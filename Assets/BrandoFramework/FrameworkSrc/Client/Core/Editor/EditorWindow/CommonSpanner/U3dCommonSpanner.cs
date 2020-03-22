#region Head

// Author:            LinYuzhou
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace Client.Core.Editor
{
    /// <summary>
    /// 提供常用的基础辅助功能。
    /// </summary>
    [Serializable]
    public class U3dCommonSpanner
    {
        #region 构造方法

        public U3dCommonSpanner()
        {
            DatiMultiId = PlayerPrefs.GetString(DATI_ID);
        }

        #endregion

        #region 创建资料

        private const string DATI_ID = "DATI_ID";

        [BoxGroup("创建资产文件")] [LabelText("多实例资料文件ID")]
        public string DatiMultiId;

        [BoxGroup("创建资产文件")]
        [HorizontalGroup("创建资产文件/创建资料按钮组")]
        [Button("创建单实例资料", ButtonSizes.Medium)]
        private void ShowCreateSingleDatiMenu()
        {
            var menu = new GenericMenu();

            foreach (var datiDesc in U3dCommonSpannerUtility.SingleDescs)
            {
                menu.AddItem(new GUIContent(datiDesc.Title), false, CreateDati, datiDesc);
            }

            menu.ShowAsContext();
        }

        //[BoxGroup("创建资产文件")]
        //[HorizontalGroup("创建资产文件/创建资料按钮组")]
        //[Button("创建多实例资料", ButtonSizes.Medium)]
        //private void ShowCreateMultiDatiMenu()
        //{
        //    if (string.IsNullOrEmpty(DatiMultiId))
        //    {
        //        YuEditorUtility.DisplayTooptx("多实例资料ID不能为空！");
        //        return;
        //    }

        //    var menu = new GenericMenu();

        //    foreach (var datiDesc in YuU3dCommonSpannerUtility.MultiDescs)
        //    {
        //        menu.AddItem(new GUIContent(datiDesc.Title), false, CreateDati, datiDesc);
        //    }

        //    menu.ShowAsContext();
        //}

        private void CreateDati(object data)
        {
            var datiDesc = (DatiDescAttribute)data;
            //switch (datiDesc.DatiSaveType)
            //{
            //    case YuDatiSaveType.Single:
            //        CreateSingleDati(datiDesc);
            //        break;
            //    case YuDatiSaveType.Multi:
            //        CreateMultiDati(datiDesc);
            //        break;
            //    case YuDatiSaveType.AutoDate:
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        //private void CreateSingleDati(YuDatiDescAttribute datiDesc)
        //{
        //    var datiType = YuU3dCommonSpannerUtility.DatiTypes.Find(t => t == datiDesc.DatiType);
        //    var scriptPath = YuU3dDatiUtility.GetSingleScriptObjectPath(datiType);
        //    CreateDatiScriptAsset(datiType, scriptPath);
        //}

        //private void CreateMultiDati(YuDatiDescAttribute datiDesc)
        //{
        //    var datiType = YuU3dCommonSpannerUtility.DatiTypes.Find(t => t == datiDesc.DatiType);
        //    var scriptPath = YuU3dDatiUtility.GetMultiScriptPath(datiType, DatiMultiId);

        //    // 在创建多实例资料文件的同时反射设置其资料实例ID
        //    CreateDatiScriptAsset(datiType, scriptPath, newDati =>
        //    {
        //        YuReflectUtility.LoopSetFieldNoCache(newDati.GetType(),
        //            "multiId", newDati, DatiMultiId);
        //        PlayerPrefs.SetString(DATI_ID, DatiMultiId);
        //    });
        //}

        //private void CreateDatiScriptAsset(Type datiType, string scriptPath,
        //    Action<object> multiDataCallback = null)
        //{
        //    if (File.Exists(scriptPath))
        //    {
        //        YuEditorUtility.DisplayError($"目标路径{scriptPath}上已有可序列化脚本资产存在！");
        //        return;
        //    }

        //    var newDati = YuAbsU3dDati.CreateDati(datiType);
        //    multiDataCallback?.Invoke(newDati);
        //    YuIOUtility.EnsureDirExist(scriptPath);
        //    AssetDatabase.CreateAsset(newDati, scriptPath);
        //    var scriptAsset = AssetDatabase.LoadAssetAtPath(scriptPath, datiType);
        //    EditorGUIUtility.PingObject(scriptAsset);
        //    AssetDatabase.Refresh();
        //}

        #endregion

        #region 菜单项配置

        //[BoxGroup("菜单项配置")] [HideLabel] public YuFeatureHubMenuItemConfig MenuItemConfig;

        #endregion

        #region 预编译指令

        [BoxGroup("预编译配置")] [HideLabel] public U3dPreComplieSetting PreComplieSetting;

        [BoxGroup("预编译配置")]
        [HorizontalGroup("预编译配置/水平按钮组")]
        [Button("重载当前预编译")]
        private void ReloadPrecompiled()
        {
            //foreach (YuU3dBuildTargetGroup yuBuildTargetGroup in Enum.GetValues(typeof(YuU3dBuildTargetGroup)))
            //{
            //    var buildTargetGroup = AsBuildTargetGroup(yuBuildTargetGroup);
            //    var platformSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            //    if (string.IsNullOrEmpty(platformSymbols))
            //    {
            //        continue;
            //    }

            //    foreach (var symbol in platformSymbols.Split(';'))
            //    {
            //        var node = PreComplieSetting.GetPrecompiledNode(symbol);

            //        if (node == null)
            //        {
            //            node = new YuU3dPrecompiledSettingNode {PrecompiledDefineId = symbol};
            //            PreComplieSetting.AddNode(node);
            //        }

            //        node.BuildTargetGroup |= yuBuildTargetGroup;
            //    }
            //}
        }

        private BuildTargetGroup AsBuildTargetGroup(U3dBuildTargetGroup yuBuildTargetGroup)
        {
            var buildTargetGroup = (BuildTargetGroup) Enum.Parse(typeof(BuildTargetGroup),
                yuBuildTargetGroup.ToString());
            return buildTargetGroup;
        }

        [BoxGroup("预编译配置")]
        [HorizontalGroup("预编译配置/水平按钮组")]
        [Button("应用")]
        private void ApplyPrecompiled()
        {
            //var precompiledCache = new Dictionary<BuildTargetGroup, string>();

            //foreach (var node in PreComplieSetting.PrecompiledSettingNodes)
            //{
            //    var array = node.BuildTargetGroup.ToString().Split(',');
            //    foreach (var s in array)
            //    {
            //        var buildTargetGroup = s.AsEnum<BuildTargetGroup>();
            //        if (!precompiledCache.ContainsKey(buildTargetGroup))
            //        {
            //            precompiledCache.Add(buildTargetGroup, node.PrecompiledDefineId);
            //        }
            //        else
            //        {
            //            var nowPrecompiled = precompiledCache[buildTargetGroup];
            //            var newPrecompiled = nowPrecompiled + ";" + node.PrecompiledDefineId;
            //            precompiledCache.Remove(buildTargetGroup);
            //            precompiledCache.Add(buildTargetGroup, newPrecompiled);
            //        }
            //    }
            //}

            //// 先清空当前的所有预编译指令
            //foreach (YuU3dBuildTargetGroup yuTargetGroup in Enum.GetValues(typeof(YuU3dBuildTargetGroup)))
            //{
            //    var buildTargetGroup = yuTargetGroup.ToString().AsEnum<BuildTargetGroup>();
            //    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, null);
            //}

            //foreach (var pair in precompiledCache)
            //{
            //    PlayerSettings.SetScriptingDefineSymbolsForGroup(pair.Key, pair.Value);
            //}
        }

        #endregion

        #region 图片转换

        [BoxGroup("图片转换")] [FilePath] [LabelText("目标图片")]
        public string TargetImagePath;

        [BoxGroup("图片转换")] [LabelText("图片转换结果")] [TextArea(5, 10)]
        public string ImageToBase64StringResult;

        [BoxGroup("图片转换")]
        [Button("将图片转换成字符串")]
        private void SwitchImageToBase64String()
        {
        }

        [BoxGroup("图片转换")] [FilePath] public string AsyncLoadPath;

        [BoxGroup("图片转换")]
        [Button("异步加载测试")]
        private void ReadAsyncTest()
        {
            IOUtility.ReadFileAsync(AsyncLoadPath, bytes =>
            {
                //var instance = YuSerializeUtility.DeSerialize<YuU3dCommonSpanner>(bytes);
                //Debug.Log(instance.ImageToBase64StringResult);
            });
        }

        #endregion
    }
}