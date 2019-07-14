using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(IISOBasis))]
    [System.Serializable]
    public class ISOBasis : MonoBehaviour, IGizmoDraw
    {
        public bool bActivated = true;
        public bool bDoNotDestroyAutomatically = true;
        public bool isOnGroundObject = true;

        [SerializeField]
        Vector3 _ISO_Offset = new Vector3(0, -0.5f, 0);

        [SerializeField]
        IISOBasis[] _ISOTarget;
        public IISOBasis[] ISOTarget { get {
                return _ISOTarget != null && _ISOTarget.Length != 0 ? _ISOTarget : _ISOTarget = GetComponents<IISOBasis>();
            }
        }

        [System.Serializable]
        class TransformNFloat
        {
            [SerializeField]
            public Transform _t;

            [SerializeField]
            public float _f;

            public TransformNFloat(Transform __t, float __f)
            {
                _t = __t;
                _f = __f;
            }

            public void SetF(float __f)
            {
                _f = __f;
            }
        }

        [SerializeField]
        List<TransformNFloat> _depthedTrasforms = new List<TransformNFloat>();
        // Dictionary<Transform, float> _depthedTrasforms = new Dictionary<Transform, float>();

        [SerializeField]
        List<Transform> transforms;

        public ISOBasis Parent
        {
            get
            {
                if (transform.parent == null)
                    return null;
                return transform.parent.GetComponent<ISOBasis>();
            }
        }

#if UNITY_EDITOR


        private void Awake()
        {
            Init();
        }

        void Init()
        {
            foreach (var one in ISOTarget)
                one.SetUp(this);
        }

        private void OnDestroy()
        {
            RevertDepth_Transforms(true);
            if (IsoMap.fCurrentOnGroundOffset != 0)
            {
                ApplyDepth_Transforms();
            }
            resetHierarchyDependence();
        }

        void resetHierarchyDependence()
        {
            foreach (var one in ISOTarget)
                one.Remove();
            _ISOTarget = null;
        }        

        protected void OnDrawGizmosSelected()
        {
            GizmoDraw();
        }

        public void GizmoDraw()
        {
            if (!bActivated || IsoMap.IsNull)
                return;

            // Draw cyan offsetBounds
            Bounds bounds = GetBounds();
            Vector3 vOffset = getSortingOrderBasis(bounds);
            if (Parent == null && isOnGroundObject)
                vOffset -= IsoMap.instance.VOnGroundOffset;

            float fHeight = 0;
            const float fGap = 0.25f;
            Collider _col = GetCollider();
            if (_col)
                _col = _col.fAboveGround(vOffset + Vector3.up * fGap, ref fHeight);
            
            fHeight -= fGap;
            Vector3 vOnGround = vOffset + fHeight * Vector3.down;

            if (IsoMap.instance.bUseIsometricSorting)
            {
                bool bSelected = UnityEditor.Selection.gameObjects.Contains(gameObject);

                Gizmos.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, bSelected ? 1 : 0.75f);
                Gizmos.DrawLine(new Vector3(bounds.min.x, vOffset.y, vOffset.z), new Vector3(bounds.max.x, vOffset.y, vOffset.z));
                Gizmos.DrawLine(new Vector3(vOffset.x, bounds.min.y, vOffset.z), new Vector3(vOffset.x, bounds.max.y, vOffset.z));
                Gizmos.DrawLine(new Vector3(vOffset.x, vOffset.y, bounds.min.z), new Vector3(vOffset.x, vOffset.y, bounds.max.z));
            }
            Gizmos.DrawWireSphere(vOnGround, 0.03f);

            // Draw white Offset via Parent
            Vector3 finalSOOffset = vOffset + GetOffsetViaParentRC();
            Gizmos.color = Color.white;
            Gizmos.DrawLine(vOffset, finalSOOffset);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(vOffset, 0.005f);
            Gizmos.DrawWireSphere(finalSOOffset, 0.015f);

            Gizmos.color = new Color32(255, 70, 0, 200);
            Gizmos.DrawLine(vOffset, vOnGround);
            if (_col)
            {
                bounds = _col.bounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        public void Update_SortingOrder_And_DepthTransform()
        {
            if (IsoMap.IsNull)
                return;

            if (ISOTarget != null)
            {
                foreach (var one in _ISOTarget)
                    one.Update_SortingOrder(true);
            }

            if (isOnGroundObject)
                ApplyDepth_Transforms();
            else
                RevertDepth_Transforms();
        }
#endif

        Vector3 GetOffsetViaParentRC()
        {
            ISOBasis parent = Parent;

            if (parent == null)
                return isOnGroundObject && !IsoMap.IsNull ? IsoMap.instance.VOnGroundOffset : Vector3.zero;

            Bounds bounds = parent.GetBounds();
            Vector3 vBasis = parent.getSortingOrderBasis(bounds) - bounds.center;
            return vBasis + parent.GetOffsetViaParentRC();
        }

        Collider GetCollider()
        {
            if (ISOTarget != null && ISOTarget.Length > 0)
            {
                if (ISOTarget.Any(r => r is RegularCollider))
                    return (ISOTarget.First(r => r is RegularCollider) as RegularCollider).BC;
            }

            return GetComponent<Collider>();
        }

        Bounds GetBounds()
        {
            if (ISOTarget == null || ISOTarget.Length == 0)
                return new Bounds(transform.position, Vector3.zero);

            if (ISOTarget.Any(r => r is RegularCollider))
                return ISOTarget.First(r => r is RegularCollider).GetBounds();

            var boundsArray = GetComponents<Collider>().Where(r => !r.isTrigger).Select(r => r.bounds).ToArray();
            if (boundsArray == null || boundsArray.Length == 0)
            {
                boundsArray = ISOTarget.Select(r => r.GetBounds()).ToArray();
            }

            Bounds bounds = boundsArray[0];
            for (int i = 1; i < boundsArray.Length; ++i)
                bounds.Encapsulate(boundsArray[i]);

            return bounds;
        }

        Vector3 getSortingOrderBasis()
        {
            return getSortingOrderBasis(GetBounds());
        }

        Vector3 getSortingOrderBasis(Bounds bounds)
        {
            Vector3 vBasis = bounds.center + Vector3.Scale(_ISO_Offset, bounds.size);
            if (Parent == null && isOnGroundObject && !IsoMap.IsNull)
                vBasis += IsoMap.instance.VOnGroundOffset;
            return vBasis;
        }

        //private void OnValidate()
        //{
        //    Update_SortingOrder();
        //}

        public int CalcSortingOrder()
        {
            return IsometricSortingOrderUtility.IsometricSortingOrder(getSortingOrderBasis() + GetOffsetViaParentRC());
        }

        public void AutoSetup_DepthTransforms()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Depthed Transfer: Auto Setup");
#endif
            // Moving the Collider will affect the game logic.
            var lookups = GetComponentsInChildren<Transform>().Where(r => r.GetComponent<Collider>() == null);
            transforms = lookups.Where(r => lookups.All(a => a == r || !r.IsChildOf(a))).ToList();
            if (isOnGroundObject)
            {
                transforms.ForEach(t =>
                {
                    if (!_depthedTrasforms.Exists(one => one._t == t))
                    {
                        if (t.GetComponentsInChildren<Iso2DObject>().All(isd2D => isd2D.bGroundOffsetMark))
                            _depthedTrasforms.Add(new TransformNFloat(t, IsoMap.fCurrentOnGroundOffset));
                    }
                });
            }
            CheckDepth_Transform();
            MarkToDepthedTransformsIso2DObjects(true);
        }

        public void Clear_DepthTransforms()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "ISOBasis : Clear_DepthTransforms()");
#endif
            MarkToDepthedTransformsIso2DObjects(false);
            if (isOnGroundObject)
            {
                RevertDepth_Transforms(true);
            }
            else
                _depthedTrasforms.Clear();
            transforms.Clear();
        }

        public void MarkToDepthedTransformsIso2DObjects(bool bMark)
        {
            foreach (var _tNF in _depthedTrasforms)
            {
                foreach (var one in _tNF._t.GetComponentsInChildren<Iso2DObject>())
                    one.bGroundOffsetMark = bMark;
            }
        }

        public void CheckDepth_Transform()
        {
            RevertDepth_Transforms(false);
            //UpdateDepth_Transforms();
            ApplyDepth_Transforms();
        }

        public void ApplyDepth_Transforms()
        {
            UpdateDepth_Transforms();

            if (transforms == null || transforms.Count == 0 || IsoMap.IsNull)
                return;

            transforms.RemoveAll(r => r == null);
            transforms = transforms.Distinct().ToList();
            var lookups = transforms.Where(r => _depthedTrasforms == null || _depthedTrasforms.Count == 0 || !_depthedTrasforms.Exists(tf => tf._t == r));
            var enumerator = lookups.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var one = enumerator.Current;
                _depthedTrasforms.Add(new TransformNFloat(one, applyFudgeToTranforms(one)));
            }
        }

        public void UpdateDepth_Transforms()
        {
            if (_depthedTrasforms == null || _depthedTrasforms.Count == 0 || IsoMap.IsNull)
                return;

            var fTarget = isOnGroundObject ? IsoMap.instance.fOnGroundOffset : 0;
            var enumerator = _depthedTrasforms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var one = enumerator.Current;
                applyDepthToTranforms(one._t, fTarget - one._f);
                one.SetF(fTarget);
            }
        }

        public void RevertDepth_Transforms(bool bAllClear = true)
        {
            if (_depthedTrasforms == null || _depthedTrasforms.Count == 0 || IsoMap.IsNull)
                return;

            List<Transform> removeList = new List<Transform>();
            var enumerator = _depthedTrasforms.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var one = enumerator.Current;
                if (!bAllClear && transforms.Contains(one._t))
                    continue;

                revertDepthedTransforms(one._t, one._f);
                removeList.Add(one._t);
//#if UNITY_EDITOR
//                Iso2DObject[] iso2Ds = one._t.GetComponentsInChildren<Iso2DObject>();
//                foreach (var iso2D in iso2Ds)
//                    iso2D.Undo_AddDepthFudgeOnly(one._f);
//#endif
            }

            _depthedTrasforms.RemoveAll(o => o._t == null || removeList.Exists(r => r == o._t));
        }

        static float applyFudgeToTranforms(Transform _t)
        {
            return applyDepthToTranforms(_t, IsoMap.instance.fOnGroundOffset);
        }

        static float applyDepthToTranforms(Transform _t, float fFudge)
        {
            if (IsoMap.IsNull)
                return fFudge;

            var vDiff = IsoMap.instance.vDepthFudge(fFudge);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_t, "DepthedTransform");
#endif
            _t.Translate(-vDiff, Space.World);
            return fFudge;
        }

        static void revertDepthedTransforms(Transform _t, float fFudge)
        {
            if (IsoMap.IsNull)
                return;

            var vDiff = IsoMap.instance.vDepthFudge(fFudge);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(_t, "DepthedTransform");
#endif
            _t.Translate(vDiff, Space.World);

        }
    }
}
