using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System.Linq;
using System.Reflection;

namespace Anonym.Isometric
{
    using Anonym.Util;

    [CustomEditor(typeof(IsoTile))]
    [CanEditMultipleObjects]
    public class IsoTileEditor : Editor
    {
        enum EditorMode
        {
            Extrude,
            Select,
        }
        EditorMode _E_Mode = EditorMode.Extrude;
        int selectedToolbar;
        //IsoTileBulk _bulk;
        IsoTile _tile_Inspector;
        IsoTile _tile_Scene;        
        IsoTile refTile;

        bool bPrefab = false;
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
        bool bPartofPrefabWorkflow = false;
#endif
        bool bRightEdge = false;
        bool bLeftEdge = false;
        bool bUpEdge = false;
        bool bDownEdge = false;
        bool bFowardEdge = false;
        bool bBackEdge = false;
        float fHandleSize = 1;
        float fHandleOffsetSize = 1.5f;
        Vector3 handlePosition_Centor = Vector3.zero;
        Camera _camera = null;
        Camera camera { get { return _camera ? _camera : _camera = FindObjectOfType<Camera>(); } }

        bool bNoBulk = false;

        bool bSingleCtlr = true;
        static bool bShowCreateAttachmentBtn = true;
        [SerializeField]
        static bool bHideSideMenu = false;
        [SerializeField]
        static bool bHideTileSetSpritesMenu = true;
        [SerializeField]
        static bool bHideCCMenu = true;
        static bool bFrameSelected = false;
        static bool bWithAttachment = false;
        SelectionType selectionType = SelectionType.NewTile;
        static GUIStyle screenInfoFontStyle = null;
        static int previewLayer = 0;

        TmpTexture2D tileIcon = new TmpTexture2D();

        SerializedProperty spTileSet;
        SerializedObject spTileInspector;

        PreviewRenderUtility previewRenderUtility;
        Camera PreviewCam { get
            {
#if UNITY_2017_1_OR_NEWER
                return previewRenderUtility.camera;
#else
                return previewRenderUtility.m_Camera;
#endif
            }
        }
        GameObject previewObject;

        void update_InspectorTile()
        {
            if (_tile_Inspector == null || _tile_Inspector.gameObject != Selection.activeGameObject)
            {
                _tile_Inspector = Selection.activeGameObject == null ? null : 
                    Selection.activeGameObject.GetComponent<IsoTile>();
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                bPartofPrefabWorkflow = PrefabUtility.IsPartOfAnyPrefab(_tile_Inspector);
#endif
                childInfoUpdate();
            }

            if (_tile_Inspector)
            {
                bNoBulk = _tile_Inspector.GetComponentInParent<IsoTileBulk>() == null;
            }

            if (spTileInspector != null && _tile_Inspector  != null && spTileInspector.targetObject != _tile_Inspector.gameObject)
            {
                spTileInspector.ApplyModifiedProperties();
                spTileInspector = null;
            }
            spTileSet = null;

            if (_tile_Inspector)
            {
                spTileInspector = new SerializedObject(_tile_Inspector);
                if (spTileInspector != null)
                    spTileSet = spTileInspector.FindProperty("_tileSetSprites");
            }
        }
        void update_SceneTile()
        {
            if (_tile_Scene == null || _tile_Scene.gameObject != Selection.activeGameObject)
            {
                _tile_Scene = Selection.activeGameObject == null ? null : 
                    Selection.activeGameObject.GetComponent<IsoTile>();
                edgeUpdate();   
            }
        }
        void childInfoUpdate()
        {
            if (_tile_Inspector != null)
            {
                _tile_Inspector.Update_AttachmentList();

                tileIcon.MakeRenderImage(_tile_Inspector.gameObject, camera, Color.clear);
            }
            //EditorUtility.SetDirty(target);
        }
        bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                childInfoUpdate();
                Repaint();
                return true;
            }
            return false;
        }
        void edgeUpdate()
        {
            if (_tile_Scene != null)
            {
                bRightEdge = _tile_Scene.IsLastTile(Vector3.right);
                bLeftEdge = _tile_Scene.IsLastTile(Vector3.left);
                bUpEdge = _tile_Scene.IsLastTile(Vector3.up);
                bDownEdge = _tile_Scene.IsLastTile(Vector3.down);
                bFowardEdge = _tile_Scene.IsLastTile(Vector3.forward);
                bBackEdge = _tile_Scene.IsLastTile(Vector3.back);
            }
        }

        void OnEnable()
        {
            if (target == null || Selection.activeGameObject == null)
                return;

            if (bPrefab = PrefabHelper.IsPrefab(targets.Select(r => (r as IsoTile).gameObject).ToArray()))
                return;
            
            update_InspectorTile();

            if (screenInfoFontStyle == null)
            {
                screenInfoFontStyle = new GUIStyle();
                screenInfoFontStyle.normal.textColor = Color.green;
                screenInfoFontStyle.fontStyle = FontStyle.Bold;
                screenInfoFontStyle.fontSize = 15;
            }

            if (SceneView.lastActiveSceneView != null && bFrameSelected)
            {
                SceneView.lastActiveSceneView.FrameSelected(false);
            }
        }
        void OnDisable ()
        {
            clearPreviewInstance();
        }
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                clearPreviewInstance();
        }
        public override void OnInspectorGUI()
        {
            if (bPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            update_InspectorTile();

            if (_tile_Inspector == null)
                return; 
                
            if (undoredo())
                return;

            serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[Tile Control]", EditorStyles.boldLabel);

            Inspector_Tile();
            Inspector_Side();
            Inspector_TileSet();
            Inspector_Collider();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("[Attachment Control]", EditorStyles.boldLabel);
            Inspector_Attached();

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (bNoBulk || bPrefab)
                return;

            if (((IsoTile)target).gameObject != Selection.activeGameObject)
                return; 

            if (Event.current.type == EventType.Layout)
            {
                update_SceneTile();

                Handles.BeginGUI();

                float fGap = 20 * EditorGUIUtility.pixelsPerPoint;
                float fHeightPixel = EditorGUIUtility.singleLineHeight * 5 * EditorGUIUtility.pixelsPerPoint;
                float fWidthPixel = 230 * EditorGUIUtility.pixelsPerPoint;
                Rect _windowRT = new Rect(Screen.width - (fGap * 0.6f + fWidthPixel), 
                    Screen.height - (fGap * 2 + fHeightPixel), fWidthPixel, fHeightPixel);
                _windowRT = EditorGUIUtility.PixelsToPoints(_windowRT);

                int windowID = EditorGUIUtility.GetControlID(FocusType.Passive, _windowRT);
                GUILayout.Window(windowID, _windowRT, ScreenInfoWindow,
                    _E_Mode == EditorMode.Extrude ?
                    "Extrude Mode" : "Select Mode");

                Handles.EndGUI();

                if (_tile_Scene.coordinates.bChangedforEditor)
                {
                    edgeUpdate();
                    _tile_Scene.coordinates.bChangedforEditor = false;
                }
            }

            if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint
                || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseMove
                || Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDrag)
            {
                HandleDraw();

                Vector3 position = _tile_Scene.transform.position + fHandleSize * (new Vector3(1f, -0.5f, 0));
                string posString = _tile_Scene.transform.position.ToString();

                Handles.color = Color.gray;
                Handles.DrawLine(_tile_Scene.transform.position, position);
                Handles.Label(position, _tile_Scene.name + "\n" + posString, screenInfoFontStyle);
            }
            return;
        }
        void HandleDraw()
        {
            handle_Init();

            if (_E_Mode == EditorMode.Extrude)
            {
                handle_Editor("DH_Right", Vector3.right, Handles.xAxisColor, bRightEdge);
                handle_Editor("DH_Left", Vector3.left, Handles.xAxisColor, bLeftEdge);

                handle_Editor("DH_Up", Vector3.up, Handles.yAxisColor, bUpEdge);
                handle_Editor("DH_Down", Vector3.down, Handles.yAxisColor, bDownEdge);

                handle_Editor("DH_Forward", Vector3.forward, Color.cyan, bFowardEdge);
                handle_Editor("DH_Back", Vector3.back, Color.cyan, bBackEdge);
            }
        }
        void ScreenInfoWindow(int id)
        {
            if (_E_Mode == EditorMode.Extrude)
            {
                bFrameSelected = GUILayout.Toggle(bFrameSelected, "Auto Frame Selected");
                bWithAttachment = GUILayout.Toggle(bWithAttachment, "Extrude with Attachment");
                GUILayout.Toggle(selectionType == SelectionType.AllTile,
                    selectionType == SelectionType.AllTile ? "Shift On : Select with last Tile(Slow)" : "Shift Off : Select only new Tile(Fast)");
                GUILayout.Toggle(!bSingleCtlr, !bSingleCtlr ? "Ctlr On : Apply to Selection(Slow)" : "Ctlr Off : Apply to a Tile(Fast)");
                // if (GUILayout.Button("Selection Mode"))
                //     _E_Mode = EditorMode.Select;
            }
            else
            {
                if (GUILayout.Button("Extrude Mode"))
                    _E_Mode = EditorMode.Extrude;
            }
        }
        void handle_Init()
        {
            fHandleSize = HandleUtility.GetHandleSize(_tile_Scene.transform.position);
            handlePosition_Centor = _tile_Scene.transform.position;
            bSingleCtlr = !Event.current.control;
            selectionType = !Event.current.shift ? SelectionType.NewTile : SelectionType.AllTile;
        }
        void handle_Editor(string handleName, Vector3 _direction, Color _color, bool _active)
        {
            Vector3 handlePosition = handlePosition_Centor + fHandleSize * _direction * fHandleOffsetSize;
            if (_tile_Scene != null)// && _tile.coordinates.IsLastTile(_direction))
            {
                int controlID = 0;
#if UNITY_2017_2_OR_NEWER
                controlID = GUIUtility.GetControlID(handleName.GetHashCode(), FocusType.Passive);
#endif
                if (_active)
                {
                    Handles.color = _color;
                    Handles.DrawLine(handlePosition_Centor, handlePosition_Centor + fHandleSize * _direction);

                    using (var result = new EditorGUI.ChangeCheckScope())
                    {
                        if (bSingleCtlr)
                            WrapedFreeMoveHandle(controlID, handlePosition, Quaternion.identity,
                                fHandleSize * 0.15f, Vector3.zero, Handles.RectangleHandleCap);
                        else
                            WrapedFreeMoveHandle(controlID, handlePosition, Quaternion.identity,
                                fHandleSize * 0.25f, Vector3.zero, Handles.SphereHandleCap);

                        if (result.changed 
#if UNITY_2017_2_OR_NEWER
                            && GUIUtility.hotControl == controlID
#endif
                        ){
                            float fDragDistance = HandleUtility.CalcLineTranslation(
                                HandleUtility.WorldToGUIPoint(handlePosition),
                                Event.current.mousePosition, handlePosition, _direction);

                            float fHurdleDistance = Vector3.Scale(_direction,
                                _tile_Scene.coordinates.GridInterval).magnitude;

                            _tile_Scene.Bulk.Do_With_SensorOff(() =>
                            {
                                if (fDragDistance >= fHurdleDistance)
                                    handle_Extrude(_direction, selectionType, bSingleCtlr, bWithAttachment);
                                else if (fDragDistance <= -fHurdleDistance)
                                    handle_Press(-_direction, bSingleCtlr);
                            });
                            _tile_Scene.Bulk.Update_ChildList();
                        }
                    }
                }
                else
                {
                    Handles.color = _color / 1.5f;
                    WrapedFreeMoveHandle(controlID,
                        handlePosition, Quaternion.LookRotation(_direction), 
                        fHandleSize * 0.1f, Vector3.one, Handles.DotHandleCap);
                }
            }
        }

        Vector3 WrapedFreeMoveHandle(int controlID, Vector3 handlePosition, 
            Quaternion rotation, float size, Vector3 snap, Handles.CapFunction capF)
        {
            return Handles.FreeMoveHandle(
#if UNITY_2017_2_OR_NEWER
                            controlID, 
#endif
                        handlePosition, rotation, size, snap, capF);
        }

        void handle_Press(Vector3 _direction, bool _singleAction)
        {
            List<GameObject> _removeTileObjects = new List<GameObject>();
            List<IsoTile> _moveTileObjects = new List<IsoTile>();
            
            if (_singleAction)
            {
                tile_Press(_tile_Scene, _direction, ref _moveTileObjects, ref _removeTileObjects);
            }
            else
            {
                for (int i = 0 ; i < Selection.gameObjects.Length; ++i)
                {
                    tile_Press(Selection.gameObjects[i].GetComponent<IsoTile>(),
                        _direction, ref _moveTileObjects, ref _removeTileObjects);
                }
            }

            for( int i = 0 ; i < _moveTileObjects.Count; ++i)
                _moveTileObjects[i].coordinates.Translate(_direction);
                
            if (_removeTileObjects.Count > 0)
            {
                Selection.objects = Selection.gameObjects.Except(_removeTileObjects.ToArray()).ToArray();
                for (int i = 0 ; i < _removeTileObjects.Count; ++i)
                    Undo.DestroyObjectImmediate(_removeTileObjects[i]);
            }
        }
        void tile_Press(IsoTile _tile, Vector3 _direction, ref List<IsoTile> _moveList, ref List<GameObject> _removeList)
        {
            if (_tile.IsLastTile(-_direction) && !_tile.IsLastTile(_direction))
            {
                IsoTile _removeTile = _tile.NextTile(_direction);
                _moveList.Add(_tile);
                if (_removeTile != null)
                    _removeList.Add(_removeTile.gameObject);
            }
        }

        void handle_Extrude(Vector3 _direction, SelectionType _selectionType, bool _singleAction, bool _bWithAttachment)
        {
            Vector3 _lastPos = _tile_Scene.coordinates._xyz;
            List<GameObject> _newTileObjects = new List<GameObject>();
            IsoTile _newTile = null;
            if (_singleAction)
            {
                if ((_newTile = _tile_Scene.Extrude(_direction, _bWithAttachment)) != null)
                    _newTileObjects.Add(_newTile.gameObject);
            }
            else
            {
                foreach (var obj in Selection.gameObjects)
                {
                    if ((_newTile = obj.GetComponent<IsoTile>().Extrude(_direction, _bWithAttachment)) != null)
                        _newTileObjects.Add(_newTile.gameObject);
                }
            }
            if (_newTileObjects.Count > 0)
            {
                Undo.IncrementCurrentGroup();
                switch (_selectionType)
                {
                    case SelectionType.LastTile:
                        Selection.objects = _newTileObjects.ToArray();
                        break;
                    case SelectionType.NewTile:                        
                        break;
                    case SelectionType.AllTile:
                        Selection.objects = Selection.objects.Concat(_newTileObjects.ToArray()).ToArray();
                        break;
                }
                edgeUpdate();
            }
        }

        void Inspector_Selection()
        {
            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_Obstacle))
            {
                if (GUILayout.Button("CDDD"))
                {
                    List<GameObject> _selection = new List<GameObject>();
                    for (int i = 0; i < Selection.gameObjects.Length; ++i)
                    {
                        _selection.Add((
                            Selection.gameObjects[i].GetComponent<IsoTile>()).Duplicate().gameObject);
                    }
                    Undo.RecordObjects(Selection.objects, "IsoTile:Dulicate");
                    Selection.objects = _selection.ToArray();
                }
            }
        }
        void Inspector_Tile()
        {    
            if (bNoBulk)
            {
                EditorGUILayout.LabelField("Not available in No Bulk Prefab!");
                return;
            }

            using (new GUIBackgroundColorScope(Color.cyan))
            {
                refTile = (IsoTile)EditorGUILayout.ObjectField(refTile, typeof(IsoTile), allowSceneObjects: true);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                CustomEditorGUI.Button(refTile != null, Color.cyan, "Copycat", () =>    {
                    for (int i = 0; i < targets.Length; ++i)
                        ((IsoTile)targets[i]).Copycat(refTile, true);
                });

                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightYellow, "Duplicate", () => {
                    List<GameObject> _selection = new List<GameObject>();
                    for (int i = 0; i < targets.Length; ++i)
                        _selection.Add(((IsoTile)targets[i]).Duplicate().gameObject);
                    Undo.RecordObjects(Selection.objects, "IsoTile:Dulicate");
                    Selection.objects = _selection.ToArray();
                });

                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightMagenta, "Select Bulk", () =>   {
                    Selection.activeGameObject = _tile_Inspector.Bulk.gameObject;
                });
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                CustomEditorGUI.Button(true, Color.magenta, "Split bulk with selected tiles.", () => {
                    IsoMap.instance.NewBulk(_tile_Inspector.Bulk, targets.Where(r => r != null && r is IsoTile).Cast<IsoTile>());
                });
            }
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(string.Format(
                "Grid's Tile Scale : Width({0:0.00}), Height({1:0.00})",
                        _tile_Inspector.coordinates.TileSize.x ,
                        _tile_Inspector.coordinates.TileSize.y));

            float fWidth = EditorGUIUtility.currentViewWidth * 0.25f;
            using (new EditorGUILayout.HorizontalScope())
            {
                string undoName = "Use AutoFit Collider";
                EditorGUILayout.LabelField("Auto Fit", GUILayout.MaxWidth(fWidth));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.ToggleLeft("Collider",
                    _tile_Inspector.bAutoFit_ColliderScale,
                    GUILayout.MaxWidth(fWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    bool _bTmp = !_tile_Inspector.bAutoFit_ColliderScale;
                    for (int i = 0; i < Selection.gameObjects.Length; ++i)
                    {
                        if (Selection.gameObjects[i] != null)
                        {
                            IsoTile _tTmp = Selection.gameObjects[i].GetComponent<IsoTile>();
                            if (_tTmp != null)
                            {
                                Undo.RecordObject(_tTmp, undoName);
                                _tTmp.bAutoFit_ColliderScale = _bTmp;
                                _tTmp.Update_ColliderScale();
                            }
                        }
                    }
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.ToggleLeft("Sprite", 
                    _tile_Inspector.bAutoFit_SpriteSize,
                    GUILayout.MaxWidth(fWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    bool _bTmp = !_tile_Inspector.bAutoFit_SpriteSize;
                    for (int i = 0; i < Selection.gameObjects.Length; ++i)
                    {
                        if (Selection.gameObjects[i] != null)
                        {
                            IsoTile _tTmp = Selection.gameObjects[i].GetComponent<IsoTile>();
                            if (_tTmp != null)
                            {
                                Undo.RecordObject(_tTmp, undoName);
                                _tTmp.bAutoFit_SpriteSize = _bTmp;
                                _tTmp.Update_Attached_Iso2DScale(true, undoName);
                            }
                        }
                    }
                }
            }
            EditorGUI.indentLevel = 0;
            EditorGUILayout.Separator();
        }

        void Inspector_TileSet()
        {
            var _tileSet = spTileSet.objectReferenceValue as TileSetSprites;

            using (new EditorGUILayout.HorizontalScope())
            {
                bHideTileSetSpritesMenu = CustomEditorGUI.LableWithToggle("[TileSetSprites]", EditorStyles.boldLabel, bHideTileSetSpritesMenu, "Hide");
                GUILayout.FlexibleSpace();
                CustomEditorGUI.Button(_tileSet, CustomEditorGUI.Color_LightBlue, "Update TileSetSprites",
                    () => {
                        IsoTile.UpdateTileSet(serializedObject.targetObjects.Select(t => t is IsoTile ? t as IsoTile : (t is GameObject ? IsoTile.Find(t as GameObject) : null)).
                            Where(t => t != null && t.tileSetSprites != null), true, true, "Update Self: TileSetSprites");
                    });
            }
            if (!bHideTileSetSpritesMenu)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(spTileSet);
                if (EditorGUI.EndChangeCheck())
                {
                    _tileSet = spTileSet.objectReferenceValue as TileSetSprites;
                    _tile_Inspector.ChangeTileSet(_tileSet);
                    if (_tileSet && _tileSet.LookupedSubTileSet)
                    {
                        SubTileSetDrawer.BakeTilesetImage(_tileSet.LookupedSubTileSet, tileIcon);
                        _tileSet.LookupedSubTileSet.bakedTileSetImg = tileIcon.texture;
                    }
                }
            }
            EditorGUILayout.Separator();
        }

        void Inspector_Side()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                bHideSideMenu = CustomEditorGUI.LableWithToggle("[Side Control]", EditorStyles.boldLabel, bHideSideMenu, "Hide");
            }

            if (!bHideSideMenu)
            {
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
                {
                    bool bDisabled = false;
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                        bDisabled = bPartofPrefabWorkflow;
#endif
                    EditorGUI.BeginDisabledGroup(bDisabled);
                    EditorGUI.BeginChangeCheck();
                    bool _bUnionMode = 1 == EditorGUILayout.Popup(
                        _tile_Inspector.GetComponent<IsoTile>().IsUnionCube()
                         ? 1 : 0, new string[] { "Side Mode", " Union Mode" });
                    if (EditorGUI.EndChangeCheck())
                    {
                        foreach (GameObject _go in Selection.objects)
                        {
                            if (_go == null)
                                continue;
                            IsoTile _t = _go.GetComponent<IsoTile>();
                            if (_t != null && _t.IsUnionCube() != _bUnionMode)
                            {
                                _t.Reset_SideObject(_bUnionMode);
                            }
                        }
                        EditorGUIUtility.ExitGUI();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    float fWidth = EditorGUIUtility.currentViewWidth / 3f;
                    Rect _Rect = EditorGUILayout.GetControlRect(
                        new GUILayoutOption[] { GUILayout.Height(fWidth), GUILayout.ExpandWidth(true) });

                    Rect[] _DescRectDivision;
                    Rect[] _SubRects;

                    if (_tile_Inspector.IsUnionCube())
                    {
                        Iso2DObject _Union = _tile_Inspector.GetSideObject(Iso2DObject.Type.Side_Union);
                        if (_Union != null)
                        {
                            SpriteRenderer sprr = _Union.sprr;
                            _DescRectDivision = _Rect.Division(
                                new float[] { 0.25f / 8f, 2.75f / 8f, 1 / 8f, 3f / 8f, 1f / 8f }, null);
                            _SubRects = _DescRectDivision[1].Division(null, new float[] { 0.05f, 0.35f, 0.6f });
                            EditorGUI.BeginDisabledGroup(_tile_Inspector && _tile_Inspector.tileSetSprites);
                            Union_Field(_SubRects[1], "Union Sprite", _Union, Handles.selectedColor);
                            EditorGUI.EndDisabledGroup();
                            Util.CustomEditorGUI.DrawSprite(_SubRects[2].Division(new float[] { 0.25f, 0.5f }, null)[1],
                                IsoMap.gui_IsoTile_Union_OutlineImage, Color.clear, true, false);
                            DrawTileSprite(_DescRectDivision[3]);
                        }
                    }
                    else
                    {
                        _DescRectDivision = _Rect.Division(
                            new float[] { 0.5f / 8f, 2.5f / 8f, 1f / 8f, 3 / 8f, 1f / 8f }, null);

                        _SubRects = _DescRectDivision[1].Division(null,
                            new float[] { 2 / 10f, 2 / 10f, 2 / 10f, 2 / 10f, 2 / 10f });

                        _Rect = _DescRectDivision[3].Division(new float[] { 0.1f, 0.8f }, new float[] { 0.2f, 0.8f })[3];

                        Iso2DObject _Iso2D_x = Side_Field(_SubRects[0], Iso2DObject.Type.Side_X, Handles.xAxisColor, IsoMap.Prefab_Side_X);
                        Iso2DObject _Iso2D_y = Side_Field(_SubRects[2], Iso2DObject.Type.Side_Y, Handles.yAxisColor, IsoMap.Prefab_Side_Y);
                        Iso2DObject _Iso2D_z = Side_Field(_SubRects[4], Iso2DObject.Type.Side_Z, Handles.zAxisColor, IsoMap.Prefab_Side_Z);

                        if (_Iso2D_x == null && _Iso2D_y == null && _Iso2D_z == null)
                            Util.CustomEditorGUI.DrawSprite(_Rect, IsoMap.gui_IsoTile_Side_OutlineImage, Color.clear, false, false);


                        //CustomEditorGUI.DrawSideSprite(_Rect, _Iso2D_y, false, false);
                        CustomEditorGUI.DrawSideSprite(_Rect, _Iso2D_x, false, false);
                        CustomEditorGUI.DrawSideSprite(_Rect, _Iso2D_z, false, false);
                        DrawTileSprite(_DescRectDivision[3]);
                    }
                }
            }
            EditorGUILayout.Separator();
        }

        void DrawTileSprite(Rect rt)
        {
            Texture2D _tex = null;
            bool bTileSet = _tile_Inspector.tileSetSprites && _tile_Inspector.tileSetSprites.LookupedSubTileSet;
            if (_tile_Inspector && bTileSet)
            {
                _tex = _tile_Inspector.tileSetSprites.LookupedSubTileSet.bakedTileSetImg;
            }

            if (_tex)
                GUI.DrawTexture(rt, _tex, ScaleMode.ScaleToFit);
            else
                tileIcon.DrawRect(rt);

            if (bTileSet)
            {
                GUIContent label = new GUIContent("TileSet Driven Sprite");
                float fWidth = GUI.skin.label.CalcSize(label).x;
                Rect rt_tileSetDesc = new Rect(rt.xMin + (rt.width - fWidth) * 0.5f, rt.yMax - EditorGUIUtility.singleLineHeight, fWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rt_tileSetDesc, label);
            }
        }

        Iso2DObject Side_Field(Rect _Rect, Iso2DObject.Type _sideType, Color _color, GameObject _prefab)
        {
            Iso2DObject _obj = _tile_Inspector.GetSideObject(_sideType);
            bool _bToggle = _obj != null;
            _Rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginChangeCheck();
            _bToggle = EditorGUI.ToggleLeft(_Rect, _sideType.ToString(), _bToggle);
            if (EditorGUI.EndChangeCheck())
            {
                for(int i = 0 ; i < Selection.gameObjects.Length; ++i)
                {
                    if (Selection.gameObjects[i] == null)
                        continue;
                    IsoTile _t = Selection.gameObjects[i].GetComponent<IsoTile>();
                    if (_t != null && !_t.IsUnionCube())
                        _t.Toggle_Side(_bToggle, _sideType);
                }
            }

            List<Iso2DObject> _lookupList = Iso2DObject.GetSideListOfTileSelection(_sideType);
            
            if (_tile_Scene && _bToggle && _lookupList.Count > 0)
            {
                if (_obj == null)
                    _obj = _tile_Scene.GetSideObject(_sideType);
                _Rect.y += _Rect.height;
                CustomEditorGUI.Undo_Iso2DSpriteField(_Rect, _obj.sprr.sprite, _lookupList, _color);
            }
            return _obj;
        }
        
        void Union_Field(Rect _Rect, string _MSG, Iso2DObject _obj, Color _color)
        {
            _Rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(_Rect, _MSG);
            _Rect.y += _Rect.height;
            CustomEditorGUI.Undo_Iso2DSpriteField(_Rect, _obj.sprr.sprite, 
                Iso2DObject.GetSideListOfTileSelection(Iso2DObject.Type.Side_Union), _color);
        }
        static bool createAttachmentBTN(Color color, GameObject prefab, GUIContent content)
        {
            if (prefab != null)
            {
                using (new GUIBackgroundColorScope(color))
                {
                    GameObject newAttachment =  CustomEditorGUI.Undo_TileDeco_Instantiate_DoAll(prefab, content, true);
                    if (newAttachment != null)
                    {
                        var _tile = newAttachment.GetComponentInParent<IsoTile>();
                        _tile.SyncIsoLight(newAttachment);
                    }
                    return true;
                }
            }
            return false;
        }
        bool colliderBTN(Color color, bool bFlag, bool bTrigger, string msg)
        {
            using (new GUIBackgroundColorScope(color))
            {
                if (GUILayout.Button(msg))
                {
                    bool bChanged = false;
                    foreach (var _targetObject in targets)
                    {
                        IsoTile _tile = (IsoTile)_targetObject;
                        if (_tile == null)
                            continue;
                        bChanged |= colliderToggle(_tile.gameObject, bFlag, bTrigger);
                    }
                    return bChanged;
                }
            }
            return false;
        }
        bool colliderToggle(GameObject go, bool bFlag, bool bTrigger)
        {
            if (go != null)
            {
                var colliders = go.GetComponentsInChildren<Collider>();
                if (colliders != null && colliders.Length > 0)
                {
                    var targetList = colliders.Where(r => bTrigger == r.isTrigger && r.enabled != bFlag).ToArray();
                    if (targetList != null && targetList.Length > 0)
                    {
                        Undo.RecordObjects(targetList, "IsoTile: Collider Toggle");
                        foreach (var col in targetList)
                            col.enabled = bFlag;
                    }
                    return true;
                }
            }
            return false;
        }
        void Inspector_Collider()
        {
            bHideCCMenu = CustomEditorGUI.LableWithToggle("[Collider Control]", EditorStyles.boldLabel, bHideCCMenu, "Hide");

            if (bHideCCMenu)
                return;

            bool bUpdate = false;
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Physical Collider");
                bUpdate |= colliderBTN(CustomEditorGUI.Color_Overlay, true, false, "Enable");
                bUpdate |= colliderBTN(CustomEditorGUI.Color_LightRed, false, false, "Disable");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Trigger Collider");
                bUpdate |= colliderBTN(CustomEditorGUI.Color_Overlay, true, true, "Enable");
                bUpdate |= colliderBTN(CustomEditorGUI.Color_LightRed, false, true, "Disable");
            }

            if (bUpdate)
                childInfoUpdate();

            EditorGUILayout.Separator();
            EditorGUI.indentLevel = 0;
        }
        void Inspector_Attached()
        {
            bool bUpdate = false;
            EditorGUI.indentLevel++;
            if (bShowCreateAttachmentBtn = EditorGUILayout.Foldout(bShowCreateAttachmentBtn, "Create Attachment! (each button has a tooltip)"))
            {
                CustomEditorGUI.DrawSeperator();
                EditorGUILayout.LabelField("In Auto ISO, SortingOrder = Tile SO + Overlay Child Index");
                bUpdate |= createAttachmentBTN(CustomEditorGUI.Color_Overlay, IsoMap.Prefab_Overlay, 
                    new GUIContent("Overlay", "This is for tile decoration, \nthere is no collider."));

                CustomEditorGUI.DrawSeperator();
                EditorGUILayout.LabelField("In Auto ISO, SortingOrder = Position-based calculation of RC");
                using (new EditorGUILayout.HorizontalScope())
                {
                    bUpdate |= createAttachmentBTN(CustomEditorGUI.Color_Trigger, IsoMap.Prefab_TriggerPlane, 
                        new GUIContent("Trigger-IsoPlane", "This is throughtable object on the tile, \nhas trigger collider."));
                    bUpdate |= createAttachmentBTN(CustomEditorGUI.Color_Trigger, IsoMap.Prefab_TriggerCube, 
                        new GUIContent("Trigger-Cube", "This is throughtable object on the tile, \nhas trigger collider."));
                    bUpdate |= createAttachmentBTN(CustomEditorGUI.Color_Obstacle, IsoMap.Prefab_Obstacle, 
                        new GUIContent("Obstacle", "This is an obstacle on the tile, \nthere is a physical collider."));
                }
                CustomEditorGUI.DrawSeperator();
                if (bUpdate)
                    childInfoUpdate();
            }
            EditorGUI.indentLevel--;

            CustomEditorGUI.AttachmentHierarchyField(serializedObject, "_attachedList");
        }

        // now Preview is not coming out. Disable until you fix the problem
        //public override bool HasPreviewGUI ()
        //{
        //    if (bPrefab)
        //        return false;

        //    return !EditorApplication.isPlayingOrWillChangePlaymode;
        //}

        public override GUIContent GetPreviewTitle ()
        {
            return new GUIContent ("Iso2DObject");
        }

        public override void OnPreviewSettings ()
        {
            if (bPrefab)
                return;

            GUIStyle preLabel = new GUIStyle ("preLabel");            

            GUILayout.Label (target.name, preLabel);
            previewInit();         
        }

        public override void OnPreviewGUI (Rect r, GUIStyle background)
        {
            if (bPrefab || previewRenderUtility == null)
                return;

            if (!(r.width > 0 && r.height > 0))
                return;

            var previewCamera = PreviewCam;

            previewRenderUtility.BeginPreview (r, background);
            var drag = Vector2.zero;
            if (Event.current.type == EventType.MouseDrag 
                && r.Contains(Event.current.mousePosition)) {
                drag = Event.current.delta;
                if (drag != Vector2.zero)
                {
                    drag *= 0.02f;
                    Vector3 delta = previewCamera.transform.up * drag.y + previewCamera.transform.right * drag.x;
                    previewCamera.transform.Translate(delta);
                }
            }

            GUI.Label(r, previewCamera.transform.position + ", " + previewCamera.transform.eulerAngles + "\n" 
                + Camera.main.transform.position + ", " + Camera.main.transform.eulerAngles + "\n"
                + previewCamera.cullingMask + ", " + (1 << previewObject.layer));
            

            if (IsEditorMode && previewObject != null)
            {
                previewObject.gameObject.SetActive(true);
                previewCamera.Render ();
                previewObject.gameObject.SetActive(false);
            }
            else
            {
                previewCamera.Render ();
            }

            Texture image = previewRenderUtility.EndPreview();
            GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false); // draw the RenderTexture in the ObjectPreview pane

            if (drag != Vector2.zero)
                Repaint();
        }
        bool IsEditorMode
        {
            get{
                return !EditorApplication.isPlayingOrWillChangePlaymode 
                    && !EditorApplication.isCompiling
                    && !EditorApplication.isPlaying
                    && !EditorApplication.isUpdating;
            }
        }
        void previewInit()
        {
            if (bPrefab)
                return;

            if (previewRenderUtility != null)
                return;

            if (previewLayer == 0)
            {
                var flags = BindingFlags.Static | BindingFlags.NonPublic;
                var propInfo = typeof(Camera).GetProperty("PreviewCullingLayer", flags);
                previewLayer = (int)propInfo.GetValue(null, new object[0]);
            }

            GameObject targetObj = _tile_Inspector.gameObject;
            if (targetObj.layer == previewLayer)
                return;

            previewRenderUtility = new PreviewRenderUtility (true);
            Camera previewCam = PreviewCam;
            previewCam.backgroundColor = Color.black;
            previewCam.orthographic = true;
            previewCam.orthographicSize = Camera.main.orthographicSize * 0.5f;
            previewCam.farClipPlane = 50;
            previewCam.nearClipPlane = 0.1f;

            Bounds bounds;       
            
            if (IsEditorMode && _tile_Inspector.gameObject.activeSelf)
            {   
                IsoTile _isoTile = Editor.Instantiate (_tile_Inspector);
                previewObject = _isoTile.gameObject;
                previewObject.transform.position = Vector3.zero;
                _isoTile.Copycat(_tile_Inspector.GetComponent<IsoTile>(), true, true, false);
                var isoLightRecivers = _isoTile.GetComponentsInChildren<IsoLightReciver>();
                foreach (var reciver in isoLightRecivers)
                    DestroyImmediate(reciver);
                previewObject.hideFlags = HideFlags.HideAndDontSave;
                previewObject.SetActive (false);

                previewObject.layer = previewLayer;
                foreach (Transform _transform in previewObject.transform.GetComponentsInChildren<Transform>())
                {
                    _transform.gameObject.layer = previewLayer;
                }
                previewCam.cullingMask = 1 << previewLayer;

                targetObj = previewObject;
            }

            bounds = new Bounds (targetObj.transform.position, Vector3.zero);
            foreach (var renderer in targetObj.GetComponentsInChildren<Renderer>()) {
                bounds.Encapsulate (renderer.bounds);
            }

            float distance = Util.CustomEditorGUI.FrameBounds(previewCam, bounds);
            previewCam.transform.eulerAngles = IsoMap.instance.TileAngle;
            previewCam.transform.position = bounds.center - previewCam.transform.forward * distance;
        }
        void clearPreviewInstance()
        {
            if (previewObject != null)
            {
                DestroyImmediate(previewObject.gameObject, true);
                previewObject = null;
            }               
            if (previewRenderUtility != null)
            {
                previewRenderUtility.Cleanup ();
                previewRenderUtility = null;
            }
        }

    }
}