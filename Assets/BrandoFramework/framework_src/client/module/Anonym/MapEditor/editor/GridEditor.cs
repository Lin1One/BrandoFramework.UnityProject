using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;
	[CustomEditor(typeof(Grid))]
    public class GridEditor : Editor
    {
		[SerializeField]
		Vector2 vRotate;
		bool bPrefab = false;
		SerializedProperty _sp_LocalGrid;
		SerializedProperty _sp_LocalScale;
		SerializedProperty _sp_LocalAxisInterval;
		Grid _grid;
		bool bRootGrid;
		static float _fMaxTileSize = 3;
		List<IsoTileBulk> _bulks = new List<IsoTileBulk>();
		List<IsoTileBulk> BulkList{get{
			if (_grid.bChildUpdatedFlagForEditor)
			{
				BulkList_Update();
				_grid.bChildUpdatedFlagForEditor = false;
			}
			return _bulks;
		}}
		void BulkList_Update()
		{
			BulkList_Clear();
			
			IsoTileBulk _bulk = _grid.gameObject.GetComponent<IsoTileBulk>();
			if (_bulk != null){
				_bulks.Add(_bulk);
				_bulks.AddRange(_bulk.GetChildBulkList(true));
			}
		}
		void BulkList_Clear()
		{
			_bulks.Clear();
		}
		
		void OnEnable()
        {
			if ((_grid = (Grid)target) == null)
				return;

			if (bPrefab = PrefabHelper.IsPrefab(_grid.gameObject))
                return;

			_sp_LocalGrid = serializedObject.FindProperty("bUseLocalGrid");
			_sp_LocalScale = serializedObject.FindProperty("_TileSize");
			_sp_LocalAxisInterval = serializedObject.FindProperty("_GridInterval");
			bRootGrid = _grid.GetComponent<IsoMap>() != null;
			BulkList_Update();
			// TileList_AutoUpdate(true);
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled or inactive.
		/// </summary>
		void OnDisable()
		{
			BulkList_Clear();	
		}

		public override void OnInspectorGUI(){
            if (bPrefab){
                base.DrawDefaultInspector();
                return;
            }  

            serializedObject.Update();
			bool bUpdateGridSize = false;
			bool bUpdateLocalGrid = false;			

			if (bRootGrid)
			{
				EditorGUILayout.HelpBox("[Root Grid]\nAll non-local grids are affected by this grid.", MessageType.Info);
			}
			else 
			using (var _result = new EditorGUI.ChangeCheckScope())
			{
				_sp_LocalGrid.boolValue = EditorGUILayout.ToggleLeft(
					"Use Local Grid", _sp_LocalGrid.boolValue, EditorStyles.boldLabel);					
				bUpdateLocalGrid = _result.changed;
			}

			if (_sp_LocalGrid.boolValue)
			{
				float fWidth = EditorGUIUtility.currentViewWidth * 0.475f;
				float mfWidth = fWidth * 0.95f;

				using (new EditorGUILayout.HorizontalScope()){
					EditorGUI.BeginChangeCheck();
					using (new EditorGUILayout.VerticalScope()){
						Vector3 _InvertInterval = new Vector3(1/_sp_LocalAxisInterval.vector3Value.x, 
							1 / _sp_LocalAxisInterval.vector3Value.y, 1/_sp_LocalAxisInterval.vector3Value.z);

						_InvertInterval = Util.CustomEditorGUI.Vector3Slider(_InvertInterval, 
							new Vector3(1, 3, 1), "[Coordinates in a Tile]", Vector3.one, 10 * Vector3.one, mfWidth);

						_sp_LocalAxisInterval.vector3Value = new Vector3(1/_InvertInterval.x, 
							1 / _InvertInterval.y, 1/_InvertInterval.z);
					}
					using (new EditorGUILayout.VerticalScope()){

						EditorGUILayout.LabelField("[Tile Size]", EditorStyles.boldLabel, 
							GUILayout.MaxWidth(mfWidth * 0.9f));                

						float _fXZ = _sp_LocalScale.vector3Value.x;
						float _fY = _sp_LocalScale.vector3Value.y;
						using (new EditorGUILayout.VerticalScope())
						{
                            _fMaxTileSize = Util.CustomEditorGUI.FloatSlider("Max Value", _fMaxTileSize, 0.5f, 10f, mfWidth);
							_fXZ = Util.CustomEditorGUI.FloatSlider("Width(XZ)", _fXZ, 0.1f, _fMaxTileSize, mfWidth);
							_fY = Util.CustomEditorGUI.FloatSlider("Height(Y)", _fY, 0.1f, _fMaxTileSize, mfWidth);
							_sp_LocalScale.vector3Value = new Vector3(_fXZ, _fY, _fXZ);
						}
						EditorGUILayout.Separator();
						using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
						if (GUILayout.Button("Reset (1, 1)", GUILayout.MaxWidth(mfWidth)))
						{
                                _sp_LocalScale.vector3Value = new Vector3(1, 1, 1);
						}
						
						// _sp_LocalScale.vector3Value = CustomEditorGUI.Vector3Slider(
						// 	_sp_LocalScale.vector3Value, Vector3.one,
						// 	"[Tile Size]", Vector3.one * 0.1f, Vector3.one * 3f, mfWidth * 0.9f);
					}
					if (EditorGUI.EndChangeCheck())
						bUpdateGridSize = true;
				}
			}
            else
            {
				using (new EditorGUILayout.HorizontalScope())
				{
                    if (_grid.parentGrid != null)
                    {
                        GUILayout.Label("Parent Grid ");
                        EditorGUI.BeginDisabledGroup(true);
                        using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
                        {
                            EditorGUILayout.ObjectField(_grid.parentGrid.gameObject, typeof(GameObject), allowSceneObjects: false);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        GUILayout.Label("Currently, there is no Parent Grid!");
                    }
                }
            }

			serializedObject.ApplyModifiedProperties();

			if (bUpdateGridSize || bUpdateLocalGrid)
			{
				if(!IsoMap.IsNull && (_grid.gameObject == IsoMap.instance.gameObject))
				{
					IsoMap.instance.Update_Grid();
				}
				else // if Bulk GameObject
				{
					BulkList.ForEach((r) => 
					{
						r.coordinates.Update_Grid(true);
						r.Update_Grid();
					});
				}
			}
        }
    }
}