using UnityEditor;
using UnityEngine;

namespace GameWorld.Editor
{
    public class MB_EditorStyles
    {
        public GUIStyle multipleMaterialBackgroundStyle = new GUIStyle();
        public GUIStyle multipleMaterialBackgroundStyleDarker = new GUIStyle();
        public GUIStyle editorBoxBackgroundStyle = new GUIStyle();

        Texture2D multipleMaterialBackgroundColor;
        Texture2D multipleMaterialBackgroundColorDarker;
        Texture2D editorBoxBackgroundColor;

        public void Init()
        {
            bool isPro = EditorGUIUtility.isProSkin;
            Color backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);
            if (multipleMaterialBackgroundColor == null)
            {
                multipleMaterialBackgroundColor = MeshCombinerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            backgroundColor = isPro
                ? new Color32(50, 50, 50, 255)
                : new Color32(190, 220, 190, 255);
            if (multipleMaterialBackgroundColorDarker == null)
            {
                multipleMaterialBackgroundColorDarker = MeshCombinerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            backgroundColor = isPro
                ? new Color32(35, 35, 35, 255)
                : new Color32(174, 174, 174, 255);

            multipleMaterialBackgroundStyle.normal.background = multipleMaterialBackgroundColor;
            multipleMaterialBackgroundStyleDarker.normal.background = multipleMaterialBackgroundColorDarker;

            if (editorBoxBackgroundColor == null)
            {
                editorBoxBackgroundColor = MeshCombinerEditorFunctions.MakeTex(8, 8, backgroundColor);
            }

            editorBoxBackgroundStyle.normal.background = editorBoxBackgroundColor;
            editorBoxBackgroundStyle.border = new RectOffset(0, 0, 0, 0);
            editorBoxBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
            editorBoxBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);
        }

        public void DestroyTextures()
        {
            if (multipleMaterialBackgroundColor != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColor);
            if (multipleMaterialBackgroundColorDarker != null) GameObject.DestroyImmediate(multipleMaterialBackgroundColorDarker);
            if (editorBoxBackgroundColor != null) GameObject.DestroyImmediate(editorBoxBackgroundColor);
        }
    }


}
