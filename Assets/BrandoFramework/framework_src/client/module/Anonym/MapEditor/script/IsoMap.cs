using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{
	using Util;

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Grid))]
	public class IsoMap : Singleton<IsoMap> {
		public static float fResolution = 100f;		
        public static float fOnGroundOffset_Default = 0.5f;
		public static Vector3 vMAXResolution = Vector3.one * fResolution;

        public static float fCurrentOnGroundOffset
        {
            get
            {
                if (IsNull || !instance.bUseGroundObjectOffset)
                    return 0;
                return instance.fOnGroundOffset;
            }
        }

        public Vector3 fResolutionOfIsometric = vMAXResolution;

		public bool bUseIsometricSorting = true;
        public static bool isAutoISOMode { get { return !IsNull && instance.bUseIsometricSorting; } }
        public static bool isNormalSOMode { get { return IsNull || !instance.bUseIsometricSorting; } }
        public static bool isUsingGlobalGroundOffset { get { return !IsNull && instance.bUseGroundObjectOffset; } }
        public bool bUseGroundObjectOffset = false;

        public float fOnGroundOffset = fOnGroundOffset_Default;
        public Vector3 VOnGroundOffset { get { return Vector3.up * fOnGroundOffset; } }

        public float ReferencePPU = 128;

        [SerializeField]
        Grid _grid;
        public Grid gGrid
        {
            get
            {
                if (_grid == null)
                    _grid = GetComponent<Grid>();
                return _grid;
            }
        }

        [SerializeField]
        public Vector2 TileAngle = DefaultTileAngle;
        public static Vector2 DefaultTileAngle = new Vector2(30f, -45f);
        public static Vector2 Angle
        {
            get
            {
                return IsNull ? DefaultTileAngle : instance.TileAngle;
            }
        }

#if UNITY_EDITOR

        [SerializeField]
        List<IsoTileBulk> _childBulkList = new List<IsoTileBulk>();
        public List<IsoTileBulk> BulkList
        {
            get
            {
                if (_childBulkList.Count == 0)
                    _childBulkList.AddRange(FindObjectsOfType<IsoTileBulk>());

                _childBulkList.RemoveAll(b => b == null);
                _childBulkList.Distinct();
                return _childBulkList;
            }
        }
        public bool Regist_Bulk(IsoTileBulk _add)
		{
			if (_add == null || PrefabHelper.IsPrefab(_add.gameObject))
				return false;

            if (_childBulkList.Contains(_add))
                return false;

			_childBulkList.Add(_add);
            return true;
		}
		public void Update_Grid()
		{
			for(int i = _childBulkList.Count - 1; i >= 0 ; --i)
			{
				if(_childBulkList[i] == null)
				{
					_childBulkList.RemoveAt(i);
					continue;
				}
				_childBulkList[i].coordinates.Update_Grid(true);
				if (_childBulkList[i].coordinates.IsInheritGrid)
				{
					_childBulkList[i].Update_Grid();
				}
			}
		}
		
		float _last_TileAngle_Y = 0;
		float _last_Scale_TA_Y = 1f;
        public static float fScale_TA_Y(Vector2 _TileAngle, Vector3 _v3Size)
        {
            bool bCosRange = (_TileAngle.y >= -45f && _TileAngle.y < 45f)
                    || (_TileAngle.y >= 135f && _TileAngle.y < 225f);

            float fLastYAngle = IsNull ? 0f : instance._last_TileAngle_Y;
            float fScaleY = IsNull ? 1f : instance._last_Scale_TA_Y;

            if (fLastYAngle != _TileAngle.y)
            {
                fLastYAngle = _TileAngle.y;
                if (bCosRange)
                    fScaleY = Mathf.Cos(Mathf.Deg2Rad * fLastYAngle);
                else
                    fScaleY = Mathf.Sin(Mathf.Deg2Rad * fLastYAngle);
            }

            if (!IsNull)
            {
                instance._last_TileAngle_Y = fLastYAngle;
                instance._last_Scale_TA_Y = fScaleY;
            }

            return Mathf.Abs((bCosRange ? _v3Size.x : _v3Size.z) / fScaleY);
        }
        private Vector2 _lastTileAngle = Vector2.zero;
		private float _lastMagicValue = 2f;
		public static float fMagicValue{
			get{
                Vector2 angle = IsoMap.IsNull ? DefaultTileAngle : IsoMap.instance.TileAngle;
                if (!IsoMap.IsNull)
                {
                    if (!angle.Equals(IsoMap.instance._lastTileAngle))
                        IsoMap.instance._lastMagicValue = clacMagicValueForGUI(angle);
                    return IsoMap.instance._lastMagicValue;
                }
                return clacMagicValueForGUI(angle);
            }
		}
        static float clacMagicValueForGUI(Vector2 IsometricAngle)
        {
            return Mathf.Abs(2f * ((3 * Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * IsometricAngle.x), 2) - 1)
                    / (3 * Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * IsometricAngle.y), 2) - 1) + 1) / 3f);
        }

        public Plane GetIsometricPlane(Vector3 normal, Vector3 inPoint)
        {
            return new Plane(Quaternion.Euler(TileAngle) * normal, inPoint);
        }
        public Plane GetIsometricPlane(Vector3 inPoint)
        {
            return new Plane(Quaternion.Euler(TileAngle) * Vector3.back, inPoint);
        }
        public Plane GetIsometricGroundPlane(Vector3 inPoint)
        {
            return new Plane(Quaternion.Euler( -TileAngle.x, TileAngle.y, 0f) * Vector3.up, inPoint);
        }

        [SerializeField]
		public Camera GameCamera;
		
		[SerializeField]
		bool bCustomResolution = false;
		[SerializeField]
		Vector3 vCustomResolution = vMAXResolution;

        // new Vector3(85.1f, 10.3f, 52.5f);
        public IsometricSortingOrder[] Revert_All_ISO()
        {
            IsometricSortingOrder[] _ISOArray = FindObjectsOfType<IsometricSortingOrder>();
            if (_ISOArray != null)
            {
                for (int i = 0; i < _ISOArray.Length; ++i)
                {
                    if (_ISOArray[i] != null)
                    {
                        _ISOArray[i].Revert_SortingOrder();
                    }
                }
            }
            return _ISOArray;
        }
        public IsometricSortingOrder[] Backup_All_ISO()
        {
            IsometricSortingOrder[] _ISOArray = FindObjectsOfType<IsometricSortingOrder>();
            if (_ISOArray != null)
            {
                for (int i = 0; i < _ISOArray.Length; ++i)
                {
                    if (_ISOArray[i] != null)
                    {
                        _ISOArray[i].Backup_SortingOrder();
                    }
                }
            }
            return _ISOArray;
        }
		public void UpdateIsometricSortingResolution()
		{
			if (bUseIsometricSorting)
			{
				if (!bCustomResolution)
				{
					fResolutionOfIsometric.Set(
						Mathf.Max(Grid.fGridTolerance, Mathf.Sin(Mathf.Deg2Rad * -TileAngle.y) * fResolution),
						Mathf.Max(Grid.fGridTolerance, Mathf.Sin(Mathf.Deg2Rad * TileAngle.x) * fResolution),
						Mathf.Max(Grid.fGridTolerance, Mathf.Cos(Mathf.Deg2Rad * -TileAngle.y) * fResolution)
					);
				}
				else
				{
					fResolutionOfIsometric = vCustomResolution;
				}
			}
			else
				fResolutionOfIsometric.Set(0f, 0f, 0f);
		}

        public void Update_All_ISO()
        {
            Update_All_ISO(FindObjectsOfType<IsometricSortingOrder>());
        }
        public void Clear_All_ISO_Backup()
        {
            var all = FindObjectsOfType<IsometricSortingOrder>();
            foreach (var one in all)
                one.Clear_Backup();
        }
        public void Update_All_ISO(IsometricSortingOrder[] _ISOArray)
		{
            if (_ISOArray == null)
                return;

			for (int i = 0 ; i < _ISOArray.Length; ++i)
			{
				if (_ISOArray[i] != null)
				{
					_ISOArray[i].Update_SortingOrder(true);
				}
			}
		}
        public static bool Regist(IsoTileBulk bulk)
        {
            if (IsoMap.IsNull)
                return false;

            return IsoMap.instance.Regist_Bulk(bulk);
        }
        public static void UpdateSortingOrder_All_ISOBasis(bool bGroundOnly = true)
        {
            ISOBasis[] _allIsoBasisCash = null;
            UpdateSortingOrder_All_ISOBasis(ref _allIsoBasisCash, bGroundOnly);
        }
        public static void Update_All_IsoTransform_Rotate(ref IsoTransform[] isoTransfoms)
        {
            if (isoTransfoms == null || isoTransfoms.Length == 0)
                isoTransfoms = FindObjectsOfType<IsoTransform>();

            foreach (var isoTransform in isoTransfoms)
            {
                if (isoTransform != null)
                    isoTransform.AdjustRotation();
            }
        }
        public static void UpdateSortingOrder_All_ISOBasis(ref ISOBasis[] _allIsoBasisCash, bool bGroundOnly = true)
        {
            if (_allIsoBasisCash == null || _allIsoBasisCash.Length == 0)
                _allIsoBasisCash = FindObjectsOfType<ISOBasis>().Where(r => !bGroundOnly || r.isOnGroundObject).ToArray();

            if (!(_allIsoBasisCash == null || _allIsoBasisCash.Length == 0))
            {
                foreach (var one in _allIsoBasisCash)
                    one.Update_SortingOrder_And_DepthTransform();
                SceneView.RepaintAll();
            }
        }

        public static void GatherGroundIIsoBasisCash(ref List<IISOBasis> _alIIsoBasisCash, bool bOnlyMarkedIso2dObjects = false)
        {
            var iIsoBasises = FindObjectsOfType<MonoBehaviour>().OfType<IISOBasis>().Where(r => r.IsOnGroundObject());
            if (bOnlyMarkedIso2dObjects)
                iIsoBasises = iIsoBasises.Where(e => (e as MonoBehaviour).gameObject.GetComponentsInChildren<Iso2DObject>().Any(iso2D => iso2D.bGroundOffsetMark));
            _alIIsoBasisCash.AddRange(iIsoBasises);
            _alIIsoBasisCash = _alIIsoBasisCash.Distinct().ToList();
        }

        public static void UpdateGroundOffsetFudge_All_ISOBasis(ref List<IISOBasis> _alIIsoBasisCash, float fDegthFudge, bool bNewFudge = false)
        {
            if (_alIIsoBasisCash.Count == 0)
            {
                GatherGroundIIsoBasisCash(ref _alIIsoBasisCash);
            }

            foreach (var one in _alIIsoBasisCash)
            {
                one.Undo_UpdateDepthFudge(-fDegthFudge, bNewFudge);
            }
        }

        public static void Update_SceneViewCam(Vector3 lootAt)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                if (SceneView.lastActiveSceneView.in2DMode)
                {
                    SceneView.lastActiveSceneView.in2DMode = false;
                    SceneView.lastActiveSceneView.orthographic = true;
                }
                else if (SceneView.lastActiveSceneView.orthographic == false)
                    SceneView.lastActiveSceneView.orthographic = true;

                SceneView.lastActiveSceneView.LookAtDirect(lootAt, Quaternion.Euler(IsoMap.Angle));
            }
        }

        public void Update_TileAngle()
		{
			UpdateIsometricSortingResolution();

            Update_SceneViewCam(IsoMap.instance.transform.position);

            if (GameCamera != null)
			{
				GameCamera.transform.rotation = Quaternion.Euler(TileAngle);
				if (GameCamera.orthographic == false)
				{
					GameCamera.orthographic = true;
					GameCamera.orthographicSize = ((GameCamera.pixelHeight)/(1f * ReferencePPU)) * 0.5f;
				}

			}
		}

        #region StaticAssetForUnityEditor
        static string staticResourcePath = "Assets/Anonym/MapEditor/sprite/etc/";
        static string staticPrefabPath = "Assets/Anonym/MapEditor/prefab/isometric/";
        static string staticUtilPrefabPath = "Assets/Anonym/Util/prefab/";

        public static string defaultIsoMapPrefabPat = staticPrefabPath + "Iso_Map.prefab";

        static Sprite TextureToSprite(Texture2D tx)
        {
            return Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), Vector2.one * 0.5f);
        }
        static ResultType PreLoad<ResultType, LoadType>(ref ResultType container, string path, System.Func<LoadType, ResultType> convert) where LoadType : class
        {
            if (container == null)
            {
                LoadType preLoaded = PreLoad<LoadType>(path);
                container = convert(preLoaded);
            }
            return container;
        }
        static T PreLoad<T>(ref T container, string path) where T : class
        {
            if (container == null)
                container = PreLoad<T>(path);

            return container;
        }
        static T PreLoad<T>(string path) where T : class
        {
            return EditorGUIUtility.Load(path) as T;
        }

        static Sprite _gui_IsoTile_Union_OutlineImage;
        public static Sprite gui_IsoTile_Union_OutlineImage
        {
            get
            {
                return PreLoad<Sprite, Texture2D>(ref _gui_IsoTile_Union_OutlineImage, staticResourcePath + "IsoTileOutline_Up.png", TextureToSprite);
            }
        }

        static Sprite _gui_IsoTile_Side_OutlineImage;
        public static Sprite gui_IsoTile_Side_OutlineImage
        {
            get
            {
                return PreLoad<Sprite, Texture2D>(ref _gui_IsoTile_Side_OutlineImage, staticResourcePath + "IsoTileOutline_Down.png", TextureToSprite);
            }
        }

        static Sprite _gui_RefTileSprite;
        public static Sprite gui_RefTileSprite
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.RefTileSprite;
                return PreLoad<Sprite, Texture2D>(ref _gui_RefTileSprite, staticResourcePath + "IsoTile.png", TextureToSprite);
            }
        }

        static GameObject _OverlayPrefab;
        public static GameObject Prefab_Overlay
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.OverlayPrefab;
                return PreLoad(ref _OverlayPrefab, staticPrefabPath + "Iso_Sprite_Tile_Overlay.prefab");
            }
        }

        static GameObject _TriggerPlanePrefab;
        public static GameObject Prefab_TriggerPlane
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.TriggerPlanePrefab;
                return PreLoad(ref _TriggerPlanePrefab, staticPrefabPath + "Trigger_IsoPlane_Overlay.prefab");
            }
        }

        static GameObject _TriggerCubePrefab;
        public static GameObject Prefab_TriggerCube
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.TriggerCubePrefab;
                return PreLoad(ref _TriggerCubePrefab, staticPrefabPath + "Trigger_Cube_Overlay.prefab");
            }
        }
        
        static GameObject _ObstaclePrefab;
        public static GameObject Prefab_Obstacle
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.ObstaclePrefab;
                return PreLoad(ref _ObstaclePrefab, staticPrefabPath + "Regualr_Collider_Obstacle.prefab");
            }
        }

        static GameObject _Collider_X_Prefab;
        public static GameObject Prefab_Collider_X
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Collider_X_Prefab;
                return PreLoad(ref _Collider_X_Prefab, staticPrefabPath + "Collider_YZ.prefab");
            }
        }

        static GameObject _Collider_Y_Prefab;
        public static GameObject Prefab_Collider_Y
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Collider_Y_Prefab;
                return PreLoad(ref _Collider_Y_Prefab, staticPrefabPath + "Collider_XZ.prefab");
            }
        }

        static GameObject _Collider_Z_Prefab;
        public static GameObject Prefab_Collider_Z
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Collider_Z_Prefab;
                return PreLoad(ref _Collider_Z_Prefab, staticPrefabPath + "Collider_XY.prefab");
            }
        }

        static GameObject _Collider_Cube_Prefab;
        public static GameObject Prefab_Cube
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Collider_Cube_Prefab;
                return PreLoad(ref _Collider_Cube_Prefab, staticPrefabPath + "Collider_Cube.prefab");
            }
        }

        static GameObject _Side_Union_Prefab;
        public static GameObject Prefab_Side_Union
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Side_Union_Prefab;
                return PreLoad(ref _Side_Union_Prefab, staticPrefabPath + "Regualr_Collider_Union.prefab");
            }
        }

        static GameObject _Side_X_Prefab;
        public static GameObject Prefab_Side_X
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Side_X_Prefab;
                return PreLoad(ref _Side_X_Prefab, staticPrefabPath + "Regualr_Collider_X.prefab");
            }
        }

        static GameObject _Side_Y_Prefab;
        public static GameObject Prefab_Side_Y
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Side_Y_Prefab;
                return PreLoad(ref _Side_Y_Prefab, staticPrefabPath + "Regualr_Collider_Y.prefab");
            }
        }

        static GameObject _Side_Z_Prefab;
        public static GameObject Prefab_Side_Z
        {
            get
            {
                if (!IsoMap.IsNull)
                    return IsoMap.instance.Side_Z_Prefab;
                return PreLoad(ref _Side_Z_Prefab, staticPrefabPath + "Regualr_Collider_Z.prefab");
            }
        }

        static GameObject _GameObject_Selector;
        public static GameObject GameObject_Selector
        {
            get
            {
                return PreLoad(ref _GameObject_Selector, staticUtilPrefabPath + "TileSelector.prefab");
            }
        }
        #endregion

        #region Prefab
        public GameObject BulkPrefab;
		public GameObject TilePrefab;
		public GameObject OverlayPrefab;
		public GameObject TriggerPlanePrefab;
		public GameObject TriggerCubePrefab;
		public GameObject ObstaclePrefab;
        public GameObject Side_Union_Prefab;
		public GameObject Side_X_Prefab;
		public GameObject Side_Y_Prefab;
		public GameObject Side_Z_Prefab;
		public GameObject Collider_X_Prefab;
		public GameObject Collider_Y_Prefab;
		public GameObject Collider_Z_Prefab;
		public GameObject Collider_Cube_Prefab;
        public GameObject TchPrefab;
		public Sprite RefTileSprite;

        public static GameObject GetSidePrefab(Iso2DObject.Type _type)
		{
			switch(_type)
			{
				case Iso2DObject.Type.Side_Union:
					return Prefab_Side_Union;
                case Iso2DObject.Type.Side_X:
                    return Prefab_Side_X;
                case Iso2DObject.Type.Side_Y:
                    return Prefab_Side_Y;
                case Iso2DObject.Type.Side_Z:
                    return Prefab_Side_Z;
            }
			return null;
		}
        #endregion

        public IsoTileBulk NewBulk()
		{			
			if (BulkPrefab == null)
			{
				Debug.LogError("IsoMap : No BulkPrefab!");
				return null;
			}
			IsoTileBulk _newBulk = GameObject.Instantiate(BulkPrefab).GetComponent<IsoTileBulk>();
			Undo.RegisterCreatedObjectUndo(_newBulk.gameObject, "IsoTile:Create");
			_newBulk.transform.SetParent(transform, false);
            _newBulk.coordinates.Move(gGrid.Centor);
			return _newBulk;
		}

        public IsoTileBulk NewBulk(IsoTileBulk syncWith, IEnumerable<IsoTile> tiles)
        {
            IsoTileBulk _newBulk = NewBulk();
            _newBulk.bAllowEmptyBulk = true;
            _newBulk.Clear();
            _newBulk.Sync(syncWith);
            var enumerator = tiles.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var current = enumerator.Current;
                Undo.SetTransformParent(current.transform, _newBulk.transform, "IsoTile: Split Bulk");
                current.transform.parent = _newBulk.transform;
            }
            _newBulk.bAllowEmptyBulk = false;
            Selection.activeGameObject = _newBulk.gameObject;
            EditorGUIUtility.PingObject(_newBulk.gameObject);
            return _newBulk;
        }

        public IsoTile NewTile_Raw()
		{
			if (TilePrefab == null)
			{
				Debug.LogError("IsoMap : No TilePrefab!");
				return null;
			}
			IsoTile _newTile = GameObject.Instantiate(TilePrefab).GetComponent<IsoTile>();
			Undo.RegisterCreatedObjectUndo(_newTile.gameObject, "IsoTile:Create");			
			return _newTile;
		}
		IsoTileBulk[] GetAllBulk()
		{
			return gameObject.GetComponentsInChildren<IsoTileBulk>();
		}

		public void BakeNavMesh()
		{
		}

        //void OnValidate()
        //{
        //	if (!PrefabUtility.GetPrefabType(this).Equals(PrefabType.Prefab)
        //		&& Application.isEditor && !Application.isPlaying 
        //		&& !EditorApplication.isPlayingOrWillChangePlaymode
        //		&& !EditorApplication.isUpdating
        //		&& !EditorApplication.isTemporaryProject
        //		&& !IsNull && isActiveAndEnabled)
        //		Update_TileAngle();
        //}
        public void Update_GroundOffset()
        {
            List<IISOBasis> _alIIsoBasisCash = new List<IISOBasis>();
            Update_GroundOffset(ref _alIIsoBasisCash);
            MarkGroundOffsetToIso2Ds(_alIIsoBasisCash, bUseGroundObjectOffset);
            _alIIsoBasisCash.Clear();
        }

        public void Apply_GroundOffset(IISOBasis iIsoBasis)
        {
            var isoBasis = iIsoBasis.GetISOBasis();

            if (bUseGroundObjectOffset)
            {
                if (isoBasis)
                    isoBasis.bDoNotDestroyAutomatically = true;

                if (bUseIsometricSorting)
                {
                    if (iIsoBasis.IsOnGroundObject())
                    {
                        if (isoBasis == null)
                        {
                            isoBasis = iIsoBasis.SetUp();
                            isoBasis.bDoNotDestroyAutomatically = false;
                        }

                        iIsoBasis.GetISOBasis().AutoSetup_DepthTransforms();
                    }
                }
                if (iIsoBasis.IsOnGroundObject())
                    iIsoBasis.Undo_UpdateDepthFudge(-fOnGroundOffset, false);
            }
            else
            {
                if (iIsoBasis.IsOnGroundObject())
                    iIsoBasis.Undo_UpdateDepthFudge(fOnGroundOffset, false);

                if (isoBasis)
                    iIsoBasis.DestroyISOBasis();
            }

            //if (isoBasis)
            //    isoBasis.Update_SortingOrder_And_DepthTransform();
            //if (!bUseGroundObjectOffset || (isoBasis && isoBasis.isOnGroundObject))
            //    iIsoBasis.Undo_UpdateDepthFudge(bUseGroundObjectOffset ? -fOnGroundOffset : fOnGroundOffset, false);
        }

        public void Update_GroundOffset(ref List<IISOBasis> _alIIsoBasisCash)
        {
            if (_alIIsoBasisCash == null || _alIIsoBasisCash.Count == 0)
                GatherGroundIIsoBasisCash(ref _alIIsoBasisCash);

            _alIIsoBasisCash.ForEach((iBasis) => Apply_GroundOffset(iBasis));
        }

        public void MarkGroundOffsetToIso2Ds(List<IISOBasis> _alIIsoBasisCash, bool bMark)
        {
            if (_alIIsoBasisCash == null || _alIIsoBasisCash.Count == 0)
                return;

            var lookups = _alIIsoBasisCash
                .Select(iIsoBasis => (iIsoBasis as MonoBehaviour).gameObject.GetComponentsInChildren<Iso2DObject>())
                .Aggregate((l, r) =>
                {
                    ArrayUtility.AddRange<Iso2DObject>(ref l, r);
                    return l;
                });

            foreach (var one in lookups)
                one.bGroundOffsetMark = bMark;
        }

#endif
        public Vector3 vDepthFudge(float fFudge)
        {
            return vDepthFudge(fFudge, instance.TileAngle);
        }

        public static Vector3 vDepthFudge(float fFudge, Vector2 vTileAngle)
        {
            return Quaternion.Euler(vTileAngle) * Vector3.forward * fFudge;
        }
    }
}