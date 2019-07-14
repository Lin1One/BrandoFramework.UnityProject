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

    public class IsometricSortingOrderUtility
    {
        public static int IsometricSortingOrder(Transform _transform)
        {
            return IsometricSortingOrder(_transform.position);
        }

        public static int IsometricSortingOrder(Vector3 _position)
        {
            if (IsoMap.isNormalSOMode)
                return 0;
            Vector3 v3Tmp = IsoMap.instance.fResolutionOfIsometric;
            return Mathf.RoundToInt(v3Tmp.x * _position.x + v3Tmp.y * _position.y - v3Tmp.z * _position.z);
        }
    }

    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [DefaultExecutionOrder(200)]
    public class IsometricSortingOrder : MonoBehaviour, IISOBasis
    {
        const int Default_LastSortingOrder = int.MinValue;

        #region Basic
        [SerializeField]
        public bool bLastISOMode = false;
        [SerializeField]
        public int iLastSortingOrder = 0;
        public void Corrupt_LastSortingOrder()
        {
            iLastSortingOrder = Default_LastSortingOrder;
        }
        public bool IsCorrupted_LastSortingOrder()
        {
            return iLastSortingOrder == Default_LastSortingOrder;
        }

        [SerializeField]
        int iParticleSortingAdd = 0;

        List<IOverrideSortingOrder> _iOverrideSOList = new List<IOverrideSortingOrder>();
        List<IOverrideSortingOrder> IOverrideSOList
        {
            get
            {
                update_Child();
                return _iOverrideSOList;
            }
        }

        List<SpriteRenderer> _dependentList = new List<SpriteRenderer>();
        List<SpriteRenderer> DependentList
        {
            get
            {
                update_Child();
                return _dependentList;
            }
        }

        List<Vector3> _particleLastPositionList = new List<Vector3>();
        List<ParticleSystemRenderer> _particleSystemRendererList = new List<ParticleSystemRenderer>();
        List<ParticleSystemRenderer> ParticleSystemRendererList
        {
            get
            {
                update_Child();
                return _particleSystemRendererList;
            }
        }


        bool bCorrupted = false;
        public void Corrupt_Child(bool bFlag)
        {
            bCorrupted = bFlag;
        }

        ParticleSystem.Particle[] particles = null;
        int setParticleArray(ParticleSystem _ps)
        {
            if (particles == null || particles.Length < _ps.main.maxParticles)
                particles = new ParticleSystem.Particle[_ps.main.maxParticles];
            return _ps.GetParticles(particles);
        }

        void update_Child(bool bJustDoIt = false)
        {
            if (bJustDoIt || bCorrupted)
            {
                _particleSystemRendererList.Clear();
                _particleLastPositionList.Clear();
                ParticleSystemRenderer[] _particlesSystemRenderers = transform.GetComponentsInChildren<ParticleSystemRenderer>();
                if (_particlesSystemRenderers != null)
                {
                    for (int i = 0; i < _particlesSystemRenderers.Length; ++i)
                    {
                        _particleSystemRendererList.Add(_particlesSystemRenderers[i]);
                        _particleLastPositionList.Add(Vector3.zero);
                    }
                }

                _regularColliderList.Clear();
                _regularColliderList.AddRange(transform.GetComponentsInChildren<RegularCollider>());

                _dependentList.Clear();
                var _sprrs = transform.GetComponentsInChildren<SpriteRenderer>().GetEnumerator();
                while(_sprrs.MoveNext())
                {
                    var _sprr = _sprrs.Current as SpriteRenderer;
                    var _iso2D = _sprr.GetComponent<Iso2DObject>();
                    if (_iso2D == null || _iso2D.RC == null)
                        _dependentList.Add(_sprr);
                }

                _iOverrideSOList.Clear();
                _iOverrideSOList.AddRange(transform.GetComponentsInChildren<IOverrideSortingOrder>());
            }
            bCorrupted = false;
        }

        List<RegularCollider> _regularColliderList = new List<RegularCollider>();
        List<RegularCollider> RegularColliderList
        {
            get
            {
                update_Child();
                return _regularColliderList;
            }
        }
        #endregion

        #region SortingOrder
        [SerializeField]
        List<IUpdateSortingOrder> _updateCallBack = new List<IUpdateSortingOrder>();
        public void AddUpdateCallBack(IUpdateSortingOrder add)
        {
            if (!_updateCallBack.Contains(add))
                _updateCallBack.Add(add);
        }
        public void RemoveUpdateCallBack(IUpdateSortingOrder remove)
        {
            if (_updateCallBack.Contains(remove))
                _updateCallBack.Remove(remove);
        }        

        [SerializeField]
        int _iExternAdd = 0;
        public int iExternAdd { set { _iExternAdd = value; } get { return _iExternAdd; } }
        public void Update_Transform_SortingOrder()
        {
            update_SortingOrder();
            _updateCallBack.ForEach(r => r.UpdateSortingOrder());
            transform.hasChanged = false;
        }
        public void Update_SortingOrder(bool bJustDoIt = false)
        {
            update_SortingOrder(bJustDoIt);
            update_particleSortingOrder(bJustDoIt);
        }
        public int CalcSortingOrder(bool bWithBasis = true)
        {
            if (bWithBasis && _ISOBasis != null && _ISOBasis.bActivated)
                return _ISOBasis.CalcSortingOrder();
            return IsometricSortingOrderUtility.IsometricSortingOrder(transform) + _iExternAdd;
        }
        void update_particleSortingOrder(bool bJustDoIt = false)
        {
            if (IsoMap.isNormalSOMode)
                return;

            Vector3 _rendererPosition;
            var RSRList = ParticleSystemRendererList;
            for (int i = 0; i < RSRList.Count; ++i)
            {
                _rendererPosition = RSRList[i].transform.position;
                if (bJustDoIt || _particleLastPositionList[i] != _rendererPosition)
                {
                    _particleLastPositionList[i] = _rendererPosition;
                    RSRList[i].sortingOrder = _iExternAdd + iParticleSortingAdd +
                        IsometricSortingOrderUtility.IsometricSortingOrder(RSRList[i].transform);
                }
            }
        }
        void update_SortingOrder(bool bJustDoIt = false)
        {
            if (IsoMap.IsNull) // || !IsoMap.instance.bUseIsometricSorting)
                return;

            bool bCurrpted = IsCorrupted_LastSortingOrder();
            if (!bCurrpted)
                Corrupt_LastSortingOrder();

            if (IsoMap.instance.bUseIsometricSorting)
            {
                int _iNewSortingOrder = CalcSortingOrder();
                if (bJustDoIt || _iNewSortingOrder != iLastSortingOrder)
                {
                    bLastISOMode = IsoMap.instance.bUseIsometricSorting;
                    iLastSortingOrder = _iNewSortingOrder;
                }
            }

            var dependentList = DependentList;

            if (IsoMap.instance.bUseIsometricSorting)
            {
                for (int i = 0; i < dependentList.Count; ++i)
                {
                    dependentList[i].sortingOrder = iLastSortingOrder + i; // +1
                }

                // For external components that inherit the IOverrideSortingOrder interface.
                var overrideList = IOverrideSOList;
                for (int i = 0; i < overrideList.Count; ++i)
                {
                    overrideList[i].sortingOrder = iLastSortingOrder;
                }
            }
#if UNITY_EDITOR
            else if (bCurrpted || bJustDoIt)
            {
                for (int i = 0; i < dependentList.Count && i < _sprrISOListForBackup.Count; ++i)
                {
                    dependentList[i].sortingOrder = _sprrISOListForBackup[i];
                }

                var particleSystemRenderList = ParticleSystemRendererList;
                for (int i = 0; i < particleSystemRenderList.Count && i < _prsISOListForBackup.Count; ++i)
                {
                    particleSystemRenderList[i].sortingOrder = _prsISOListForBackup[i];
                }

                var iOverrideList = IOverrideSOList;
                for (int i = 0; i < iOverrideList.Count && i < _iOverrideISOBackup.Count; ++i)
                {
                    iOverrideList[i].sortingOrder = _iOverrideISOBackup[i];
                }
            }
#endif           

            // For, calculated and updated by themselves.
            var regularColliderList = RegularColliderList;
            for (int i = 0; i < regularColliderList.Count; ++i)
            {
                regularColliderList[i].Update_SortingOrder();
            }
        }
        #endregion

        #region ISOBasis
        [SerializeField]
        ISOBasis _ISOBasis = null;
        public ISOBasis GetISOBasis()
        {
            if (_ISOBasis == null)
                _ISOBasis = GetComponent<ISOBasis>();
            return _ISOBasis;
        }
        public ISOBasis SetUp(ISOBasis basis = null)
        {
            if (basis == null)
                basis = gameObject.AddComponent<ISOBasis>();
            _ISOBasis = basis;
            Update_SortingOrder(true);
            return _ISOBasis;
        }
        public void Remove()
        {
            _ISOBasis = null;
            Update_SortingOrder(true);
        }
        public void DestroyISOBasis()
        {
            if (_ISOBasis == null || _ISOBasis.bDoNotDestroyAutomatically)
                return;
#if UNITY_EDITOR
            Editor.DestroyImmediate(_ISOBasis);
#else
            Destroy(this);
#endif
            Remove();
        }
        public Bounds GetBounds()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0)
                return new Bounds(transform.position, IsoMap.instance.gGrid.TileSize);

            Bounds bounds = new Bounds(renderers[0].bounds.center, renderers[0].bounds.size);
            for (int i = 1; i < renderers.Length; ++i)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }

        public bool IsOnGroundObject()
        {
            if (_ISOBasis)
                return _ISOBasis.isOnGroundObject;

            return IsoMap.isUsingGlobalGroundOffset && GetComponentInParent<IsometricMovement>();
        }

        public void Undo_UpdateDepthFudge(float fFudge, bool bNewFudge = false)
        {
#if UNITY_EDITOR
            if (_ISOBasis)
                _ISOBasis.Update_SortingOrder_And_DepthTransform();
            else
            {
                DependentList.ForEach(r =>
                {
                    var Iso2D = r.GetComponent<Iso2DObject>();
                    if (Iso2D != null)
                    {
                        if (bNewFudge)
                            Iso2D.Undo_NewDepthFudge(fFudge);
                        else
                            Iso2D.Undo_AddDepthFudge(fFudge);
                    }
                });
            }
#endif
        }

        #endregion

        #region MonoBehaviour
        void OnTransformChildrenChanged()
        {
            update_Child(true);
            Update_SortingOrder(true);
        }
        void OnTransformParentChanged()
        {
            Update_SortingOrder(true);
        }
        void OnEnable()
        {
            update_Child(true);
            Update_SortingOrder(true);
        }
        void Update()
        {
            if (!enabled || IsoMap.isNormalSOMode)
                return;

            if (transform.hasChanged)
            {
                Update_Transform_SortingOrder();
            }
            update_particleSortingOrder();
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (enabled)
            {
                update_Child(true);
                Reset_SortingOrder(0, false);
            }
        }
#endif
        #endregion

        #region Runtime
        public void Reset_SortingOrder(int iNewSortingOrder, bool bUndoable)
        {
            const string undoName = "SortingOrder update";

              // bLastISOMode = IsoMap.instance.bUseIsometricSorting;
          iLastSortingOrder = iNewSortingOrder;
#if UNITY_EDITOR
            _sprrISOListForBackup.Clear();
            _prsISOListForBackup.Clear();
#endif
            var dependentList = DependentList;
            if (bUndoable)
                UndoUtil.Record(dependentList.ToArray(), undoName);

            if (IsoMap.isAutoISOMode)
            {
                for (int i = 0; i < dependentList.Count; ++i)
                {
                    dependentList[i].sortingOrder = iLastSortingOrder + i;
                }
            }
            else
            {
                for (int i = 0; i < dependentList.Count; ++i)
                {
                    dependentList[i].sortingOrder = iNewSortingOrder;
                }

                var particleSystemRenderList = ParticleSystemRendererList;
                if (bUndoable)
                    UndoUtil.Record(dependentList.ToArray(), undoName);

                for (int i = 0; i < particleSystemRenderList.Count; ++i)
                {
                    particleSystemRenderList[i].sortingOrder = iNewSortingOrder;
                }
            }

#if UNITY_EDITOR
            _regularColliderList.ForEach((r) => r.ResetSortingOrder(iNewSortingOrder, bUndoable));
#endif

            if (bUndoable)
                UndoUtil.Record(_iOverrideSOList.Select(o => o as Object).Where(o => o != null).ToArray(), undoName);

            _iOverrideSOList.ForEach((o) => o.sortingOrder = iNewSortingOrder);
        }
        #endregion

#if UNITY_EDITOR
        #region MapEditor
        [SerializeField]
        List<int> _sprrISOListForBackup = new List<int>();
        [SerializeField]
        List<int> _prsISOListForBackup = new List<int>();
        [SerializeField]
        List<int> _iOverrideISOBackup = new List<int>();
        public void Clear_Backup(bool bUndoable = true)
        {
            Reset_SortingOrder(iLastSortingOrder, bUndoable);
        }
        public void Revert_SortingOrder()
        {
            var dependentList = DependentList;
            for (int i = 0; i < dependentList.Count; ++i)
            {
                dependentList[i].sortingOrder = _sprrISOListForBackup.Count > i ? _sprrISOListForBackup[i] : 0;
            }
            _sprrISOListForBackup.Clear();

            var particleSystemRenderList = ParticleSystemRendererList;
            for (int i = 0; i < particleSystemRenderList.Count; ++i)
            {
                particleSystemRenderList[i].sortingOrder = _prsISOListForBackup.Count > i ? _prsISOListForBackup[i] : 0;
            }
            _prsISOListForBackup.Clear();

            var iOverrideList = IOverrideSOList;
            for (int i = 0; i < iOverrideList.Count; ++i)
            {
                iOverrideList[i].sortingOrder = _iOverrideISOBackup.Count > i ? _iOverrideISOBackup[i] : 0;
            }
            iOverrideList.Clear();

            var regularColliderList = RegularColliderList;
            regularColliderList.ForEach(r => r.Revert_SortingOrder());
        }
        public void Backup_SortingOrder()
        {
            _sprrISOListForBackup.Clear();
            var dependentList = DependentList;
            dependentList.ForEach(r => _sprrISOListForBackup.Add(r != null ? r.sortingOrder : 0));

            _prsISOListForBackup.Clear();
            var particleSystemRenderList = ParticleSystemRendererList;
            particleSystemRenderList.ForEach(r => _prsISOListForBackup.Add(r != null ? r.sortingOrder : 0));

            _iOverrideISOBackup.Clear();
            var iOverrideList = IOverrideSOList;
            iOverrideList.ForEach(o => _iOverrideISOBackup.Add(o.sortingOrder));

            var regularColliderList = RegularColliderList;
            regularColliderList.ForEach(r => r.Backup_SortingOrder());

        }       

        #endregion MapEditor
#endif
    }
}
