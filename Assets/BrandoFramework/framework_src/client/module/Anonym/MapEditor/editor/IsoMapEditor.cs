using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
	using Util;
	[CustomEditor(typeof(IsoMap))]
    public class IsoMapEditor : Editor
    {
		[SerializeField]
		Vector2 vRotate;

		bool IsPrefab = true;
		bool bEditPrefab;
        bool bFoldoutISODesc = false;

        IsoMap isoMap = null;

        SerializedProperty spTileAngle;
		SerializedProperty spReferencePPU;
        SerializedProperty spTchPrefab;
        SerializedProperty spBulkPrefab;
		SerializedProperty spTilePrefab;
		SerializedProperty spObstacle;
        SerializedProperty spTriggerCubeOverlay;
        SerializedProperty spTriggerPlaneOverlay;
        SerializedProperty spOverlay;
        SerializedProperty spSideUnion;
		SerializedProperty spSideX;
		SerializedProperty spSideY;
		SerializedProperty spSideZ;
		SerializedProperty spRCU;
		SerializedProperty spRCX;
		SerializedProperty spRCY;
		SerializedProperty spRCZ;
		SerializedProperty spGameCamera;
		SerializedProperty spBISSO;
		SerializedProperty spCustomResolution;
		SerializedProperty spUseCustomResolution;
        SerializedProperty spGroundObjectOffset;
        SerializedProperty spUseGroundObjectOffset;

        List<IISOBasis> _alI_IIsoBasisCash = new List<IISOBasis>();
        IsoTransform[] _isoTransforms = null;

        void CorruptCash()
        {
            _alI_IIsoBasisCash.Clear();
        }

        void OnEnable()
        {
            isoMap = target as IsoMap;

			if (IsPrefab = PrefabHelper.IsPrefab(isoMap.gameObject))
                return;

            isoMap.UpdateIsometricSortingResolution();
			// isoMap.Update_TileAngle();
			spBISSO = serializedObject.FindProperty("bUseIsometricSorting");
			spTileAngle = serializedObject.FindProperty("TileAngle");
			spReferencePPU = serializedObject.FindProperty("ReferencePPU");
			bEditPrefab = false;
			spBulkPrefab = serializedObject.FindProperty("BulkPrefab");
			spTilePrefab = serializedObject.FindProperty("TilePrefab");
			spObstacle = serializedObject.FindProperty("ObstaclePrefab");
            spTriggerCubeOverlay = serializedObject.FindProperty("TriggerCubePrefab");
            spTriggerPlaneOverlay = serializedObject.FindProperty("TriggerPlanePrefab");
            spOverlay = serializedObject.FindProperty("OverlayPrefab");
			spSideUnion = serializedObject.FindProperty("Side_Union_Prefab");
			spSideX = serializedObject.FindProperty("Side_X_Prefab");
			spSideY = serializedObject.FindProperty("Side_Y_Prefab");
			spSideZ = serializedObject.FindProperty("Side_Z_Prefab");
			spRCU = serializedObject.FindProperty("Collider_Cube_Prefab");
			spRCX = serializedObject.FindProperty("Collider_X_Prefab");
			spRCY = serializedObject.FindProperty("Collider_Y_Prefab");
			spRCZ = serializedObject.FindProperty("Collider_Z_Prefab");
			spGameCamera = serializedObject.FindProperty("GameCamera");
			spUseCustomResolution = serializedObject.FindProperty("bCustomResolution");
			spCustomResolution = serializedObject.FindProperty("vCustomResolution");
            spTchPrefab = serializedObject.FindProperty("TchPrefab");
            spUseGroundObjectOffset = serializedObject.FindProperty("bUseGroundObjectOffset");
            spGroundObjectOffset = serializedObject.FindProperty("fOnGroundOffset");

        }

		public override void OnInspectorGUI()
        {
			if (IsPrefab)
            {
                base.DrawDefaultInspector();
                return;
            } 

			bool bAngleChanged = false;
            bool bISOChanged = false;
            bool bGroundOffsetToggleChanged = false;
            float fGroundOffsetValue = 0;

            serializedObject.Update();
				
			CustomEditorGUI.NewParagraph("[Game Camera]");
			spGameCamera.objectReferenceValue = EditorGUILayout.ObjectField(
				spGameCamera.objectReferenceValue, typeof(Camera), allowSceneObjects:true);
			EditorGUILayout.Separator();

			CustomEditorGUI.NewParagraph("[Isometric Angle]");
				
			EditorGUI.BeginChangeCheck();
			spTileAngle.vector2Value = new Vector2(
				Util.CustomEditorGUI.FloatSlider("Up/Down", spTileAngle.vector2Value.x, -90f, 90f, EditorGUIUtility.currentViewWidth, true),
				Util.CustomEditorGUI.FloatSlider("Left/Right", spTileAngle.vector2Value.y, -90f, 90f, EditorGUIUtility.currentViewWidth, true));
			if (EditorGUI.EndChangeCheck())
			{
				bAngleChanged = true;
			}

			EditorGUILayout.Separator();
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("Reset", GUILayout.Width(75f));
				using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
				{
					if (GUILayout.Button("30°"))
					{
						spTileAngle.vector2Value = new Vector2(30f, -45f);
						bAngleChanged = true;
					}
					if (GUILayout.Button("35.264°"))
					{
						spTileAngle.vector2Value = new Vector2(35.264f, -45f);
						bAngleChanged = true;
					}
				}
			}

			EditorGUILayout.Separator();
			CustomEditorGUI.NewParagraph("[Ref Tile Sprite]");
			using (new EditorGUILayout.HorizontalScope())
			{
				float fWidth = 120f;
				Rect _rt = EditorGUI.IndentedRect(EditorGUI.IndentedRect(GUILayoutUtility.GetRect(fWidth, fWidth * 0.5f)));
				CustomEditorGUI.DrawSprite(_rt, isoMap.RefTileSprite, Color.clear, true, false);

				using (new EditorGUILayout.VerticalScope())
				{
					EditorGUILayout.Separator();

					spReferencePPU.floatValue = EditorGUILayout.FloatField(
						string.Format("Pixel Per Unit : Ref({0})", isoMap.RefTileSprite.pixelsPerUnit),
						spReferencePPU.floatValue);
							
					EditorGUILayout.Separator();

					EditorGUI.BeginChangeCheck();
					Sprite _newSprite = (Sprite) EditorGUILayout.ObjectField(
						isoMap.RefTileSprite, typeof(Sprite), allowSceneObjects:false);
					if (EditorGUI.EndChangeCheck())
					{
						if (_newSprite != null)
						{
							isoMap.RefTileSprite = _newSprite;
							//spReferencePPU.floatValue = isoMap.RefTileSprite.pixelsPerUnit;
						}
					}		
				}
			}          

			EditorGUILayout.Separator();
            Util.CustomEditorGUI.NewParagraph("[Util]");
			using (new EditorGUILayout.HorizontalScope())
			{
				using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
				{
					if (GUILayout.Button("New Bulk"))
					{
                        isoMap.NewBulk();
                        CorruptCash();
                    }
				}

				using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightGreen))
				{
					if (GUILayout.Button("Reset Scene Camera"))
					{
                        isoMap.Update_TileAngle();
					}
				}
			}

            #region Global ISO
            EditorGUILayout.Separator();
            CustomEditorGUI.NewParagraph("[Isometric Sorting Order]");
            using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
            {
                bFoldoutISODesc = CustomEditorGUI.CAUTION_Foldout(EditorGUILayout.GetControlRect(),
                        bFoldoutISODesc, "Plz, Foldout & Read before use Auto ISO.");

                if (bFoldoutISODesc)
                {
                    EditorGUILayout.HelpBox(
                        "IsometricSortingOrder(ISO) overrides the SortingOrder of all SpriteRenderers and " +
                        "ParticleSystemRenderers attached to game objects (including children).", MessageType.Info);
                    EditorGUILayout.HelpBox(
                        "ISO calculates camera direction Depth using weight " +
                        "and position of x, y, z axis based on isometric angle.", MessageType.Info);
                    EditorGUILayout.HelpBox("If you want to batch edit the SortingOrder of multiple tiles, " +
                        "set below 'Auto ISO' to false and use the CAUTION function in IsoTileBulk.", MessageType.Info);
                    EditorGUILayout.HelpBox("When Auto ISO On / Off is switched, " +
                        "\nthe existing SO values of the renderers are backed up by their respective ISO components.", MessageType.Info);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.HelpBox("It can be cleared with the right button " +
                            "\nif it is reset to an unintended value during switching or if the backup value is meaningless.", MessageType.Warning);
                        if (GUILayout.Button("Delete\nBackups"))
                        {
                            isoMap.Clear_All_ISO_Backup();
                            isoMap.Update_All_ISO();
                        }
                    }
                    EditorGUILayout.HelpBox(
                        "If the newly added renderer is not drawn, " +
                        "make sure that the object has an IsometricSortingOrder component.", MessageType.Warning);

                    EditorGUI.BeginChangeCheck();
                    spBISSO.boolValue = EditorGUILayout.ToggleLeft("Use Auto ISO", spBISSO.boolValue);

                    if (isoMap.bUseIsometricSorting)
                    {
                        EditorGUI.indentLevel++;
                        spUseCustomResolution.boolValue = !EditorGUILayout.ToggleLeft("Use Auto Resolution", !spUseCustomResolution.boolValue);
                        EditorGUI.indentLevel--;
                        if (spUseCustomResolution.boolValue)
                        {
                            spCustomResolution.vector3Value = Util.CustomEditorGUI.Vector3Slider(spCustomResolution.vector3Value,
                                IsoMap.vMAXResolution, "Custom Resolution of Axis", Vector3.zero, IsoMap.vMAXResolution, EditorGUIUtility.currentViewWidth);
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Resolution: " + isoMap.fResolutionOfIsometric);
                        }
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        bISOChanged = true;
                    }
                }
            }
            #endregion

            #region Offset_GroundObject
            EditorGUILayout.Separator();
            bool bUseGroundObjectOffset = spUseGroundObjectOffset.boolValue;

            bUseGroundObjectOffset  = Util.CustomEditorGUI.NewParagraphWithHideToggle(
                "[Global Offset for GroundObject]", 
                string.Format("Batch Process ({0})", bUseGroundObjectOffset ? "Revert all ground objects" : "Apply to all ground objects"), 
                bUseGroundObjectOffset);

            if (bGroundOffsetToggleChanged = (spUseGroundObjectOffset.boolValue != bUseGroundObjectOffset))
                spUseGroundObjectOffset.boolValue = bUseGroundObjectOffset;

            if (bUseGroundObjectOffset)
            {
                fGroundOffsetValue = spGroundObjectOffset.floatValue;
                spGroundObjectOffset.floatValue = Util.CustomEditorGUI.FloatSlider("Global Offset for OnGroundObject",
                    spGroundObjectOffset.floatValue, 0, 1, EditorGUIUtility.currentViewWidth);
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightBlue, "Default",
                    () => spGroundObjectOffset.floatValue = IsoMap.fOnGroundOffset_Default,
                    GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
                fGroundOffsetValue = spGroundObjectOffset.floatValue - fGroundOffsetValue;
            }
            else
            {
                EditorGUILayout.HelpBox("With this option, the depth of the following ground objects is adjusted in a batch.\n" +
                    "All components that inherit IsometricMovement or RegularCollider.\n" +
                    "Those are Characters and Tile Attachments.", MessageType.Info);
            }
            #endregion

            EditorGUILayout.Separator();
            Util.CustomEditorGUI.NewParagraph("[Prefab]");
			if (bEditPrefab = EditorGUILayout.ToggleLeft("Edit Prefab", bEditPrefab))
			{
				EditorGUILayout.LabelField("Core Object");
				EditorGUI.indentLevel++;
				spBulkPrefab.objectReferenceValue = 
					EditorGUILayout.ObjectField("Bulk", spBulkPrefab.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spTilePrefab.objectReferenceValue = 
					EditorGUILayout.ObjectField("Tile", spTilePrefab.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
                spOverlay.objectReferenceValue = 
					EditorGUILayout.ObjectField("Overlay", spOverlay.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
                spTriggerPlaneOverlay.objectReferenceValue =
                    EditorGUILayout.ObjectField("Trigger IsoPlane Overlay", spTriggerPlaneOverlay.objectReferenceValue,
                    typeof(GameObject), allowSceneObjects: false);
                spTriggerCubeOverlay.objectReferenceValue =
                    EditorGUILayout.ObjectField("Trigger Cube Overlay", spTriggerCubeOverlay.objectReferenceValue,
                    typeof(GameObject), allowSceneObjects: false);
				spObstacle.objectReferenceValue = 
					EditorGUILayout.ObjectField("Obstacle", spObstacle.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				EditorGUILayout.Separator();
				EditorGUI.indentLevel--;

				EditorGUILayout.LabelField("Side Object");
				EditorGUI.indentLevel++;
				spSideUnion.objectReferenceValue = 
					EditorGUILayout.ObjectField("Union", spSideUnion.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spSideX.objectReferenceValue = 
					EditorGUILayout.ObjectField("Axis-X", spSideX.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spSideY.objectReferenceValue = 
					EditorGUILayout.ObjectField("Axis-Y", spSideY.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spSideZ.objectReferenceValue = 
					EditorGUILayout.ObjectField("Axis-Z", spSideZ.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				EditorGUILayout.Separator();
				EditorGUI.indentLevel--;

				EditorGUILayout.LabelField("Regular Collider Object");
				EditorGUI.indentLevel++;
				spRCU.objectReferenceValue = 
					EditorGUILayout.ObjectField("Cube", spRCU.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spRCX.objectReferenceValue = 
					EditorGUILayout.ObjectField("Plane-YZ", spRCX.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spRCY.objectReferenceValue = 
					EditorGUILayout.ObjectField("Plane-XZ", spRCY.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				spRCZ.objectReferenceValue = 
					EditorGUILayout.ObjectField("Plane-XY", spRCZ.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
				EditorGUILayout.Separator();
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Regular Collider Object");
                EditorGUI.indentLevel++;
                spTchPrefab.objectReferenceValue =
					EditorGUILayout.ObjectField("Tall Character Helper", spTchPrefab.objectReferenceValue, 
					typeof(GameObject), allowSceneObjects:false);
                EditorGUI.indentLevel--;
            }

			serializedObject.ApplyModifiedProperties();

            if (bISOChanged)
            {
                isoMap.UpdateIsometricSortingResolution();
                if (spBISSO.boolValue) // false -> true
                {
                    isoMap.Update_All_ISO( isoMap.Backup_All_ISO() );
                }
                else // true -> false
                {
                    // 백업된 so로 복구
                    isoMap.Update_All_ISO( isoMap.Revert_All_ISO() );
                }
            }
            else if (bAngleChanged)
			{
				isoMap.Update_TileAngle();
                isoMap.Update_All_ISO();
                isoMap.Update_Grid();
                IsoMap.Update_All_IsoTransform_Rotate(ref _isoTransforms);
            }

            if (bGroundOffsetToggleChanged)
            {
                _alI_IIsoBasisCash.Clear();
                isoMap.bUseGroundObjectOffset = !isoMap.bUseGroundObjectOffset;
                IsoMap.GatherGroundIIsoBasisCash(ref _alI_IIsoBasisCash, true);
                isoMap.bUseGroundObjectOffset = !isoMap.bUseGroundObjectOffset;
                isoMap.Update_GroundOffset(ref _alI_IIsoBasisCash);
                isoMap.MarkGroundOffsetToIso2Ds(_alI_IIsoBasisCash, isoMap.bUseGroundObjectOffset);
                _alI_IIsoBasisCash.Clear();
            }
            else if (fGroundOffsetValue != 0)
            {
                IsoMap.UpdateSortingOrder_All_ISOBasis();
                IsoMap.UpdateGroundOffsetFudge_All_ISOBasis(ref _alI_IIsoBasisCash, fGroundOffsetValue);
            }
        }
    }
}