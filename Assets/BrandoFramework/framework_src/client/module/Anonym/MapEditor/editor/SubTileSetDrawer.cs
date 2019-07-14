using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace Anonym.Util
{
    using Isometric;

    [CustomPropertyDrawer(typeof(SubTileSet))]
    public class SubTileSetDrawer : PropertyDrawer 
    {
        // True : baseField, False : outField
        // Type order : Normal = 0, OneWay = 1, TwoWay = 2, ThreeWay = 3, Ex_3_Diagonal = 4, Ex_2_Diagonal_Together = 5, Ex_2_Diagonal_Apart_V = 6, Ex_2_Diagonal_Apart_H = 7, Ex_1_Diagonal = 8, Custom = 10,
        // Direction order : rt rt, rt dr, dr dr, dr d, d d, d dl, dl dl, dl l, l l, l tl, tl tl, tl t, t t, t tr, tr tr, tr t
        static readonly bool[,] extraTileTypes = {
            { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
            { false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false },
            { false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true },
            { false, false, true, false, false, false, true, false, false, false, true, false, false, false, true, false },
            { false, true, true, false, false, false, true, true, false, true, true, false, false, false, true, true },
            { false, false, true, true, false, true, true, false, false, false, true, true, false, true, true, false },
            { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true },
        };

        const int iLayerForPreview = 31;

        SubTileSet target;

        SerializedProperty spTileSetType;
        SerializedProperty spOutfield;

        SerializedProperty spDirectionalFieldType;
        SerializedProperty spDirectionalTile;
        SerializedProperty spDirectionalSprite;
        SerializedProperty spCustomOutField;

        SerializedProperty spIsometricAngle;
        SerializedProperty spIsSimpleView;
        SerializedProperty spPriority;
        SerializedProperty spAllowNullOutfield;

        TmpTexture2D tmpTexture2D = new TmpTexture2D();

        [SerializeField]
        bool bOnDrag = false;
        [SerializeField]
        Vector2 vLastMousePosition = Vector2.zero;

        [SerializeField]
        Sprite cLastBaseSprite = null;

        bool bSpriteChanged = true;
        bool bAngleChanged = false;
        bool isSimpleView { get { return spIsSimpleView != null ? spIsSimpleView.boolValue : false; } }

        public static bool bOffElementLabel = false;

        #region Height_Property
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight + 2 * Height_SingleSeperator + fElementsHeight;

            target = property.objectReferenceValue as SubTileSet;
            if (!target.bFlodOut && !bOffElementLabel)
                return EditorGUIUtility.singleLineHeight;

            if (isSimpleView)
                return Height_SimpleView;

            return Height_Max + fElementsHeight;
        }

        static float fElementsHeight { get { return bOffElementLabel ? 0 : EditorGUIUtility.singleLineHeight; } }
        public static float Height_SimpleView { get { return EditorGUIUtility.singleLineHeight + Height_SingleSeperator * 2 + Height_SubTileSetPreview + fElementsHeight; } }
        static float Height_ObjectField
        {
            get { return EditorGUIUtility.singleLineHeight * 2f + Height_SingleSeperator; }
        }
        static float Height_OutField
        {
            get { return EditorGUIUtility.singleLineHeight * 2f; }
        }
        static float Height_SingleSeperator
        {
            get { return EditorGUIUtility.singleLineHeight * 0.125f; }
        }
        public static float Height_SubTileSetPreview
        {
            get { return 120; }
        }
        static float Height_IsometricAngle
        {
            get { return EditorGUIUtility.singleLineHeight; }
        }
        static float Height_SpriteObjectField
        {
            get { return EditorGUIUtility.singleLineHeight * 6f; }
        }
        public static int Height_Max
        {
            get { return Mathf.CeilToInt(Height_ObjectField + Height_OutField + Height_SubTileSetPreview + Height_SpriteObjectField + Height_IsometricAngle + Height_SingleSeperator * 6); }
        }
        #endregion

        static void DrawSeperator(ref Rect position)
        {
            Rect rt = RectUtil.Sub_Vertical(ref position, Height_SingleSeperator);
            EditorGUI.DrawRect(rt, CustomEditorGUI.Color_LightGray);
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                bSpriteChanged = true;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }


            // EditorGUI.BeginProperty(position, label, property);
            target = property.objectReferenceValue as SubTileSet;

            if (!bOffElementLabel)
            {
                if (target)
                {
                    Rect rt_label = RectUtil.Sub_Vertical(ref position);
                    float fBtnWidth = EditorGUIUtility.singleLineHeight;
                    rt_label.width -= fBtnWidth;
                    Rect rt_btn = new Rect(rt_label.xMax, rt_label.y, fBtnWidth, rt_label.height);
                    target.bFlodOut = EditorGUI.Foldout(rt_label, target.bFlodOut, target.UpdateName());
                    if (GUI.Button(rt_btn, "x", EditorStyles.miniButton))
                    {
                        target.parent.Erase(target);
                    }
                }
            }

            if (!target || target.bFlodOut || bOffElementLabel)
            {
                DrawSeperator(ref position);
                Rect rt_ObjectField = RectUtil.Sub_Vertical(ref position);
                var guiSimpleView = new GUIContent("Simple View");
                Rect rt_SimplView = rt_ObjectField;

                if (property.objectReferenceValue)
                {
                    rt_SimplView.width = GUI.skin.toggle.CalcSize(guiSimpleView).x;
                    rt_ObjectField.xMin += 25 + rt_SimplView.width;
                }

                if (property.objectReferenceValue == null)
                {
                    DrawSeperator(ref position);
                    return;
                }

                SerializedObject so = new SerializedObject(property.objectReferenceValue);
                so.Update();
                spTileSetType = so.FindProperty("tileSetType");
                spOutfield = so.FindProperty("outField");

                spDirectionalFieldType = so.FindProperty("FieldTypes");
                spDirectionalTile = so.FindProperty("DirectionalTiles");
                spDirectionalSprite = so.FindProperty("DirectionalSprites");
                spCustomOutField = so.FindProperty("CustomOutField");

                spIsometricAngle = so.FindProperty("vIsometricAngle");
                spIsSimpleView = so.FindProperty("bSimpleView");
                spPriority = so.FindProperty("iPriority");
                spAllowNullOutfield = so.FindProperty("bAllowNullOutField");

                EditorGUI.BeginDisabledGroup(!tmpTexture2D.texture);
                spIsSimpleView.boolValue = EditorGUI.ToggleLeft(rt_SimplView, guiSimpleView, spIsSimpleView.boolValue);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rt_ObjectField, spTileSetType);
                if (EditorGUI.EndChangeCheck())
                {
                    bSpriteChanged = true;
                    spTileSetType.serializedObject.ApplyModifiedProperties();
                }

                if (!isSimpleView)
                {
                    if (!bOffElementLabel)
                    {
                        EditorGUI.LabelField(RectUtil.Sub_Vertical(ref position), GetDesc(target));
                        DrawSeperator(ref position);
                    }

                    position = DrawOutFiled(position);
                    DrawSeperator(ref position);
                }
                position = DrawTileSpritesFiled(position);
                bSpriteChanged |= cLastBaseSprite != target.GetParentBaseSprite;

                if (!isSimpleView)
                {
                    position = DrawSpriteObjectsFiled(position);
                }
                so.ApplyModifiedProperties();
            }
            //EditorGUI.EndProperty();
        }

        Rect  DrawOutFiled(Rect position)
        {
            Rect rt = RectUtil.Sub_Vertical(ref position);
            
            EditorGUI.BeginChangeCheck();
            if (!bOffElementLabel)
            {
                EditorGUI.BeginDisabledGroup(bOffElementLabel);
                spOutfield.objectReferenceValue = EditorGUI.ObjectField(rt, spOutfield.objectReferenceValue, typeof(TileSetSprites), allowSceneObjects: false);
                EditorGUI.EndDisabledGroup();

                rt = RectUtil.Sub_Vertical(ref position);
            }
            if (EditorGUI.EndChangeCheck())
            {
                bSpriteChanged = true;
                spTileSetType.serializedObject.ApplyModifiedProperties();
            }

            Rect[] rts = rt.Division(2, 1);
            var _type = (SubTileSet.Type)System.Enum.GetValues(typeof(SubTileSet.Type)).GetValue(spTileSetType.enumValueIndex);
            if (_type != SubTileSet.Type.Custom)
            {
                EditorGUI.PropertyField(rts[0], spAllowNullOutfield);
                spPriority.intValue = EditorGUI.IntSlider(rts[1], spPriority.intValue, 0, 10);
            }
            else
            {
                spPriority.intValue = EditorGUI.IntSlider(rt, spPriority.intValue, 0, 10);
            }
            return position;
        }

        Rect DrawTileSpritesFiled(Rect position)
        {
            position = DrawTilesetImage(position);
            return position;
        }

        Rect DrawSpriteObjectsFiled(Rect position)
        {
            Rect[] rt_Line_Top = RectUtil.Sub_Vertical(ref position).Division(6, 0);
            Rect[] rt_Line_Second = RectUtil.Sub_Vertical(ref position).Division(6, 0);
            Rect[] rt_Line_Third = RectUtil.Sub_Vertical(ref position).Division(6, 0);
            Rect[] rt_Line_Fourth = RectUtil.Sub_Vertical(ref position).Division(6, 0);
            Rect[] rt_Line_Fifth = RectUtil.Sub_Vertical(ref position).Division(6, 0);

            EditorGUI.BeginChangeCheck();

            var _type = (SubTileSet.Type)System.Enum.GetValues(typeof(SubTileSet.Type)).GetValue(spTileSetType.enumValueIndex);

            Rect rt = rt_Line_Top[2]; rt.width *= 2f;
            if (_type != SubTileSet.Type.OneWay && _type != SubTileSet.Type.ThreeWay && _type != SubTileSet.Type.Ex_2_Diagonal_Together && _type != SubTileSet.Type.Ex_2_Diagonal_Apart_H)
                DirectionalPropertyField(rt, _type, InGameDirection.Top_Move, _type, GUIContent.none);

            if (_type != SubTileSet.Type.Ex_1_Diagonal && _type != SubTileSet.Type.Ex_3_Diagonal)
            {
                rt = rt_Line_Second[1]; rt.width *= 2f;
                DirectionalPropertyField(rt, _type, InGameDirection.TL_Move, _type, GUIContent.none);
                rt = rt_Line_Second[3]; rt.width *= 2f;
                DirectionalPropertyField(rt, _type, InGameDirection.TR_Move, _type, GUIContent.none);
            }

            rt = rt_Line_Third[0]; rt.width *= 2f;
            if (_type != SubTileSet.Type.Ex_2_Diagonal_Together && _type != SubTileSet.Type.Ex_2_Diagonal_Apart_V)
            {
                if (_type != SubTileSet.Type.OneWay && _type != SubTileSet.Type.ThreeWay)
                    DirectionalPropertyField(rt, _type, InGameDirection.Left_Move, _type, GUIContent.none);

                if (_type != SubTileSet.Type.ThreeWay && _type != SubTileSet.Type.Ex_1_Diagonal && _type != SubTileSet.Type.Ex_3_Diagonal && _type != SubTileSet.Type.Ex_2_Diagonal_Apart_H)
                {
                    EditorGUI.BeginDisabledGroup(_type == SubTileSet.Type.TwoWay);
                    rt = rt_Line_Third[2]; rt.width *= 2f;
                    if (_type != SubTileSet.Type.Custom)
                        DirectionalPropertyField(rt, _type, InGameDirection.BaseField, _type, GUIContent.none);
                    else
                    {
                        SubTileSet.Type _typeForGetSpriteSP = SubTileSet.Type.Normal;
                        DirectionalPropertyField(rt, _typeForGetSpriteSP, InGameDirection.BaseField, _typeForGetSpriteSP, GUIContent.none);
                    }
                    EditorGUI.EndDisabledGroup();
                }

                rt = rt_Line_Third[4]; rt.width *= 2f;
                if (_type != SubTileSet.Type.OneWay && _type != SubTileSet.Type.ThreeWay)
                {
                    EditorGUI.BeginDisabledGroup(_type == SubTileSet.Type.Ex_2_Diagonal_Apart_H);
                    DirectionalPropertyField(rt, _type, InGameDirection.Right_Move, _type, GUIContent.none);
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (_type != SubTileSet.Type.Ex_1_Diagonal && _type != SubTileSet.Type.Ex_3_Diagonal)
            {
                bool bReversed = _type == SubTileSet.Type.TwoWay;
                EditorGUI.BeginDisabledGroup(bReversed);
                rt = rt_Line_Fourth[1]; rt.width *= 2f;
                DirectionalPropertyField(rt, _type, bReversed ? InGameDirection.TR_Move : InGameDirection.DL_Move, _type, GUIContent.none);
                rt = rt_Line_Fourth[3]; rt.width *= 2f;
                DirectionalPropertyField(rt, _type, bReversed ? InGameDirection.TL_Move : InGameDirection.DR_Move, _type, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }

            rt = rt_Line_Fifth[2]; rt.width *= 2f;
            if (_type != SubTileSet.Type.OneWay && _type != SubTileSet.Type.ThreeWay && _type != SubTileSet.Type.Ex_2_Diagonal_Together && _type != SubTileSet.Type.Ex_2_Diagonal_Apart_H)
            {
                EditorGUI.BeginDisabledGroup(_type == SubTileSet.Type.Ex_2_Diagonal_Apart_V);
                DirectionalPropertyField(rt, _type, InGameDirection.Down_Move, _type, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }

            bSpriteChanged |= EditorGUI.EndChangeCheck();

            RectUtil.Sub_Vertical(ref position, EditorGUIUtility.singleLineHeight * 0.5f);
            DrawSeperator(ref position);
            return position;
        }

        static string GetDesc(SubTileSet tileSet)
        {
            string _string = "Null(not set)";
            if (tileSet != null && tileSet.parent != null)
                _string = tileSet.parent.name;

            string surround = "Null(not set)";
            if (tileSet != null && tileSet.OutField != null)
                surround = tileSet.OutField.name;

            return string.Format("This SubTileSet is for when [{0}] is surrounded by [{1}].", _string, surround);
        }

        Rect DrawTilesetImage(Rect position)
        {
            if (IsoMap.IsNull)
            {
                Rect rt_Tileset = RectUtil.Sub_Vertical(ref position, Height_SubTileSetPreview);
                EditorGUI.HelpBox(rt_Tileset, "This Tool needs a IsoMap instance in the Scene!", MessageType.Warning);
                DrawSeperator(ref position);
            }
            else
            {
                UpdateSampleTiles();

                if (tmpTexture2D.texture)
                {
                    Rect rt_Tileset = RectUtil.Sub_Vertical(ref position, Height_SubTileSetPreview);
                    target.bakedTileSetImg = tmpTexture2D.texture;
                    MouseProcess(rt_Tileset);
                    tmpTexture2D.DrawRect(rt_Tileset);
                }
                DrawSeperator(ref position);

                if (!isSimpleView)
                {
                    EditorGUI.BeginChangeCheck();
                    Rect rt_Angle = RectUtil.Sub_Vertical(ref position);
                    rt_Angle.xMin += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rt_Angle, spIsometricAngle, new GUIContent("Preview Angle"));
                    bAngleChanged |= EditorGUI.EndChangeCheck();
                    DrawSeperator(ref position);
                    RectUtil.Sub_Vertical(ref position, EditorGUIUtility.singleLineHeight * 0.5f);
                }
            }
            return position;
        }

        void MouseProcess(Rect rt)
        {
            var vMousePos = Event.current.mousePosition;
            if (rt.Contains(vMousePos))
            {
                switch(Event.current.type)
                {
                    case EventType.MouseDown:
                        bOnDrag = true;
                        vLastMousePosition = vMousePos;
                        Event.current.Use();
                        break;
                    case EventType.MouseUp:
                        bOnDrag = false;
                        Event.current.Use();
                        break;
                    case EventType.MouseDrag:
                        if (bOnDrag)
                        {
                            bAngleChanged = true;
                            var vDiff = vMousePos - vLastMousePosition;
                            target.vIsometricAngle += new Vector2(vDiff.y, vDiff.x) * 0.6f;
                            vLastMousePosition = vMousePos;
                        }
                        Event.current.Use();
                        break;
                }
            }
        }

        bool UpdateSampleTiles()
        {
            if (bSpriteChanged || bAngleChanged)
            {
                if (BakeTilesetImage(target, tmpTexture2D))
                {
                    bSpriteChanged = bAngleChanged = false; 
                    cLastBaseSprite = target.GetParentBaseSprite;
                    return true;
                }
            }
            return false;
        }

        SerializedProperty GetDirectionalSP(SubTileSet.Type _type, InGameDirection _dir)
        {
            int index = (int)_dir;
            SerializedProperty spArray = null;
            if (_type == SubTileSet.Type.Custom)
                spArray = spCustomOutField;
            else
            {
                var _fieldType = (SubTileSet.RelativeShape)System.Enum.GetValues(typeof(SubTileSet.RelativeShape)).GetValue((spDirectionalFieldType.GetArrayElementAtIndex(index).enumValueIndex));
                switch (_fieldType)
                {
                    case SubTileSet.RelativeShape.Sprite:
                        spArray = spDirectionalSprite;
                        break;
                    case SubTileSet.RelativeShape.Tile:
                        spArray = spDirectionalTile;
                        break;
                }
            }

            if (index < 0 || index >= spArray.arraySize)
                return null;
            return spArray.GetArrayElementAtIndex(index);
        }

        void DirectionalPropertyField(Rect rt, SubTileSet.Type subType, InGameDirection dir, SubTileSet.Type _type, GUIContent gUIContent)
        {
            int index = (int)dir;
            var iFieldType = spDirectionalFieldType.GetArrayElementAtIndex(index).enumValueIndex;
            var fieldType = (SubTileSet.RelativeShape)System.Enum.GetValues(typeof(SubTileSet.RelativeShape)).GetValue(iFieldType);
            GUIContent btnContent = new GUIContent(fieldType.ToString().First().ToString(), fieldType.ToString());
            SerializedProperty sp = GetDirectionalSP(subType, dir);
            Rect rt_btn = RectUtil.Sub_Horizontal(ref rt, GUI.skin.button.CalcSize(btnContent).x);
            rt.xMin = (rt_btn.xMax + rt_btn.xMin) * 0.5f;

            if (_type == SubTileSet.Type.Custom)
            {
                sp.objectReferenceValue = EditorGUI.ObjectField(rt, gUIContent, sp.objectReferenceValue, typeof(TileSetSprites), allowSceneObjects:false);
            }
            else
            {
                if (fieldType == SubTileSet.RelativeShape.Tile)
                {
                    // Object Filed로 해서 IsoTile이 있는지 체크한다.
                    EditorGUI.BeginChangeCheck();
                    var obj = EditorGUI.ObjectField(rt, gUIContent, sp.objectReferenceValue, typeof(GameObject), allowSceneObjects: false) as GameObject;
                    if (EditorGUI.EndChangeCheck())
                    {
                        var tile = IsoTile.Find(obj);
                        if (tile)
                            sp.objectReferenceValue = tile;
                        else
                        {
                            sp.objectReferenceValue = obj;
                            //EditorUtility.DisplayDialog("Info", "This is not Iso File Component. Please, choose again.", "OK");
                        }
                    }

                }
                else
                    EditorGUI.PropertyField(rt, sp, gUIContent);
            }

            if (GUI.Button(rt_btn, btnContent))
            {
                spDirectionalFieldType.GetArrayElementAtIndex(index).enumValueIndex = (iFieldType + 1) % (int) SubTileSet.RelativeShape.Count;
            }
        }

        SerializedProperty GetCustomOutField(InGameDirection _dir)
        {
            int index = (int)_dir;
            if (index < 0 || index >= spCustomOutField.arraySize)
                return null;
            return spCustomOutField.GetArrayElementAtIndex(index);
        }

        public static bool BakeTilesetImage(SubTileSet subTileSet, TmpTexture2D tmpTexture2D)
        {
            if (subTileSet == null || tmpTexture2D == null)
                return false;

            Camera cam = SceneView.GetAllSceneCameras().First(c => c != null);
            if (cam == null)
                return false;

            Vector2 vIsometricAngle = subTileSet.vIsometricAngle;
            List<IsoTile> tiles = CreateBaseTiles(vIsometricAngle);
            List<IsoTile> destroyTiles = new List<IsoTile>();
            List<IsoTile> extraTiles = new List<IsoTile>();
            List<IsoTile> lateUpdateList = new List<IsoTile>();

            int iType = (int)subTileSet.tileSetType;
            if (subTileSet.OutField != null && iType >= 0 && iType < (int)SubTileSet.Type.Custom) //extraTileTypes.Length / 16)
            {
                extraTiles.AddRange(CreateExtraTiles(subTileSet.tileSetType, vIsometricAngle));
                for (int i = 0; i < extraTiles.Count; i++)
                {
                    extraTiles[i].tileSetSprites = extraTileTypes[iType, i] ? subTileSet.parent : subTileSet.OutField;
                    lateUpdateList.Add(extraTiles[i]);
                }
            }

            for (InGameDirection _dir = InGameDirection.BaseField; _dir <= InGameDirection.TR_Move; ++_dir)
            {
                IsoTile _t = tiles[(int)_dir];
                if (subTileSet.tileSetType == SubTileSet.Type.Custom && _dir != InGameDirection.BaseField)
                {
                    var _tileSet = subTileSet.GetCustomDirectionalTileSet(_dir);
                    if (_tileSet == null)
                    {
                        destroyTiles.Add(_t);
                        continue;
                    }
                    _t.ChangeBaseSprite(_tileSet != null ? _tileSet.baseSprite : null);
                }
                else
                {
                    // 지정 슬롯이 아닌 base나 out field면 IsoTile.UpdateTileSetSprite 를 한다
                    InGameDirection outDir;
                    TileSetSprites _tileset = subTileSet.getDirectionalTileSetSprites(_dir, out outDir);

                    // null인 경우는 BaseField의 지정 Sprite인 경우다. 추가: null Outfield -> Destroy Tile
                    if (_tileset == null)
                    {
                        if (outDir == InGameDirection.OutField && subTileSet.OutField == null)
                        {
                            destroyTiles.Add(_t);
                            continue;
                        }
                        else
                        {
                            _t.tileSetSprites = subTileSet.parent;
                            subTileSet.ApplyShape(_t, _dir, true, false);
                        }
                    }
                    else
                    {
                        _t.tileSetSprites = _tileset;
                        lateUpdateList.Add(_t);
                    }
                }
            }

            tiles.RemoveAll(t => destroyTiles.Contains(t));
            foreach (var one in destroyTiles)
            {
                GameObject.DestroyImmediate(one.gameObject, true);
            }

            tiles.AddRange(extraTiles);

            // LateUpdateUtil.Register_LateUpdate(() =>
            {
                foreach (var one in lateUpdateList)
                {
                    one.ChangeTileSet(one.tileSetSprites, true, false, TileRestrictions: tiles, iLayerMask: ~(1 << iLayerForPreview));
                    foreach (var sprr in one.GetComponentsInChildren<SpriteRenderer>())
                    {
                        sprr.color = new Color(1, 0.8f, 0.8f, 1);
                    }
                }

                // this null allow do not update neibhours when destroy
                tiles.ForEach(t => t.tileSetSprites = null);
                tmpTexture2D.MakeRenderImage(tiles.Select(t => t.gameObject).ToList(), cam, Color.clear, 512, 256, vIsometricAngle.ToString());

                foreach (var tile in tiles)
                    GameObject.DestroyImmediate(tile.gameObject, true);
            }
            //);
            return true;
        }

        static List<IsoTile> CreateExtraTiles(SubTileSet.Type _type, Vector3 vIsometricAngle)
        {
            List<IsoTile> extras = new List<IsoTile>();
            while (extras.Count < 16)
            {
                int indexforPosition = extras.Count();
                InGameDirection firstDirection = (InGameDirection)(indexforPosition / 2 + 1);
                InGameDirection secondDirection = indexforPosition % 2 == 0 ? firstDirection : (InGameDirection)(((int)firstDirection) % 8 + 1);
                Vector3 vPos = firstDirection.ToVector3() + secondDirection.ToVector3();

                var tile = IsoTile.NewTile(vPos);
                tile.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
                tile.gameObject.layer = iLayerForPreview;
                
                tile.coordinates.bSnapFree = true;
                tile.coordinates.enabled = false;

                var isoTrans = tile.GetComponentInChildren<IsoTransform>();
                isoTrans.AdjustRotation(vIsometricAngle);

                extras.Add(tile);
            }
            return extras;
        }

        static List<IsoTile> CreateBaseTiles(Vector3 vIsometricAngle)
        {
            List<IsoTile> tiles = new List<IsoTile>();
            while (tiles.Count < 9)
            {
                var tile = IsoTile.NewTile(((InGameDirection)tiles.Count).ToVector3());
                tile.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
                tile.gameObject.layer = iLayerForPreview;

                tile.coordinates.bSnapFree = true;
                tile.coordinates.enabled = false;

                var isoTrans = tile.GetComponentInChildren<IsoTransform>();
                isoTrans.AdjustRotation(vIsometricAngle);

                tiles.Add(tile);
            }
            return tiles;
        }
    }
}
