#region Head

// Author:            Yu
// CreateDate:        2018/8/16 19:50:05
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.LegoUI;
using Common;
using Common.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.LegoUI.Editor
{
    public class YuLegoComponentHierarchyItemChangeHandler : AbsHierarchyItemChangeHandler
    {
        #region 层次面板扩展接口

        public override bool SpecialCheck(GameObject go)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (go.name.StartsWith("LegoComponent")
                && go.GetComponent<YuLegoComponentHelper>() != null
                || go.name.StartsWith("LegoView") && go.GetComponent<YuLegoViewHelper>() != null)
            {
                return true;
            }

            return false;
        }

        public override void MakeFixedMenu()
        {
            EditorUtility.DisplayPopupMenu(MenuRect, "GameObject/Yu/LegoUI/MetaInfo/Component", null);
            CurrentEvent.Use();
        }

        public override void MakeDynamicMenu(GenericMenu menu)
        {
            if (SelectGo == null)
            {
                return;
            }

            var metaIds = YuLegoEditorUtility.GetLatestMetaIds();

            if (metaIds == null)
            {
                return;
            }

            var viewIds = metaIds.Where(i => i.StartsWith(YuLegoEditorUtility.LEGO_VIEW)).ToList();
            YuLegoEditorUtility.AddUIMenus(menu, viewIds, 9, YuLegoEditorUtility.LEGO_VIEW, BuildLegoUI);

            var componentIdws = metaIds.Where(i => i.StartsWith(YuLegoEditorUtility.LEGO_COMPONENT)).ToList();
            YuLegoEditorUtility.AddUIMenus(menu, componentIdws, 14, YuLegoEditorUtility.LEGO_COMPONENT, BuildLegoUI);
        }

        #endregion

        #region 注册编辑器帧循环

        public YuLegoComponentHierarchyItemChangeHandler()
        {
            EditorApplication.update += Update;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (builder != null)
            {
                builder.Update();
            }
        }

        #endregion

        private void BuildLegoUI(object data)
        {
            var selectRect = SelectGo.GetComponent<RectTransform>();
            if (selectRect == null)
            {
                EditorUtility.DisplayDialog(
                    "错误",
                    "乐高UI组件必须挂载在RectTransform组件下!",
                    "OK"
                );

                return;
            }

            var uiId = (string) data;
            if (uiId.StartsWith("LegoView"))
            {
                if (SelectGo.GetComponentInParent<YuLegoViewHelper>() != null)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "乐高视图不允许嵌套挂载!",
                        "OK"
                    );

                    return;
                }
            }

            if (selectRect.name == uiId)
            {
                if (SelectGo.GetComponentInParent<YuLegoViewHelper>() != null)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "挂载父物体的名子和组件名不能相同!",
                        "OK"
                    );

                    return;
                }
            }

            var lowerId = uiId.ToLower();

            if (!Application.isPlaying)
            {
                var tempBuilders = Resources.FindObjectsOfTypeAll<LegoBuilder>();
                if (tempBuilders != null && tempBuilders.Length > 0)
                {
                    foreach (var b in tempBuilders)
                    {
                        Object.DestroyImmediate(b.gameObject);
                    }
                }

                var tempGo = new GameObject("TempBuilder");
                builder = tempGo.AddComponent<LegoBuilder>();
                var task = builder.CreateTask(lowerId, OnUiBuilded, selectRect, 1000, AllComplete);
                builder.PushSonTask(task);
                builder.StartBuild();
            }
            else
            {
                if (builder == null)
                {
                    builder = Injector.Instance.Get<LegoBuilder>();
                }

                var task = builder.CreateTask(lowerId, OnUiBuilded, selectRect, 1000);
                builder.PushSonTask(task);
                builder.StartBuild();
            }
        }

        private LegoBuilder builder;

        private void AllComplete()
        {
            Selection.activeGameObject = builder.RootRect.gameObject;
            Object.DestroyImmediate(builder.gameObject);
            builder = null;
        }

        private void OnUiBuilded(LegoBuildTask buildTask)
        {
            var uiRect = buildTask.RootRect;
            var parentRect = buildTask.ParentRect;
            var cRectMeta = buildTask.ComponentMountMeta;

            uiRect.SetParent(parentRect);

            if (cRectMeta == null)
            {
                uiRect.localPosition = Vector3.zero;
                uiRect.localScale = Vector3.one;
            }
            else
            {
                uiRect.name = cRectMeta.Name;

                uiRect.localPosition = new Vector3(
                    cRectMeta.X,
                    cRectMeta.Y,
                    cRectMeta.Z
                );

                uiRect.localScale = new Vector3(
                    cRectMeta.ScaleX,
                    cRectMeta.ScaleY,
                    cRectMeta.ScaleZ
                );
            }
        }

        private static class YuLegoUIMenu
        {
            private static GameObject CurrentGo => Selection.activeGameObject;

            #region 构建乐高元数据

            [MenuItem("GameObject/Yu/LegoUI/单一构建元数据")]
            private static void SingleBuildMeta()
            {
                var selectGo = Selection.activeGameObject;
                var builder = new YuLegoMetaBuilder(true, false);
                builder.BuildMeta(selectGo);
                AssetDatabase.Refresh();
            }

            [MenuItem("GameObject/Yu/LegoUI/单一构建元数据及脚本")]
            private static void SingleBuildMetaAndScript()
            {
                var selectGo = Selection.activeGameObject;
                var builder = new YuLegoMetaBuilder(true, true);
                builder.BuildMeta(selectGo);
                AssetDatabase.Refresh();
            }

            [MenuItem("GameObject/Yu/LegoUI/递归构建元数据")]
            private static void LoopBuildMeta()
            {
                var gos = Selection.gameObjects;
                foreach (var go in gos)
                {
                    var builder = new YuLegoMetaBuilder(false, false);
                    builder.BuildMeta(go);
                }

                AssetDatabase.Refresh();
            }

            [MenuItem("GameObject/Yu/LegoUI/递归构建元数据及脚本")]
            private static void LoopBuildMetaAndScript()
            {
                var gos = Selection.gameObjects;
                foreach (var go in gos)
                {
                    var builder = new YuLegoMetaBuilder(false, true);
                    builder.BuildMeta(go);
                }

                AssetDatabase.Refresh();
            }

            #endregion

            #region 创建脚本

            //[MenuItem("Yu/LegoUI/创建及刷新UI静态路由器")]
            private static void CreateRegisterUIScript()
            {
                ////var app = YuU3dAppSettingDati.CurrentActual;
                ////var creator = new YuLegoUIRegisterRouterScriptCreator(app);
                ////creator.CreateScript();
                AssetDatabase.Refresh();
                //Debug.Log($"应用{app.LocAppId}的UI路由器已刷新！");
            }

            #endregion

            #region 乐高UI生命周期脚本

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/OnCreated")]
            private static void CreateOnCreated()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.OnCreated);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/BeforeShow")]
            private static void CreateBeforeShow()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.BeforeShow);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/AfterShow")]
            private static void CreateAfterShow()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.AfterShow);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/BeforeHide")]
            private static void CreateBeforeHide()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.BeforeHide);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/AfterHide")]
            private static void CreateAfterHide()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.AfterHide);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/BeforeClose")]
            private static void CreateBeforeClose()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.BeforeClose);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建生命周期处理器/AfterClose")]
            private static void CreateAfterClose()
            {
                CreateLegoUIPipelineHandler(UIPipelineType.AfterClose);
            }

            private static void CreateLegoUIPipelineHandler(UIPipelineType pipelineType)
            {
                if (CurrentGo.GetComponent<YuLegoViewHelper>() == null &&
                    CurrentGo.GetComponent<YuLegoComponentHelper>() == null)
                {
                    EditorUtility.DisplayDialog(
                        "错误",
                        "必须是一个界面或者组件!",
                        "OK"
                    );

                    return;
                }

                YuLegoPipelineHandlerScriptCreator.CreateScript(
                    CurrentGo.GetComponent<RectTransform>(), pipelineType);
                AssetDatabase.Refresh();
            }

            #endregion

            #region 滚动视图生命周期脚本

            [MenuItem("GameObject/Yu/LegoUI/创建ScrollView生命周期处理器/OnCreated")]
            private static void CreateScrollViewOnCreatedHandler()
            {
                CreateLegoScrollViewPipelineHandler(LegoScrollViewPipelineType.OnCreated);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建ScrollView生命周期处理器/OnReorganize")]
            private static void CreateScrollViewOnReorganizeHandler()
            {
                CreateLegoScrollViewPipelineHandler(LegoScrollViewPipelineType.OnReorganize);
            }

            [MenuItem("GameObject/Yu/LegoUI/创建ScrollView生命周期处理器/AfterClose")]
            private static void CreateScrollViewAfterCloseHandler()
            {
                CreateLegoScrollViewPipelineHandler(LegoScrollViewPipelineType.AfterClose);
            }

            private static void CreateLegoScrollViewPipelineHandler(LegoScrollViewPipelineType pipelineType)
            {
                ////if (!CurrentGo.GetComponent<YuLegoScrollViewHelper>())
                ////{
                ////    EditorUtility.DisplayDialog(
                ////        "错误",
                ////        "所选物体必须是一个 ScrollView !",
                ////        "OK"
                ////    );

                ////    return;
                ////}

               /// YuLegoScrollViewPipelineHandlerScriptCreator.CreateScript(YuU3dAppSettingDati.CurrentActual
                   /// , CurrentGo.GetComponent<RectTransform>(), pipelineType);
                AssetDatabase.Refresh();
            }

            #endregion

            #region 具象交互类型事件处理器

            private static readonly LegoMetaHelper metaHelper
                = Injector.Instance.Get<LegoMetaHelper>();

            [MenuItem("GameObject/Yu/LegoUI/创建交互事件处理器/OnDrag")]
            private static void CreateDrag()
                => CreateSpeiclHandlerScript(LegoInteractableType.OnDrag);

            private static void CreateSpeiclHandlerScript(LegoInteractableType interactableType)
            {
                var uiMeta = metaHelper.GetMeta(CurrentGo.transform.parent.name);
                ////var app = YuU3dAppSettingDati.CurrentActual;
                ////var fullName = app.LocAppId + "_" + uiMeta.RootMeta.Name + "_" + CurrentGo.name + "_" + interactableType;

                ////YuLegoHandlerScriptCreator.CreateHandlerScript(
                ////    uiMeta, app, fullName, CurrentGo.name);
                ////AssetDatabase.Refresh();
            }

            #endregion
        }
    }
}