using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    public class IsoHierarchyWindow : EditorWindow
    {
        bool bFoldoutMSG = false;
        const string HelpMSG_00 = "What tools for?";
        const string HelpMSG_01 = "This is for objects made up of Transform and SpriteRenderer without Iso component.\n" +
            "Helps to align the transform hierarchy of the target to Isometric Angle.";
        const string HelpMSG_02 = "Steps";
        const string HelpMSG_03 = "Firstly, Select Target GameObject on Scene.\n" +
            "Then a hierarchical view of Transform and SpriteRenderer is drawn.\n" +
            "The Depth slider makes it easy to set the front and back position.\n" +
            "You can also easily select the hierarchy object.";

        GameObject lookUpSource;
        public IsoHierarchy_TnS isoHierarchy = new IsoHierarchy_TnS();

        bool bActivateTransformNode = true;
        Vector2 vScrollPos = Vector2.zero;

        [MenuItem("Window/Anonym/Isometric Hierarchy Helper")]
        public static void CreateWindow()
        {
            EditorWindow window = EditorWindow.CreateInstance<IsoHierarchyWindow>();
            window.titleContent.text = "IsoHierarchy";
            window.Show();
        }

        private void OnEnable()
        {
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            showHelpMSG();

            CustomEditorGUI.DrawSeperator();
            GUILayout.Label(new GUIContent("Target Object", "Root of Object"), EditorStyles.boldLabel);
            UpdateSelectedObject();

            if (lookUpSource != null)
            {
                UtilBtns();

                CustomEditorGUI.DrawSeperator();
                CustomEditorGUI.AttachmentListDraw(this, isoHierarchy, ref vScrollPos);
            }
        }

        void UpdateSelectedObject()
        {
            EditorGUI.BeginChangeCheck();
            lookUpSource = EditorGUILayout.ObjectField("", lookUpSource, typeof(GameObject), allowSceneObjects: true) as GameObject;
            bActivateTransformNode = EditorGUILayout.ToggleLeft("Allow Transform Only", bActivateTransformNode);
            bool bChanged = EditorGUI.EndChangeCheck();

            if (bChanged)
            {
                isoHierarchy.Init(lookUpSource);
                if (!bActivateTransformNode)
                {
                    isoHierarchy.childList.RemoveAll(r => r.AttachedObj._Sprr == null);
                }
            }
        }

        void showHelpMSG()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUILayout.Label("Not available in Play mode!", EditorStyles.boldLabel);
                return;
            }

            CustomEditorGUI.DrawSeperator();
            if (bFoldoutMSG = EditorGUILayout.Foldout(bFoldoutMSG, "[Help MSG]"))
            {
                GUILayout.Label(HelpMSG_00, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(HelpMSG_01, MessageType.Info);
                GUILayout.Label(HelpMSG_02, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(HelpMSG_03, MessageType.Info);
            }
        }

        void UtilBtns()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUIBackgroundColorScope(CustomEditorGUI.Color_LightBlue))
                {
                    if (GUILayout.Button(new GUIContent("Isometric Rotate", "Rotate the Root GameObject to conform to the Isometric Angle.")))
                    {
                        Undo.RecordObject(lookUpSource.transform, "IsoHierarchyWindow: IsoRotate");
                        lookUpSource.transform.eulerAngles = (Vector3)IsoMap.Angle;
                    }
                }

                using (new GUIBackgroundColorScope(CustomEditorGUI.Color_LightBlue))
                {
                    if (GUILayout.Button("Close Up"))
                    {
                        Bounds bound = new Bounds();
                        foreach(var b in lookUpSource.GetComponentsInChildren<Collider>())
                        {
                            if (b is BoxCollider)
                                bound.Encapsulate((b as BoxCollider).GetStatelessBounds());
                            else
                                bound.Encapsulate(b.bounds);
                        }
                        foreach (var s in lookUpSource.GetComponentsInChildren<SpriteRenderer>())
                        {
                            bound.Encapsulate(s.bounds);
                        }
                        bound.size *= 0.25f;

                        if (IsoMap.IsNull)
                            IsoMap.Update_SceneViewCam(bound.center);
                        else
                            IsoMap.instance.Update_TileAngle();

#if UNITY_20
                        SceneView.lastActiveSceneView.Frame(bound, true);
#else
                        SceneView.lastActiveSceneView.FrameSelected(true);
#endif
                    }
                }
            }
        }
    }
}