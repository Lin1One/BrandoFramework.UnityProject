using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;
    [CustomEditor(typeof(IsoLight))]
    [DisallowMultipleComponent]
    public class IsoLightEditor : Editor
    {
        const string helpMSG = "First, you must select the Iso2D target that will be affected by IsoLight.\nUse the tools below to easily register and edit Targets!";
        bool bPrefab = false;
        bool bIncludeChild = true;
        bool bLightListFoldout = false;
        bool bTemporaryToggle_Static = false;
        bool bTemporaryToggle_Dynamic = false;

        Vector2 scrollPosition;
        Vector2 scrollPosition2;

        IsoLight light;
        List<IsoLight> lightList = new List<IsoLight>();
        List<IsoLight> MutelightList = new List<IsoLight>();

        LayerMask layerMask = 0;
        GameObject lookupObject;
        bool bFoldoutSelectorList = true;
        List<GameObjectSelector> gameObjectSelectors = new List<GameObjectSelector>();
        SerializedProperty targetList;

        void OnEnable()
        {
            if ((light = (IsoLight)target) == null)
                return;
            if (bPrefab = PrefabHelper.IsPrefab(light.gameObject))
                return;
            
            targetList = serializedObject.FindProperty("targetList");

            MutelightList.Clear();
            lightList.Clear();
            lightList.AddRange(FindObjectsOfType<IsoLight>());

            bTemporaryToggle_Static = bTemporaryToggle_Dynamic = false;
            Selector_Refresh();
        }

        private void OnDisable()
        {
            unmuteSolo(true);
            unmuteSolo(false);
        }

        public override void OnInspectorGUI()
        {
            if (bPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }
            base.DrawDefaultInspector();

            SetUniquePriority();
            Util();            
        }

        public void SetTarget()
        {            
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[Target Select Helper]", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                CustomEditorGUI.FitLabel(new GUIContent("Add or Remove"));
                lookupObject = EditorGUILayout.ObjectField(lookupObject, typeof(GameObject), allowSceneObjects: true) as GameObject;
            }
            if (lookupObject != null && PrefabHelper.IsPrefab(lookupObject))
            {
                lookupObject = null;
                Debug.Log("Prefab is not allowed. Please select only the GameObject in the scene.");
            }
            if (lookupObject)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    bIncludeChild = EditorGUILayout.ToggleLeft("Include Child", bIncludeChild);
                    if (GUILayout.Button("Create"))
                    {
                        light.AddTarget(lookupObject, bIncludeChild);
                    }
                    if (GUILayout.Button("Remove"))
                    {
                        light.RemoveTarget(lookupObject, bIncludeChild);
                    }
                }
            }
            CustomEditorGUI.DrawSeperator();

            bool bSeletorExist = gameObjectSelectors.Count > 0;
            using (new EditorGUILayout.HorizontalScope())
            {
                CustomEditorGUI.FitLabel(new GUIContent("Target Selector"));
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightGreen, "Create", Selector_Create);
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightBlue, "Refresh", Selector_Refresh);                
                CustomEditorGUI.Button(bSeletorExist, CustomEditorGUI.Color_LightRed, "Destroy All", Selector_Delete);
            }

            EditorGUI.indentLevel++;
            using (new EditorGUILayout.HorizontalScope())
            {
                bFoldoutSelectorList = EditorGUILayout.Foldout(bFoldoutSelectorList, string.Format("Selectors({0})", gameObjectSelectors.Count));
                GUILayout.FlexibleSpace();
                CustomEditorGUI.Button(bSeletorExist, CustomEditorGUI.Color_LightYellow, "[Add]", Selector_Add);
                CustomEditorGUI.FitLabel(new GUIContent(" or "));
                CustomEditorGUI.Button(bSeletorExist, CustomEditorGUI.Color_LightMagenta, "[Remove] ", Selector_Remove);
                CustomEditorGUI.FitLabel(new GUIContent(" all Iso2DBases selected by Selector"));
            }

            if (bSeletorExist)
            {
                if (bFoldoutSelectorList)
                    gameObjectSelectors.ForEach((a) => EditorGUILayout.ObjectField(a, typeof(GameObjectSelector), allowSceneObjects: true));
            }
            EditorGUI.indentLevel--;

            CustomEditorGUI.DrawSeperator();
            EditorGUILayout.Separator();

            layerMask = CustomEditorGUI.LayerMaskField("Layer Mask for [Add]/[Remove]", layerMask);
            using (new EditorGUILayout.HorizontalScope())
            {
                bool bLayerMaskReady = layerMask != 0;
                CustomEditorGUI.Button(bLayerMaskReady, CustomEditorGUI.Color_LightYellow, "[Add] all Masked Iso2DBase", () => light.AddTarget_All(layerMask));
                CustomEditorGUI.Button(bLayerMaskReady, CustomEditorGUI.Color_LightMagenta, "[Remove] all Masked layer's", () => light.RemoveTarget_All(layerMask));
            }

            EditorGUILayout.Separator();
            CustomEditorGUI.DrawSeperator();
            EditorGUILayout.Separator();

            using (new EditorGUILayout.HorizontalScope())
            {
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightYellow, "[Add] all Iso2DBase", () => light.AddTarget_All(-1));
                CustomEditorGUI.Button(light.TargetList.Count > 0, CustomEditorGUI.Color_LightMagenta, "[Clear] All TargetList", () => light.RemoveTarget_All());
            }
        }
        public void Util()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(helpMSG, MessageType.Info);
            SetTarget();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[Temporary Help Features]", EditorStyles.boldLabel);
            SoloToggle();
            updateReciver();
            InstanceList();
        }
        public void InstanceList()
        {
            bLightListFoldout = CustomEditorGUI.ObjectListField(lightList, typeof(IsoLightReciver), "IsoListList", bLightListFoldout, true, ref scrollPosition2);
            targetList.isExpanded = CustomEditorGUI.ObjectListField(light.TargetList, typeof(IsoLightReciver), targetList.name, targetList.isExpanded, true, ref scrollPosition);
        }

        void SetUniquePriority()
        {
            int iStartWith = light.UniquePriority;
            bool bStaticLight = light.bStaticLight;
            var enumerator = lightList.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current != null && current != light &&
                    current.bStaticLight == bStaticLight &&
                    iStartWith == current.UniquePriority)
                {
                    Debug.Log(string.Format("[Preoccupied] \"UniquePriority({1})\" by \"{2}\"\n", light.name, iStartWith, current.name));
                    enumerator = lightList.GetEnumerator();
                    iStartWith++;
                }
            }
            if (light.UniquePriority != iStartWith)
            {
                light.UniquePriority = iStartWith;
                Debug.Log(string.Format("[Set] \"{0}.UniquePriority\" to {1}", light.name, iStartWith));
            }
        }

        void SoloToggle()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                bTemporaryToggle_Static = soloToggle(light.bStaticLight ?  "Solo(Static)" :  "Mute(Static)", true, bTemporaryToggle_Static);
                bTemporaryToggle_Dynamic = soloToggle(light.bStaticLight ? "Mute(Dynamic)" : "Solo(Dynamic)", false, bTemporaryToggle_Dynamic);
            }
        }
        bool soloToggle(string name, bool bStatic, bool bSolo, params GUILayoutOption[] options)
        {
            EditorGUI.BeginChangeCheck();
            bSolo = EditorGUILayout.ToggleLeft(name, bSolo, options);
            if (EditorGUI.EndChangeCheck())
            {
                if (!bSolo)
                {
                    unmuteSolo(bStatic);
                }
                else
                {
                    var muteList = lightList.Where(r => r.bStaticLight == bStatic && r.TurnOnOff && r != light);
                    foreach (var _light in muteList)
                    {
                        _light.TurnOnOff = false;
                        MutelightList.Add(_light);
                    }
                }
            }
            return bSolo;
        }
        void unmuteSolo(bool bStatic)
        {
            var muteList = MutelightList.Where(r => r.bStaticLight == bStatic);
            foreach (var _light in muteList)
            {
                if (_light)
                {
                    _light.TurnOnOff = true;
                }
            }
            MutelightList.RemoveAll(r => muteList.Contains(r));
        }
        void updateReciver()
        {
            if (GUILayout.Button("Update Target Color"))
                light.UpdateAllReciver();
        }

        void Selector_Create()
        {
            if (IsoMap.GameObject_Selector != null)
            {
                var go = GameObject.Instantiate(IsoMap.GameObject_Selector, light.transform);

                if (go != null)
                    gameObjectSelectors.Add(go.GetComponent<GameObjectSelector>());
            }
        }

        void Selector_Refresh()
        {
            gameObjectSelectors.Clear();
            gameObjectSelectors.AddRange(light.transform.GetComponentsInChildren<GameObjectSelector>());
        }

        void Selector_Delete()
        {
            gameObjectSelectors.ForEach((e) => { if (e != null) GameObject.DestroyImmediate(e.gameObject, true); });
            gameObjectSelectors.Clear();
        }

        void Selector_Add()
        {
            gameObjectSelectors.ForEach((s) => s.Selected.ForEach(e => light.AddTarget(e, true)));
        }

        void Selector_Remove()
        {
            gameObjectSelectors.ForEach((s) => s.Selected.ForEach(e => light.RemoveTarget(e, true)));

        }
    }
}
