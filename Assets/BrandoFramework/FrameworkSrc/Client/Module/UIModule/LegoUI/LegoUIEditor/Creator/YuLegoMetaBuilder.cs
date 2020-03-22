#if UNITY_EDITOR

#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Client.Extend;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Client.LegoUI.Editor
{
    public class YuLegoMetaBuilder
    {
        #region 字段

        private readonly GameProjectHelper appHelper;
        private readonly LegoUIMeta uiMeta = new LegoUIMeta();
        private readonly HashSet<string> componetIds = new HashSet<string>();
        private RectTransform rootRect;

        /// <summary>
        /// 是否只构建选择的UI对象的元数据。
        /// </summary>
        private readonly bool isSingleBuild;

        private static bool IsCreateScript;

        #endregion

        #region 静态初始化

        private static readonly List<string> complexControlIds
            = new List<string>
            {
                "Button",
                "Toggle",
                "PlaneToggle",
                "Slider",
                "Progressbar",
                "InputField",
                "Dropdown",
                "ListView",
                "ScrollView"
            };

        #endregion

        #region 实例构造

        public YuLegoMetaBuilder(bool isSingleBuild, bool isCreateScript)
        {
            this.isSingleBuild = isSingleBuild;
            IsCreateScript = isCreateScript;
            //appHelper = YuU3dAppSettingDati.CurrentActual.Helper;
        }

        #endregion

        #region 静态API

        private const string LEGO_VIEW = "LegoView";
        private const string LEGO_COMPONENT = "LegoComponent";
        private const string LEGO_CONTAINER = "LegoContainer";

        private static bool IsLegoControlSet(Transform tram)
        {
            return IsLegoView(tram) || IsLegoComponent(tram) || IsLegoContainer(tram);
        }

        private static bool IsLegoContainer(Transform tram) => tram.name.StartsWith(LEGO_CONTAINER);

        private static bool IsLegoView(Transform tram)
        {
            if (tram.name.StartsWith(LEGO_VIEW))
            {
                if (tram.GetComponent<YuLegoViewHelper>() == null)
                {
                    tram.AddComponent<YuLegoViewHelper>();
                    return true;
                }

                return true;
            }

            return false;
        }

        private static bool IsLegoComponent(Transform tram)
        {
            if (tram.name.StartsWith(LEGO_COMPONENT))
            {
                if (tram.GetComponent<YuLegoComponentHelper>() == null)
                {
                    tram.AddComponent<YuLegoComponentHelper>();
                    return true;
                }

                return true;
            }

            return false;
        }

        private static bool IsComplex(Transform transform)
        {
            var type = transform.name.Split('_').First();
            var result = complexControlIds.Find(n => n == type);
            return result != null;
        }

        #endregion

        #region 实例API

        public void BuildMeta(GameObject selectGo)
        {
            ////if (string.IsNullOrEmpty(YuU3dAppSettingDati.CurrentActual.CurrentDeveloper.Name))
            ////{
            ////    EditorUtility.DisplayDialog(
            ////        "错误",
            ////        $"应用{YuU3dAppSettingDati.CurrentActual.LocAppId}没有填写开发者配置！",
            ////        "OK"
            ////    );

            ////    return;
            ////}

            if (!IsLegoControlSet(selectGo.transform))
            {
                EditorUtility.DisplayDialog(
                    "错误",
                    "目标游戏对象必须是一个乐高视图、组件、容器",
                    "OK"
                );

                return;
            }

            rootRect = selectGo.GetComponent<RectTransform>();
            componetIds.Add(selectGo.name);
            InitUIMeta();
            ScanControl(rootRect);
            CreateMetaAsset();
            CreateScript();
        }

        private void CreateScript()
        {
            if (IsCreateScript && !IsLegoContainer(rootRect))
            {
                ////var currentApp = YuU3dAppSettingDati.CurrentActual;
                ////YuLegoLogicerScriptCreator.CreateLogicerScrpt(uiMeta, currentApp);
                ////YuLegoRxModelScriptCreator.CreateScript(uiMeta, currentApp, rootRect);
                ////YuLegoUIConstScriptCreator.CreateScript(uiMeta, currentApp);
                ////YuLegoContainerScriptCreator.CreateScript(uiMeta, currentApp);
                ////YuLegoCommonLogicScriptCreator.CreateScript(uiMeta, currentApp);
            }
        }

        private void InitUIMeta()
        {
            uiMeta.Init();
            //uiMeta.RootMeta = YuLegoRectTransformMeta.Create(rootRect.RectTransform());
            var componentHelper = rootRect.GetComponent<YuLegoComponentHelper>();
            if (componentHelper != null)
            {
                uiMeta.RootMeta.Name = uiMeta.RootMeta.Name.Split('@')[0];
                uiMeta.LogicId = componentHelper.LogicId;
                uiMeta.PaddingLeft = componentHelper.PaddingLeft;
                uiMeta.PaddingLastY = componentHelper.PaddingLastY;
            }

            var viewHelper = rootRect.GetComponent<YuLegoViewHelper>();
            if (viewHelper != null)
            {
                uiMeta.HideTargets = viewHelper.HideTargets;
            }
        }

        private void CreateMetaAsset()
        {
            ////var metaInfoGo = YuLegoUIMetaAsset.GetAsset(uiMeta);
            ////var jsContent = JsonUtility.ToJson(uiMeta);
            ////var formatedJson = YuEditorUtility.PrettifyJsonString(jsContent);
            ////var jsonPath = GetFullJsonPath(metaInfoGo.name);
            ////YuIOUtility.WriteAllText(jsonPath, formatedJson);
            ////var jsObj = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);
            ////EditorGUIUtility.PingObject(jsObj);
            ////YuU3dAppUtility.Injector.Get<YuLegoMetaHelper>().ReloadAllMeta();
            ////Debug.Log($"元数据{metaInfoGo.name}已经创建成功！");
        }

        private string GetFullJsonPath(string goName)
        {
            ////if (goName.StartsWith(LEGO_VIEW))
            ////{
            ////    var path = appHelper.AssetDatabaseLegoMetaDir + $"LegoView/{goName}.txt";
            ////    return path;
            ////}

            ////if (goName.StartsWith(LEGO_COMPONENT))
            ////{
            ////    var path = appHelper.AssetDatabaseLegoMetaDir + $"LegoComponent/{goName}.txt";
            ////    return path;
            ////}

            ////if (goName.StartsWith(LEGO_CONTAINER))
            ////{
            ////    var path = appHelper.AssetDatabaseLegoMetaDir + $"LegoContainer/{goName}.txt";
            ////    return path;
            ////}

            throw new Exception($"发现命名不合法的UI游戏对象{goName}！");
        }

        private void AddOperateMeta(LegoUIType elementType,
            RectTransform rectTransform)
        {
            var rectMeta = LegoRectTransformMeta.Create(rectTransform);
            uiMeta.AddOperateMeta(elementType, rectMeta, rectTransform);
        }

        private void AddOperateMeta(LegoUIType elementType,
            Transform transform)
        {
            AddOperateMeta(elementType, transform.RectTransform());
        }

        private void ScanControl(Transform control)
        {
            UpdateViewSetting(control);
            var index = 0;
            var childCount = control.childCount;

            while (index < childCount)
            {
                var child = control.GetChild(index);

                if (child.name.StartsWith("LegoContainer"))
                {
                    AddOperateMeta(LegoUIType.Container, child);
                    var builder = new YuLegoMetaBuilder(true, IsCreateScript);
                    builder.BuildMeta(child.gameObject);
                    index++;
                    continue;
                }

                if (IsLegoComponent(child))
                {
                    if (!isSingleBuild)
                    {
                        AddOperateMeta(LegoUIType.Component, child);
                        var builder = new YuLegoMetaBuilder(isSingleBuild, IsCreateScript);
                        builder.BuildMeta(child.gameObject);
                        index++;
                        continue;
                    }

                    AddOperateMeta(LegoUIType.Component, child);
                    index++;
                    continue;
                }

                if (IsComplex(child))
                {
                    if (child.name.StartsWith("ScrollView"))
                    {
                        var content = child.Find("ScrollRect/Content");
                        if (content.childCount == 0)
                        {
                            Debug.LogError($"滚动视图{child.name}下没有组件！");
                        }
                        else
                        {
                            var component = content.GetChild(0);
                            BuildScrollViewComponentMeta(component);
                        }
                    }

                    AddOperateMeta(GetViewElementType(child), child);
                    index++;
                    continue;
                }

                AddOperateMeta(GetViewElementType(child), child);
                index++;
            }
        }

        private void BuildScrollViewComponentMeta(Transform control)
        {
            var builder = new YuLegoMetaBuilder(true, true);
            builder.BuildMeta(control.gameObject);
        }

        private void UpdateViewSetting(Transform control)
        {
            if (!control.name.StartsWith(LEGO_VIEW))
            {
                return;
            }

            var helper = control.GetComponent<YuLegoViewHelper>();
            uiMeta.ViewType = helper.ViewType;
            uiMeta.HideTargets = helper.HideTargets;
            uiMeta.IsBlankClose = helper.IsBlankClose;
        }

        private static LegoUIType GetViewElementType(Transform control)
        {
            return control.name.StartsWith(LEGO_COMPONENT)
                ? LegoUIType.Component
                : control.name.Split('_').First().AsEnum<LegoUIType>();
        }

        #endregion
    }
}

#endif