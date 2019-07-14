using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    [CustomEditor(typeof(ISOBasis))]
    public class ISOBasisEditor : Editor
    {
        bool bPrefab = true;
        bool bEditOnGroundOffset = false;
        SerializedProperty _spDoNotDestroyAutomatically;
        SerializedProperty _spISOOffest;
        SerializedProperty _spOnGroundObject;
        SerializedProperty _spTransformsForFudge;
        ISOBasis _targetIsoBasis;
        IISOBasis _ISOTarget;
        ISOBasis[] _all_IsoBasisCash;
        List<IISOBasis> _alll_IIsoBasisCash = new List<IISOBasis>();

        void OnEnable()
        {
            _targetIsoBasis = target as ISOBasis;
            if (_targetIsoBasis == null || target != null && (bPrefab = PrefabHelper.IsPrefab(_targetIsoBasis.gameObject)))
                return;

            _ISOTarget = _targetIsoBasis.GetComponent<IISOBasis>();
            _spDoNotDestroyAutomatically = serializedObject.FindProperty("bDoNotDestroyAutomatically");
            _spISOOffest = serializedObject.FindProperty("_ISO_Offset");
            _spOnGroundObject = serializedObject.FindProperty("isOnGroundObject");
            _spTransformsForFudge = serializedObject.FindProperty("transforms");
        }

        public override void OnInspectorGUI()
        {	
            if (bPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            if (undoredo())
                return;

            serializedObject.Update();
                        
            bool bLocalOnGroundToggle = false, bGlobalUpdate = false, bChangedDepthTransform = false;
            SortingOrder(ref bLocalOnGroundToggle, ref bGlobalUpdate, ref bChangedDepthTransform);

            serializedObject.ApplyModifiedProperties();

            if (bGlobalUpdate)
            {
                if (!IsoMap.IsNull && IsoMap.instance.bUseGroundObjectOffset)
                    IsoMap.UpdateGroundOffsetFudge_All_ISOBasis(ref _alll_IIsoBasisCash, 
                        _spOnGroundObject.boolValue ? IsoMap.instance.fOnGroundOffset : IsoMap.fCurrentOnGroundOffset, true);
                else
                    IsoMap.UpdateSortingOrder_All_ISOBasis(ref _all_IsoBasisCash);
            }
            else if (bLocalOnGroundToggle)
            {
                bool _isGroundObject = _spOnGroundObject.boolValue;
                if (!_isGroundObject)
                    _targetIsoBasis.MarkToDepthedTransformsIso2DObjects(false);

                _ISOTarget.Update_SortingOrder(true);
                _ISOTarget.Undo_UpdateDepthFudge(_ISOTarget.IsOnGroundObject() ? -IsoMap.fCurrentOnGroundOffset : 0, true);

                if (_isGroundObject)
                {
                    _targetIsoBasis.MarkToDepthedTransformsIso2DObjects(true);
                    _targetIsoBasis.CheckDepth_Transform();
                }
                SceneView.RepaintAll();
            }
            else if (bChangedDepthTransform)
            {
                if (_spOnGroundObject.boolValue)
                    _targetIsoBasis.CheckDepth_Transform();
            }
		}    

        void SortingOrder(ref bool bLocalOnGroundToggle, ref bool bGlobalChanged, ref bool bChangedDepthTransform)
        {
            _spDoNotDestroyAutomatically.boolValue = EditorGUILayout.ToggleLeft(
                new GUIContent("[Tooltip] This Will not automatically destroyed.",
                "If you enable this option, this ISOBasis will not be automatically destroyed" +
                " even if the [Offset for GroundObject setting of IsoMap] is turned off."), 
                _spDoNotDestroyAutomatically.boolValue);

            CustomEditorGUI.DrawSeperator();
            if (IsoMap.isNormalSOMode)
            {
                EditorGUILayout.HelpBox("SortingOrder Basis only works in Auto ISO mode.", MessageType.Info);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                _spISOOffest.vector3Value = Util.CustomEditorGUI.Vector3Slider(
                    _spISOOffest.vector3Value, Vector3.zero,
                    "SortingOrder Basis", -0.5f * Vector3.one, 0.5f * Vector3.one, EditorGUIUtility.currentViewWidth);
                bLocalOnGroundToggle |= EditorGUI.EndChangeCheck();
            }

            if (!IsoMap.IsNull)
            {
                CustomEditorGUI.DrawSeperator();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUI.BeginChangeCheck();
                    _spOnGroundObject.boolValue = EditorGUILayout.ToggleLeft("IsOnGroundObject", _spOnGroundObject.boolValue);
                    bLocalOnGroundToggle |= EditorGUI.EndChangeCheck();
                    if (_spOnGroundObject.boolValue)
                        bEditOnGroundOffset = EditorGUILayout.Toggle("Edit Global Offset", bEditOnGroundOffset);
                }
            }

            if (_spISOOffest == null)
                return;

            if (_ISOTarget == null)
            {
                Debug.LogWarning("ISOBasis must be located in the game object where the RC or ISO components reside.");
                return;
            }

            if (_spOnGroundObject.boolValue)
            {
                Util.CustomEditorGUI.NewParagraph("[Tooltip], What is OnGroundOffset? ", 
                    "Give them a global offset so that things on the ground are not covered by the edge of tile image of the ground.\n" +
                    "This has the effect of changing the screen depth of the Sprite object. In particular, it affects the SO calculation in AutoIso mode.");
                if (IsoMap.isAutoISOMode)
                {
                    string desc = string.Format("New SortingOrder is {0}: Modified from {1}", _ISOTarget.CalcSortingOrder(true), _ISOTarget.CalcSortingOrder(false));
                    EditorGUILayout.LabelField(desc);
                }

                if (_targetIsoBasis.Parent != null)
                    EditorGUILayout.ObjectField("Parent ISOBasis", _targetIsoBasis.Parent, typeof(ISOBasis), allowSceneObjects: true);
                else
                {
                    if (bEditOnGroundOffset)
                    {
                        float fOnGroundOffset = IsoMap.instance.fOnGroundOffset;
                        EditorGUI.BeginChangeCheck();
                        fOnGroundOffset = Util.CustomEditorGUI.FloatSlider("Global Offset",
                            fOnGroundOffset, 0, 1, EditorGUIUtility.currentViewWidth);
                        CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightBlue, 
                            string.Format("Reset ({0})", IsoMap.fOnGroundOffset_Default),
                            () => fOnGroundOffset = IsoMap.fOnGroundOffset_Default,
                            GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
                        bGlobalChanged |= EditorGUI.EndChangeCheck();
                        if (bGlobalChanged)
                        {
                            Undo.RecordObject(IsoMap.instance, "ISOBasis: Edit Ground Offset");
                            IsoMap.instance.fOnGroundOffset = fOnGroundOffset;
                        }
                    }
                    EditorGUILayout.Space();
                    bChangedDepthTransform = CustomTransform_ForFudge();
                    CustomEditorGUI.DrawSeperator();
                }
            }
            return;
        }

        bool CustomTransform_ForFudge()
        {
            GUIContent label = new GUIContent(string.Format("Transforms({0}) have the Depth", _spTransformsForFudge.arraySize),
                "Transforms registered here automatically apply Depth as much as Global Ground Offset. " +
                "Excluding it from the list restores the origin transform.");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_spTransformsForFudge, label, true);
            if (_spTransformsForFudge.isExpanded)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Auto Setup"))
                    {
                        serializedObject.ApplyModifiedProperties();
                        _targetIsoBasis.AutoSetup_DepthTransforms();
                        serializedObject.Update();
                    }

                    CustomEditorGUI.Button(true, Color.gray, "Clear Transforms", () =>
                    {
                        // before : _spTransformsForFudge.ClearArray();
                        serializedObject.ApplyModifiedProperties();
                        _targetIsoBasis.Clear_DepthTransforms();
                        serializedObject.Update();
                    });
                }
            }
            return EditorGUI.EndChangeCheck();
        }

        bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                // IsoMap.UpdateSortingOrder_All_ISOBasis(ref _allIsoBasisCash);
                Repaint();
                return true;
            }
            return false;
        }
    }
}
