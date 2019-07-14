using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
	using Isometric;	
    public partial class CustomEditorGUI
	{
#if UNITY_EDITOR

        public static Color Iso2DTypeColor(Isometric.Iso2DObject.Type _type)
        {
            switch (_type)
            {
                case Iso2DObject.Type.Obstacle:
                    return CustomEditorGUI.Color_Obstacle;
                case Iso2DObject.Type.Overlay:
                    return CustomEditorGUI.Color_Overlay;
                case Iso2DObject.Type.Side_Union:
                case Iso2DObject.Type.Side_X:
                case Iso2DObject.Type.Side_Y:
                case Iso2DObject.Type.Side_Z:
                    return CustomEditorGUI.Color_Tile;
                default:
                    return CustomEditorGUI.Color_Side;
            }
        }

        public static bool CAUTION_Foldout(Rect rt, bool bFoldout, string msg)
        {
            var sub_rts = rt.Division(new float[] { 0.25f, 1f }, null);

            EditorGUI.DrawRect(rt, Color_LightYellow);
            Color col_BG = GUI.backgroundColor;
            Color col_con = GUI.contentColor;
            Color col = GUI.color;

            GUI.backgroundColor = GUI.contentColor = GUI.color = Color.black;

            EditorGUI.LabelField(sub_rts[0], "CAUTION!", EditorStyles.boldLabel);
            bFoldout = EditorGUI.Foldout(sub_rts[1], bFoldout, msg);

            GUI.backgroundColor = col_BG;
            GUI.contentColor = col_con;
            GUI.color = col;

            return bFoldout;
        }

        public static void Iso2DObjectField(SerializedObject _Iso2DSerializedObject)
        {
            _Iso2DSerializedObject.Update();

            SerializedProperty vScaler = _Iso2DSerializedObject.FindProperty("localScale");
            SerializedProperty vRotator = _Iso2DSerializedObject.FindProperty("localRotation");
            
            Iso2DObject _Iso2D = (Iso2DObject)_Iso2DSerializedObject.targetObject;
            IsoTile _parentTile = _Iso2D.GetComponentInParent<IsoTile>();
            SpriteRenderer sprr = _Iso2D.GetComponent<SpriteRenderer>();

            //_Iso2D._Type = (Iso2DObject.Type) EditorGUILayout.EnumPopup("Type", _Iso2D._Type);

            bool bUpdateScale = false;
            bool bToggledApplyGlobalPPU = false;
            EditorGUI.indentLevel = 0;
            bUpdateScale = Undo_Iso2DSpriteField(_Iso2D, Color.cyan);

            if (sprr.sprite != null)
            {
                EditorGUILayout.LabelField("Type : " + _Iso2D._Type);

                float iWidth = EditorGUIUtility.currentViewWidth / 2 - 4;

                EditorGUILayout.BeginHorizontal();
                using (new EditorGUILayout.VerticalScope(
                        GUILayout.MaxWidth(iWidth)))
                {
                    GUILayout.Space(5);

                    Iso2DObjectEditor.Max_Slider = Mathf.Max(new float[]{1f, vScaler.vector3Value.x, vScaler.vector3Value.y,
                        EditorGUILayout.FloatField("Cap of Scale Slider", Iso2DObjectEditor.Max_Slider)});

                    vScaler.vector3Value = Vector3Slider(vScaler.vector3Value, Vector3.one, "[Scale]",
                        -Iso2DObjectEditor.Max_Slider * Vector3.one, Iso2DObjectEditor.Max_Slider * Vector3.one, iWidth);
                    // vScaler.vector3Value = EditorGUILayout.Vector3Field("",vScaler.vector3Value, GUILayout.MaxWidth(iWidth));
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Flip", GUILayout.MaxWidth(iWidth * 0.3f));
                        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(iWidth * 0.3f)))
                            vScaler.vector3Value = Vector3.Scale(vScaler.vector3Value, new Vector3(-1, 1, 1));
                        if (GUILayout.Button("Y", EditorStyles.miniButton, GUILayout.MaxWidth(iWidth * 0.3f)))
                            vScaler.vector3Value = Vector3.Scale(vScaler.vector3Value, new Vector3(1, -1, 1));
                    }

                    if (!IsoMap.IsNull)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.ToggleLeft(
                            string.Format("Use Global PPU Scale(x{0:0.00})", _Iso2D.PPURefScale),
                            _Iso2D.bApplyPPUScale, GUILayout.MaxWidth(iWidth));
                        bUpdateScale |= bToggledApplyGlobalPPU = EditorGUI.EndChangeCheck();

                        EditorGUI.indentLevel++;
                        EditorGUILayout.HelpBox("Global PPU Scale = Source PPU / Ref PPU", MessageType.None);
                        EditorGUILayout.LabelField(
                            "Image Source PPU " + sprr.sprite.pixelsPerUnit,
                            GUILayout.MaxWidth(iWidth));
                        EditorGUILayout.LabelField(
                            "IsoMap Reference PPU " + IsoMap.instance.ReferencePPU,
                            GUILayout.MaxWidth(iWidth));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Separator();
                    }

                    Util.CustomEditorGUI.NewParagraph("[Rotation]");
                    EditorGUILayout.LabelField("Tile local rotation adjustment", GUILayout.MaxWidth(iWidth));
                    vRotator.vector3Value = EditorGUILayout.Vector3Field("", vRotator.vector3Value, GUILayout.MaxWidth(iWidth));
                    EditorGUILayout.LabelField(
                        string.Format("+ global tile rotation(X {0}, Y {1})",
                            _Iso2D.IsometricAngle.x, _Iso2D.IsometricAngle.y), GUILayout.MaxWidth(iWidth));
                    EditorGUILayout.Separator();

                    //EditorGUILayout.EndVertical();
                    //GUILayout.EndArea();         
                }
                drawPackedTexture(_Iso2D, Mathf.Min(125f, iWidth * 0.75f));
                EditorGUILayout.EndHorizontal();
            }
            else
                EditorGUILayout.HelpBox("There no Sprite.", MessageType.None);


            if (_parentTile != null && _Iso2D.gameObject != _parentTile.gameObject)
            {
                EditorGUILayout.Separator();
                Util.CustomEditorGUI.NewParagraph("[Object Selector]");
                if (_Iso2D.RC != null)
                    Util.CustomEditorGUI.ComSelector<RegularCollider>(_Iso2D.RC, "GO Controller");
                else
                    CustomEditorGUI.ComSelector<SubColliderHelper>(_Iso2D.SC, "GO SubCollider");
                Util.CustomEditorGUI.ComSelector<IsoTile>(_parentTile, "GO IsoTile");
            }

            _Iso2DSerializedObject.ApplyModifiedProperties();

            if (bToggledApplyGlobalPPU)
            {
                _Iso2D.Toggle_ApplyPPUScale();
            }

            if (bUpdateScale)
            { 
                _Iso2D.AdjustScale();
            }
        }   
        static void drawPackedTexture(Iso2DObject _Iso2D, float _fMaxWidth)
        {
            if (_Iso2D == null)
                return;

            // Vector2 GUIPoint = EditorGUIUtility.GUIToScreenPoint(new Vector2(_rt.x, _rt.y));
            // Rect __rt = new Rect(_rt.xMin + GUIPoint.x, _rt.yMin + GUIPoint.y, _rt.xMax + GUIPoint.x, _rt.yMax + GUIPoint.y);

            // EditorGUI.DrawRect(__rt, Color.gray);
            EditorGUILayout.BeginVertical();
            //GUILayout.BeginArea(_rt);
            EditorGUI.indentLevel = 0;
            
            EditorGUILayout.LabelField("[Sprite]", EditorStyles.boldLabel, GUILayout.MaxWidth(_fMaxWidth));
            GUILayoutOption[] _options = new GUILayoutOption[]{
                    GUILayout.MinWidth(20), GUILayout.Width(_fMaxWidth),
                    GUILayout.MinHeight(20), GUILayout.Height(_fMaxWidth)};
            DrawSideSprite(EditorGUILayout.GetControlRect(_options), _Iso2D, false, false);
            EditorGUILayout.Separator();

            Util.CustomEditorGUI.ShowPackInfo(_Iso2D.sprr.sprite, GUILayout.MaxWidth(_fMaxWidth));
            EditorGUILayout.Separator();
            //GUILayout.EndArea();
            EditorGUILayout.EndVertical();
        }

        #region Attachment
        public static void AttachmentHierarchyField(SerializedObject _root, string _dataPath)
        {
            SerializedProperty attachedHierarchy = _root.FindProperty(_dataPath);
            AttachmentListDraw(attachedHierarchy);
        }
        static void AttachmentListDraw(SerializedProperty attachedHierarchy)
        {
            if (attachedHierarchy == null)
                return;

            attachedHierarchy.serializedObject.Update();

            SerializedProperty bFoldout = attachedHierarchy.FindPropertyRelative("bFoldout");
            SerializedProperty childList = attachedHierarchy.FindPropertyRelative("childList");

            EditorGUI.indentLevel++;
            if (childList.arraySize > 0)
            {
                if (bFoldout.boolValue = EditorGUILayout.Foldout(bFoldout.boolValue, "Attchment List", true))
                {
                    for (int i = 0 ; i < childList.arraySize; ++i)  
                    {
                        using (new EditorGUIIndentLevelScope())
                        {
                            AttachmentDraw(childList.FindPropertyRelative(string.Format("Array.data[{0}]", i)));
                        }
                    }
                }
            }

            attachedHierarchy.serializedObject.ApplyModifiedProperties();
        }
        static void AttachmentDraw(SerializedProperty attachment)
        {
            if (attachment == null)
                return;

            SerializedProperty attachedObj = attachment.FindPropertyRelative("AttachedObj");
            SerializedProperty indentLevel = attachment.FindPropertyRelative("indentLevel");

            int indentLVBackup = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel.intValue;  
            EditorGUILayout.PropertyField(attachedObj);
            EditorGUI.indentLevel = indentLVBackup;
        }

        public static void AttachmentListDraw<T>(Object owner, AttachmentHierarchy<T> attachedHierarchy, ref Vector2 vScrollPos) 
            where T : class, IGameObject, IAttachment, IFloatValue, new()
        {
            if (attachedHierarchy == null)
                return;

            if (attachedHierarchy.childList.Count > 0)
            {
                if (attachedHierarchy.bFoldout = EditorGUILayout.Foldout(attachedHierarchy.bFoldout, "Attchment List", true))
                {
                    using (var scroll = new EditorGUILayout.ScrollViewScope(vScrollPos))
                    {
                        vScrollPos = scroll.scrollPosition;

                        using (new EditorGUIIndentLevelScope())
                        {
                            EditorGUI.indentLevel++;
                            for (int i = 0; i < attachedHierarchy.childList.Count; ++i)
                            {
                                AttachmentDraw(owner, attachedHierarchy.childList[i]);
                            }
                        }
                    }
                }
            }
        }
        static void AttachmentDraw<T>(Object owner, T attachment) where T : class, IGameObject, IAttachment, IFloatValue, new()
        {
            if (attachment == null)
                return;

            int indentLVBackup = EditorGUI.indentLevel;
            EditorGUI.indentLevel = attachment.IndentLevel;
            AttachmentField(owner, attachment);
            EditorGUI.indentLevel = indentLVBackup;
        }
        static void AttachmentField<T>(Object owner, T attachment) where T : class, IGameObject, IAttachment, IFloatValue, new()
        {
            const int cellSize = 44;
            const int fudgeWidth = 175;
            const int border = 2;

            Transform _transform = attachment.gameObject.transform;
            SpriteRenderer _sprr = attachment.gameObject.GetComponent<SpriteRenderer>();
            float _fDepthFudge = attachment.FloatValue;
            int IndentLevel = attachment.IndentLevel;

            Color borderColor = Selection.activeObject == _transform.gameObject 
                ? CustomEditorGUI.Color_LightYellow : Color.gray;

            Rect rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(GUILayout.Height(cellSize + border * 2)));
            Rect rect_inside = new Rect(rect.xMin + border, rect.yMin + border, rect.width - border * 2, rect.height - border * 2);
            Rect rect_preview = new Rect(rect_inside.xMin, rect_inside.yMin, cellSize, rect_inside.height);
            Rect rect_info_name = new Rect(rect_preview.xMax, rect_inside.yMin, rect_inside.width - cellSize - fudgeWidth, rect_inside.height * 0.5f);
            Rect rect_Fudge = new Rect(rect_info_name.xMax, rect_inside.yMin, fudgeWidth, rect_inside.height * 0.5f - border);
            Rect rect_info_Sub = new Rect(rect_info_name.xMin, rect_info_name.yMin + cellSize * 0.5f, rect_info_name.width, rect_inside.height - rect_info_name.height);
            Rect rect_select_reset = new Rect(rect_inside.xMax - cellSize * 3.1f, rect_info_Sub.yMin, cellSize * 1.5f, rect_info_Sub.height);
            Rect rect_select_go = new Rect(rect_inside.xMax - cellSize * 1.5f, rect_info_Sub.yMin, cellSize * 1.5f, rect_info_Sub.height);

            CustomEditorGUI.DrawBordereddRect(rect, borderColor, rect_inside, Color.clear);
            if (_sprr != null)
                CustomEditorGUI.DrawSprite(rect_preview, _sprr.sprite, Color.white, false, false);
            else
                EditorGUI.LabelField(rect_preview, "Trans\nform\nNode");

            int iLv = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUI.LabelField(rect_info_name, _transform.name, EditorStyles.boldLabel);

            float _fNewDepthFudge = CustomEditorGUI.FloatSlider(rect_Fudge, "Depth", _fDepthFudge, -1f, 1f);
            

            // 서브 인포 출력
            //using (new EditorGUILayout.HorizontalScope())
            {
                float _fMinSize = Mathf.Min(rect_info_Sub.width, rect_info_Sub.height);
                SpriteRenderer[] _sprrList = _transform.GetComponentsInChildren<SpriteRenderer>();

                for (int i = 0; i < _sprrList.Length; ++i)
                {
                    if (_sprrList[i].sprite != null && _sprrList[i] != _sprr)
                    {
                        Rect _rt = EditorGUI.IndentedRect(rect_info_Sub);
                        _rt.width = _rt.height = _fMinSize;
                        rect_info_Sub.xMin += _fMinSize;
                        CustomEditorGUI.DrawSprite(_rt, _sprrList[i].sprite, _sprrList[i].color, true, true);
                    }
                }
            }
            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightMagenta))
            {
                if (GUI.Button(rect_select_reset, "Depth 0"))
                {
                    _fNewDepthFudge = 0;
                }
            }

            using (new GUIBackgroundColorScope(Util.CustomEditorGUI.Color_LightYellow))
            {
                if (GUI.Button(rect_select_go, "Select"))
                {
                    Selection.activeGameObject = _transform.gameObject;
                }
            }

            if (_fNewDepthFudge != _fDepthFudge)
            {
                Vector2 _tileAngle = IsoMap.IsNull ? new Vector2(_sprr.transform.localEulerAngles.x, _sprr.transform.localEulerAngles.y) : IsoMap.instance.TileAngle;
                Undo.RecordObject(_transform, "Transform : LocalOffset");
                _transform.position = _transform.position - IsoMap.vDepthFudge(_fDepthFudge, _tileAngle) + IsoMap.vDepthFudge(_fNewDepthFudge, _tileAngle);

                Undo.RecordObject(owner, "DepthFudge changed");
                attachment.FloatValue = _fNewDepthFudge;
            }

            EditorGUI.indentLevel = iLv;
        }
        #endregion Attachment
        
        public static void Iso2DSelector(GameObject _target)
        {
            EditorGUILayout.ObjectField(_target, typeof(Iso2DObject), allowSceneObjects:true);
        }

        static bool Undo_Iso2DSpriteField(Iso2DObject _obj, Color _color)
        {
            bool bChanged = false;
            using (new GUIBackgroundColorScope(_color))
            {
                using (var result = new EditorGUI.ChangeCheckScope())
                {             
                    Sprite _newSprite = (Sprite)EditorGUILayout.ObjectField(_obj.sprr.sprite, 
                        typeof(Sprite), allowSceneObjects: false);
                    if (bChanged = result.changed)
                    {
                        _obj.ChangeSprite(_newSprite);
                        EditorUtility.SetDirty(Selection.activeObject);
                    }
                }
            }
            return bChanged;
        }
        //static void Undo_Iso2DSpriteField(Sprite _origin, List<Iso2DObject> _objs, Color _color, params GUILayoutOption[] _options)
        //{
        //    using (new GUIBackgroundColorScope(_color))
        //    {
        //        using (var result = new EditorGUI.ChangeCheckScope())
        //        {             
        //            Sprite _newSprite = (Sprite)EditorGUILayout.ObjectField(_origin, 
        //                typeof(Sprite), allowSceneObjects: false, options:_options);
        //            if (result.changed)
        //            {
        //                Undo_Iso2DSprite(_objs, _newSprite);
        //                EditorUtility.SetDirty(Selection.activeObject);
        //            }
        //        }
        //    }
        //}
        public static void Undo_Iso2DSpriteField(Rect _rect, Sprite _origin, List<Iso2DObject> _objs, Color _color)
        {
            _rect.x += 5;
            using (new GUIBackgroundColorScope(_color))
            {
                using (var result = new EditorGUI.ChangeCheckScope())
                {
                    Sprite _newSprite = (Sprite) EditorGUI.ObjectField(_rect, _origin,
                        typeof(Sprite), allowSceneObjects: false);
                    if (result.changed)
                    {
                        Undo_Iso2DSprite(_objs, _newSprite);
                        EditorUtility.SetDirty(Selection.activeObject);
                    }
                }
            }
        }
        public static void Undo_Iso2DSprite(List<Iso2DObject> _objs, Sprite _newSprite)
        {
            foreach(Iso2DObject _obj in _objs)
            {
                if (_obj != null)
                    _obj.ChangeSprite(_newSprite);
            }
        }

        public static void DrawSideSprite(Rect _FullRect, Iso2DObject _Iso2D, 
            bool _bSquare, bool _bSimpleDraw)
        {
            if (_Iso2D == null || _Iso2D.sprr.sprite == null)
                return;

            Rect _rt = _FullRect.Divid_TileSide(_Iso2D._Type);
            if (_Iso2D.transform.localScale.x < 0)
            {
                _rt.x += _rt.width;
                _rt.width *= -1f;
            }
            if (_Iso2D.transform.localScale.y < 0)
            {
                _rt.y += _rt.height;
                _rt.height *= -1f;
            }
            Util.CustomEditorGUI.DrawSprite(_rt, _Iso2D.sprr.sprite, _Iso2D.sprr.color , _bSquare, _bSimpleDraw);
        }
        public static void DrawSideSprite(Rect _FullRect, Sprite _sprite, Color _color, Iso2DObject.Type _side, 
            bool _bSquare, bool _bSimpleDraw)
        {
            // Rect _rt = _sprite.textureRectOffset.Equals(Vector2.zero) 
            //     ? _FullRect.Divid_TileSide(_side) : _FullRect;
            Rect _rt = _FullRect.Divid_TileSide(_side);
            Util.CustomEditorGUI.DrawSprite(_rt, _sprite, _color, _bSquare, _bSimpleDraw);
        }
        
        public static GameObject Undo_TileDeco_Instantiate_DoAll(GameObject _prefab, GUIContent content, bool _buttonAction)
        {
            GameObject _go = null;
            if (!_buttonAction || GUILayout.Button(content))
            {
                IsoTile _tile;
                RegularCollider _rc;
                for (int i = 0; i < Selection.gameObjects.Length; ++i)
                {
                    _tile = Selection.gameObjects[i].GetComponent<IsoTile>();
                    if (_tile != null)
                    {
                        _go = Undo_Instantiate(_prefab, Selection.gameObjects[i].transform, content.text);
                        if (_go != null)
                        {
                            if ((_rc = _go.GetComponent<RegularCollider>()) != null)
                            {
                                _rc.Toggle_UseGridTileScale(_tile.bAutoFit_ColliderScale);
                                _rc.BC.DropToFloor(_go);
                            }

                            if (_tile.bAutoFit_SpriteSize)
                            {
                                foreach (var _iso2D in _go.GetComponentsInChildren<Iso2DObject>())
                                {
                                    if (_iso2D != null)
                                        _tile.Update_Attached_Iso2DScale(_iso2D, true, content.text);
                                }
                            }
                        }
                    }
                }
                return _go;
            }
            return _go;
        }

        public static bool ObjectListField(IList list, System.Type type, string message, bool bFoldout, bool bReadOnly, ref Vector2 scrollPos)
        {
            EditorGUILayout.Separator();
            if (bFoldout = EditorGUILayout.Foldout(bFoldout, string.Format("{0}({1})", message, list.Count)))
            {
                using (var result = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    using (new EditorGUI.DisabledGroupScope(bReadOnly))
                    {
                        scrollPos = result.scrollPosition;
                        EditorGUI.indentLevel++;
                        foreach (var one in list)
                            EditorGUILayout.ObjectField(one as Object, typeof(IsoLightReciver), allowSceneObjects: true);
                        EditorGUI.indentLevel--;
                    }
                }
            }
            return bFoldout;
        }        

        static bool bFoldout_ColliderControlGUI = false;
        public static void ColliderControlHelperGUI(TmpTexture2D tmpTexture2D, Object[] objects)
        {
            SubColliderHelper[] _cols = objects.Where(r => r is SubColliderHelper).Cast<SubColliderHelper>().ToArray();

            bool bHasBC = _cols.All(r => r.GetComponent<BoxCollider>() != null);
            bool isNotIsoTile = _cols.All(r => r.GetComponent<IsoTile>() == null);

            if (isNotIsoTile)
            {
                Util.CustomEditorGUI.NewParagraph("[Object Selector]");
                Util.CustomEditorGUI.ComSelector<IsoTile>(Selection.activeGameObject.GetComponentInParent<IsoTile>(), "GO IsoTile");
                var _iso2Ds = Selection.activeGameObject.GetComponentsInChildren<Iso2DObject>();
                foreach (var one in _iso2Ds)
                    Util.CustomEditorGUI.ComSelector<Iso2DObject>(one, "GO Iso2D");
                CustomEditorGUI.DrawSeperator();
            }

            if (bHasBC || isNotIsoTile)
            {
                NewParagraph("[Collider Control]");
                float fRayCastHeight = 10f;
                if (bHasBC && GUILayout.Button(string.Format("Drop to the floor collider[Max({0:0.0})]", fRayCastHeight)))
                {
                    foreach (var _col in _cols)
                    {
                        if (_col.BC != null)
                        {
                            float fDropHeight = _col.BC.DropToFloor(_col.gameObject, fRayCastHeight);
                            if (fDropHeight > 0)
                                Debug.Log(string.Format("{0} fell {1} high.", _col.gameObject.name, fDropHeight));
                            else if (fDropHeight < 0)
                                Debug.Log(string.Format("{0} went up to {1} height because there is no floor within {2} range.", 
                                    _col.gameObject.name, -fDropHeight, fRayCastHeight));
                            else
                                Debug.Log(string.Format("There is no floor within {0} height.", fRayCastHeight));
                        }
                    }
                }
                if (isNotIsoTile && GUILayout.Button(string.Format("Change Parent to the nearest tile.", fRayCastHeight)))
                {
                    foreach (var _col in _cols)
                        _col.ReParent(0.5f);
                }
                if (isNotIsoTile && GUILayout.Button("Sync Light to Tile."))
                {
                    foreach (var _col in _cols)
                    {
                        var _tile = _col.GetComponentInParent<IsoTile>();
                        _tile.SyncIsoLight(_col.gameObject);
                    }
                }
                CustomEditorGUI.DrawSeperator();
            }

            if (isNotIsoTile && (bFoldout_ColliderControlGUI = !NewParagraphWithHideToggle("[Etc Control]", "Hide", !bFoldout_ColliderControlGUI)))
            {
                List<Iso2DObject> Iso2Ds = new List<Iso2DObject>();
                foreach (var _col in _cols)
                    Iso2Ds.AddRange(_col.GetComponentsInChildren<Iso2DObject>());
                if (Iso2Ds.Count > 0)
                {
                    Iso2Ds = Iso2Ds.Distinct().ToList();
                    GUILayoutOption heightLayout = GUILayout.Height(EditorGUIUtility.singleLineHeight * 3);
                    Iso2Ds.ForEach(r => Iso2DDrawer.Drawer(EditorGUILayout.GetControlRect(heightLayout), r.gameObject, tmpTexture2D));
                }
            }
        }

        public delegate void SimpleAction();
        public static void Button(bool bEnable, Color color, string name, SimpleAction action, params GUILayoutOption[] options)
        {
            button_action(bEnable, color, () =>
            {
                if (GUILayout.Button(name, options))
                    action();
            });
        }
        public static void Button(Rect rect, bool bEnable, Color color, string name, SimpleAction action)
        {
            button_action(bEnable, color, () =>
            {
                if (GUI.Button(rect, name))
                    action();
            });
        }
        static void button_action(bool bEnable, Color col, SimpleAction action)
        {
            using (new GUIBackgroundColorScope(col))
            {
                EditorGUI.BeginDisabledGroup(!bEnable);
                action();
                EditorGUI.EndDisabledGroup();
            }
        }

        public static bool LableWithToggle(string labelText, GUIStyle style, bool bToggle, string bToggleText)
        {
            GUIContent label = new GUIContent(labelText);
            float fWidth = style.CalcSize(label).x;

            Rect rt = EditorGUILayout.GetControlRect();
            Rect rt_label = new Rect(rt.position, new Vector2(fWidth, rt.height));
            Rect rt_Toggle = new Rect(rt_label.xMax, rt.yMin, rt.width - fWidth, rt.height);

            EditorGUI.LabelField(rt_label, label, style);
            return EditorGUI.ToggleLeft(rt_Toggle, bToggleText, bToggle);
        }

        public static void Label(GUIStyle style, string text, TextAnchor alignment, bool bFitWidth = true, params GUILayoutOption[] options)
        {
            GUIContent content = new GUIContent(text);
            if (bFitWidth)
                ArrayUtility.Add(ref options, GUILayout.Width(style.CalcSize(content).x));

            TextAnchor textAnchor = style.alignment;
            style.alignment = alignment;

            bool richText = style.richText;
            style.richText = true;

            GUILayout.Label(content, style, options);

            style.richText = richText;
            style.alignment = textAnchor;
        }

        public static void Label(Rect rt, GUIStyle style, string text, TextAnchor alignment)
        {
            GUIContent content = new GUIContent(text);

            TextAnchor textAnchor = style.alignment;
            style.alignment = alignment;

            bool richText = style.richText;
            style.richText = true;

            GUI.Label(rt, content, style);

            style.richText = richText;
            style.alignment = textAnchor;
        }
#endif
    }
}
