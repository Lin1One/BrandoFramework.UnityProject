using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    using Isometric;

    [CustomPropertyDrawer(typeof(TileSetSprites))]
    public class TileSetSpritesDrawer : PropertyDrawer 
    {

        #region SerializedProperty
        SerializedProperty spBaseSprite;
        SerializedProperty spSubTileSets;
        SerializedProperty spLookupedSubTileset;
        #endregion

        #region Height
        static float Height_SingleSeperator
        {
            get { return EditorGUIUtility.singleLineHeight * 0.125f; }
        }
        static int Height_Base {
            get { return Mathf.CeilToInt(EditorGUIUtility.singleLineHeight * 4); }
        }
        #endregion

        [SerializeField]
        TileSetSprites outField;
        TileSetSprites target;

        [SerializeField]
        bool bInited = false;
        [SerializeField]
        bool bBaseSpriteChanged = false;

        [SerializeField]
        int index_SubTileSet = -1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight + 2 * Height_SingleSeperator;
            if (spLookupedSubTileset != null)
            {
                SubTileSet _sub = spLookupedSubTileset.objectReferenceValue as SubTileSet;
                if (_sub)
                {
                    float fImageHeightforSub = _sub.bakedTileSetImg == null ? SubTileSetDrawer.Height_SubTileSetPreview : 0;
                    return Height_Base + (_sub.bSimpleView ? SubTileSetDrawer.Height_SimpleView : SubTileSetDrawer.Height_Max) - fImageHeightforSub;
                }
            }
            return Height_Base + EditorGUIUtility.singleLineHeight;
        }

        static void DrawSeperator(ref Rect position)
        {
            DrawSeperator(ref position, CustomEditorGUI.Color_LightGray);
        }

        static void DrawSeperator(ref Rect position, Color color)
        {
            Rect rt = RectUtil.Sub_Vertical(ref position, Height_SingleSeperator);
            EditorGUI.DrawRect(rt, color);
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawSeperator(ref position);

            Rect rt_space = RectUtil.Sub_Vertical(ref position, EditorGUIUtility.singleLineHeight * 0.125f);
            Rect rt_ObjectField = RectUtil.Sub_Vertical(ref position, EditorGUIUtility.singleLineHeight);
            using (new GUIBackgroundColorScope(Color.cyan))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(rt_ObjectField, property, new GUIContent(""));
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            if (property.objectReferenceValue == null)
            {
                DrawSeperator(ref position);
                return;
            }

            target = property.objectReferenceValue as TileSetSprites;

            SerializedObject so = new SerializedObject(property.objectReferenceValue);
            so.Update();
            spBaseSprite = so.FindProperty("baseSprite");
            spSubTileSets = so.FindProperty("subTiles");

            DrawSeperator(ref position);
            position = AddTilesetField(position, property.serializedObject);
            position = DrawTileSpritesFiled(position);

            so.ApplyModifiedProperties();
        }

        Rect SelectSubAssetField(Rect position)
        {
            Rect[] field_rect = RectUtil.Sub_Vertical(ref position).Division(2, 1);
            field_rect[0].xMax += field_rect[0].width * 0.2f;
            field_rect[1].xMin = field_rect[0].xMax + EditorGUIUtility.singleLineHeight;

            Rect[] rt_popups = field_rect[0].Division(2, 1);
            rt_popups[0].xMax -= rt_popups[0].width * 0.6f;
            rt_popups[1].xMin = rt_popups[0].xMax + EditorGUIUtility.singleLineHeight;

            string[] subTileNames = new string[spSubTileSets.arraySize];
            for (int i = 0; i < subTileNames.Length; ++i)
            {
                var _sub = spSubTileSets.GetArrayElementAtIndex(i).objectReferenceValue as SubTileSet;
                if (_sub)
                    subTileNames[i] = string.Format("{0}. {1} ({2})", i, _sub.OutField != null ? _sub.OutField.name : "None OutField", _sub.tileSetType);
            }

            EditorGUI.LabelField(rt_popups[0], "SubTiles");
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(subTileNames.Length == 0);
            index_SubTileSet = EditorGUI.Popup(rt_popups[1], "" , index_SubTileSet, subTileNames);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                spLookupedSubTileset = spSubTileSets.GetArrayElementAtIndex(index_SubTileSet);
                // tagHolder.Set(spSubTileSets.GetArrayElementAtIndex(index_SubTileSet).objectReferenceValue as SubTileSet);
            }

            Rect[] rt_lr = field_rect[1].Division(2, 1);
            CustomEditorGUI.Button(rt_lr[0], target ? target.Contain(outField) : false, CustomEditorGUI.Color_LightYellow, "Duplicate", () => { });
            CustomEditorGUI.Button(rt_lr[1], target ? target.Contain(outField) : false, CustomEditorGUI.Color_LightRed, "Del", () =>
            {
                EraseLookupedSubTileset();
                index_SubTileSet = Mathf.Min(index_SubTileSet, target.subTiles.Count - 1);
                spSubTileSets.serializedObject.Update();
                if (index_SubTileSet >= 0)
                {
                    var element = spSubTileSets.GetArrayElementAtIndex(index_SubTileSet);
                    if (element != null)
                    {
                        Update_spLookupedSubTileset(element.objectReferenceValue as SubTileSet);
                    }
                }
            });
            return position;
        }

        Rect AddTilesetField(Rect position, SerializedObject so)
        {            
            Rect sprite_rect = RectUtil.Sub_Vertical(ref position);
            DrawSeperator(ref position);

            Rect[] finder_rect = RectUtil.Sub_Vertical(ref position).Division(2, 1);
            finder_rect[0].xMax += finder_rect[0].width * 0.2f;
            finder_rect[1].xMin = finder_rect[0].xMax + EditorGUIUtility.singleLineHeight;

            DrawSeperator(ref position);
            DrawSeperator(ref position, Color.gray);
            DrawSeperator(ref position);
            using (new GUIBackgroundColorScope(CustomEditorGUI.Color_LightGreen))
            {
                position = SelectSubAssetField(position);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(sprite_rect, spBaseSprite);
            bBaseSpriteChanged |= EditorGUI.EndChangeCheck();

            InitTagHolder();

            using (new GUIBackgroundColorScope(CustomEditorGUI.Color_LightGreen))
            {
                // EditorGUI.BeginChangeCheck();
                outField = EditorGUI.ObjectField(finder_rect[0], outField, typeof(TileSetSprites), allowSceneObjects: false) as TileSetSprites;
                //if (EditorGUI.EndChangeCheck())
                //{
                //    Update_spLookupedSubTileset();
                //}
            }

            CustomEditorGUI.Button(finder_rect[1], outField != target, CustomEditorGUI.Color_LightBlue, "Add", () => {
                Update_spLookupedSubTileset(Add(outField));
            });            
            return position;
        }

        void InitTagHolder()
        {
            if (bInited)
            {
                if (spLookupedSubTileset != null && spLookupedSubTileset.objectReferenceValue != null)
                    outField = (spLookupedSubTileset.objectReferenceValue as SubTileSet).OutField;
                return;
            }

            bInited = true;
            if (outField == null || index_SubTileSet < 0)
            {
                for(int i = 0; i < spSubTileSets.arraySize; ++i)
                {
                    var element = spSubTileSets.GetArrayElementAtIndex(i);
                    if (element != null)
                    {
                        var subTileset = element.objectReferenceValue as SubTileSet;
                        if (subTileset != null && (outField == null || index_SubTileSet < 0))
                        {
                            if (subTileset.OutField != target)
                            {
                                spLookupedSubTileset = element;
                                outField = subTileset.OutField;
                                index_SubTileSet = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        Rect DrawTileSpritesFiled(Rect position)
        {
            if (spLookupedSubTileset != null)
            {
                SubTileSetDrawer.bOffElementLabel = true;
                EditorGUI.PropertyField(position, spLookupedSubTileset);
                SubTileSetDrawer.bOffElementLabel = false;                
                DrawSeperator(ref position);
            }
            return position;
        }

        void Update_spLookupedSubTileset(SubTileSet _target = null)
        {
            spLookupedSubTileset = null;
            spSubTileSets.serializedObject.Update();
            for (int i = 0; i < spSubTileSets.arraySize; ++i)
            {
                var element = spSubTileSets.GetArrayElementAtIndex(i);
                if (element != null)
                {
                    var subTileset = element.objectReferenceValue as SubTileSet;
                    if (subTileset != null)
                    {
                        if (_target == null)
                        {
                            if (outField == subTileset.OutField)
                            {
                                spLookupedSubTileset = element;
                                index_SubTileSet = i;
                                break;
                            }
                        }else if (subTileset == _target)
                        {
                            spLookupedSubTileset = element;
                                index_SubTileSet = i;
                            break;
                        }
                    }
                }
            }
        }

        SubTileSet Add(TileSetSprites _tileSet)
        {
            var subTileSet = target.Add(_tileSet);            
            return subTileSet;
        }
        bool EraseLookupedSubTileset()
        {
            var _sub = spLookupedSubTileset.objectReferenceValue as SubTileSet;
            var bResult = target.Erase(_sub);
            spLookupedSubTileset = null;
            return bResult;
        }


    }
}
