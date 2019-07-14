using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
    public static class ColliderHelper
    {
        public static Bounds GetStatelessBounds(this BoxCollider boxCollider)
        {
            if (boxCollider == null)
                return new Bounds();

            if (boxCollider.enabled && boxCollider.bounds.size != Vector3.zero)
                return boxCollider.bounds;

            var transform = boxCollider.transform;
            var bounds = new Bounds(transform.TransformPoint(boxCollider.center), Vector3.zero);
            bounds.Encapsulate(transform.TransformPoint(boxCollider.center - boxCollider.size * 0.5f));
            bounds.Encapsulate(transform.TransformPoint(boxCollider.center + boxCollider.size * 0.5f));
            return bounds;
        }

        public static Collider fAboveGround(this GameObject passGameObject, Vector3 position, ref float fMaxOut, float _fMaxHeight = 10f,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            var _hits = Physics.RaycastAll(position, Vector3.down, _fMaxHeight, -1, queryTriggerInteraction).OrderBy(r => r.distance).GetEnumerator();

            while (_hits.MoveNext())
            {
                var _hit = (RaycastHit)_hits.Current;
                if (_hit.collider.gameObject != passGameObject)
                {
                    fMaxOut = Mathf.Max(fMaxOut, _hit.distance);
                    return _hit.collider;
                }
            }
            return null;
        }

        public static Collider fAboveGround(this Collider passCollider, Vector3 position, ref float fMaxOut, float _fMaxHeight = 10f,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            return passCollider.gameObject.fAboveGround(position, ref fMaxOut, _fMaxHeight, queryTriggerInteraction);
        }

        public static bool Intersect(this Bounds _bounds, Plane _plane)
        {
            Vector3 vMin = _bounds.min;
            Vector3 vMax = _bounds.max;
            bool bResult = _plane.GetSide(vMin);
            if (bResult != _plane.GetSide(vMax) ||
                bResult != _plane.GetSide(new Vector3(vMin.x, vMin.y, vMax.z)) ||
                bResult != _plane.GetSide(new Vector3(vMin.x, vMax.y, vMin.z)) ||
                bResult != _plane.GetSide(new Vector3(vMax.x, vMin.y, vMin.z)) ||
                bResult != _plane.GetSide(new Vector3(vMin.x, vMax.y, vMax.z)) ||
                bResult != _plane.GetSide(new Vector3(vMax.x, vMin.y, vMax.z)) ||
                bResult != _plane.GetSide(new Vector3(vMax.x, vMax.y, vMin.z)))
                return true;
            return false;
        }

        public static float DropToFloor(this Bounds _bound, GameObject _go, float _fMaxHeight = 10f, bool bDontMove = false, 
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            float fTipHeight = 0.1f;
            _bound.center += Vector3.up * fTipHeight;
            _bound.extents = Vector3.Scale(new Vector3(0.9f, 1, 0.9f), _bound.extents);
            Vector3[] vPoints = new Vector3[]{  new Vector3(_bound.center.x, _bound.min.y, _bound.center.z),
                new Vector3(_bound.min.x, _bound.min.y, _bound.min.z),  new Vector3(_bound.min.x, _bound.min.y, _bound.max.z),
                new Vector3(_bound.max.x, _bound.min.y, _bound.min.z),  new Vector3(_bound.max.x, _bound.min.y, _bound.max.z)};

            float fMax = 0;
            foreach (var position in vPoints)
            {
                _go.fAboveGround(position, ref fMax, _fMaxHeight, queryTriggerInteraction);
                // var col = _go.fAboveGround(position, ref fMax, _fMaxHeight, queryTriggerInteraction);
                //IsoTile tile = _go.GetComponentInParent<IsoTile>();
                //if (tile != null)
                //{
                //    Bounds _tile_bounds = tile.GetBounds_SideOnly();
                //    fDistanceToTriggerBox = position.y - _tile_bounds.max.y;
                //}
            }

            if (fMax >= 0)
                fMax -= fTipHeight;

            if (!bDontMove && fMax != 0)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(_go.transform, "Drop to flop!");
#endif
                _go.transform.Translate(0, -fMax, 0);
            }
            return fMax;
        }

        public static float DropToFloor(this Collider _col, GameObject _go = null, float _fMaxHeight = 10f, bool bDontMove = false,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            if (_col == null)
                return 0;

            if (_go == null)
                _go = _col.transform.root.gameObject;

            return DropToFloor(_col.bounds, _go, _fMaxHeight, bDontMove, queryTriggerInteraction);
        }

        public static float DropToFloor(this IsoTile _tile, float _fMaxHeight = 10f, bool bDontMove = false,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        {
            if (_tile == null || _tile.gameObject == null)
                return 0;

            var fResult = DropToFloor(_tile.GetBounds_SideOnly(), _tile.gameObject, _fMaxHeight, bDontMove, queryTriggerInteraction);
            _tile.coordinates.Apply_SnapToGrid();
            return fResult;
        }
    }
}