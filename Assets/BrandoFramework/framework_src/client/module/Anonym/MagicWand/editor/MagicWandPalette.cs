using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    [System.Serializable]
    public abstract class AbstractMagicWandPalette : ScriptableObject
    {
        public bool bMultiSelectable = true;
        public List<string> tags = new List<string>();

        public abstract void AddMagicWand(MagicWand newWand);
        public abstract List<MagicWand> GetMagicWands();
    }

    [System.Serializable]
    public abstract class MagicWandPalette<T> : AbstractMagicWandPalette where T : MagicWand
    {
        [SerializeField]
        protected List<T> MagicWands = new List<T>();
        protected void removeNull()
        {
            MagicWands.RemoveAll(t => t == null);
        }

        public override void AddMagicWand(MagicWand newWand)
        {
            if (!MagicWands.Contains(newWand as T))
                MagicWands.Add(newWand as T);
        }
        public override List<MagicWand> GetMagicWands()
        {
            removeNull();
            return MagicWands.Cast<MagicWand>().ToList();
        }
        public virtual IEnumerable<W> GetMagicWands<W>() where W : MagicWand
        {
            removeNull();
            return MagicWands.Cast<W>();
        }
    }

    [CreateAssetMenu(fileName = "New Tile Template Palette", menuName = "Anonym/Magic Wand/Tile Template Palette", order = 999)]
    public class MagicWandPalette : MagicWandPalette<MagicWand>
    {
        public static MagicWandPalette CreateAsset(string path, bool bOverride = false)
        {
            var _instance = ScriptableObject.CreateInstance<MagicWandPalette>();
            string _path = bOverride ? path : AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(_instance, _path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return _instance;
        }
    }
}
