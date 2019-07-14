using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{
    public static class IsometricUtility
    {
        #region IsoTile
        public static bool Press(this IsoTile tile, Vector3 _direction)
        {
            if (tile.IsLastTile(-_direction) && !tile.IsLastTile(_direction))
            {
                IsoTile _removeTile = tile.NextTile(_direction);
                tile.coordinates.Translate(_direction);

                if (_removeTile != null)
                {
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(_removeTile.gameObject);
#else
                    GameObject.DestroyImmediate(_removeTile.gameObject);
#endif
                }
            }
            return true;
        }

        public static IsoTile Extrude(this IsoTile tile, Vector3 _direction, bool _bWithAttachment)
        {
            if (tile.IsLastTile(_direction))
            {
                tile.coordinates.Translate(_direction);
                if (!tile.IsAccumulatedTile_Collider(-_direction))
                {
                    return tile.extrude(-_direction, false, _bWithAttachment);
                }
            }
            return null;
        }

        public static IsoTile Extrude_Separately(this IsoTile tile, Vector3 _direction, bool _bWithAttachment)
        {
            var lastTile = tile.Extrude(_direction, true);
            int iPReventLoop = tile.coordinates.grid.CoordinatesCountInTile(_direction) - 1;
            tile.coordinates.Translate(Vector3.up * iPReventLoop);

            return lastTile;
            //IsoTile lastTile = null;
            //while(iPReventLoop-- > 0)
            //{
            //    lastTile = tile.Extrude(_direction, _bWithAttachment);
            //    //이번 업데이트에 생긴 오브젝트는 콜리더/바운드 체크가 잘 되지 않는다.
                
            //    //Duplicate 하고 움직이지 말고, 움직이고 Duplicate하면 안되나?
            //    // 그럼 언제까지 움직이는데?

            //    //그냥 계산 된 높이 까지 올리는 로직으로 대체.
            //    if (lastTile)
            //    {
            //        // 타일이 생성되면 중첩되지 않을 만큼 공간을 더 벌린다.
            //        while(tile.IsAccumulatedTile_Collider(-_direction))
            //        {
            //            tile.coordinates.Translate(_direction);
            //        }
            //        tile.coordinates.Translate(-_direction);
            //        break;
            //    }
            //}
            //return lastTile;
        }

        static IsoTile extrude(this IsoTile tile, Vector3 _direction, bool _bContinuously, bool _withAttachment)
        {
            const string undoName = "IsoTile: Extrude";
            IsoTile _new = tile.Duplicate();
            if (!_withAttachment)
                _new.Clear_Attachment(false);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(_new.gameObject, undoName);
#endif
            _new.coordinates.Translate(_direction, undoName);
#if UNITY_EDITOR
            Undo.RecordObject(tile.gameObject, undoName);
#endif            
            IsoTile.UpdateTileSet(_new, false, true, undoName);
            return _new;
        }

        public static IsoTile FindTop(this IsoTile tile)
        {
            var result = tile.Bulk.GetTiles_At(tile.coordinates._xyz, Vector3.up, true, true);
            return result.Count == 0 ? tile : result.Last();
        }

        public static IsoTile FindBottom(this IsoTile tile)
        {
            var result = tile.Bulk.GetTiles_At(tile.coordinates._xyz, Vector3.down, true, true);
            return result.Count == 0 ? tile : result.Last();
        }

        public static IsoTile Duplicate(this IsoTile tile)
        {
            IsoTile result = GameObject.Instantiate(tile);
            result.transform.SetParent(tile.transform.parent, false);
#if UNITY_EDITOR
            result.Rename();
            Undo.RegisterCreatedObjectUndo(result.gameObject, "IsoTile:Dulicate");
#endif
            return result;
        }

        public static bool IsLastTile(this IsoTile tile, Vector3 _direction)
        {
            if (tile.Bulk)
                return tile.Bulk.GetTiles_At(tile.coordinates._xyz, _direction, false, true).Count() == 0;

            return IsoTile.GetTile_At_OverlapBox(tile.GetBounds_SideOnly()).Count() == 0;
        }

        public static IsoTile NextTile(this IsoTile tile, Vector3 _direction)
        {
            IEnumerable<IsoTile> _tiles = tile.Bulk.GetTiles_At(tile.coordinates._xyz, _direction, false, false);
            return (_tiles.Count() > 0) ? _tiles.First() : null;
        }

        public static T FindComponentInParent<T>(this GameObject start) where T: Component
        {
#if UNITY_EDITOR
            for (Transform t = start.transform; t != null; t = t.parent)
            {
                T test = t.GetComponent<T>();
                if (test != null)
                    return test;
            }
            return null;
#else
            return start.GetComponentInParent<T>();
#endif
        }
        #endregion
    }
}