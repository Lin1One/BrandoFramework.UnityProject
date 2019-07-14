using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
    [ExecuteInEditMode]
	[DisallowMultipleComponent]
    public class SubColliderHelper : MonoBehaviour
    {
        BoxCollider _box;
		public BoxCollider BC   {   get {   return _box != null ? _box : (_box = GetComponent<BoxCollider>());  }   }
        public Bounds GlobalBounds  {   get {   return BC.GetStatelessBounds();    }   }

#if UNITY_EDITOR
        public static Vector3 V3positiveInfinity
        {
            get
            {
                return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            }
        }
        [SerializeField, HideInInspector]
		bool bApplyGridScale = true;

        [SerializeField, HideInInspector]
        Vector3 _localScale = V3positiveInfinity;
		Vector3 LocalScale{get{
				if (_localScale.Equals(V3positiveInfinity))
				{
					_localScale = BC.size;
				}
				return _localScale;
			}
		}
		[SerializeField, HideInInspector]
		Vector3 _localPosition = V3positiveInfinity;
		Vector3 LocalPosition{get{
			if (_localPosition.Equals(V3positiveInfinity))
				_localPosition = transform.localPosition;
			return _localPosition;
		}}
		[SerializeField, HideInInspector]
		Vector3 _localCenter = V3positiveInfinity;
		Vector3 LocalCenter{get{
			if (_localCenter.Equals(V3positiveInfinity))
				_localCenter = BC.center;
			return _localCenter;
		}}
		[SerializeField, HideInInspector]
		Vector3 _LastTileScale = Vector3.one;

		public virtual void Toggle_UseGridTileScale(bool _bApplyGridScale)
		{
			if (BC == null)
			{
				bApplyGridScale = _bApplyGridScale;
				return;
			}

			UnityEditor.Undo.RecordObject(this, "Update SubCollider");
			if (!_bApplyGridScale)
			{
				_localPosition = transform.localPosition;
				_localCenter = BC.center;
				_localScale = BC.size;					
			}
			else
			{
				_localPosition = new Vector3(transform.localPosition.x / _LastTileScale.x , 
					transform.localPosition.y / _LastTileScale.y, transform.localPosition.z / _LastTileScale.z);
				_localCenter = new Vector3(BC.center.x / _LastTileScale.x , 
					BC.center.y / _LastTileScale.y, BC.center.z / _LastTileScale.z);
				_localScale = new Vector3(BC.size.x / _LastTileScale.x , 
					BC.size.y / _LastTileScale.y, BC.size.z / _LastTileScale.z);
			}
			bApplyGridScale = _bApplyGridScale;

			ScaleMultiplier(_LastTileScale);
		}
		public void ScaleMultiplier(Vector3 _tileSize)
		{
			if (BC != null)
			{				
				UnityEditor.Undo.RecordObject(BC, "Update SubCollider");
				_LastTileScale = _tileSize;
				transform.localPosition = bApplyGridScale ? Vector3.Scale(LocalPosition, _LastTileScale) : LocalPosition;
				BC.size = bApplyGridScale ? Vector3.Scale(LocalScale, _LastTileScale) : LocalScale;
				BC.center = bApplyGridScale ? Vector3.Scale(LocalCenter, _LastTileScale) : LocalCenter;
			}
		}

        public void ReParent(float fFindRange = 1)
        {
            Bounds _bound = GlobalBounds;
            var position = GlobalBounds.center - Vector3.up * GlobalBounds.size.y * 0.5f;
            var cols = Physics.OverlapSphere(position, fFindRange);
            var tiles = cols.Select(r => {
                var com = r.GetComponent<RegularCollider>();
                return com != null ? com.Tile : null;
            }).Where(r => r!= null).Distinct();
            if (tiles != null)
            {
                float fMin = float.MaxValue;
                IsoTile result = null;
                var enumerator = tiles.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    Bounds bounds = current.GetBounds_SideOnly();
                    Vector3 pos2 = bounds.center + Vector3.up * bounds.size.y * 0.5f;
                    float fdistance = Vector3.Distance(position, pos2);
                    if (fdistance < fMin)
                    {
                        fMin = fdistance;
                        result = current;
                    }
                }
                if (result != null)
                {
                    if (transform.IsChildOf(result.transform))
                        return;

                    UnityEditor.Undo.SetTransformParent(transform, result.transform, "Attachment: Change Tile");
                    UnityEditor.Undo.RecordObject(transform, "Attachment: Change Tile");
                    transform.SetParent(result.transform, true);
                    UnityEditor.EditorGUIUtility.PingObject(result.gameObject);
                }
            }
        }

        virtual protected void OnDrawGizmosSelected()
        {
            if (BC == null)
                return;

            GameObject target = UnityEditor.Selection.activeObject as GameObject;
            if (target == BC.gameObject)
            {
                IsoTile _tile = GetComponentInParent<IsoTile>();
                if (_tile == null || 
                    !_tile.GetSideObjects(Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_X, Iso2DObject.Type.Side_Y, Iso2DObject.Type.Side_Z).All(r => target != r.SC.gameObject))
                    return;

                Gizmos.color = new Color(0.95f, 0.5f, 0.05f, 0.65f);
                Bounds _bounds = GlobalBounds;
                Bounds _tile_bounds = _tile.GetBounds_SideOnly();
                Gizmos.DrawWireCube(_tile_bounds.center, _tile_bounds.size);
                _bounds.center = new Vector3(_bounds.center.x, _tile_bounds.max.y, _bounds.center.z);
                _bounds.size = new Vector3(_bounds.size.x, 0, _bounds.size.z);
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }

        public void Gizmo_SimpleBounds()
        {
            if (BC != null)
            {
                Bounds _bounds = GlobalBounds;
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
        }
#endif
    }
}