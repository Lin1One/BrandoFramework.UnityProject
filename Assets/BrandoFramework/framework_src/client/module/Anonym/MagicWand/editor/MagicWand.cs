using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    [System.Serializable]
    public abstract class MagicWand : ScriptableObject, ITag
    {
        public enum ParamType
        {
            Color,
            fWeight,
            IsoTile,
            Parts,
            Position,
            AutoIsoLight,
            New,
            IsoBulk,
            KeepColor,
        }

        [SerializeField]
        List<string> tags = new List<string>();
        public bool Tag(string _tag)
        {
            return tags.Any(r => r.Equals(_tag, System.StringComparison.CurrentCultureIgnoreCase));
        }
        public bool Tag(List<string> _tags)
        {
            return _tags.All(r => Tag(r));
        }
        public List<string> GetTags() {
            return tags;
        }
        public void AddTags(string _tag)
        {
            if (!Tag(_tag))
                tags.Add(_tag);
        }
        public void RemoveTags(string _tag)
        {
            if (Tag(_tag))
                tags.Remove(_tag);
        }
        public void ClearTags()
        {
            tags.Clear();
        }

        [SerializeField]
        public bool bAllowMultipleApplyOnAClick = true;

        [SerializeField, ConditionalHide("bAllowMultipleApplyOnAClick", "False", hideInInspector: true)]
        bool isExclusive = false;
        public bool IsExclusive { get { return isExclusive; } }

#if UNITY_EDITOR
        public abstract bool MakeUp(ref GameObject target, params object[] values);
        public virtual GameObject TargetGameObject(GameObject target)
        {
            return target;
        }

        public abstract Texture[] GetTextures();
        public virtual Color[] GetColors()
        {
            return null;
        }

        public static void OnCustomGUI(MagicWand wand, Rect rect)
        {
            OnCustomGUI(wand, rect, false);
        }
        public static void OnCustomGUI(MagicWand wand, Rect rect, bool bShowEX)
        {
            wand.OnCustomGUI(rect);
            rect = ExLabelRect(rect);
            if (bShowEX)
            {
                if (wand.IsExclusive)
                {
                    OnCustomGUIExLabel(rect, Color.red * 0.5f, "Exclusive");
                    rect.y += rect.height;
                }
                if (wand.bAllowMultipleApplyOnAClick)
                    OnCustomGUIExLabel(rect, Color.blue * 0.5f, "Multiple");
            }
        }

        public static void OnCustomGUIWithLabel(MagicWand wand, Rect rect)
        {
            OnCustomGUIWithLabel(wand, rect, false);
        }
        public static void OnCustomGUIWithLabel(MagicWand wand, Rect rect, bool bShowEX)
        {
            OnCustomGUI(wand, rect, bShowEX);
            wand.OnCustomGUIName(rect);
        }
        public static void OnCustomGUIExLabel(Rect rect, Color color, string msg)
        {
            EditorGUI.DrawRect(rect, color);
            GUI.Label(rect, msg);
        }
        static Rect ExLabelRect(Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.xMin = rect.xMax - rect.width * 0.4f;
            return rect;
        }
        protected virtual void OnCustomGUI(Rect rect)
        {
            Texture[] textures = GetTextures();
            Color[] colors = GetColors();

            if (textures != null)
            {
                for (int i = 0; i < textures.Length; ++i)
                    GUI.DrawTexture(rect, textures[i], ScaleMode.ScaleToFit, true, 0, 
                        colors != null && colors.Length > i ? colors[i] : Color.white, 0, 0);
            }
        }
        protected virtual void OnCustomGUIName(Rect rect)
        {
            Rect tmpRect = rect;
            tmpRect.yMin = rect.yMax - EditorGUIUtility.singleLineHeight;
            tmpRect.height = EditorGUIUtility.singleLineHeight;

            GUI.Label(tmpRect, name);
        }

        public virtual ParamType[] Params { get { return null; } }
#endif

        #region Static Methos
        public static string TypeArray(List<MagicWand> magicWands)
        {
            var enumerator = magicWands.Where(w => w != null);
            return enumerator.Count() == 0 ? "None!" : string.Join(", ",
                enumerator.Select(r => string.Format("{0}({1})", r.GetType().Name, enumerator.Where(rr => rr.GetType() == r.GetType()).Count())).Distinct().ToArray());
        }
        public static string NameArray(List<MagicWand> magicWands)
        {
            return magicWands.Count == 0 ? "None!" : string.Join(", ", magicWands.Select(r => r.name).ToArray());
        }
        #endregion
    }
}