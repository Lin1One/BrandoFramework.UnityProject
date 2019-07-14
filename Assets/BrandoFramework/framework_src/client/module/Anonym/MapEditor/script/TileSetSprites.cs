using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{
    using Util;

    [System.Serializable]
    [CreateAssetMenu(fileName = "TileSet", menuName = "Anonym/TileSet", order = 600)]
    [HelpURL("https://hgstudioone.wixsite.com/isometricbuilder/isometric-sorting-order")]
    public class TileSetSprites : ScriptableObject
    {
        public static readonly bool bLogWhenMultiProperSprite = false;

        [SerializeField]
        public Sprite baseSprite;

        [SerializeField]
        public List<SubTileSet> subTiles =  new List<SubTileSet>();

        [SerializeField, HideInInspector]
        int iLastIndex = 0;

        public SubTileSet LookupedSubTileSet
        {
            get
            {
                if (iLastIndex < 0 || iLastIndex >= subTiles.Count)
                    return null;
                return subTiles[iLastIndex];
            }
        }

        public bool Contain(TileSetSprites outfield)
        {
            if (outfield == null || subTiles == null || subTiles.Count == 0)
                return false;
            return subTiles.Exists(Match(outfield));
        }

        public bool Contain(SubTileSet _sub)
        {
            if (_sub == null || subTiles == null || subTiles.Count == 0)
                return false;
            return subTiles.Exists(Match(_sub));
        }

        public bool Erase(SubTileSet _sub)
        {
            if (Contain(_sub))
            {
                subTiles.Remove(_sub);
                EraseNull();
                _sub.DeleteAsset();
                return true;
            }

            return false;
        }

        public void EraseNull()
        {
            subTiles.RemoveAll(s => s == null);
        }

        public bool Erase(TileSetSprites outfield)
        {
            if (Contain(outfield))
            {
                var index = subTiles.FindIndex(Match(outfield));
                if (index >= 0 && index < subTiles.Count)
                    return Erase(subTiles[index]);
            }

            return false;
        }

        public SubTileSet[] Get(TileSetSprites outfield)
        {
            if (Contain(outfield))
                return subTiles.FindAll(Match(outfield)).ToArray();

            return null;
        }

        public SubTileSet Add(TileSetSprites outfield, bool bAllowNullOutField = false)
        {
#if UNITY_EDITOR
            var result = SubTileSet.CreateAsset(this, outfield == null ? "SubTileSet" : outfield.name);
            if (result)
            {
                result.OutField = outfield;
                subTiles.Add(result);
            }
            return result;
#else
            return null;
#endif
        }

        public bool IsRelative(TileSetSprites _compareWith)
        {
            if (_compareWith == this)
                return true;

            bool bResult = subTiles.Exists(s => s.IsRelative(_compareWith));
            return bResult;
        }

        /// <summary>
        /// Send Tags of neighborhood Tiles as parameter.
        /// (T_Tag, TL_Tag, TR_Tag, L_Tag, R_Tag, DL_Tag, DR_Tag, D_Tag)
        /// </summary>
        public bool Apply(IsoTile _target, Dictionary<InGameDirection, TileSetSprites> neighbours, bool bRecordForUndo = true, string undoName = "Update: TileSet")
        {
            Dictionary<SubTileSet, InGameDirection> nominees = new Dictionary<SubTileSet, InGameDirection>();
            _target.tileSetSprites = this;
            
            foreach (var one in subTiles)
            {
                if (one == null)
                    continue;

                var nominee = one.FindProperDir(neighbours);
                if (nominee == InGameDirection.None)
                    continue;

                nominees.Add(one, nominee);
            }

            if (nominees.Count == 0)
            {
                var priorityTileSet = subTiles.Find(s => s.tileSetType == SubTileSet.Type.Normal);
                if (priorityTileSet != null)
                {
                    var NeighbourInfo = priorityTileSet.ConvertNeighbourInfo(neighbours);

                    //마주닿아 있는 대각선 방향에 BaseField 또는 OutField가 있는데도 대상을 찾을 수 없는 경우.
                    if (SubTileSet.Neighbors_Check_Any(SubTileSet.matchType.BaseField, false, NeighbourInfo, SubTileSet.DiagonalDirections)
                        || SubTileSet.Neighbors_Check_Any(SubTileSet.matchType.OutField, false, NeighbourInfo, SubTileSet.DiagonalDirections))
                    {
                        nominees.Add(priorityTileSet, InGameDirection.BaseField);
                    }
                }
            }

            if (nominees.Count > 0)
            {
#if UNITY_EDITOR
                if (bLogWhenMultiProperSprite && nominees.Count > 1)
                    Debug.LogWarning("Two or more satisfied conditions have been detected in the SubTileSet. : " + string.Join(" ", nominees.Select(r => r.Key.name).ToArray()));
#endif
                var winner = nominees.Aggregate((l, r) => l.Key.iPriority > r.Key.iPriority ? l : r);
                return winner.Key.ApplyShape(_target, winner.Value, false, bRecordForUndo, undoName);
            }

            _target.ChangeBaseSprite(baseSprite, false, bRecordForUndo, undoName);
            return false;
        }

        static System.Predicate<SubTileSet> Match(TileSetSprites outfield)
        {
            return s => outfield != null && s != null && s.OutField == outfield;
        }

        static System.Predicate<SubTileSet> Match(SubTileSet subTileSet)
        {
            return s => subTileSet != null && s != null && s == subTileSet;
        }
    }
}