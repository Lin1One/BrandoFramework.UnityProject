using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    [CustomEditor(typeof(RegularCollider))]    
	public class RegularColliderEditor : Editor {

        bool IsPrefab = false;
        static float fScaleSliderValue = 2f;
        static bool bHide_Iso2D = false;

        RegularCollider _rc;
		IsoTile _t;
        Iso2DObject _childIso2D_0;
        SerializedProperty _spIso2DScaleMultiplier;

        TmpTexture2D tmpTexture2D = new TmpTexture2D();

        bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                Repaint();
                return true;
            }
            return false;
        }

		void OnEnable()
        {
			_rc = (RegularCollider) target;
            if (_rc == null)
                return;

            if (target != null && (IsPrefab = PrefabHelper.IsPrefab(_rc.gameObject)))
                return;


			_t = _rc.GetComponentInParent<IsoTile>();

            _spIso2DScaleMultiplier = serializedObject.FindProperty("_vIso2DScaleMultiplier");

            update_childIso2D_0();
            // _r.update_subColliders();
        }

        void update_childIso2D_0()
        {
            if (_rc.Iso2Ds != null && _rc.Iso2Ds.Length > 0)
                _childIso2D_0 = _rc.Iso2Ds[0];
        }

		public override void OnInspectorGUI()
        {	
            if (IsPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            if (undoredo())
                return;

            serializedObject.Update();

            if (Event.current.type == EventType.Layout)
                update_childIso2D_0();

            sortingOrder();
            iso2D_controller();
            SubCollider_Create();
            SubCollider_List();            

            CustomEditorGUI.ColliderControlHelperGUI(tmpTexture2D, targets);

            serializedObject.ApplyModifiedProperties();
		}        

        void sortingOrder()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Util.CustomEditorGUI.NewParagraph(string.Format("[SortingOrder {0}]", _rc.CalcSortingOrder(true)));
                // EditorGUILayout.Toggle(new GUIContent("Force to OnGroundObject", ""), false);
            }

            var basis = _rc.GetISOBasis();
            if (basis != null)
            {
                EditorGUILayout.ObjectField("Modified by", basis, typeof(ISOBasis), allowSceneObjects:true);
            }
            CustomEditorGUI.DrawSeperator();
        }

        void iso2D_controller()
        {
            if (_childIso2D_0 == null)
                return; 

            if (!(bHide_Iso2D = Util.CustomEditorGUI.NewParagraphWithHideToggle("[Iso2DObject Control Helper]", "Hide", bHide_Iso2D)))
            {
                EditorGUI.indentLevel = 0;
            
                float fWidth = EditorGUIUtility.currentViewWidth * 0.475f;
                float mfWidth = fWidth * 0.95f;

                fScaleSliderValue = Mathf.Max(1f, EditorGUILayout.FloatField("Cap of Sliders", fScaleSliderValue));
                Vector3 v3Max = Vector3.one * fScaleSliderValue;

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUI.BeginChangeCheck();
                        _spIso2DScaleMultiplier.vector3Value = Util.CustomEditorGUI.Vector3Slider(
                            _spIso2DScaleMultiplier.vector3Value, Vector3.one, 
                            "Scale", Vector3.one * 0.1f, v3Max, mfWidth);
                        if (EditorGUI.EndChangeCheck())
                        {
                            foreach(var r in _rc.Iso2Ds)
                                r.Undo_LocalScale(_spIso2DScaleMultiplier.vector3Value);
                        }
                    }
                    using (new EditorGUILayout.VerticalScope())
                    {
                        Vector3 _origin = _rc.Iso2Ds[0].GetPosition_WithoutFudge() - _rc.transform.position;
                        EditorGUI.BeginChangeCheck();
                        Vector3 _new = Util.CustomEditorGUI.Vector3Slider(
                            _origin, Vector3.zero, "Iso2D Position Handle", 
                            -v3Max, v3Max, mfWidth);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Vector3 _diff = _new - _origin;
                            for (int i = 0 ; i < _rc.Iso2Ds.Length; ++i)
                                _rc.Iso2Ds[i].Undo_PositionOffset(_rc.Iso2Ds[i].GetPosition_WithoutFudge() + _diff);
                        }
                    }
                }
            }
            CustomEditorGUI.DrawSeperator();
            EditorGUILayout.Separator();
        }

        void SubCollider_Init(GameObject _go)
        {
            if (_go == null)
                return;

            SubColliderHelper _sch = _go.GetComponent<SubColliderHelper>();
            if (_t.bAutoFit_ColliderScale)
			{
                _sch.ScaleMultiplier(_t.coordinates.TileSize);
            }
            _sch.Toggle_UseGridTileScale(_t.bAutoFit_ColliderScale);
        }

        void SubCollider_Create()
        {
            Util.CustomEditorGUI.NewParagraph("[Sub Collider Creator]");
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                {
                    EditorGUI.BeginChangeCheck();

                    SubCollider_Init(Util.CustomEditorGUI.Undo_Instantiate(
                        IsoMap.Prefab_Cube, _rc.transform, "Create Cube", true));
                    SubCollider_Init(Util.CustomEditorGUI.Undo_Instantiate(
                        IsoMap.Prefab_Collider_X, _rc.transform, "Axis X", true));
                    SubCollider_Init(Util.CustomEditorGUI.Undo_Instantiate(
                        IsoMap.Prefab_Collider_Y, _rc.transform, "Axis Y", true));
                    SubCollider_Init(Util.CustomEditorGUI.Undo_Instantiate(
                        IsoMap.Prefab_Collider_Z, _rc.transform, "Axis Z", true));

                    if (EditorGUI.EndChangeCheck())
                    {
                        // _r.update_subColliders();
                    }
                }
            }
        }

        void SubCollider_List()
        {
            if (_rc.SubColliders != null && _rc.SubColliders.Length > 0)
            {
                Util.CustomEditorGUI.NewParagraph("[Sub Colliders(Can not change)]");

                foreach (var subCollider in _rc.SubColliders)
                {
                    if (subCollider != null)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                            {
                                if (GUILayout.Button("Del"))
                                {
                                    Undo.RecordObject(_rc, "Destroy : SubCollider");
                                    Undo.DestroyObjectImmediate(subCollider.gameObject);
                                    EditorUtility.SetDirty(_rc.gameObject);
                                }
                            }
                            EditorGUILayout.ObjectField(subCollider, typeof(BoxCollider), allowSceneObjects: true);
                        }
                    }
                }
            }
            CustomEditorGUI.DrawSeperator();
        }
	}
}
