using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    public class AtlasHelperWindow : EditorWindow
    {
#if UNITY_2017_1_OR_NEWER
        bool bFoldoutMSG = false;
        const string HelpMSG_00 = "Collect sprites in the SpriteAtlas!";
        const string HelpMSG_01 = "Ultimately, when all the sprites are contained in a single atlas, rendering speed is the fastest.";
        const string HelpMSG_02 = "BUILD FIRST!";
        const string HelpMSG_03 = "The atlas is packed during the first build after creation.";
        const string HelpMSG_04 = "Pack Preview!";
        const string HelpMSG_05 = "When adding a sprite to the Atlas, the Pack Preview button is pressed in the Inspector.";

        GameObject lookUpSource;

        List<Sprite> SpritesInSource = new List<Sprite>();
        bool bFoldoutSpriteList = true;
        Vector2 vSpriteScroll = Vector2.zero;
        float fSpriteGridSize = 60;

        List<Sprite> SelectedSprites = new List<Sprite>();

        Dictionary<SpriteAtlas, CustomEditorGUI.SimpleGrid<Sprite>> AtlasGridDic = new Dictionary<SpriteAtlas, CustomEditorGUI.SimpleGrid<Sprite>>();
        Dictionary<SpriteAtlas, Color> AtlasColorSwatch = new Dictionary<SpriteAtlas, Color>();
        Dictionary<Sprite, SpriteAtlas> SpriteAtlasDic = new Dictionary<Sprite, SpriteAtlas>();
        
        [MenuItem("Window/Anonym/Atlas Helper")]
        public static void CreateWindow()
        {            
            EditorWindow window = EditorWindow.CreateInstance<AtlasHelperWindow>();
            window.titleContent.text = "Atlas Helper";
            window.Show();
        }

        private void OnEnable()
        {
            updateLists();
        }

        void updateLists()
        {
            updateAtlasList();
            updateSpriteList();
        }

        void OnInspectorUpdate()
        {
            updateLists();
            Repaint();
        }

        void OnGUI()
        {
            showHelpMSG();            

            // Select Lookup Object
            CustomEditorGUI.DrawSeperator();
            GUILayout.Label(new GUIContent("Lockup Object", "Root of Object"), EditorStyles.boldLabel);
            lookUpSource = EditorGUILayout.ObjectField("", lookUpSource, typeof(GameObject), allowSceneObjects: true) as GameObject;

            ShowAtlasList();
            ShowSpriteList();
        }

        void showHelpMSG()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUILayout.Label("Not available in Play mode!", EditorStyles.boldLabel);
                return;
            }

            CustomEditorGUI.DrawSeperator();
            if (bFoldoutMSG = EditorGUILayout.Foldout(bFoldoutMSG, "[Help MSG]"))
            {
                GUILayout.Label(HelpMSG_00, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(HelpMSG_01, MessageType.Info);
                GUILayout.Label(HelpMSG_02, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(HelpMSG_03, MessageType.Info);
                GUILayout.Label(HelpMSG_04, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(HelpMSG_05, MessageType.Info);
            }
        }

        void updateSpriteAtlasDic()
        {
            SpriteAtlasDic.Clear();
            SpritesInSource.ForEach(r =>
            {
                var atlas = FindAtlas(r);
                if (atlas)
                    SpriteAtlasDic.Add(r, atlas);
            });
        }

        #region Sprite
        void updateSpriteList()
        {
            // Find all sprites used in baked
            if (lookUpSource)
            {
                SpritesInSource.Clear();
                SpritesInSource.AddRange(lookUpSource.GetComponentsInChildren<SpriteRenderer>().Where(r => r.sprite != null).Select(r => r.sprite).Distinct().ToArray());
                updateSpriteAtlasDic();
                SpritesInSource.Sort((x, y) => {
                    if (!SpriteAtlasDic.ContainsKey(y))
                        return 1;
                    if (!SpriteAtlasDic.ContainsKey(x))
                        return -1;
                    return SpriteAtlasDic[x].name.CompareTo(SpriteAtlasDic[y].name);
                });
            }
        }

        void ShowSpriteList()
        {
            if (!lookUpSource)
                return;

            CustomEditorGUI.DrawSeperator();
            GUILayout.Label("Sprite List", EditorStyles.boldLabel);

            if (SpritesInSource.Count > 0)
            {
                CustomEditorGUI.SimpleGrid<Sprite>.ShowGrid(
                    string.Format("{0} Sprites used in Source Object", SpritesInSource.Count),
                    ShowClassifiedSprite_ImageOnly, SpritesInSource.GetEnumerator(), "Clear Selection", () => {  SelectedSprites.Clear();    },
                    ref fSpriteGridSize, ref bFoldoutSpriteList, ref vSpriteScroll, SelectedSprites, true, true, true, Color.gray);
            }
        }

        void ShowClassifiedSprite_ImageOnly(Sprite sprite, Rect rect)
        {
            CustomEditorGUI.DrawSprite(rect, sprite, Color.clear, true, true);
        }
        #endregion

        #region Atlas
        void updateAtlasList()
        {
            var resources = Resources.FindObjectsOfTypeAll<SpriteAtlas>();
            var enumerator = resources.Where(r => !r.isVariant).GetEnumerator();
            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (!AtlasGridDic.ContainsKey(current))
                {
                    var sprites = new Sprite[current.spriteCount];
                    current.GetSprites(sprites);

                    var simpleGridData = new CustomEditorGUI.SimpleGrid<Sprite>();
                    simpleGridData.Init(current.name, 40, new List<Sprite>(sprites), null, null, Color.gray, false, false, dragable:true);
                    AtlasGridDic.Add(current, simpleGridData);
                }
                else
                {
                    var sprites = new Sprite[current.spriteCount];
                    current.GetSprites(sprites);
                    AtlasGridDic[current].UpdateList(new List<Sprite>(sprites));
                }
            }

            float fGap = 0.6f;
            var keys = AtlasGridDic.Keys;
            var removeList = new List<SpriteAtlas>();
            foreach(var key in keys)
            {
                if (!resources.Contains(key))
                {
                    removeList.Add(key);
                    continue;
                }

                if (!AtlasColorSwatch.ContainsKey(key))
                {
                    float hMax = AtlasColorSwatch.Count * fGap % 1f;
                    AtlasColorSwatch.Add(key, Random.ColorHSV(hMax - 0.1f, hMax, 0.9f, 1f, 0.9f, 1f));
                }
            }

            removeList.ForEach(r => AtlasGridDic.Remove(r));
        }

        SpriteAtlas FindAtlas(Sprite sprite)
        {
            if (!sprite.packed)
                return null;

            Texture2D texture = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(sprite, true);
            var keys = AtlasGridDic.Keys;
            foreach(var key in keys)
            {
                Sprite[] sprites = new Sprite[1];
                key.GetSprites(sprites);
                if (sprites[0] != null && UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(sprites[0], true) == texture)
                {
                    return key;
                }
            }
            return null;
        }

        void AtlasField(SpriteAtlas atlas, bool bWithColorField)
        {
            if (atlas == null)
                return;

            var color = AtlasColorSwatch[atlas];

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUIBackgroundColorScope(color))
                    EditorGUILayout.ObjectField(atlas, typeof(SpriteAtlas), allowSceneObjects: false);
                if (bWithColorField)
                    AtlasColorSwatch[atlas] = EditorGUILayout.ColorField(color, GUILayout.Width(40));
            }
        }

        void ShowAtlasList()
        {
            CustomEditorGUI.DrawSeperator();
            GUILayout.Label("SpriteAtlas List", EditorStyles.boldLabel);

            var keys = AtlasGridDic.Keys.GetEnumerator();
            while (keys.MoveNext())
            {
                var key = keys.Current;
                AtlasField(key, true);
                AtlasGridDic[key].ShowGrid(ShowClassifiedSprite_ImageOnly);
            } 
        }
        #endregion
#endif // UNITY_2017_1_OR_NEWER
    }
}