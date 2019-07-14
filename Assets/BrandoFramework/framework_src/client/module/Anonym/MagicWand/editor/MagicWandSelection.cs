using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Util
{
    // [CreateAssetMenu(fileName = "[Selection] New", menuName = "Anonym/Magic Wand/Selection", order = 1002)]
    public class MagicWandSelection : ScriptableObject
    {
        public List<MagicWand> MagicWands = new List<MagicWand>();

        public bool Save(string assetPath, string assetName)
        {

            return false;
        }
    }
}
