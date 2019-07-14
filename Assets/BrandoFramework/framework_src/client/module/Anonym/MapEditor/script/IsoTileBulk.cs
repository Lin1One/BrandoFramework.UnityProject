using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{
    [System.Serializable]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Grid))]
    [ExecuteInEditMode]
    public class IsoTileBulk : MonoBehaviour {
        #region ForGridMovement
        [SerializeField]
        GridCoordinates _coordinates;
        [HideInInspector]
        public GridCoordinates coordinates
        {
            get
            {
                return _coordinates == null ?
                    _coordinates = GetComponent<GridCoordinates>() : _coordinates;
            }
        }
        #endregion

        public IEnumerable<IsoTile> GetAllTiles()
        {
#if UNITY_EDITOR
            return _attachedList;
#else
            return GetComponentsInChildren<IsoTile>();
#endif
        }

        public IEnumerable<IsoTile> GetTiles_At(Vector3 atCoordinates)
        {
            Vector3 _position = coordinates.grid.transform.position + coordinates.CoordinatesToPosition(atCoordinates);
            var nominess = IsoTile.GetTile_At_OverlapBox(_position, coordinates.grid.TileSize * 0.125f);
            return nominess.Where(t => t != null && GridCoordinates.IsSameWithTolerance(t.coordinates._xyz, atCoordinates));
        }

        public List<IsoTile> GetTiles_At(Vector3 startingCoordinates,
            Vector3 _direction, bool bContinuously, bool bApplyRefTileSize)
        {
            if (bContinuously)
                return GetTiles_At_Continuously(startingCoordinates, _direction, bApplyRefTileSize);

            List<IsoTile> _tiles = new List<IsoTile>();
            int iSubGridCount = !bApplyRefTileSize ? 1 : coordinates.CoordinatesCountInTile(_direction);
            for (int j = 0; j < iSubGridCount; j++)
            {
                startingCoordinates += _direction;
                _tiles.AddRange(GetTiles_At(startingCoordinates));
            }
            _tiles = _tiles.Distinct().ToList();
            _tiles.Remove(null);
            return _tiles;
        }

        List<IsoTile> GetTiles_At_Continuously(Vector3 startingCoordinates, Vector3 _direction, bool bApplyRefTileSize)
        {
            var _bulkTiles = GetAllTiles();
            int iSubGridCount = !bApplyRefTileSize ? 1 : coordinates.CoordinatesCountInTile(_direction);

            Plane plane = new Plane(_direction, startingCoordinates);
            Ray ray = new Ray(startingCoordinates, _direction.normalized);
            var _tileEnumerator = _bulkTiles.Where(t =>
            {
                var _coordinates = t.coordinates._xyz;
                return plane.GetSide(_coordinates) && (Vector3.Cross(ray.direction, _coordinates - ray.origin).magnitude < Grid.fGridTolerance);
            }).OrderBy(t =>
            {
                return Vector3.Distance(t.coordinates._xyz, startingCoordinates);
            });

            bool bSeparated = false;
            Vector3 lastCoordinates = startingCoordinates;
            return _tileEnumerator.TakeWhile(t =>
            {
                if (bSeparated)
                    return false;

                var _coordinates = t.coordinates._xyz;
                bSeparated = Vector3.Distance(lastCoordinates, _coordinates) > iSubGridCount;
                lastCoordinates = _coordinates;
                return true;
            }).ToList();
        }

#if UNITY_EDITOR
        [SerializeField]
        public Rect SizeXZ;

        [HideInInspector, SerializeField]        
        public List<IsoTile> _attachedList;

        [SerializeField]
        bool bFrozen = false;

        [SerializeField]
        GameObject _referenceTile;
        public GameObject RefTile { get { return _referenceTile; } }

        [SerializeField]
        bool _bSensorOff_ChildChange = false;
        public void Do_With_SensorOff(System.Action _underAction)
        {
            _bSensorOff_ChildChange = true;
            try{
                _underAction();
            }catch{ }
            _bSensorOff_ChildChange = false;
        }
        Vector3 lastPosition = new Vector3();
        void Update()
		{
			if (!Application.isEditor || Application.isPlaying || !enabled)
				return;

            if (!lastPosition.Equals(transform.position))
            {
                lastPosition = transform.position;
                for(int i = 0 ; i < _attachedList.Count; ++i)
                {
                    if (_attachedList[i] != null)
                    {
                        _attachedList[i].Update_SortingOrder();
                        // 수정
                    }
                }
            }
        }
        public bool bAllowEmptyBulk = false;
        bool preventZeroTileBulk()
        {
            if (!bAllowEmptyBulk && transform.childCount == 0)
            {
                NewTile(Vector3.zero);
                Update_ChildList();
                EditorUtility.SetDirty(this);
                return true;
            }
            return false;
        }

        void OnValidate()
        {
            if (isActiveAndEnabled)
                IsoMap.Regist(this);
        }
        private void OnEnable()
        {
            IsoMap.Regist(this);
        }
        void OnTransformParentChanged()
        {
            if (!_bSensorOff_ChildChange)
                preventZeroTileBulk();
        }
        void OnTransformChildrenChanged()
        {
            if (!_bSensorOff_ChildChange)
            {
                if (!preventZeroTileBulk())
                    Update_ChildList();
            }
        }

        public void Update_ChildList(bool bSort = true)
        {
            _attachedList.Clear();
            SizeXZ_Clear();
            for(int i = 0 ; i < transform.childCount; ++i)
            {
                IsoTile _obj = transform.GetChild(i).GetComponent<IsoTile>();
                if (_obj != null)
                {
                    _attachedList.Add(_obj);
                    SizeXZ_Update(_obj.coordinates);
                }
            }
            if (bSort)
                Sort();
        }

        public void Update_Grid()
        {
            for(int i = 0 ; i < _attachedList.Count; ++i)
            {
                if (_attachedList[i] != null)
                {
                    _attachedList[i].Update_Grid();
                }
            }
        }

        public bool bYFirstSort;
        public void Sort(bool bToggle = false)
        {            
            _attachedList.Sort((r1, r2) => 
            {   
                Vector3 r1Coordinates = r1.coordinates._xyz;
                Vector3 r2Coordinates = r2.coordinates._xyz;
                int iResult;                
                if (bYFirstSort && ((iResult = r2Coordinates.y.CompareTo(r1Coordinates.y)) != 0))
                    return iResult;
                if ((iResult = r2Coordinates.x.CompareTo(r1Coordinates.x)) != 0)
                    return iResult;
                if (!bYFirstSort && ((iResult = r2Coordinates.y.CompareTo(r1Coordinates.y)) != 0))
                    return iResult;
                return r2Coordinates.z.CompareTo(r1Coordinates.z);
            });
            if (bToggle)
                bYFirstSort = !bYFirstSort;
        }
        public void addToAttachedList(IsoTile tile)
        {
            if (_attachedList.Contains(tile))
                return;

            int i = 0;
            for(; i < _attachedList.Count; ++i)
            {
                Vector3 va = tile.coordinates._xyz;
                Vector3 vb = _attachedList[i].coordinates._xyz;

                if (bYFirstSort)
                {
                    if (va.y > vb.y || 
                        (va.y == vb.y && (va.x > vb.x || 
                        (va.x == vb.x && va.z > vb.z))))
                        break;
                }
                else
                {
                    if (va.x > vb.x ||
                        (va.x == vb.x && (va.y > vb.y ||
                        (va.y == vb.y && va.z > vb.z))))
                        break;
                }
            }
            _attachedList.Insert(i, tile);
        }

        public void Flat()
        {
            Undo.RecordObject(this, "Bulk : Flat");
            List<IsoTile> _DelList = new List<IsoTile>();
            for (int i = _attachedList.Count - 1 ; i >= 0; --i)
            {
                if (!_attachedList[i].IsLastTile(Vector3.up))
                {
                    _DelList.Add(_attachedList[i]);
                    _attachedList.RemoveAt(i);
                }
            }
            _attachedList.ForEach(r => r.MoveToZeroground());
            Do_With_SensorOff(() => _DelList.ForEach(r => Undo.DestroyObjectImmediate(r.gameObject)));
        }

        public void Clear()
        {
            Do_With_SensorOff(() =>
            {
                Undo.RecordObject(this, "Bulk : Clear");
                for (int i = _attachedList.Count - 1 ; i >= 0; --i)
                {
                    IsoTile _tile = _attachedList[i];
                    if (_tile == null)
                        continue;

                    GameObject _go = _tile.gameObject;
                    if (_go == null)
                        continue;
#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
                    if (Util.PrefabHelper.IsPrefab(_go))
                    {
                        Debug.LogWarning("In Unity 2018.3 or later, To modify the game object of Prefab, you must open and modify the Prefab. That is new Prefab Work Flow.");
                        continue;
                    }
#endif
                    Undo.DestroyObjectImmediate(_go);
                }
                _attachedList.RemoveAll(a => a == null || a.gameObject == null);
            });
            preventZeroTileBulk();
        }

        public void Earthquake(float yMinDef, float yMaxDef)
        {
            int yMin = Mathf.RoundToInt(SizeXZ.yMin);
            int yMax = Mathf.RoundToInt(SizeXZ.yMax);
            int xMin = Mathf.RoundToInt(SizeXZ.xMin);
            int xMax = Mathf.RoundToInt(SizeXZ.xMax);

            for(int z = yMin ; z <= yMax ; ++z)
            {
                for(int x = xMin ; x <= xMax ; ++x) 
                {
                    int _iHeighDef = Mathf.RoundToInt(UnityEngine.Random.Range(yMinDef, yMaxDef));
                    _attachedList.ForEach(r =>{
                        if (r.coordinates.IsSame(new Vector3(x,0,z), true, false, true))
                        {
                            r.coordinates.Translate(0, _iHeighDef, 0);
                        }
                    });
                }
            }
        }

        public bool NoRedundancy()
        {
            Undo.RecordObject(this, "Bulk : No Redundancy");
            List<IsoTile> _UniqueList = new List<IsoTile>();
            List<IsoTile> _DelList = new List<IsoTile>();
            for (int i = _attachedList.Count - 1 ; i >= 0; --i)
            {
                IsoTile _lookupTile = _attachedList[i];
                IsoTile _RedundantTile = 
                    _UniqueList.Find(r => r.coordinates.IsSame(_lookupTile.coordinates._xyz));
                if (_RedundantTile != null)
                {
                    if (_lookupTile.GetComponentsInChildren<Iso2DObject>().Length > 
                        _RedundantTile.GetComponentsInChildren<Iso2DObject>().Length)
                    {
                        _UniqueList.Remove(_RedundantTile);
                        _RedundantTile = null;
                    }
                }
                _UniqueList.Add(_lookupTile);
                if (_RedundantTile != null)
                    _DelList.Add(_RedundantTile);
            }
            bool bResult = _DelList.Count > 0;
            Do_With_SensorOff(() => _DelList.ForEach(r => Undo.DestroyObjectImmediate(r.gameObject)));
            return bResult;
        }

        void SizeXZ_Clear()
        {
            SizeXZ.x = SizeXZ.y = SizeXZ.width = SizeXZ.height = 0;
        }
        void SizeXZ_Update(GridCoordinates _GC)
        {
            Vector3 coordV3 = _GC._xyz;
            Vector2 coord = new Vector2(coordV3.x , coordV3.z);
            if (!SizeXZ.Contains(coord))
            {
                SizeXZ = Rect.MinMaxRect(
                    Mathf.Min(coord.x, SizeXZ.xMin),
                    Mathf.Min(coord.y, SizeXZ.yMin),
                    Mathf.Max(coord.x, SizeXZ.xMax),
                    Mathf.Max(coord.y, SizeXZ.yMax));               
            }
        }
        public List<IsoTileBulk> GetChildBulkList(bool bLocalBulkOnly)
        {
            return GetComponentsInChildren<IsoTileBulk>().Where(b => b != this && (!bLocalBulkOnly || b.coordinates.IsInheritGrid)).ToList();
        }
        
        public Bounds GetBounds()
        {
            Bounds bounds = new Bounds();
            GetChildBulkList(true).Select(b => b.GetBounds()).ToList().ForEach(b => bounds.Encapsulate(b));
            GetAllTiles().Select(t => t.GetBounds_SideOnly()).ToList().ForEach(b => bounds.Encapsulate(b));
            return bounds;
        }

        public void Resize(Rect _reSize, bool _bFillIn)
        {
            Undo.RecordObject(this, "Bulk:Resize");
            
            Vector3 _xyz = new Vector3();
            int yMin = Mathf.RoundToInt(_reSize.yMin);  int yMax = Mathf.RoundToInt(_reSize.yMax);
            int xMin = Mathf.RoundToInt(_reSize.xMin);  int xMax = Mathf.RoundToInt(_reSize.xMax);
            bool bExtention = _reSize.xMin < SizeXZ.xMin || _reSize.yMin < SizeXZ.yMin
                || _reSize.xMax > SizeXZ.xMax || _reSize.yMax > SizeXZ.yMax;

            Do_With_SensorOff(() => {
                for(int z = yMin ; z <= yMax ; ++z){
                    bool bZEdige = z == yMin || z == yMax;
                    for(int x = xMin ; x <= xMax ; ++x){
                        bool bXEdige = x == xMin || x == xMax;
                        if (bExtention && (bZEdige || bXEdige)){
                            _xyz.Set(x, 0, z);                        
                            if (!_attachedList.Exists(r => r.coordinates.IsSame(_xyz, true, false, true)))
                                NewTile(_xyz);
                        }
                    }
                }
                SizeXZ = _reSize;
                for(int i = 0 ; i < _attachedList.Count; ++i){
                    if (_attachedList[i] == null || _attachedList[i].gameObject == null)
                        continue;
                    Vector3 _coord = _attachedList[i].coordinates._xyz;                
                    if (_coord.x < SizeXZ.xMin || _coord.x > SizeXZ.xMax ||
                        _coord.z < SizeXZ.yMin || _coord.z > SizeXZ.yMax ){
                        Undo.DestroyObjectImmediate(_attachedList[i].gameObject);                
                    }
                }
            });
        }

        public IsoTile NewTile(Vector3 _at, bool _isCoordinates = true, IsoTile _connectedPtrfab = null, bool bUseNewPrefabWorkflow = false)
        {
            const string UndoName = "IsoTile:Create";
            IsoTile _newTile = null;
            IsoTile _baseTile = _connectedPtrfab ? _connectedPtrfab : (_referenceTile ? _referenceTile : IsoMap.instance.TilePrefab).GetComponent<IsoTile>();

            if (bUseNewPrefabWorkflow)
                _newTile = PrefabUtility.InstantiatePrefab(_baseTile) as IsoTile;
            else
            {
                _newTile = GameObject.Instantiate(_baseTile).GetComponent<IsoTile>();
                Undo.RegisterCreatedObjectUndo(_newTile.gameObject, UndoName);			
            }

            if (!_newTile)
                return null;

            if (!_isCoordinates)
                _at = coordinates.PositionToCoordinates(_at);

            _newTile.transform.SetParent(transform, false);
            _newTile.coordinates.Move(_at);
            _newTile.Init();
            _newTile.Rename();
            SizeXZ_Update(_newTile.coordinates);
            addToAttachedList(_newTile);
            IsoTile.UpdateTileSet(_newTile, false, true, UndoName);
            return _newTile;
        }

        public void Rename()
        {
            _attachedList.ForEach(r => {
                IsoTile _tile = r.GetComponent<IsoTile>();
                if (_tile != null)
                    _tile.Rename();
            });
        }

        public IsoTileBulk Duplicate()
		{
			IsoTileBulk result = GameObject.Instantiate(this);
			result.transform.SetParent(transform.parent, false);
			result.Rename();
			Undo.RegisterCreatedObjectUndo(result.gameObject, "IsoTileBulk:Dulicate");			
			return result;
		}

        public void Sync(IsoTileBulk with)
        {
            //Transform, Grid, GridCoordinates 를 맞춘다
            transform.SetPositionAndRotation(with.transform.position, with.transform.rotation);
            coordinates.Sync(with.coordinates);
            coordinates.grid.Sync(with.coordinates.grid);

            transform.parent = with.transform.parent;
        }
#endif
    }
}