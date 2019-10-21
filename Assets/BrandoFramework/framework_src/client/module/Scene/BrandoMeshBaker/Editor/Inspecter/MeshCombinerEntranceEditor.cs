using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameWorld.Editor
{
    [CustomEditor(typeof(MeshCombinerEntrance))]
    [CanEditMultipleObjects]
    public class MeshCombinerEntranceEditor : UnityEditor.Editor
    {
        private SerializedObject meshBaker;
        private SerializedProperty combiner;
        private SerializedProperty outputOptions;
        private SerializedProperty textureBakeResults;
        private SerializedProperty useObjsToMeshFromTexBaker;
        private SerializedProperty objsToMesh;
        private SerializedProperty mesh;
        private SerializedProperty sortOrderAxis;
        private SerializedProperty settingsHolder;

        private MeshCombineSettingEditor meshBakerSettingsThis;
        private MeshCombineSettingEditor meshBakerSettingsExternal;

        bool showInstructions = false;
        bool showContainsReport = true;

        #region GUI 样式

        MB_EditorStyles editorStyles = new MB_EditorStyles();

        private static GUIContent gc_outputOptoinsGUIContent = new GUIContent("输出方式");
        private static GUIContent gc_openToolsWindowLabelContent = new GUIContent("打开添加添加游戏物体工具窗口",
            "Use these tools to find out what can be combined, discover problems with meshes, and quickly add objects.");
        private static GUIContent gc_objectsToCombineGUIContent = new GUIContent("自定义合并物体列表",
            "You can add objects here that were not on the list in the MB3_TextureBaker as long as they use a material that is in the Texture Bake Results");
        private static GUIContent gc_textureBakeResultsGUIContent = new GUIContent("已合并材质资源",
            "When materials are combined a MB2_TextureBakeResult Asset is generated. Drag that Asset to this field to use the combined material.");
        private static GUIContent gc_useTextureBakerObjsGUIContent = new GUIContent("与参与材质合并物体相同",
            "Build a combined mesh using using the same list of objects that generated the Combined Material");
        private static GUIContent gc_combinedMeshPrefabGUIContent = new GUIContent("合并网格预制体",
            "Create a new prefab asset an drag an empty game object to it. Drag the prefab asset to here.");
        private static GUIContent gc_SortAlongAxis = new GUIContent("SortAlongAxis",
            "Transparent materials often require that triangles be rendered in a certain order. " +
            "This will sort Game Objects along the specified axis. Triangles will be added to the combined mesh in this order.");
        private static GUIContent gc_Settings = new GUIContent("通用设置 ",
            "Different bakers can share the same settings. If this field is None, then the settings below will be used. " +
            "Assign one of the following:\n" +
            "   - Mesh Baker Settings project asset \n" +
            "   - Mesh Baker Grouper scene instance \n");

        Color buttonColor = new Color(1f, .4f, 0.1f, 1f);

        #endregion

        #region Unity 函数

        public void OnEnable()
        {
            Init(serializedObject);
        }

        public void OnDisable()
        {
            editorStyles.DestroyTextures();
            if (meshBakerSettingsThis != null)
                meshBakerSettingsThis.OnDisable();
            if (meshBakerSettingsExternal != null)
                meshBakerSettingsExternal.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            DrawGUI(serializedObject, (MeshBakerCommon)target, typeof(MeshCombineEditorWindow));
        }


        #endregion

        private void Init(SerializedObject mb)
        {
            meshBaker = mb;
            objsToMesh = meshBaker.FindProperty("objsToMesh");
            combiner = meshBaker.FindProperty("_meshCombiner");
            outputOptions = combiner.FindPropertyRelative("_outputOption");
            useObjsToMeshFromTexBaker = meshBaker.FindProperty("useObjsToMeshFromTexBaker");
            textureBakeResults = combiner.FindPropertyRelative("_textureBakeResults");
            mesh = combiner.FindPropertyRelative("_mesh");
            sortOrderAxis = meshBaker.FindProperty("sortAxis");
            settingsHolder = combiner.FindPropertyRelative("_settingsHolder");

            meshBakerSettingsThis = new MeshCombineSettingEditor();
            meshBakerSettingsThis.OnEnable(combiner, meshBaker);
            editorStyles.Init();
        }

        private void DrawGUI(SerializedObject meshBaker, MeshBakerCommon target, System.Type editorWindowType)
        {
            if (meshBaker == null)
            {
                return;
            }

            meshBaker.Update();

            //说明窗口
            showInstructions = EditorGUILayout.Foldout(showInstructions, "说明:");
            if (showInstructions)
            {
                EditorGUILayout.HelpBox("1. Bake combined material(s).\n\n" +
                                        "2. If necessary set the 'Texture Bake Results' field.\n\n" +
                                        "3. Add scene objects or prefabs to combine or check 'Same As Texture Baker'. For best results these should use the same shader as result material.\n\n" +
                                        "4. Select options and 'Bake'.\n\n" +
                                        "6. Look at warnings/errors in console. Decide if action needs to be taken.\n\n" +
                                        "7. (optional) Disable renderers in source objects.", UnityEditor.MessageType.None);

                EditorGUILayout.Separator();
            }

            MeshBakerCommon momm = (MeshBakerCommon)target;
            //已合并材质资源
            EditorGUILayout.PropertyField(textureBakeResults, gc_textureBakeResultsGUIContent);
            if (textureBakeResults.objectReferenceValue != null)
            {
                showContainsReport = EditorGUILayout.Foldout(showContainsReport, "Shaders & Materials Contained");
                if (showContainsReport)
                {
                    EditorGUILayout.HelpBox(((TextureBakeResults)textureBakeResults.objectReferenceValue).GetDescription(), MessageType.Info);
                }
            }

            EditorGUILayout.BeginVertical(editorStyles.editorBoxBackgroundStyle);
            //参与网格合并物体列表
            EditorGUILayout.LabelField("合并物体列表", EditorStyles.whiteBoldLabel);
            if (momm.GetTextureBaker() != null)
            {
                EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
            }
            else
            {
                useObjsToMeshFromTexBaker.boolValue = false;
                momm.useObjsToMeshFromTexBaker = false;
                GUI.enabled = false;
                EditorGUILayout.PropertyField(useObjsToMeshFromTexBaker, gc_useTextureBakerObjsGUIContent);
                GUI.enabled = true;
            }

            //与参与材质合并物体不同
            if (!momm.useObjsToMeshFromTexBaker)
            {
                //添加工具
                if (GUILayout.Button(gc_openToolsWindowLabelContent))
                {
                    IMeshCombinerEditorWindow mmWin = (IMeshCombinerEditorWindow)EditorWindow.GetWindow(editorWindowType);
                    mmWin.target = (MeshBakerRoot)target;
                }

                object[] objs = EditorMethods.DropZone("Drag & Drop Renderers Or Parents Here To Add Objects To Be Combined", 300, 50);
                EditorMethods.AddDroppedObjects(objs, momm);

                EditorGUILayout.PropertyField(objsToMesh, gc_objectsToCombineGUIContent, true);
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Objects In Scene"))
                {
                    Selection.objects = momm.GetObjectsToCombine().ToArray();
                    if (momm.GetObjectsToCombine().Count > 0)
                    {
                        SceneView.lastActiveSceneView.pivot = momm.GetObjectsToCombine()[0].transform.position;
                    }
                }
                if (GUILayout.Button(gc_SortAlongAxis))
                {
                    MeshBakerRoot.ZSortObjects sorter = new MeshBakerRoot.ZSortObjects();
                    sorter.sortAxis = sortOrderAxis.vector3Value;
                    sorter.SortByDistanceAlongAxis(momm.GetObjectsToCombine());
                }
                EditorGUILayout.PropertyField(sortOrderAxis, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(objsToMesh, gc_objectsToCombineGUIContent, true);
                GUI.enabled = true;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("输出：", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(outputOptions, gc_outputOptoinsGUIContent);
            //输出方式设置
            if (momm.meshCombiner.outputOption == OutputOptions.bakeIntoSceneObject)
            {
                momm.meshCombiner.resultSceneObject = (GameObject)EditorGUILayout.ObjectField("合并后游戏物体",
                    momm.meshCombiner.resultSceneObject, typeof(GameObject), true);
                if (momm is MeshCombinerEntrance)
                {
                    string l = "Mesh";
                    Mesh m = (Mesh)mesh.objectReferenceValue;
                    if (m != null)
                    {
                        l += " (" + m.GetInstanceID() + ")";
                    }
                    Mesh nm = (Mesh)EditorGUILayout.ObjectField(new GUIContent(l), m, typeof(Mesh), true);
                    if (nm != m)
                    {
                        Undo.RecordObject(momm, "Assign Mesh");
                        ((MeshCombineHandler)momm.meshCombiner).SetMesh(nm);
                        mesh.objectReferenceValue = nm;
                    }
                }
            }
            //合并为预制体
            else if (momm.meshCombiner.outputOption == OutputOptions.bakeIntoPrefab)
            {
                momm.resultPrefab = (GameObject)EditorGUILayout.ObjectField(
                    gc_combinedMeshPrefabGUIContent, momm.resultPrefab, typeof(GameObject), true);
                if (momm is MeshCombinerEntrance)
                {
                    string l = "Mesh";
                    Mesh m = (Mesh)mesh.objectReferenceValue;
                    if (m != null)
                    {
                        l += " (" + m.GetInstanceID() + ")";
                    }
                    Mesh nm = (Mesh)EditorGUILayout.ObjectField(new GUIContent(l), m, typeof(Mesh), true);
                    if (nm != m)
                    {
                        Undo.RecordObject(momm, "Assign Mesh");
                        ((MeshCombineHandler)momm.meshCombiner).SetMesh(nm);
                        mesh.objectReferenceValue = nm;
                    }
                }
            }
            //合并磁盘中文件
            else if (momm.meshCombiner.outputOption == OutputOptions.bakeMeshAssetsInPlace)
            {
                EditorGUILayout.HelpBox("Try the BatchPrefabBaker component. " +
                    "It makes preparing a batch of prefabs for static/ dynamic batching much easier.", MessageType.Info);
                if (GUILayout.Button("Choose Folder For Bake In Place Meshes"))
                {
                    string newFolder = EditorUtility.SaveFolderPanel("Folder For Bake In Place Meshes", Application.dataPath, "");
                    if (!newFolder.Contains(Application.dataPath)) Debug.LogWarning("The chosen folder must be in your assets folder.");
                    momm.bakeAssetsInPlaceFolderPath = "Assets" + newFolder.Replace(Application.dataPath, "");
                }
                EditorGUILayout.LabelField("Folder For Meshes: " + momm.bakeAssetsInPlaceFolderPath);
            }

            //if (momm is MB3_MultiMeshBaker)
            //{
            //    ////MB3_MultiMeshCombiner mmc = (MB3_MultiMeshCombiner)momm.meshCombiner;
            //    ////mmc.maxVertsInMesh = EditorGUILayout.IntField("Max Verts In Mesh", mmc.maxVertsInMesh);
            //}

            //-----------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("网格合并设置：", EditorStyles.boldLabel);
            bool settingsEnabled = true;

            //------------- Mesh Baker Settings is a bit tricky because it is an interface.

            UnityEngine.Object obj = settingsHolder.objectReferenceValue;

            //通用合并网格设置
            // Don't use a PropertyField because we may not be able to use the assigned object. 
            // It may not implement requried interface.
            obj = EditorGUILayout.ObjectField(gc_Settings, obj, typeof(UnityEngine.Object), true);
            //通用设置为空，使用下方设置
            if (obj == null)
            {
                settingsEnabled = true;
                settingsHolder.objectReferenceValue = null;
                if (meshBakerSettingsExternal != null)
                {
                    meshBakerSettingsExternal.OnDisable();
                    meshBakerSettingsExternal = null;
                }
            }
            else if (obj is GameObject)
            {
                // Check to see if there is a component on this game object that implements MB_IMeshBakerSettingsHolder
                IMeshCombinerSettingHolder itf = (IMeshCombinerSettingHolder)((GameObject)obj).GetComponent(typeof(IMeshCombinerSettingHolder));
                if (itf != null)
                {
                    settingsEnabled = false;
                    Component settingsHolderComponent = (Component)itf;
                    if (settingsHolder.objectReferenceValue != settingsHolderComponent)
                    {
                        settingsHolder.objectReferenceValue = settingsHolderComponent;
                        meshBakerSettingsExternal = new MeshCombineSettingEditor();
                        meshBakerSettingsExternal.OnEnable(itf.GetMeshBakerSettingsAsSerializedProperty());
                    }
                }
                else
                {
                    settingsEnabled = true;
                    settingsHolder = null;
                    if (meshBakerSettingsExternal != null)
                    {
                        meshBakerSettingsExternal.OnDisable();
                        meshBakerSettingsExternal = null;
                    }
                }
            }
            else if (obj is IMeshCombinerSettingHolder)
            {
                settingsEnabled = false;
                if (settingsHolder.objectReferenceValue != obj)
                {
                    settingsHolder.objectReferenceValue = obj;
                    meshBakerSettingsExternal = new MeshCombineSettingEditor();
                    meshBakerSettingsExternal.OnEnable(((IMeshCombinerSettingHolder)obj).GetMeshBakerSettingsAsSerializedProperty());
                }
            }
            else
            {
                Debug.LogError("Object was not a Mesh Baker Settings object.");
            }
            EditorGUILayout.Space();

            if (settingsHolder.objectReferenceValue == null)
            {
                // Use the meshCombiner settings
                meshBakerSettingsThis.DrawGUI(momm.meshCombiner, settingsEnabled);
            }
            else
            {
                if (meshBakerSettingsExternal == null)
                {
                    meshBakerSettingsExternal = new MeshCombineSettingEditor();
                    meshBakerSettingsExternal.OnEnable(((IMeshCombinerSettingHolder)obj).GetMeshBakerSettingsAsSerializedProperty());
                }
                var set = settingsHolder.objectReferenceValue;
                var settings = (IMeshCombinerSettingHolder)settingsHolder.objectReferenceValue;
                meshBakerSettingsExternal.DrawGUI(settings.GetMeshBakerSettings(), settingsEnabled);
            }

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUILayout.Button("合并"))
            {
                bake(momm, ref meshBaker);
            }
            GUI.backgroundColor = oldColor;

            string enableRenderersLabel;
            bool disableRendererInSource = false;
            if (momm.GetObjectsToCombine().Count > 0)
            {
                Renderer r = MeshBakerUtility.GetRenderer(momm.GetObjectsToCombine()[0]);
                if (r != null && r.enabled)
                    disableRendererInSource = true;
            }
            if (disableRendererInSource)
            {
                enableRenderersLabel = "隐藏源游戏物体";
            }
            else
            {
                enableRenderersLabel = "显示源游戏物体";
            }
            if (GUILayout.Button(enableRenderersLabel))
            {
                momm.EnableDisableSourceObjectRenderers(!disableRendererInSource);
            }

            meshBaker.ApplyModifiedProperties();
            meshBaker.SetIsDifferentCacheDirty();
        }

        public static bool bake(MeshBakerCommon mom)
        {
            SerializedObject so = null;
            return bake(mom, ref so);
        }

        /// <summary>
        /// Bakes a combined mesh.
        /// </summary>
        /// <param name="mom"></param>
        /// <param name="so">This is needed to work around a Unity bug where UnpackPrefabInstance corrupts 
        /// a SerializedObject. Only needed for bake into prefab.</param>
        public static bool bake(MeshBakerCommon mom, ref SerializedObject so)
        {
            bool createdDummyTextureBakeResults = false;
            bool success = false;
            try
            {
                if (mom.meshCombiner.outputOption == OutputOptions.bakeIntoSceneObject ||
                    mom.meshCombiner.outputOption == OutputOptions.bakeIntoPrefab)
                {
                    success = MeshCombinerEditorFunctions.BakeIntoCombined(mom, out createdDummyTextureBakeResults, ref so);
                }
                else
                {
                    //bake meshes in place
                    if (mom is MeshCombinerEntrance)
                    {
                        ValidationLevel vl = Application.isPlaying ? ValidationLevel.quick : ValidationLevel.robust;
                        if (!MeshBakerRoot.DoCombinedValidate(mom, ObjsToCombineTypes.prefabOnly, new EditorMethods(), vl))
                            return false;

                        List<GameObject> objsToMesh = mom.GetObjectsToCombine();
                        ////success = MB3_BakeInPlace.BakeMeshesInPlace((MeshCombineHandler)((MeshCombinerEntrance)mom).meshCombiner, objsToMesh, mom.bakeAssetsInPlaceFolderPath, mom.clearBuffersAfterBake, updateProgressBar);
                    }
                    else
                    {
                        //多网格合并无法 Bake In Place
                        Debug.LogError("Multi-mesh Baker components cannot be used for Bake In Place. Use an ordinary Mesh Baker object instead.");
                    }
                }
                mom.meshCombiner.CheckIntegrity();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (createdDummyTextureBakeResults && mom.textureBakeResults != null)
                {
                    MeshBakerUtility.Destroy(mom.textureBakeResults);
                    mom.textureBakeResults = null;
                }
                EditorUtility.ClearProgressBar();
            }
            return success;
        }

        public static void updateProgressBar(string msg, float progress)
        {
            EditorUtility.DisplayProgressBar("Combining Meshes", msg, progress);
        }

        #region GameObject 栏选项

        [MenuItem("GameObject/MeshCombine/TextureBaker and MeshBaker", false, 100)]
        public static GameObject CreateNewMeshBaker()
        {
            TextureCombineEntrance[] mbs = (TextureCombineEntrance[])GameObject.FindObjectsOfType(typeof(TextureCombineEntrance));
            Regex regex = new Regex(@"\((\d+)\)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            int largest = 0;
            try
            {
                for (int i = 0; i < mbs.Length; i++)
                {
                    Match match = regex.Match(mbs[i].name);
                    if (match.Success)
                    {
                        int val = Convert.ToInt32(match.Groups[1].Value);
                        if (val >= largest)
                            largest = val + 1;
                    }
                }
            }
            catch (Exception e)
            {
                if (e == null) e = null; //Do nothing supress compiler warning
            }
            GameObject nmb = new GameObject("TextureCombineEntrance (" + largest + ")");
            nmb.transform.position = Vector3.zero;
            TextureCombineEntrance tb = nmb.AddComponent<TextureCombineEntrance>();
            tb.packingAlgorithm = PackingAlgorithmEnum.MeshBakerTexturePacker;
            ////MB3_MeshBakerGrouper mbg = nmb.AddComponent<MB3_MeshBakerGrouper>();
            GameObject meshBaker = new GameObject("MeshBaker");
            MeshCombinerEntrance mb = meshBaker.AddComponent<MeshCombinerEntrance>();
            meshBaker.transform.parent = nmb.transform;
            ////mb.meshCombiner.settingsHolder = mbg;
            return nmb.gameObject;
        }

        [MenuItem("GameObject/MeshCombine/MeshBaker", false, 100)]
        public static GameObject CreateNewMeshBakerOnly()
        {
            MeshCombinerEntrance[] mbs = (MeshCombinerEntrance[])GameObject.FindObjectsOfType(typeof(MeshCombinerEntrance));
            Regex regex = new Regex(@"\((\d+)\)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
            int largest = 0;
            try
            {
                for (int i = 0; i < mbs.Length; i++)
                {
                    Match match = regex.Match(mbs[i].name);
                    if (match.Success)
                    {
                        int val = Convert.ToInt32(match.Groups[1].Value);
                        if (val >= largest)
                            largest = val + 1;
                    }
                }
            }
            catch (Exception e)
            {
                if (e == null) e = null; //Do nothing supress compiler warning
            }
            GameObject meshBaker = new GameObject("MeshBaker (" + largest + ")");
            meshBaker.AddComponent<MeshCombinerEntrance>();
            return meshBaker.gameObject;
        }

        #endregion
    }
}
