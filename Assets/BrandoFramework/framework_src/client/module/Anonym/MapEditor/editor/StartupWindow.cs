using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    public class StartupWindow : EditorWindow
    {
        static Dictionary<string, GUILayoutOption> btnWidthOptions = new Dictionary<string, GUILayoutOption>();
        GUILayoutOption BTNWidthOption(string key)
        {
            if (!btnWidthOptions.ContainsKey(key))
                btnWidthOptions.Add(key, GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(key)).x));

            return btnWidthOptions[key];
        }

        GUILayoutOption _BTNHeightOption = null;
        GUILayoutOption BTNHeightOption { get
            {
                if (_BTNHeightOption == null)
                {
                    GUIContent sampleBtn = new GUIContent("Button");
                    Vector2 size = GUI.skin.button.CalcSize(sampleBtn);
                    _BTNHeightOption = GUILayout.Height(size.y);
                }
                return _BTNHeightOption;
            }
        }

        [MenuItem("Window/Anonym/[Startup Window]", priority = 100)]
        public static void CreateWindow()
        {
            EditorWindow window = EditorWindow.CreateInstance<StartupWindow>();
            window.titleContent.text = "Startup Window";
            window.Show();
        }

        private void OnEnable()
        {
            
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            GUI_WebLink();
            GUI_FirstStep();

            GUI_Tools();

            GUI_ModeDependentMisc();
            GUI_ModeIndependentMisc();

        }

        #region MISC
        void GUI_WebLink()
        {
            label("[Web Guide]", EditorStyles.boldLabel);
            CustomEditorGUI.DrawSeperator();
            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "https://hgstudioone.wixsite.com/isometricbuilder", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder"));
                label("(All buttons with this color are guide links)");
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

        }

        void GUI_FirstStep()
        {
            IsoMap isoMap = IsoMap.instance;
            IsoTileBulk firstBulk = isoMap && isoMap.BulkList.Count > 0 ? isoMap.BulkList.First() : null;
            IsoTile firstTile = firstBulk && firstBulk.GetAllTiles().Count() > 0 ? firstBulk.GetAllTiles().First() : null;

            label("[First Step]", EditorStyles.boldLabel);
            CustomEditorGUI.DrawSeperator();
            using (new EditorGUILayout.HorizontalScope())
            {
                label("Add");
                btn(!isoMap, "IsoMap", CustomEditorGUI.Color_LightYellow, () =>
                {
                    if (IsoMap.IsNull)
                    {
                        var prefab = EditorGUIUtility.Load(IsoMap.defaultIsoMapPrefabPat) as GameObject;
                        var newOne = CustomEditorGUI.Undo_Instantiate(prefab, null, "Create IsoMap Instance", false);
                        if (newOne != null)
                            IsoMap.instance = newOne.GetComponent<IsoMap>();
                    }
                });
                label("and");
                btn(!firstBulk, "IsoTileBulk", CustomEditorGUI.Color_LightYellow, () => isoMap.NewBulk());
                label("and");
                btn(isoMap, "Set the Camera", CustomEditorGUI.Color_Side, () =>
                {
                    if (isoMap.GameCamera == null)
                        isoMap.GameCamera = Camera.main;
                    isoMap.Update_TileAngle();
                });
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                label("Select the ");
                btn(isoMap, "IsoMap", CustomEditorGUI.Color_LightGreen, () => Selection.activeObject = isoMap);
                btn(firstBulk, "IsoTileBulk", CustomEditorGUI.Color_LightGreen, () => Selection.activeObject = firstBulk);
                label("and");
                btn(firstTile, "IsoTile", CustomEditorGUI.Color_LightGreen, () => Selection.activeObject = firstTile);
                label("instances that are in the Scene.");
            }
            label("These are the basis of IsometricBuilder. And you can do a lot in the <b>Inspector.</b>");
            interSpace();
        }

        void GUI_Tools()
        {
            label("[Key elements]", EditorStyles.boldLabel);
            CustomEditorGUI.DrawSeperator();
            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "Open Magic Wand Palatte", CustomEditorGUI.Color_Obstacle, () => MasterPaletteWindow.CreateWindow());
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/new-extra-features"));
                label("Place the Tiles directly via <b>draw interface in the SceneView.</b>");
            }
            label("You can register <b>templates</b> as well as <b>place, coloring, change, delete, raise, and lower</b> tiles.");
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                IsometricMovement character = FindObjectOfType<IsometricMovement>();

                label("Add");
                btn(true, "Character", character ? CustomEditorGUI.Color_Side : CustomEditorGUI.Color_LightYellow, () => {
                    var prefab = EditorGUIUtility.Load(IsometricCharacterController.defaultIsoMapPrefabPat) as GameObject;
                    Selection.activeObject = CustomEditorGUI.Undo_Instantiate(prefab, null, "Sample Character", false);
                });
                label("or");
                btn(true, "Character for NavMesh", character ? CustomEditorGUI.Color_Side : CustomEditorGUI.Color_LightYellow, () => {
                    var prefab = EditorGUIUtility.Load(IsometricNavMeshAgent.defaultIsoMapPrefabPat) as GameObject;
                    Selection.activeObject = CustomEditorGUI.Undo_Instantiate(prefab, null, "Sample NavMeshAgent Character", false);
                });
                label(", And");
                btn(KeyInputAssist.IsNull, "Key Input Assist", CustomEditorGUI.Color_LightYellow, () => {
                    var prefab = EditorGUIUtility.Load(KeyInputAssist.defaultIsoMapPrefabPat) as GameObject;
                    var go = CustomEditorGUI.Undo_Instantiate(prefab, null, "Key Input Assist", false);
                    var newOne = go.GetComponent<KeyInputAssist>();
                    Selection.activeObject = go;
                    newOne.Init();
                });
                label("to the Scene.");
            }
            label("To use NavMeshAgent's pathfinding, starting with <b>Character for NavMesh</b>.");

            interSpace();

        }

        void GUI_ModeDependentMisc()
        {
            label("[Sorting Mode Details]", EditorStyles.boldLabel);

            if (IsoMap.IsNull)
            {
                EditorGUILayout.HelpBox("First things first!", MessageType.Info);
                CustomEditorGUI.DrawSeperator();
                interSpace();
                return;
            }
            CustomEditorGUI.DrawSeperator();

            using (new EditorGUILayout.HorizontalScope())
            {
                label("Normal mode has");
                btn(true, "limit 1", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/new-how-to-start"));
                btn(true, "limit 2", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/tch"));
                label("when you project needs the tall character to move freely in the three-dimensional structure map.");
            }
            EditorGUILayout.Space();

            IsoMap isoMap = IsoMap.instance;
            using (new EditorGUILayout.HorizontalScope())
            {
                label("If your project uses a 3D structured map, use");
                if (IsoMap.instance.bUseIsometricSorting)
                    btn(true, "Toggle Off (now Auto ISO Mode)", CustomEditorGUI.Color_Side, () => {
                        isoMap.bUseIsometricSorting = false;
                        isoMap.UpdateIsometricSortingResolution();
                        isoMap.Update_All_ISO(isoMap.Revert_All_ISO());
                    });
                else
                    btn(true, "Toggle Auto ISO (now Normal Mode)", CustomEditorGUI.Color_Tile, () => {
                        isoMap.bUseIsometricSorting = true;
                        isoMap.UpdateIsometricSortingResolution();
                        isoMap.Update_All_ISO(isoMap.Backup_All_ISO());
                    });
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/isometric-sorting"));
                GUILayout.FlexibleSpace();
            }
            label("If not three-dimensional structure map, it is OK with set the <b>floor's Layer</b> to below the Default layer or do nothing.");
            EditorGUILayout.Space();

            EditorGUILayout.Space();
            label("<b>Normal Mode</b>", EditorStyles.largeLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                label("If problems occur around between character and a tile’s edges with depth sorting,");
                label("you can use the");
                if (IsoMap.instance.bUseGroundObjectOffset)
                    btn(true, "Toggle Off (now ON)", CustomEditorGUI.Color_Side, () => {
                        isoMap.bUseGroundObjectOffset = false;
                        isoMap.Update_GroundOffset();
                    });
                else
                    btn(true, "Toggle OnGroundOffset (now OFF)", CustomEditorGUI.Color_Tile, () => {
                        isoMap.bUseGroundObjectOffset = true;
                        isoMap.Update_GroundOffset();
                    });
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/isobasis"));
                label("to adjust the sorting.");
            }

            EditorGUILayout.Space();
            label("<b>Auto Isometric Sorting Order Mode</b>", EditorStyles.largeLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                label("Aside from OnGroundOffset, the");
                btn(true, "ISO Basis script", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/template"));
                label("&");
                btn(true, "TCH(Tall Character Helper)", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/isometric-sorting-order"));
                label("also have powerful calibration function to help resolve sorting problems.");
            }
            label("For instance, the TCH is used as child of the character object, an IsoBasis component can be added to any ISO object.");
            interSpace();
        }

        void GUI_ModeIndependentMisc()
        {
            label("[Misc]", EditorStyles.boldLabel);
            CustomEditorGUI.DrawSeperator();

            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "GameObject Seletor", CustomEditorGUI.Color_LightYellow, () => {
                    var go = new GameObject("GameObject Selector");
                    go.AddComponent<BoxCollider>();
                    go.AddComponent<GameObjectSelector>().UpdateCollider();
                    Selection.activeObject = go;
                });
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/etc-tools"));
                label(", ");

                btn(true, "ScreenShot Cam", CustomEditorGUI.Color_LightYellow, () => {
                    var prefab = EditorGUIUtility.Load(SSHelper.defaultIsoMapPrefabPat) as GameObject;
                    var newOne = CustomEditorGUI.Undo_Instantiate(prefab, null, "ScreenShot Cam", false);
                    Selection.activeObject = newOne;
                });
                label(", ");

                btn(true, "IsoLight", CustomEditorGUI.Color_LightYellow, () => {
                    var go = new GameObject("IsoLight");
                    go.AddComponent<IsoLight>().CreateDefaultSelector();
                    Selection.activeObject = go;
                });
                btn(true, "Link", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://youtu.be/XeB5KAI9MaM"));
                label(", ");

                btn(true, "Trick Cam for 3D Object", CustomEditorGUI.Color_LightYellow, () => {
                    var go = new GameObject("Selfie Cam");
                    Selection.activeObject = go;
                });
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => EditorUtility.OpenWithDefaultApp("Assets/Anonym/Util/SelfieCam Setup Guide.pdf"));

                label("and");
                btn(true, "TileSet", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/ongroundobject"));
                label("are also helpful.");
            }
            label("<b>TileSet</b>, <b>IsoLight</b> and <b>TrickCam (SelfieCam)</b> are pretty complicated. Please refer to the guide.");
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "Open IsoHierarchy", CustomEditorGUI.Color_Overlay, () => IsoHierarchyWindow.CreateWindow());
                // btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/new-extra-features"));
                label("Make Isometric a new raw object that is not a tile.");
                label("<b>Rotate the transform</b> to fit the isometric. And it is easy to <b>manage the Depth of all the Child</b>.");
            }
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "Open Atlas Helper", CustomEditorGUI.Color_Overlay, () => AtlasHelperWindow.CreateWindow());
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://youtu.be/N5qIpdpOI0c"));
                label("When <b>packing</b> image resources. You can easily identify and manage <b>all the sprites contained in all Atlas assets and GameObject.</b>");
            }
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                btn(true, "Open Bakery", CustomEditorGUI.Color_LightBlue, () => IsoComRemover.CreateWindow());
                btn(true, "Guide", CustomEditorGUI.Color_LightRed, () => Application.OpenURL("https://hgstudioone.wixsite.com/isometricbuilder/isobakery"));
                label("After you complete the map.");
                label("This will help to <b>remove the components of this asset</b> or create a whole mesh map for <b>NavMesh Bake</b>.");
            }

            interSpace();
        }

        void interSpace()
        {
            GUILayout.FlexibleSpace();
        }

        void label(string text, GUIStyle style = null, TextAnchor alignment = TextAnchor.LowerLeft, bool bFitWidth = true)
        {
            if (style == null)
                style = GUI.skin.label;
            CustomEditorGUI.Label(style, text, alignment, bFitWidth, BTNHeightOption);
        }

        void btn(bool enable, string text, Color color, CustomEditorGUI.SimpleAction action)
        {
            CustomEditorGUI.Button(enable, color, text, action, BTNHeightOption, BTNWidthOption(text));
        }
        #endregion
    }
}