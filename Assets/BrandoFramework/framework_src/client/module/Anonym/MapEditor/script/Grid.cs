using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
	using Util;
    [DisallowMultipleComponent]
    [System.Serializable]
    [ExecuteInEditMode]
    public class Grid : MonoBehaviour, IGridOperator
    {
        #region ForGridMovement
        [SerializeField, HideInInspector]
        bool bUseLocalGrid = true;

        [HideInInspector]
        Grid _parentGrid;
        [ConditionalHide("!bUseLocalGrid", hideInInspector:true)]
        [SerializeField]
        public Grid parentGrid
        {
            get
            {
                if (_parentGrid == null && transform.parent != null)
                    _parentGrid = transform.parent.GetComponent<Grid>();
                if (_parentGrid == null && !IsoMap.IsNull)// && gameObject != IsoMap.instance.gameObject)
                    _parentGrid = IsoMap.instance.gGrid;

                return _parentGrid;
            }
        }

        #region IGridOperator
        [ConditionalHide("bUseLocalGrid", hideInInspector: true)]
        public bool IsInheritGrid { get { return !bUseLocalGrid; } }// && parentGrid != null; } }

        [ConditionalHide("bUseLocalGrid", hideInInspector: true)]
        [SerializeField]
        Vector3 _TileSize = Vector3.one;
        public Vector3 TileSize
        {
            get
            {
                if (bUseLocalGrid)
                    return _TileSize;

                if (parentGrid)
                    return parentGrid.TileSize;

                return CalcEstimatedTileSize();
            }
        }

        [System.NonSerialized]
        Vector3 _vEstimatedTileSize = Vector3.zero;
        [System.NonSerialized]
        Vector3 _vEstimatedGridInterval = Vector3.zero;

        void ResetEstimatedValue()
        {
            _vEstimatedTileSize = Vector3.zero;
            _vEstimatedGridInterval = Vector3.zero;
        }
        Vector3 CalcEstimatedTileSize()
        {
            if (_vEstimatedTileSize != Vector3.zero)
                return _vEstimatedTileSize;

            _vEstimatedTileSize = Vector3.one;
            var _tiles = GetComponentsInChildren<IsoTile>().GetEnumerator();

            while (_tiles.MoveNext())
            {
                var _tile = _tiles.Current as IsoTile;
                _vEstimatedTileSize = Vector3.Min(_vEstimatedTileSize, _tile.GetBounds_SideOnly().size);
            }
            return _vEstimatedTileSize;
        }
        Vector3 CalcEstimatedGridInterval()
        {
            if (_vEstimatedGridInterval != Vector3.zero)
                return _vEstimatedGridInterval;

            _vEstimatedGridInterval = CalcEstimatedTileSize();
            var _tiles = GetComponentsInChildren<IsoTile>().GetEnumerator();
            if (!_tiles.MoveNext())
                return _vEstimatedGridInterval;

            IsoTile _lastTile = _tiles.Current as IsoTile;
            bool bFindX = false, bFindY = false, bFindZ = false;

            while (_tiles.MoveNext())
            {
                var _tile = _tiles.Current as IsoTile;
                Vector3 _vCoordinatesDiff = _tile.coordinates._xyz - _lastTile.coordinates._xyz;
                Vector3 _vPositionDiff = _tile.transform.position - _lastTile.transform.position;

                if (!bFindX && _vCoordinatesDiff.x != 0)
                {
                    _vEstimatedGridInterval.x = Mathf.Abs(_vPositionDiff.x);
                    bFindX = true;
                }
                if (!bFindY && _vCoordinatesDiff.y != 0)
                {
                    _vEstimatedGridInterval.y = Mathf.Abs(_vPositionDiff.y);
                    bFindY = true;
                }
                if (!bFindZ && _vCoordinatesDiff.z != 0)
                {
                    _vEstimatedGridInterval.z = Mathf.Abs(_vPositionDiff.z);
                    bFindZ = true;
                }

                if (bFindX && bFindY && bFindZ)
                    break;
            }
            return _vEstimatedGridInterval;
        }        

        [ConditionalHide("bUseLocalGrid", hideInInspector: true)]
        [SerializeField]
        Vector3 _GridInterval = new Vector3(1f, 1f / 3f, 1f);
        public Vector3 GridInterval
        {
            get
            {
                if (bUseLocalGrid)
                    return Vector3.Scale(TileSize, _GridInterval);

                if (parentGrid)
                    return parentGrid.GridInterval;

                return CalcEstimatedGridInterval();
            }
        }

        public int CoordinatesCountInTile(Vector3 _direction)
        {
            Vector3 result = Vector3.Scale(_direction, TileSize);
            Vector3 size = GridInterval;
            return Mathf.Abs(Mathf.RoundToInt(result.x / size.x + result.y / size.y + result.z / size.z));
        }
        public Vector3 CoordinatesToPosition(Vector3 coordinates, bool bSnap = false)
        {
            if (bSnap)
                coordinates = RoundToIntVector(coordinates);

            coordinates.Scale(GridInterval);

            //coordinates.Scale(GridInterval);

            //if (bSnap)
            //    coordinates = RoundToIntVector(coordinates);

            return coordinates;
        }
        public Vector3 PositionToCoordinates(Vector3 globalPosiion, bool bSnap = false)
        {
            globalPosiion.x = globalPosiion.x / GridInterval.x;
            globalPosiion.y = globalPosiion.y / GridInterval.y;
            globalPosiion.z = globalPosiion.z / GridInterval.z;

            if (bSnap)
                globalPosiion = RoundToIntVector(globalPosiion);

            return globalPosiion;
        }
        #endregion


        public Vector3 SnapedPosition(Vector3 position, bool bIsGlobalPosition = false)
        {
            Vector3 vGap = bIsGlobalPosition ? position - transform.position : Vector3.zero;
            return CoordinatesToPosition(PositionToCoordinates(position - vGap, true), false) + vGap;
        }
        public static Vector3 RoundToIntVector(Vector3 vector)
        {
            vector.x = Mathf.RoundToInt(vector.x);
            vector.y = Mathf.RoundToInt(vector.y);
            vector.z = Mathf.RoundToInt(vector.z);
            return vector;
        }
        
        public static float fGridTolerance = 0.01f;
        #endregion 
#if UNITY_EDITOR

        [HideInInspector]
		GridCoordinates _coordinates;
		[HideInInspector]
		public GridCoordinates coordinates{get{
			return _coordinates == null ?
				_coordinates = GetComponent<GridCoordinates>() : _coordinates;
		}}

        public Vector3 Centor
        {
            get{
                if (IsInheritGrid)
                {
                    Vector3 v3Result = new Vector3();
                    v3Result.x = transform.localPosition.x / parentGrid.GridInterval.x;
                    v3Result.y = transform.localPosition.y / parentGrid.GridInterval.y;
                    v3Result.z = transform.localPosition.z / parentGrid.GridInterval.z;
                    //v3Result -= parentGrid.Centor;
                    return v3Result;
                }
                //Debug.Log("Grid(" + gameObject.name + ") Centor : " + v3Result);
                return transform.position;
            }
        }
        
        public bool bChildUpdatedFlagForEditor = false;
        void OnTransformChildrenChanged()
		{
			bChildUpdatedFlagForEditor = true;
		} 

        public void Sync(Grid with)
        {
            bUseLocalGrid = with.bUseLocalGrid;
            _TileSize = with._TileSize;
            _GridInterval = with._GridInterval;
        }

        private void OnValidate()
        {
            ResetEstimatedValue();
        }
#endif
    }
}