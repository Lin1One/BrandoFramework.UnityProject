using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TileSetSprites))] 
    public class TileSetSpritesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Add SubTileSet btn & Delete Unlinked SubTileSet
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIContent gUIContent = new GUIContent("Update all tiles", "Update all tiles in the scene that use this tile set.");
                if (GUILayout.Button(gUIContent))
                {
                    var tileSetSprites = serializedObject.targetObjects.Cast<TileSetSprites>();
                    var tiles = FindObjectsOfType<IsoTile>().Where(t => t.tileSetSprites != null && tileSetSprites.Contains(t.tileSetSprites));
                    IsoTile.UpdateTileSet(tiles, true, true, gUIContent.text);
                }
                string _name = "Add a New SubTileSet";
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightGreen, _name, () => {
                    foreach (var one in serializedObject.targetObjects.Cast<TileSetSprites>())
                    {
                        UndoUtil.Record(one, "Add a New SubTileSet");
                        SubTileSet newSub = null;
                        IEnumerable<TileSetSprites> nominees = null;

                        if (one.subTiles != null && one.subTiles.Count > 0)
                        {
                            nominees = one.subTiles.Where(s => s != null && s.OutField != null).Select(s => s.OutField);
                        }

                        newSub = one.Add((nominees != null && nominees.Count() > 0) ? nominees.Last() : null, true);

                        if (newSub)
                            UndoUtil.Create(newSub, _name);
                    }
                });
                CustomEditorGUI.Button(true, CustomEditorGUI.Color_LightRed, "Delete All garbage SubTileSet & Null", () => {
                    foreach(var one in serializedObject.targetObjects.Cast<TileSetSprites>())
                    {
                        one.DeleteAsset_All_Unregistered_Child(one.subTiles);
                        one.EraseNull();
                    }
                });
            }

            base.DrawDefaultInspector();

        }
    }
}
