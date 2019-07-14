using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{	
	using Util;

	public enum SelectionType
	{
		LastTile,
		NewTile,
		AllTile,
	}

    [System.Serializable]
    public class AttachedIso2D : Attachment<Iso2DObject> { }
    [System.Serializable]
    public class AttachedIso2Ds : AttachmentHierarchy<AttachedIso2D> { }

    [SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(GridCoordinates))]
	[RequireComponent(typeof(IsometricSortingOrder))]
	[RequireComponent(typeof(RegularCollider))]
	[ExecuteInEditMode]
    public class IsoTile : MonoBehaviour
    {
        #region Static
        [SerializeField]
        public static bool bGlobalOption_AddAttachment = true;
        #endregion

        #region Basic
        [SerializeField]
        GridCoordinates _coordinates = null;
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

        #region GetBounds
        public Bounds GetBounds_SideOnly()
        {
            return GetBounds(Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_X, Iso2DObject.Type.Side_Y, Iso2DObject.Type.Side_Z);
        }


        public Bounds GetBounds()
        {
            Collider[] _colliders = transform.GetComponentsInChildren<Collider>();
            if (_colliders == null || _colliders.Length == 0)
                return new Bounds(transform.position, Vector3.zero);

            Bounds _bounds = new Bounds(_colliders[0].bounds.center, Vector3.zero);
            for (int i = 0; i < _colliders.Length; ++i)
            {
                if (_colliders[i] is BoxCollider)
                    _bounds.Encapsulate((_colliders[i] as BoxCollider).GetStatelessBounds());
                else
                    _bounds.Encapsulate(_colliders[i].bounds);
            }
            _bounds.Expand(Grid.fGridTolerance);
            return _bounds;
        }

        public Bounds GetBounds(params Iso2DObject.Type[] _types)
        {
            Iso2DObject[] _Iso2Ds = GetSideObjects(_types);
            Bounds _bounds = new Bounds(transform.position, Vector3.zero);
            if (_Iso2Ds != null)
            {
                for (int i = 0; i < _Iso2Ds.Length; ++i)
                    _bounds.Encapsulate(_Iso2Ds[i].RC.GlobalBounds);
            }
            return _bounds;
        }
        #endregion

        #region GetSideObject
        AttachedIso2Ds AttachedList { get
            {
#if UNITY_EDITOR
                return _attachedList;
#else
                var tmp = new AttachedIso2Ds();
                tmp.Init(gameObject);
                return tmp;
#endif
            }
        }
        public Iso2DObject GetSideObject(Iso2DObject.Type _type)
        {
            if (AttachedList.childList.Exists(r => r.AttachedObj._Type == _type))
                return AttachedList.childList.Find(r => r.AttachedObj._Type == _type).AttachedObj;
            return null;
        }
        public Iso2DObject[] GetSideObjects(params Iso2DObject.Type[] _types)
        {
            if (_types == null || _types.Length == 0)
                _types = new Iso2DObject.Type[]{
                    Iso2DObject.Type.Obstacle, Iso2DObject.Type.Overlay,
                    Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_X,
                    Iso2DObject.Type.Side_Y, Iso2DObject.Type.Side_Z,
                };
            List<Iso2DObject> results = new List<Iso2DObject>();
            AttachedList.childList.ForEach(r => {
                if (r.AttachedObj != null && _types.Contains(r.AttachedObj._Type))
                    results.Add(r.AttachedObj);
            });
            return results.ToArray();
        }
        #endregion

        #region RuntimeEtc
        [SerializeField]
        IsoTileBulk _bulk;
        [HideInInspector]
        public IsoTileBulk Bulk
        {
            get
            {
                if (_bulk != null)
                    return _bulk;
                if (transform.parent != null)
                    return _bulk = transform.parent.GetComponent<IsoTileBulk>();
                return null;
            }
        }

        [SerializeField]
        IsometricSortingOrder _so = null;
        [HideInInspector]
        public IsometricSortingOrder sortingOrder
        {
            get
            {
                return _so != null ? _so : _so = GetComponent<IsometricSortingOrder>();
            }
        }

        public void Update_SortingOrder()
        {
            if (sortingOrder != null)
            {
                sortingOrder.Update_SortingOrder(true);
            }
        }

        [SerializeField]
        public TileSetSprites _tileSetSprites;
        public TileSetSprites tileSetSprites {
            get { return _tileSetSprites; }
            set { _tileSetSprites = value; }
        }

        public static void UpdateTileSet(IEnumerable<IsoTile> _tiles, bool bSelfOnly = false, bool bRecordForUndo = true, string undoName = "Changed: TileSetSprites", IEnumerable<IsoTile> TileRestrictions = null)
        {
            foreach (var _tile in _tiles)
                UpdateTileSet(_tile, bSelfOnly, bRecordForUndo, undoName, TileRestrictions);
        }

        public void NextUpdateTileSprite(bool bRecordForUndo = true, string undoName = "Changed: TileSetSprites")
        {
#if UNITY_EDITOR
            var currentUndo = Undo.GetCurrentGroup();
#endif
            LateUpdateUtil.Register_NextUpdate(() =>
            {
                UpdateTileSet(this, bRecordForUndo: bRecordForUndo, undoName: undoName);
#if UNITY_EDITOR
                if (currentUndo < Undo.GetCurrentGroup())
                    Undo.CollapseUndoOperations(currentUndo);
#endif
            });
        }

        public static void UpdateTileSet(IsoTile _tile, bool bSelfOnly = false, 
            bool bRecordForUndo = true, string undoName = "Changed: TileSetSprites", 
            IEnumerable<IsoTile> allowedTiles = null, IEnumerable<IsoTile> exceptedTiles = null,
            int iLayerMask = -1)
        {                
            if (_tile == null || !_tile.enabled || _tile.tileSetSprites == null || !_tile.gameObject.scene.IsValid() || (_tile.hideFlags == HideFlags.HideAndDontSave && allowedTiles == null))
                return;

            Vector3 vDelta = _tile.coordinates.GetDelta();
            List<IsoTile> neighbourTiles = new List<IsoTile>();
            Dictionary<InGameDirection, TileSetSprites> neighbourTileSets = new Dictionary<InGameDirection, TileSetSprites>();
            for(InGameDirection _dir = InGameDirection.Right_Move; _dir <= InGameDirection.TR_Move; ++_dir)
            {
                var _t = FindNeighbourTile(_tile, vDelta, _dir, allowedTiles: allowedTiles, exceptedTiles: exceptedTiles, iLayerMask: iLayerMask);
                neighbourTileSets.Add(_dir, _t ? _t.tileSetSprites : null);
                if (_t != null)
                    neighbourTiles.Add(_t);
            }

            _tile.tileSetSprites.Apply(_tile, neighbourTileSets, bRecordForUndo, undoName);

            if (!bSelfOnly)
                UpdateTileSet(neighbourTiles, true, bRecordForUndo, undoName);
        }

        public void UpdateTileSet_LastNeighbours(bool bRecordForUndo = true, string undoName = "Changed: TileSetSprites", int iLayerMask = -1)
        {
            if (tileSetSprites != null)
            {
                // Since this tile is moving, firstly removed this tile.
                List<IsoTile> _exceptions = new List<IsoTile>();
                _exceptions.Add(this);

                Vector3 vDellta = coordinates.GetDelta();
                for (InGameDirection _dir = InGameDirection.Right_Move; _dir <= InGameDirection.TR_Move; ++_dir)
                {
                    var _t = FindNeighbourTile(this, vDellta, _dir, iLayerMask: iLayerMask);
                    if (_t != null && _t.tileSetSprites != null)
                        UpdateTileSet(_t, true, bRecordForUndo, undoName, exceptedTiles: _exceptions);
                }
            }
        }

        static IsoTile FindNeighbourTile(IsoTile _this, Vector3 _vDelta, InGameDirection _dir, bool bSameBulkOnly = false, bool bSurfaceTile = false, 
            IEnumerable<IsoTile> allowedTiles = null, IEnumerable<IsoTile> exceptedTiles = null, 
            int iLayerMask = -1)
        {
            if (_this == null)
                return null;

            Vector3 vAdd = _dir.ToVector3();
            IEnumerable<IsoTile> _tiles = null;

            _this.GetBounds();

#if UNITY_EDITOR
            Vector3 vCentor = _this.coordinates._lastLocalPosition;
#else
            Vector3 vCentor = _this.transform.localPosition;
#endif
            Vector3 vSize = _this.coordinates.TileSize;

            if (_this.transform.parent != null)
                vCentor += _this.transform.parent.position;

            //if (_vDelta != Vector3.zero)
            //    vCentor -= _vDelta;

            Bounds _b = new Bounds(vCentor, vSize);
            float fMaxdistance = _b.size.y;
            _b.center += Vector3.up * fMaxdistance;

            var _hits = Physics.BoxCastAll(_b.center + vAdd, _b.extents * 0.5f, Vector3.down, Quaternion.identity, fMaxdistance * 1f, iLayerMask);
            _tiles = _hits.Select(h => Find(h.collider.gameObject)).Distinct().
                Where(t => t != null && t != _this).
                Where(t => !bSameBulkOnly && _this.Bulk == _this.Bulk).
                Where(t => allowedTiles == null || allowedTiles.Contains(t)).
                Where(t => exceptedTiles == null || !exceptedTiles.Contains(t));

            if (_tiles != null && _tiles.Count() > 0)
            {
                if (bSurfaceTile)
                    return _tiles.Aggregate((l, r) => l.coordinates._xyz.y > r.coordinates._xyz.y ? l : r);

                float _y = _this.coordinates._xyz.y;

                // 1티어: 같은 타일 셋이거나, SubTileSet에 언급된 타일 셋
                var topTiers = _tiles.Where(t => t.tileSetSprites != null && t.tileSetSprites.IsRelative(_this.tileSetSprites));
                if (topTiers.Count() > 0)
                    return topTiers.Aggregate((l, r) => Mathf.Abs(l.coordinates._xyz.y - _y) > Mathf.Abs(r.coordinates._xyz.y - _y) ? l : r);

                // 기타 티어
                return _tiles.Aggregate((l, r) => Mathf.Abs(l.coordinates._xyz.y - _y) < Mathf.Abs(r.coordinates._xyz.y - _y) ? l : r);
            }

            return null;
        }

        public void ChangeBaseSprite(Sprite newSprite, bool bNullable = false, bool bRecordForUndo = true, string undoName = "Changed: Sprites")
        {
            if (newSprite || bNullable)
            {
                var _iso2Ds = GetSideObjects(Iso2DObject.Type.Side_Union, Iso2DObject.Type.Side_Y);
                foreach (var _iso2D in _iso2Ds)
                {
                    if (bRecordForUndo)
                        UndoUtil.Record(_iso2D, undoName);
                    _iso2D.ChangeSprite(newSprite, bRecordForUndo: bRecordForUndo, undoName: undoName);
                }
            }
        }

        public bool ChangeTileSet(TileSetSprites newSprites, bool bSelfOnly = false, bool bRecordForUndo = true, string undoName = "Changed: TileSetSprites", IEnumerable<IsoTile> TileRestrictions = null, int iLayerMask = -1)
        {
            if (bRecordForUndo)
                UndoUtil.Record(this, undoName);

            tileSetSprites = newSprites;
            UpdateTileSet(this, bSelfOnly, bRecordForUndo, undoName, TileRestrictions, iLayerMask: iLayerMask);
            return true;
        }

        public static IsoTile Find(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            var tile = gameObject.GetComponentInChildren<IsoTile>();
            if (tile == null)
                tile = gameObject.GetComponentInParent<IsoTile>();
            return tile;
        }

        public bool IsAccumulatedTile_Collider(Vector3 _direction)
        {
            Vector3 _xyz = coordinates._xyz;
            var _tiles = Bulk.GetTiles_At(_xyz, _direction, false, true);

            Bounds _bounds = GetBounds();
            // Vector3 _diff = transform.position - _bounds.center;
            // _bounds.SetMinMax(_bounds.min + 2f * _diff, _bounds.max + 2f * _diff);
            for (int i = 0; i < _tiles.Count(); ++i)
            {
                IsoTile _t = _tiles.ElementAt(i);
                if (_t != this && _t.GetBounds().Intersects(_bounds))
                    return true;
            }
            return false;
        }

        public void Clear_Attachment(bool bCanUndo, bool bSaveBasicOverlay = false)
        {
            Iso2DObject[] _iso2Ds = transform.GetComponentsInChildren<Iso2DObject>();
            for (int i = 0; i < _iso2Ds.Length; ++i)
            {
                Iso2DObject _iso2D = _iso2Ds[i];
                if (_iso2D != null && (bSaveBasicOverlay ? _iso2D.IsColliderAttachment : _iso2D.IsAttachment))
                    _iso2D.DestoryGameObject(bCanUndo, false);
            }
        }
        public void Copycat(IsoTile from, bool bCopyBody, bool bCopyChild = true, bool _bKeepColor = false, 
            bool bUndoable = true, string undoName = "IsoTile:Copycat", 
            bool bBasicallyClear = true, bool _bRandomAttachmentPosition = false)
        {
            copycat(from, bCopyBody, bCopyChild, _bKeepColor, bUndoable, undoName, bBasicallyClear, _bRandomAttachmentPosition);
        }

        void UndergroundAttachmentCheck(ref bool _bAttachmentAvailable)
        {
#if UNITY_EDITOR
            if (bGlobalOption_NoUndergroundAttachment && _bAttachmentAvailable)
            {
                if (this != this.FindTop())
                    _bAttachmentAvailable = false;
            }
#endif
        }

        void copycat(IsoTile from, bool _bCopyBody = true, bool _bCopyAttachment = true, bool _bKeepColor = false,
            bool _bUndoable = true, string _undoName = "IsoTile:Copycat", bool _bBasicallyClear = true, bool _bRandomAttachmentPosition = false)
        {
            if (from == null || from == this || !(_bCopyBody || _bCopyAttachment))
                return;

            if (this.IsPrefabConnected())
            {
                UndoUtil.PrintNotSupportedMSG();
                return;
            }

            if (_bUndoable)
                UndoUtil.Record(this, _undoName);

            Color bodyColor = Color.white;
            Color attachmentColor = Color.white;

            if (_bKeepColor)
                GetSprrColor(ref bodyColor, ref attachmentColor);

            UndergroundAttachmentCheck(ref _bCopyAttachment);

            List<GameObject> newList = new List<GameObject>();
            List<IsoLight> lights = GetLights();
            ////// var isoEnumerator = GetComponentsInChildren<Iso2DObject>().GetEnumerator();

            foreach (Transform child in from.transform)
            {
                if (KeepOrKick(child.gameObject, _bCopyBody, _bCopyAttachment))
                {
                    var newOne = GameObject.Instantiate(child.gameObject, transform, false);
                    newList.Add(newOne);
                    UndoUtil.Create(newOne, _undoName);
                }
            }

            LightRecivers_RemoveAll(_bUndoable);

            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                GameObject current = transform.GetChild(i).gameObject;
                if (newList.Contains(current))
                    continue;

                // 오래된 것들 중 Body는 신규 Body가 있을 경우에만 제거
                // Attachment는 추가가 아닐 경우 제거
                if (KeepOrKick(current, _bCopyBody, false) || (!bGlobalOption_AddAttachment && KeepOrKick(current, false, _bCopyAttachment)))
                {
                    if (_bUndoable)
                        UndoUtil.Delete(current);
                    else
                        DestroyImmediate(current);
                }
            }

            ChangeTileSet(from.tileSetSprites, false, true, _undoName);

            if (_bRandomAttachmentPosition)
            {
                var eWillMove = GetComponentsInChildren<Iso2DObject>()
                    .Where(r => !r.IsTileRCAttachment && !r.IsSideOfTile && r.sprr != null && newList.Any(newOne => r.transform.IsChildOf(newOne.transform)))
                    .Select(r => r.RC).Distinct();

                foreach(var one in eWillMove)
                {
                    var others = eWillMove.Where(rc => !one.transform.IsChildOf(rc.transform));
                    int iMaxTryCount = 20;
                    while (iMaxTryCount-- > 0)
                    {
                        one.Randomize();

                        if (!one.Iso2Ds.Any(r => others.Any(rc => rc.Iso2Ds.Any(
                            rr => rr.sprr != null && rr.sprr.sprite == r.sprr.sprite && rr.transform.localToWorldMatrix == r.transform.localToWorldMatrix))))
                            break;
                    }
                }
            }

            for (int i = 0; i < lights.Count; ++i)
            {
                if (lights[i] != null)
                    lights[i].AddTarget(transform.gameObject, true);
            }

            LightRecivers_UpdateAll();

            if (_bKeepColor)
                SetSprrColor(bodyColor, attachmentColor);

            if (IsoMap.isAutoISOMode)
                sortingOrder.Update_SortingOrder();
            else
                sortingOrder.Reset_SortingOrder(0, false);

            coordinates.Apply_SnapToGrid();
        }

        public void SetSprrColor(Color color_of_body, Color color_of_attachment)
        {
            var iso2Ds = GetComponentsInChildren<Iso2DObject>();
            foreach(var iso2D in iso2Ds)
            {
                if (iso2D.IsSideOfTile || iso2D.IsTileRCAttachment)
                    iso2D.sprr.color = color_of_body;
                else if (iso2D.IsColliderAttachment)
                    iso2D.sprr.color = color_of_attachment;
            }
        }

        public void GetSprrColor(ref Color color_of_body, ref Color color_of_attachment)
        {            
            var iso2Ds = GetComponentsInChildren<Iso2DObject>();
            bool bFoundBody = false, bFoundAttachment = false;
            foreach(var one in iso2Ds)
            {
                if (one.sprr == null)
                    continue;   

                if (!bFoundBody && (one.IsSideOfTile || one.IsTileRCAttachment))
                {
                    color_of_body = one.sprr.color;
                    bFoundBody = true;
                }
                else if (!bFoundAttachment && one.IsColliderAttachment)
                {
                    color_of_attachment = one.sprr.color;
                    bFoundAttachment = true;
                }

                if (bFoundBody && bFoundAttachment)
                    break;
            }
            if (bFoundBody && !bFoundAttachment)
                color_of_attachment = color_of_body;
        }

        public List<IsoLight> GetLights()
        {
            List<IsoLight> lights = new List<IsoLight>();
            var lightsEnum = transform.GetComponentsInChildren<IsoLightReciver>().Select(r => r.GetAllLightList()).GetEnumerator();
            while (lightsEnum.MoveNext())
            {
                lights.AddRange(lightsEnum.Current as IsoLight[]);
            }
            return lights.Distinct().ToList();
        }

        public void LightRecivers_RemoveAll(bool bUndoable)
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();

            foreach (var one in lightRecivers)
            {
                if (bUndoable)
                    UndoUtil.Delete(one);
                else
                    DestroyImmediate(one, true);
            }
        }

        public void LightRecivers_UpdateAll()
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();
            for (int i = 0; i < lightRecivers.Length; ++i)
                lightRecivers[i].UpdateLightColor();
        }

        /// <summary>
        /// Function to determine whether to leave Body or Attachment in Tile CopyCat.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="bIncludeSide">Side is Body & Body's Overlay</param>
        /// <param name="bIncludeAttachment">Attachment is not Side the rest</param>
        /// <returns>true is Keep, false is Kick</returns>
        private static bool KeepOrKick(GameObject go, bool bIncludeSide, bool bIncludeAttachment)
        {
            var enumerator = go.GetComponentsInChildren<Iso2DObject>();
            return enumerator.Any(iso2D => (bIncludeSide && (iso2D.IsTileRCAttachment || iso2D.IsSideOfTile)) || (bIncludeAttachment && iso2D.IsColliderAttachment));
        }

        /// <summary>
        /// Find IsoTiles contain box
        /// </summary>
        /// <param name="vCentor"></param>
        /// <param name="vHalfExtents"></param>
        /// <returns></returns>
        public static IEnumerable<IsoTile> GetTile_At_OverlapBox(Vector3 vCentor, Vector3 vHalfExtents)
        {
            return Physics.OverlapBox(vCentor, vHalfExtents).Select(h => IsoTile.Find(h.gameObject)).Distinct();
        }

        public static IEnumerable<IsoTile> GetTile_At_OverlapBox(Bounds bounds)
        {
            return GetTile_At_OverlapBox(bounds.center, bounds.extents);
        }

        private void OnDestroy()
        {
            if ((hideFlags & HideFlags.HideAndDontSave) != 0)
                return; 

            UpdateTileSet_LastNeighbours();
        }
        #endregion

#if UNITY_EDITOR
        #region MapEditor
        [SerializeField]
        public static bool bGlobalOption_NoUndergroundAttachment = true;

        

        [SerializeField]
        AutoNaming _autoName = null;
        [HideInInspector]
        public AutoNaming autoName
        {
            get
            {
                return _autoName == null ?
                    _autoName = GetComponent<AutoNaming>() : _autoName;
            }
        }

		public void Rename()
		{
            if (autoName)
                autoName.AutoName(); 
		}		
		
		[HideInInspector, SerializeField]
        public AttachedIso2Ds _attachedList = new AttachedIso2Ds();
		public void Update_AttachmentList()
        { 
			_attachedList.Init(gameObject);   
        }

		[SerializeField]
		public bool bAutoFit_ColliderScale = true;
		[SerializeField]
		public bool bAutoFit_SpriteSize = true;

		public bool IsUnionCube()
		{
			return _attachedList.childList.Exists(r => r.AttachedObj._Type == Iso2DObject.Type.Side_Union);
		}		

#region MonoBehaviour
		void OnEnable()
		{
			Update_AttachmentList();
		}
		void OnTransformParentChanged()
		{
			_bulk = null;
		}
		void OnTransformChildrenChanged()
		{
			if (autoName && autoName.bPostfix_Sprite)
				Rename();
			Update_AttachmentList();
		}
#endregion MonoBehaviour      
        //void copycat_origin(IsoTile from, bool bCopyBody = true, bool bCopyChild = true, bool bUndoable = true)
        //{
        //    if (from == this)
        //        return;

        //    string undoName = "IsoTile:Copycat";
        //    Undo.RecordObject(gameObject, undoName);

        //    if (bCopyChild)
        //    {
        //        List<GameObject> newList = new List<GameObject>();
        //        List<IsoLight> lights = GetLights();

        //        foreach (Transform child in from.transform)
        //        {
        //            newList.Add(GameObject.Instantiate(child.gameObject, transform, false));
        //            var newOne = newList.Last();
        //            if (bUndoable)
        //                Undo.RegisterCreatedObjectUndo(newOne, undoName);
        //        }

        //        LightRecivers_RemoveAll(bUndoable);

        //        for (int i = transform.childCount - 1; i >= 0; --i)
        //        {
        //            GameObject current = transform.GetChild(i).gameObject;
        //            if (newList.Contains(current))
        //                continue;

        //            if (bUndoable)
        //                Undo.DestroyObjectImmediate(current);
        //            else
        //                DestroyImmediate(current);
        //        }

        //        for (int i = 0; i < lights.Count; ++i)
        //        {
        //            if (lights[i] != null)
        //                lights[i].AddTarget(transform.gameObject, true);
        //        }
        //        LightRecivers_UpdateAll();
        //    }
        //    sortingOrder.Reset_SortingOrder(0, false);
        //    coordinates.Apply_SnapToGrid();
        //    // Update_AttachmentList();
        //}        

        public bool IsAccumulatedTile_Coordinates(Vector3 _direction)
        {
			Vector3 _xyz = coordinates._xyz;
            List<IsoTile> _tiles = Bulk.GetTiles_At(_xyz, _direction, false, true);

            int iCheckValue = coordinates.CoordinatesCountInTile(_direction);
			
            iCheckValue *= iCheckValue;
            for(int i = 0 ; i < _tiles.Count ; ++i)
            {
                Vector3 diff = Vector3.Scale(_xyz - _tiles[i].coordinates._xyz, _direction);
                if (Mathf.RoundToInt(diff.sqrMagnitude) < iCheckValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static IsoTile NewTile(Vector3 vPosition, bool bUnionMode = true)
        {
            GameObject baseObj = new GameObject("New Tile");
            GameObject obj = GameObject.Instantiate(baseObj, vPosition, Quaternion.identity);
            var newTile = obj.AddComponent<IsoTile>();
            newTile.Reset_SideObject(bUnionMode);
            DestroyImmediate(baseObj, true);
            return newTile;
        }

        public static IsoTile NewTile(bool bUnionMode = true)
        {
            GameObject obj = new GameObject("New Tile");
            var newTile = obj.AddComponent<IsoTile>();
            newTile.Reset_SideObject(bUnionMode);
            return newTile;
        }

        public void Reset_SideObject(bool _bTrueUnion)
        {
            if (!IsoMap.IsNull)
            {
                Clear_SideObject(true);
                Add_SideObject(_bTrueUnion ? IsoMap.Prefab_Side_Union : IsoMap.Prefab_Side_Y, "Change Tile Style");
            }
        }

        void Clear_SideObject(bool bCanUndo)
        {
            Iso2DObject[] _sideObjects = GetSideObjects(
                Iso2DObject.Type.Side_X, Iso2DObject.Type.Side_Y,
                Iso2DObject.Type.Side_Z, Iso2DObject.Type.Side_Union);

            for (int i = 0; i < _sideObjects.Length; ++i)
            {
                if (_sideObjects[i] != null)
                {
                    _sideObjects[i].DestoryGameObject(bCanUndo, true);
                }
            }
        }

        void Add_SideObject(GameObject _prefab, string _UndoMSG)
        {
            GameObject _obj = GameObject.Instantiate(_prefab, transform, false);
            _obj.transform.SetAsFirstSibling();
            RegularCollider _rc = _obj.GetComponent<RegularCollider>();
            _rc.Toggle_UseGridTileScale(bAutoFit_ColliderScale);

            if (bAutoFit_SpriteSize)
            {
                Iso2DObject _iso2D = _obj.GetComponentInChildren<Iso2DObject>();
                _iso2D.UpdateIsometricRotationScale();
                _iso2D.AdjustScale();
            }

            Undo.RegisterCreatedObjectUndo(_obj, _UndoMSG);
            Update_AttachmentList();
        }

        public void Toggle_Side(bool _bToggle, Iso2DObject.Type _toggleType)
        {
            Iso2DObject _obj = GetSideObject(_toggleType);
            if (_bToggle)
            {
                if (_obj == null)
                {
                    Add_SideObject(IsoMap.GetSidePrefab(_toggleType),
                        "Created : " + _toggleType + " Object");
                }
            }
            else
            {
                if (_obj != null)
                {
                    _obj.DestoryGameObject(true, true);
                    Update_AttachmentList();
                }
            }
        }
        
        public void MoveToZeroground()
        {
            Vector3 _ZeroGround = coordinates._xyz;
            coordinates.Move(_ZeroGround.x, 0, _ZeroGround.z, "IsoTile:MoveToZeroGround");
        }

		public void Init()
		{
            Update_ColliderScale();
            Update_Attached_Iso2DScale();
        }

        public void Update_Grid()
		{
			coordinates.Update_Grid(true);
            Update_ColliderScale();
            Update_Attached_Iso2DScale(true, "IsoTile: Update Grid");
        }

        public void Update_ColliderScale()
        {
            RegularCollider[] _RCs = GetComponentsInChildren<RegularCollider>();
            foreach (var _RC in _RCs)
            {
                _RC.Toggle_UseGridTileScale(bAutoFit_ColliderScale);
                _RC.AdjustScale();
            }
        }

		public void Update_Attached_Iso2DScale(bool bUndoable = false, string undoName = null)
		{
            foreach (var _attached in _attachedList.childList)
                Update_Attached_Iso2DScale(_attached.AttachedObj, bUndoable, undoName);
		}

        public void Update_Attached_Iso2DScale(Iso2DObject _Iso2D, bool bUndoable = false, string undoName = null)
        {
            if (_Iso2D != null)
            {
                if (bUndoable)
                    Undo.RecordObject(_Iso2D, undoName);
                _Iso2D.UpdateIsometricRotationScale();
                if (bUndoable)
                    Undo.RecordObject(_Iso2D.transform, undoName);
                _Iso2D.AdjustScale();
            }
        }

        public void SyncIsoLight(GameObject target)
        {
            var allLightRecivers = target.GetComponentsInChildren<IsoLightReciver>();
            if (allLightRecivers != null && allLightRecivers.Length > 0)
            {
                foreach (var one in allLightRecivers)
                    one.ClearAllLights();
            }

            allLightRecivers = GetComponentsInChildren<IsoLightReciver>().Where(r => !allLightRecivers.Contains(r)).ToArray();
            allLightRecivers.Select(r => r.GetAllLightList());
            List<IsoLight> allLights = new List<IsoLight>();
            foreach (var one in allLightRecivers)
                allLights.AddRange(one.GetAllLightList().Where(r => !allLights.Contains(r)));
            allLights.ForEach(r => r.AddTarget(target, true));
        }

        public void LightRecivers_RevertAll()
        {
            var lightRecivers = transform.GetComponentsInChildren<IsoLightReciver>();

            foreach (var one in lightRecivers)
                one.RevertSpriteRendererColor();
        }

        public static void Destroy(ref GameObject target, bool bAttachmentOnly)
        {
            if (target != null)
            {
                var tile = IsoTile.Find(target);
                if (bAttachmentOnly)
                {
                    // tile.Clear_Attachment(true);
                    tile.Clear_Attachment(true, true);
                }
                else
                    UndoUtil.Delete(tile ? tile.gameObject : target);
            }
            target = null;
        }

        public static bool Create(out GameObject targetOut, Vector3 position, IsoTile refTile, IsoTileBulk bulk,
            bool bIncludeAttachments, bool bAutoIsoLight, bool bNewPrefabStyle = false, bool bRandomizeAttachment = false)
        {
            targetOut = null;
            var bulks = IsoMap.instance.BulkList;

            if (bulk == null)
            {
                float fMin = float.MaxValue;
                for (int i = 0; i < bulks.Count; ++i)
                {
                    if (!bulks[i].isActiveAndEnabled)
                        continue;

                    float fDistance = Vector3.Distance(position, bulks[i].GetBounds().ClosestPoint(position));
                    if (fDistance < fMin)
                    {
                        bulk = bulks[i];
                        fMin = fDistance;
                    }
                }
                if (bulk == null)
                {
                    Debug.LogWarning("To create a tile, you need at least one Bulk in the Scene.");
                    return false;
                }
            }

            IsoTile tile = bulk.NewTile(position - bulk.transform.position, false, refTile, bNewPrefabStyle);
            if (tile == null)
                return false;

            tile.UndergroundAttachmentCheck(ref bIncludeAttachments);

            for (int i = tile.transform.childCount - 1; i >= 0; --i)
            {
                var child = tile.transform.GetChild(i);
                if (!KeepOrKick(child.gameObject, true, bIncludeAttachments))
                    DestroyImmediate(child.gameObject, true);
            }

            if (bRandomizeAttachment)
            {
                var RCs = tile.GetComponentsInChildren<RegularCollider>().Where(rc => rc.Iso2Ds.All(iso2D => !(iso2D.IsTileRCAttachment || iso2D.IsSideOfTile)));
                foreach (var rc in RCs)
                    rc.Randomize();
            }

            targetOut = tile.gameObject;
            return true;
        }

        /*
        public static bool CreateORCopy(ref GameObject target, Vector3 vAt, IsoTile refTile, IsoTileBulk refBulk, 
            bool bBody, bool bAttachments, bool bRandomAttachmentPosition, bool bKeepColor,
            bool bAutoIsoLight, bool bNewPrefabStyle, bool bAutoCreation, bool bRandomizeAttachment)
        {
            IsoTile TargetTile = IsoTile.Find(target);
            bool bPrefabConnected_TargetTile = target != null && target.IsPrefabConnected();
            bool bPrefabConnected_RefTile = refTile != null && refTile.IsPrefabConnected();

#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
            if (bNewPrefabStyle)
            {
                if (bPrefabConnected_RefTile)// && bPrefabConnected_TargetTile)
                {
                    if (bPrefabConnected_TargetTile)
                    {
                        var one = PrefabUtility.GetCorrespondingObjectFromSource(target);
                        if (one.transform.IsChildOf(refTile.transform))
                            return false;
                    }
                }
            }
#endif

            Vector3 vSnap = TargetTile ? TargetTile.Bulk.coordinates.PositionToCoordinates(vAt, !TargetTile.coordinates.bSnapFree) : vAt;
            bool bTileAtPosition = TargetTile ? GridCoordinates.IsSameWithTolerance(vSnap, TargetTile.transform.position) : false;

            if (TargetTile != null)
            {

                if (bPrefabConnected_TargetTile)
                {
                    if (bTileAtPosition)
                        Undo.DestroyObjectImmediate(TargetTile.gameObject);
                }

            }

            if (!bTileAtPosition || target == null)
            {
                if (!bAutoCreation)
                    return false;

                if (Create(out target, vAt, refTile, refBulk, bAttachments, bAutoIsoLight, bNewPrefabStyle, bRandomizeAttachment))
                {
                    TargetTile = IsoTile.Find(target);
                    bPrefabConnected_TargetTile = target.IsPrefabConnected();
                }
            }

            if (refTile == null || TargetTile == null)
                return false;

            // Undo.Record() code is aleady in Copycat()
            if (!bPrefabConnected_TargetTile)
                TargetTile.Copycat(refTile, bBody, bAttachments, bKeepColor, _bRandomAttachmentPosition:bRandomAttachmentPosition);

            target = TargetTile.gameObject;
            return true;
        }
        */
#endregion MapEditor
#endif
    }
}