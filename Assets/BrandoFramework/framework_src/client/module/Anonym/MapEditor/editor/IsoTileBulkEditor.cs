using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Isometric
{
    using Util;

    [CustomEditor(typeof(IsoTileBulk))]
    [CanEditMultipleObjects]
    public class IsoTileBulkEditor : Editor
    {
        IsoTileBulk _bulk;
        bool bFoldout = true;
        bool bPrefab = false;
        bool bCollapseOtherItems = false;
        bool bWarningZone = false;
        int iSortingOrderForSelection = 0;
        Grid _bulkGrid;
        Vector2 tileListScroll;
        string filterMsg = "Filter string(Seperator{',', ' '})";
        string filterString;
        string[] filters;
        SerializedProperty _SizeXZ;
        SerializedProperty _RefTile;
        SerializedProperty _bFrozen;
        List<int> filteredChildIndexList = new List<int>();
        List<IsoTileBulk> connectedBulkList = new List<IsoTileBulk>();
        bool undoredo()
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                refresh_Child(true);
                return true;
            }
            return false;
        }
        void refresh_ConnectedBulk()
        {
            connectedBulkList.Clear();
            connectedBulkList.AddRange(_bulk.GetComponentsInParent<IsoTileBulk>());
            connectedBulkList.AddRange(_bulk.GetComponentsInChildren<IsoTileBulk>());
            connectedBulkList.RemoveAll(r => r == _bulk);
        }
        void refresh_Child(bool bForceSPUpdate = false)
        {
            tileListScroll = Vector2.zero;
            filterString = filterMsg;
            if (bForceSPUpdate)
                _bulk.Update_ChildList();
            refresh_FilteredChild(bForceSPUpdate);
        }

        void refresh_FilteredChild(bool bForceSPUpdate = false)
        {
            if (bForceSPUpdate)
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            filteredChildIndexList.Clear();
            bool bAll = filterString.Equals(filterMsg);
            if (!bAll)
                filters = filterString.Split(',', ' ');

            for (int i = 0; i < _bulk._attachedList.Count; ++i)
            {
                if (bAll || SPNameFilterCheck(_bulk._attachedList[i].name, ref filters))
                {   
                    filteredChildIndexList.Add(i);
                }
            }
        }

        void OnEnable()
        {
            _bulk = (IsoTileBulk)target;
            if (bPrefab = PrefabHelper.IsPrefab(_bulk.gameObject))
                return;

            _SizeXZ = serializedObject.FindProperty("SizeXZ");
            _RefTile = serializedObject.FindProperty("_referenceTile");
            _bFrozen = serializedObject.FindProperty("bFrozen");
            _bulkGrid = _bulk.gameObject.GetComponent<Grid>();
            refresh_Child(true);
            refresh_ConnectedBulk();
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

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_bFrozen, new GUIContent("Freeze", "When this option is enabled, all Iso Components belonging to this IsoTileBulk are Unabled. This is different from InActive of GameObject.\n" +
                "Use this to prevent the editor from slowing down when creating large maps. You can manage the map by dividing it into several sections.\nWhen editing is needed, you can turn off the option again."));
            bool isFrozen = _bFrozen.boolValue;
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var one in serializedObject.targetObjects)
                {
                    IsoTileBulk _tb = one as IsoTileBulk;
                    if (_tb != null)
                    {
                        IsoComRemover.SetEnable(!isFrozen, _tb.gameObject);
                    }
                }
                serializedObject.ApplyModifiedProperties();
                
                // Strangely enough, this will change the IsoTileBulk normally.
                foreach (var one in serializedObject.targetObjects.Select(t => t as IsoTileBulk).Where(b => b != null))
                {
                    one.enabled = !isFrozen;
                }

                EditorGUIUtility.ExitGUI();
            }

            if (isFrozen)
            {
                EditorGUILayout.HelpBox("This IsoTileBulk and all the child's Iso Components are frozen. This is a way to prevent the Unity Editor from slowing down. " +
                    "When isometric work is needed, turn off Freeze at the above.", MessageType.Warning);
                return;
            }
            EditorGUILayout.Separator();

            if (!bCollapseOtherItems)
            {
                ShowSelector();

                EditorGUILayout.LabelField("[Bulk Control]", EditorStyles.boldLabel);
                ShowResize();
                ShowBulkControl();
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("[Tile Control]", EditorStyles.boldLabel);
                ShowTileControl();
                EditorGUILayout.Separator();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("[Selection Filter]", EditorStyles.boldLabel);
                bCollapseOtherItems = EditorGUILayout.Toggle("Collapse other items", bCollapseOtherItems);
            }
            ShowTileFilter();
            CustomEditorGUI.DrawSeperator();
            ShowTileFilterControl();
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
			if (PrefabHelper.IsPrefab(_bulk.gameObject))
                return;

            if (_bulkGrid.IsInheritGrid)
                return;
                
            int iOddCount = 9;
            Vector3 _centor = _bulkGrid.transform.position;
            for (int i = 0 ; i < iOddCount; ++i)
            {
                Handles.color = Color.red;
                Handles.CircleHandleCap(iOddCount + i * 3, _centor
                    + new Vector3((i - (iOddCount - 1)/2) * _bulkGrid.GridInterval.x, 0, 0), 
                    SceneView.currentDrawingSceneView.camera.transform.rotation, 0.05f, EventType.Repaint);
                Handles.color = Color.green;
                Handles.CircleHandleCap(iOddCount + i * 3 + 1, _centor
                    + new Vector3(0, (i - (iOddCount - 1)/2) * _bulkGrid.GridInterval.y, 0), 
                    SceneView.currentDrawingSceneView.camera.transform.rotation, 0.05f, EventType.Repaint);
                Handles.color = Color.blue;
                Handles.CircleHandleCap(iOddCount + i * 3 + 2, _centor
                    + new Vector3(0, 0, (i - (iOddCount - 1)/2) * _bulkGrid.GridInterval.z), 
                    SceneView.currentDrawingSceneView.camera.transform.rotation, 0.05f, EventType.Repaint);
            }
        }

        bool SPNameFilterCheck(string targetName, ref string[] filters)
        {
            for (int i = 0; i < filters.Length; ++i)
            {
                if (!string.IsNullOrEmpty(filters[i]) && !targetName.Contains(filters[i]))
                    return false;
            }
            return true;
        }

        private void ShowAdjustButtons(Rect _rt,
            string sL, string s, string sR,
            System.Action _action_L,
            System.Action _action_B,
            System.Action _action_R)
        {

            Rect[] _rectList = _rt.Division(new float[] { 0.35f, 0.3f, 0.35f }, null);
            bool bChanged = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUI.Button(_rectList[0], sL, EditorStyles.miniButtonLeft))
                {
                    bChanged = true;
                    _action_L();
                }
                if (GUI.Button(_rectList[1], s, EditorStyles.miniButtonMid))
                {
                    bChanged = true;
                    _action_B();
                }
                if (GUI.Button(_rectList[2], sR, EditorStyles.miniButtonRight))
                {
                    bChanged = true;
                    _action_R();
                }
            }

            if (bChanged)
            {
                refresh_Child(true);
                EditorUtility.SetDirty(this);
            }
        }

        private void ShowResizeAxis(Rect _rt, string _name, bool _Axis_XTrue_YFalse)
        {
            Rect _newSize = _SizeXZ.rectValue;
            System.Action<bool, bool, bool, int> buttonAction = (bool bAxis, bool bMax, bool bMin, int i) =>
            {
                if (bAxis)
                {
                    if (bMax)
                        _newSize.xMax += i;
                    if (bMin)
                        _newSize.xMin += -i;
                }
                else
                {
                    if (bMax)
                        _newSize.yMax += i;
                    if (bMin)
                        _newSize.yMin += -i;
                }
                _newSize.xMin = Mathf.Min(0, _newSize.xMin);
                _newSize.xMax = Mathf.Max(0, _newSize.xMax);
                _newSize.yMin = Mathf.Min(0, _newSize.yMin);
                _newSize.yMax = Mathf.Max(0, _newSize.yMax);

                _bulk.Resize(_newSize, false);
            };

            using (new EditorGUILayout.VerticalScope())
            {
                Rect[] _rectList = _rt.Division(null, new float[] { 0.3f, 0.35f, 0.35f });
                EditorGUI.LabelField(_rectList[0], _name, EditorStyles.largeLabel);
                ShowAdjustButtons(_rectList[1], "<", "+", ">",
                    () => { buttonAction(_Axis_XTrue_YFalse, false, true, 1); },
                    () => { buttonAction(_Axis_XTrue_YFalse, true, true, 1); },
                    () => { buttonAction(_Axis_XTrue_YFalse, true, false, 1); });
                ShowAdjustButtons(_rectList[2], ">", "-", "<",
                    () => { buttonAction(_Axis_XTrue_YFalse, false, true, -1); },
                    () => { buttonAction(_Axis_XTrue_YFalse, true, true, -1); },
                    () => { buttonAction(_Axis_XTrue_YFalse, true, false, -1); });
            }
        }
        private void ShowBulkControl()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                {
                    if (GUILayout.Button("Duplicate"))
                    {
                        List<GameObject> _selection = new List<GameObject>();
                        for (int i = 0; i < Selection.gameObjects.Length; ++i)
                        {
                            _selection.Add((
                                Selection.gameObjects[i].GetComponent<IsoTileBulk>()).Duplicate().gameObject);
                        }
                        Undo.RecordObjects(Selection.objects, "IsoTileBulk:Dulicate");
                        Selection.objects = _selection.ToArray();
                    }
                    // if (duplicatedObject != null)
                    //     CustomEditorGUI.Iso2DSelector(duplicatedObject);
                }
            }
        }

        private List<T> selection<T>()
        {
            List<T> _selectList = new List<T>();
            for (int i = 0; i < filteredChildIndexList.Count; ++i)
                _selectList.Add((_bulk._attachedList[filteredChildIndexList[i]].GetComponent<T>()));
            return _selectList;
        }

        private void DoToSelection<T>(System.Func<T, bool> Do, string msg)
        {
            var list = selection<T>();
            int count = 0;
            list.ForEach(
                r => 
                {
                    if (Do(r))
                        count++;
                });
            Debug.Log(string.Format("Total {0}/{1} {2} has changed. {3}", count, list.Count, typeof(T).Name, msg));
        }

        private void SelectTiles()
        {
            Selection.objects = selection<IsoTile>().Select(r => r.gameObject).ToArray();
            EditorUtility.SetDirty(this);
        }

        private void ShowSelector()
        {
            EditorGUILayout.LabelField("[Object Selector]", EditorStyles.boldLabel);
            Util.CustomEditorGUI.ComSelector<IsoMap>(IsoMap.instance, "GO IsoMap");
            for(int i = 0 ; i < connectedBulkList.Count; ++i)
                Util.CustomEditorGUI.ComSelector<IsoTileBulk>(connectedBulkList[i], "GO Bulk");
            EditorGUILayout.Separator();
        }

        private void ShowResize()
        {
            using (var view = new EditorGUILayout.HorizontalScope(GUILayout.Height(75)))
            {
                Rect[] _rtList = view.rect.Division(new float[] { 0.3f, 0.05f, 0.3f, 0.05f, 0.3f }, null);

                float fSize;

                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightRed))
                {
                    fSize = _SizeXZ.rectValue.width + 1;
                    ShowResizeAxis(_rtList[0], string.Format("Size X({0})", fSize), true);
                }
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightGreen))
                {
                    fSize = _SizeXZ.rectValue.height + 1;
                    ShowResizeAxis(_rtList[2], string.Format("Size Z({0})", fSize), false);
                }
                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        _rtList = _rtList[4].Division(null, new float[] { 0.15f, 0.25f, 0.05f, 0.25f, 0.05f, 0.25f });
                        if (GUI.Button(_rtList[1], "No Redundancy"))
                        {

                            while (_bulk.NoRedundancy())
                                _bulk.Update_ChildList();
                            refresh_Child(true);
                        }

                        var _rt_Hor = _rtList[3].Division(new float[] { 0.48f, 0.04f, 0.48f }, null);
                        if (GUI.Button(_rt_Hor[0], "Flat"))
                        {
                            _bulk.Flat();
                            refresh_Child(true);
                        }     

                        if (GUI.Button(_rt_Hor[2], "Clear"))
                        {
                            _bulk.Clear();
                            refresh_Child(true);
                        }

                        if (GUI.Button(_rtList[5], "Update All TileSetSprites"))
                        {
                            foreach(var oneBulk in serializedObject.targetObjects.Select(t => t as IsoTileBulk).Where(b => b != null))
                            {
                                IsoTile.UpdateTileSet(oneBulk.GetAllTiles().Where(t => t != null && t.tileSetSprites != null), true, true, "Update All TileSetSprites");
                            }
                        }
                        ;
                    }
                }
            }
        }

        private void ShowTileControl()
        {
            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
            {
                IsoTile _tile = _RefTile.objectReferenceValue == null ? null : ((GameObject)_RefTile.objectReferenceValue).GetComponent<IsoTile>();
                IsoTile _newTile = EditorGUILayout.ObjectField(
                    new GUIContent("Style Reference Tile", "The default style of the tile that is created in this bulk."),
                    _tile, typeof(IsoTile), allowSceneObjects:true) as IsoTile;
                if (_tile != _newTile)
                    _RefTile.objectReferenceValue = _newTile == null ? null : _newTile.gameObject;

                using (new EditorGUILayout.HorizontalScope())
                {
                    string _BtnMsg = _bulk.bYFirstSort ? "Sort Xyz -> Yxz" : "Sort Yxz -> Xyz";
                    if (GUILayout.Button(_BtnMsg))
                    {
                        _bulk.Sort(bToggle: true);
                        refresh_FilteredChild();
                        EditorUtility.SetDirty(this);
                    }

                    if (GUILayout.Button("Rename All!"))
                    {
                        _bulk.Rename();
                        EditorUtility.SetDirty(this);
                    }

                    using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
                    {
                        if (GUILayout.Button("New Tile!"))
                        {
                            _bulk.NewTile(Vector3.zero);
                            refresh_FilteredChild(true);
                            EditorUtility.SetDirty(this);
                        }
                    }
                }
            }
        }

        private void ShowTileFilterControl()
        {
            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightBlue))
            {
                // for future
                //using (new EditorGUILayout.HorizontalScope())
                //{
                //    EditorGUILayout.LabelField("Apply to All ISO of filtered tiles", EditorStyles.boldLabel);
                //    using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightGreen))
                //    {
                //        if (GUILayout.Button("Enable"))
                //        {
                //            DoToSelection<IsometricSortingOrder>(
                //                r => {
                //                    if (r == null || r.enabled)  return false;
                //                    Undo.RecordObject(r, "IsoTileBulk: ISO Enable");
                //                    return r.enabled = true;
                //                },  "These ISOs are Enabled.");
                //        }
                //    }
                //    using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
                //    {
                //        if (GUILayout.Button("Disable"))
                //        {
                //            DoToSelection<IsometricSortingOrder>(
                //                r => {
                //                    if (r == null || !r.enabled)  return false;
                //                    Undo.RecordObject(r, "IsoTileBulk: ISO Disable");
                //                    return !(r.enabled = false);
                //                },  "These ISOs are Disabled.");
                //        }
                //    }
                //}

                if (!IsoMap.IsNull && !IsoMap.instance.bUseIsometricSorting)
                {
                    EditorGUILayout.Separator();
                    bWarningZone = CustomEditorGUI.CAUTION_Foldout(EditorGUILayout.GetControlRect(), 
                        bWarningZone, "Folded functions can not do Undo!");

                    if (bWarningZone)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightRed))
                            {
                                EditorGUILayout.LabelField("Sorting Order", EditorStyles.boldLabel);
                                iSortingOrderForSelection = EditorGUILayout.IntField(iSortingOrderForSelection);
                                if (GUILayout.Button("Apply to Selection"))
                                {
                                    DoToSelection<IsoTile>(
                                        r => {
                                            if (r == null || r.sortingOrder == null)
                                                return false;
                                            r.sortingOrder.Reset_SortingOrder(iSortingOrderForSelection, false);
                                            return true;
                                        }, string.Format("New SortingOrder is {0}.\nThis can not be undone. " +
                                            "When you want to modify it, overwrite it with the new value.", iSortingOrderForSelection));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShowTileFilter()
        {
            // if (Event.current.type != EventType.Layout
            //     && Event.current.type != EventType.Repaint
            //     && Event.current.type != EventType.ScrollWheel)
            //     return;

            using (new EditorGUILayout.HorizontalScope())
            {
                bFoldout = EditorGUILayout.Foldout(bFoldout, 
                    string.Format("Fold({0}/{1})", filteredChildIndexList.Count, _bulk._attachedList.Count), true);
                EditorGUI.BeginChangeCheck();
                filterString = EditorGUILayout.TextField(filterString);
                if (EditorGUI.EndChangeCheck())
                    refresh_FilteredChild();

                using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
                {
                    if (GUILayout.Button("Select!", GUILayout.Width(75)))
                    {
                        SelectTiles();
                    }
                }
            }
            if (bFoldout && _bulk._attachedList.Count > 0)
            {
                float cellHeight = Iso2DDrawer.RectHeight;
                int iMaxRowCount = 18;

                using (var scrolled = new EditorGUILayout.ScrollViewScope(tileListScroll, GUILayout.ExpandHeight(true)))
                {
                    tileListScroll = scrolled.scrollPosition;
                    //if (Event.current.type == EventType.ScrollWheel)
                    //{
                    //    EditorGUIUtility.ExitGUI();
                    //    return;
                    //}

                    int iStart = Mathf.Max(Mathf.FloorToInt(tileListScroll.y / cellHeight) -1 , 0);
                    int iEnd = Mathf.Min(filteredChildIndexList.Count, iStart + iMaxRowCount);
                    // Debug.LogFormat("Tile List : start({0}), end({1})", iStart, iEnd);

                    try
                    {
                        if (iStart > 0)                 
                            EditorGUILayout.GetControlRect(GUILayout.Height(cellHeight * (iStart)));

                        for (int i = iStart; i < iEnd; ++i)
                            EditorGUI.PropertyField(Iso2DDrawer.GetRect(), 
                                serializedObject.FindProperty(
                                    string.Format(@"_attachedList.Array.data[{0}]", 
                                    filteredChildIndexList[i])));
                                    
                        if ((iEnd = filteredChildIndexList.Count - iEnd) > 0)
                            EditorGUILayout.GetControlRect(GUILayout.Height(cellHeight * iEnd));
                    }
                    catch
                    {
                        // Debug.Log(Event.current.type);
                        // There is a bug here. But there is no harm to use. It will be updated next time.
                    }

                }
            }
        }
    }
}