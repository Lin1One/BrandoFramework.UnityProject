using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;
   
    [DisallowMultipleComponent]
    public class IsometricMovement : MethodBTN_MonoBehaviour
    {
        const string Name_X_Axis_Param = "X-Dir";
        const string Name_Z_Axis_Param = "Z-Dir";
        const string Name_Moving_Param = "OnMoving";

        protected InGameDirection _currentInGameDirection;

        public InGameDirection currentInGameDirection
        {
            get { return _currentInGameDirection; }
        }

        [SerializeField]
        Queue<InGameDirection> DirQ = new Queue<InGameDirection>();

        #region MovementCondition

        [Header("Condition"), SerializeField]
        protected LayerMask CollisionLayerMask = new LayerMask();

        [SerializeField]
        public bool b8DirectionalMovement = false;

        [SerializeField]
        protected bool bSnapToGroundGrid = true;

        [SerializeField]
        [Util.ConditionalHide("bSnapToGroundGrid", hideInInspector: true)]
        protected bool bRevertPositionOnCollision = true;

        [SerializeField]
        [Util.ConditionalHide("bSnapToGroundGrid", conditionalSourceValue:"False", hideInInspector: true)]
        bool _bContinuousMovement = true;
        public bool bContinuousMovement { get { return bSnapToGroundGrid == false && _bContinuousMovement == true; } }

        [SerializeField]
        protected bool bJumpWithMove = true;

        [Header("Movement")]
        [SerializeField]
        protected float fSpeed = 2f;

        [SerializeField]
        float fDashSpeedMultiplier = 4f;

        float fTickSpeed
        {
            get
            {
                return (bDashing ? fDashSpeedMultiplier : 1f) * fSpeed * Time.deltaTime;
            }
        }

        [SerializeField]
        protected float fMaxDropHeight = 100f;

        [SerializeField]
        int iMaxQSize = 2;
        #endregion MovementCondition

        #region Jump
        [System.NonSerialized]
        protected Vector3 vJumpOffset = Vector3.zero;

        [Header("Jump"), SerializeField]
        protected AnimationCurve JumpCurve =
            new AnimationCurve(
                new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) }
                );

        [SerializeField]
        float fJumpingHeightMultiplier = 1;

        [SerializeField]
        float fJumpingDurationMultiplier = 1;

        [SerializeField]
        float fJumpingHeight
        {
            get
            {
                return JumpingHeight(Mathf.Clamp(fJumpCurveDuration - fJumpingTime, 0, fJumpCurveDuration));
            }
        }

        protected float JumpingHeight(float fNormalizedJumingTime)
        {
            return fJumpingHeightMultiplier * JumpCurve.Evaluate(fNormalizedJumingTime);
        }

        [System.NonSerialized]
        float fJumpingTime = 0;
        protected bool isOnJumping
        {
            get
            { return fJumpingTime > 0f;
            }
        }

        [System.NonSerialized]
        float fJumpCurveDuration = 1;
        protected float fTotalJumpingDuration
        {
            get
            {
                return fJumpCurveDuration * fJumpingDurationMultiplier;
            }
        }

        virtual public void Jump()
        {
            if (bJumpWithMove && isOnGround && !isOnJumping)
            {
                jumpStart();
            }
            else
                EnQueueDirection(InGameDirection.Jump_Move);
            return;
        }

        protected void jumpInit()
        {
            fJumpCurveDuration = JumpCurve.keys.Last().time - JumpCurve.keys.First().time;
            jumpReset();
        }

        protected void jumpStart()
        {
            fJumpingTime = fJumpCurveDuration;
        }

        virtual protected void jumpReset()
        {
            fJumpingTime = 0;
            vJumpOffset = Vector3.zero;
        }

        #endregion Jump

        #region Character
        [Header("Character")]
        [SerializeField]
        protected bool bRotateToDirection = false;

        virtual public bool isOnGround
        {
            get { return true; }
        }
        virtual public Transform cTransform
        {
            get
            {
                return transform;
            }
        }

        [SerializeField]
        protected Collider characterCollider;
        Collider cCollider
        {
            get
            {
                if (characterCollider == null)
                    characterCollider = GetComponentInChildren<CapsuleCollider>();

                if (characterCollider == null)
                    characterCollider = GetComponentInChildren<CharacterController>();

                if (characterCollider == null)
                    characterCollider = GetComponentInChildren<Collider>();

                return characterCollider;
            }
        }

        virtual public Bounds GetBounds()
        {
            return cCollider.bounds;
        }
        #endregion

        #region Animator
        [SerializeField]
        protected Animator animator;

        [SerializeField]
        [Util.ConditionalHide("animator", Inverse =true,  ConditionalValueString = "null", HidenInspector = true, HeaderString ="Params: X-Dir, Z-Dir, OnMoving")]
        protected bool bUseParameterForAnimator = false;

        [SerializeField]
        [Util.ConditionalHide("animator", Inverse = true, ConditionalValueString = "null", HidenInspector = true, HeaderString = "")]
        protected bool bUseXAxisFlipAnimation = false;

        protected void UpdateAnimatorParams()
        {
            UpdateAnimatorParams(bOnMoving, vHorizontalMovement.x, vHorizontalMovement.z);
        }

        protected void UpdateAnimatorParams(bool bMovingFlag, float xDir, float zDir)
        {
            if (animator == null)
                return;

            if (bUseParameterForAnimator)
            {
                animator.SetBool(   Name_Moving_Param, bMovingFlag);
                if (bMovingFlag)
                {
                    animator.SetFloat(Name_X_Axis_Param, xDir);
                    animator.SetFloat(Name_Z_Axis_Param, zDir);
                }
            }

            if (bUseXAxisFlipAnimation)
            {
                if (xDir * animator.gameObject.transform.localScale.x < 0)
                    animator.gameObject.transform.localScale = Vector3.Scale(animator.gameObject.transform.localScale, new Vector3(-1, 1, 1));
            }
        }

        #endregion

        #region MovingStatus
        [Header("Movement Status")]
        [SerializeField]
        InGameDirection LastDirection = InGameDirection.Top_Move;

        [SerializeField]
        protected GameObject destination;

        [SerializeField]
        protected bool bOnMoving = false;

        [SerializeField]
        bool bDashing = false;

        public bool bMovingDirection(InGameDirection direction)
        {
            return bOnMoving && LastDirection.Equals(direction);
        }

        [SerializeField]
        protected Vector3 vHorizontalMovement;

        [SerializeField]
        protected Vector3 vDestinationCoordinates;

        [SerializeField]
        protected Vector3 vLastCoordinates;

        protected float fMinMovement = 0f;
        protected float fSqrtMinMovement = 0f;
        #endregion

        #region Grounding        
        [Header("Ground")]
        [SerializeField]
        protected float fGridTolerance = 0.05f;

        [SerializeField]
        IsoTileBulk groundBulk;

        protected Grid groundGrid
        {
            get
            {
                return groundBulk == null ? IsoMap.instance.gGrid : groundBulk.coordinates.grid;
            }
        }

        protected Vector3 groundGridPos(Vector3 coordinates, bool bSnap)
        {
            return groundGrid ? groundGrid.CoordinatesToPosition(coordinates, bSnap) : 
                SnapPosition(coordinates, bSnap);
        }

        Vector3 groundGridCoordinates(Vector3 position, bool bSnap)
        {
            return groundGrid ? groundGrid.PositionToCoordinates(position, bSnap) : 
                SnapPosition(position, bSnap);
        }

        Vector3 groundGridUnSnapedPosition(Vector3 position)
        {
            return groundGridPos(groundGridCoordinates(position, true) - groundGridCoordinates(position, false), false);
        }

        void groundBulkUpdate(GameObject groundObject)
        {
            groundBulk = groundObject.GetComponentInParent<IsoTileBulk>();
        }

        virtual public bool Grounding(Vector3 position, float fRayLength)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(position, Vector3.down, out raycastHit,
                fRayLength, CollisionLayerMask, QueryTriggerInteraction.Ignore))
            {
                groundBulkUpdate(raycastHit.collider.gameObject);
                return true;
            }
            return false;
        }
        #endregion

        #region SortingOrder
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

        virtual public int SortingOrder_Adjustment()
        {
            return sortingOrder ? sortingOrder.iExternAdd : 0;
        }
        #endregion

        #region MoveFunction
        void Dash()
        {
            if (!bDashing)
                ExecuteDir(InGameDirection.Dash);
        }

        virtual public bool EnQueueDirection(InGameDirection dir)
        {
            if (dir == InGameDirection.Dash)
            {
                Dash();
                return true;
            }

            if (DirQ.Count >= iMaxQSize)
                return false;

            DirQ.Enqueue(dir);
            return true;
        }

        virtual public void DirectTranslate(Vector3 vTranslate)
        {
            Bounds bounds = GetBounds();
            var vRadius = bounds.extents;
            var vDir = vTranslate.normalized;
            vTranslate = vTranslate * fSpeed;
            if (Grounding(bounds.center + vTranslate + vDir * (vRadius.x + vRadius.z) * 0.5f, fMaxDropHeight))
                SetHorizontalMovement(vTranslate);
        }

        protected void SetHorizontalMovement(Vector3 vTmp)
        {
            LastDirection = Convert(vTmp);
            vHorizontalMovement = vTmp;
            vHorizontalMovement.y = 0;

            if (vHorizontalMovement.sqrMagnitude >= fSqrtMinMovement)
                bOnMoving = true;

            UpdateAnimatorParams();
        }

        void RotateTo(InGameDirection dir)
        {
            Vector3 vDir = HorizontalVector(dir);
            UpdateAnimatorParams(false, vDir.x, vDir.z);
        }

        protected void ExecuteDir(InGameDirection dir)
        {
            bool bMove = dir > 0;
            if (!bMove)
            {
                dir = (InGameDirection)(-1 * (int)dir);
            }

            if (dir.Equals(InGameDirection.Dash) || (!bOnMoving && (isOnGround || bJumpWithMove)))
            {
                if (!bJumpWithMove && dir.Equals(InGameDirection.Jump_Move))
                {
                    jumpStart();
                    dir = LastDirection;
                }

                if (bDashing = dir.Equals(InGameDirection.Dash))
                    dir = LastDirection;
                else
                    LastDirection = dir;

                // float fAngle = IsometricMovement.AngleOfForward(dir);

                if (bRotateToDirection)
                {
                    RotateTo(dir);
                    // cTransform.localEulerAngles = new Vector3(0, fAngle, 0);
                }

                if (bMove)
                {
                    Vector3 v3TmpCoordinates = IsometricMovement.HorizontalVector(dir);
                    if (!bDashing && bSnapToGroundGrid)
                    {
                        Vector3 vUnSnapedPosition = groundGridUnSnapedPosition(cTransform.position);
                        if (vUnSnapedPosition.magnitude > 0.2f &&
                            Vector3.Dot(v3TmpCoordinates.normalized, vUnSnapedPosition.normalized) > 0.866f) //0.866 means angle < 30
                        {
                            v3TmpCoordinates = -vUnSnapedPosition;
                        }
                    }

                    Vector3 v3TmpPosition = cTransform.position + groundGridPos(v3TmpCoordinates, bSnapToGroundGrid);
                    if (!bDashing && bSnapToGroundGrid)
                    {
                        v3TmpPosition += groundGridUnSnapedPosition(v3TmpPosition);
                    }

                    if (bDashing)
                        v3TmpPosition += vHorizontalMovement;

                    bMove = Grounding(v3TmpPosition, fMaxDropHeight);
                    if (bMove)
                    {
                        if (!bDashing)
                            vLastCoordinates = vDestinationCoordinates;
                        vDestinationCoordinates += v3TmpCoordinates;
                        SetHorizontalMovement(v3TmpPosition - cTransform.position);
                    }
                }
            }
            else
            {
                EnQueueDirection(dir);
            }
        }

        protected Vector3 GetRevertVector()
        {
            Vector3 vDest = Vector3.zero;
            if (bSnapToGroundGrid && bRevertPositionOnCollision)
            {
                vDest = groundGrid ? groundGrid.PositionToCoordinates(cTransform.position, false) : cTransform.position;
                vDest.y = 0;
                float fLength = (vLastCoordinates - vDest).magnitude;
                vLastCoordinates = Vector3.MoveTowards(vDest, vLastCoordinates, fLength - Mathf.FloorToInt(fLength));
                vDestinationCoordinates = vLastCoordinates;
                vDest = groundGridPos(vDestinationCoordinates, bSnapToGroundGrid) - cTransform.position;
            }
            return vDest;
        }

        protected Vector3 GetVerticalMovementVector()
        {
            float fDeltaTime = Time.deltaTime;
            Vector3 vGravity = Physics.gravity;

            if (isOnJumping)
            {
                fJumpingTime -= fDeltaTime / fJumpingDurationMultiplier;
                if (!isOnJumping)
                    jumpReset();

                return -vGravity.normalized * fJumpingHeight;
            }

            if (isOnGround)
                return Vector3.zero;

            return vGravity * fDeltaTime;
        }

        protected Vector3 GetTickMovementVector(Vector3 destination)
        {
            return Vector3.MoveTowards(Vector3.zero, destination, fTickSpeed);
        }

        virtual protected Vector3 GetHorizontalMovementVector()
        {
            Vector3 vMovementTmp = Vector3.zero;

            if (bOnMoving)
            {
                if (vHorizontalMovement.magnitude <= fMinMovement)
                {
                    Grounding(transform.localPosition, 1f);
                    Arrival();
                }
                else
                {
                    vMovementTmp = GetTickMovementVector(vHorizontalMovement);
                    vHorizontalMovement -= vMovementTmp;
                }
            }
            return vMovementTmp;
        }

        virtual protected Vector3 GetMovementVector()
        {
            Vector3 vTmp = GetVerticalMovementVector();
            if (isOnJumping)
            {
                Vector3 vGap = vTmp - vJumpOffset;
                vJumpOffset = vTmp;
                vTmp = vGap;
            }
            return GetHorizontalMovementVector() + vTmp;
        }

        virtual protected void ApplyMovement(Vector3 vMovement)
        {
            if (!vMovement.Equals(Vector3.zero))
                cTransform.position += vMovement;
        }

        virtual protected void Arrival()
        {         
            vHorizontalMovement = Vector3.zero;
            bOnMoving = bDashing = false;
            UpdateAnimatorParams();
        }

        #endregion

        #region GameObject
        virtual public void Start()
        {
            if (animator == null)
                animator = gameObject.GetComponent<Animator>();
            if (animator == null)
                animator = gameObject.GetComponentInChildren<Animator>();

            jumpInit();

            if (CollisionLayerMask == 0)
                CollisionLayerMask = 1 << LayerMask.NameToLayer("Default");

            SetMinMoveDistance(fGridTolerance);

            //if (destination)
            //    vDestinationCoordinates = SnapPosition(destination.transform.position, bSnapToGroundGrid);
        }

        protected void SetMinMoveDistance(float _fMin)
        {
            fMinMovement = Mathf.Abs(_fMin);
            fSqrtMinMovement = fMinMovement * fMinMovement;
        }

        virtual public void Update()
        {
            if (Application.isPlaying)
            {
                if (!bOnMoving && DirQ.Count > 0)
                {
                    ExecuteDir(DirQ.Dequeue());
                }
                ApplyMovement(GetMovementVector());
            }

            if (transform.hasChanged && IsoMap.instance.bUseIsometricSorting && sortingOrder != null)
            {
                sortingOrder.iExternAdd = SortingOrder_Adjustment();
            }
        }
        #endregion

        #region Static Methods
        protected static Vector3 SnapPosition(Vector3 position, bool bSnap)
        {
            if (bSnap)
                return new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
            return position;
        }
        protected Vector3 HorizontalVector(Vector3 vDir)
        {
            return HorizontalVector(Convert(vDir));
        }
        public static Vector3 HorizontalVector(InGameDirection dir)
        {
            float fMultiplier = 1f;
            if (dir.Equals(InGameDirection.Left_Move) || dir.Equals(InGameDirection.Top_Move)
                        || dir.Equals(InGameDirection.Right_Move) || dir.Equals(InGameDirection.Down_Move))
                fMultiplier *= 1.414f;
            return Quaternion.AngleAxis(AngleOfForward(dir), Vector3.up) * Vector3.forward * fMultiplier;
        }
        protected static float AngleOfForward(InGameDirection dir)
        {
            return (int)dir * 45f;
        }
        protected InGameDirection Convert(Vector3 vMoveTo)
        {
            if (b8DirectionalMovement)
            {
                _currentInGameDirection = convert_8Dir(vMoveTo);
            }
            else
            {
                _currentInGameDirection = convert_4Dir(vMoveTo);
            }

            return currentInGameDirection;
            
            //return b8DirectionalMovement ? convert_8Dir(vMoveTo) : convert_4Dir(vMoveTo);
        }
        
        static InGameDirection convert_4Dir(Vector3 vMoveTo)
        {
            bool isXAxis = Mathf.Abs(vMoveTo.x) >= Mathf.Abs(vMoveTo.z);
            InGameDirection result = InGameDirection.Jump_Move;
            if (isXAxis)
            {
                if (vMoveTo.x > 0)
                    result = InGameDirection.RD_Move;
                else
                    result = InGameDirection.LT_Move;
            }
            else
            {
                if (vMoveTo.z > 0)
                    result = InGameDirection.RT_Move;
                else
                    result = InGameDirection.LD_Move;
            }
            return result;
        }
        static InGameDirection convert_8Dir(Vector3 vMoveTo)
        {
            InGameDirection result = InGameDirection.None;
            float x = vMoveTo.x;
            float z = vMoveTo.z;
            float absX = Mathf.Abs(x);
            float absZ = Mathf.Abs(z);
            bool bPositiveX = x > 0;
            bool bPositiveZ = z > 0;

            if (x * z == 0)
            {
                if (bPositiveX)         result = InGameDirection.RD_Move;
                else if (!bPositiveX)   result = InGameDirection.LT_Move;
                else if (bPositiveZ)    result = InGameDirection.RT_Move;
                else if (!bPositiveZ)   result = InGameDirection.LD_Move;
            } 
            else if (absX / absZ > 1.5f)
                result = bPositiveX ? InGameDirection.RD_Move : InGameDirection.LT_Move;
            else if (absX / absZ < 0.5f)
                result = bPositiveZ ? InGameDirection.RT_Move : InGameDirection.LD_Move;
            else // 1.5f > x / z > 0.5f
            {
                if (bPositiveX)
                    result = bPositiveZ ? InGameDirection.Right_Move : InGameDirection.Down_Move;
                else
                    result = bPositiveZ ? InGameDirection.Top_Move : InGameDirection.Left_Move;
            }

            return result;
        }
        protected static bool IsOppositSide(InGameDirection dirA, InGameDirection dirB)
        {
            return Mathf.Abs(dirA - dirB) == (int) InGameDirection.OppositeDir;
        }
        #endregion

#if UNITY_EDITOR
        #region MethodBTN
        [Header("Tall Character Helper"), SerializeField]
        TallCharacterHelper _tch;
        
        [MethodBTN(false)]
        public void Add_TallCharacterHelper()
        {
            if (PrefabHelper.IsPrefab(gameObject))
            {
                Debug.LogError("Not available when Prefab.");
                return;
            }

            findTCH();
            if (_tch != null)
            {
                Debug.Log("This already has a TCH.");
                return;
            }

            string err = null;
            if (IsoMap.IsNull)
                err = "There is no IsoMap instance!";
            else if (IsoMap.instance.TchPrefab == null)
                err = "There is no IsoMap.Instance.TCHPrefab!";
            else
            {
                _tch = Instantiate(IsoMap.instance.TchPrefab).GetComponent<TallCharacterHelper>();
                if (_tch == null)
                    err = "There is no TallCharacterHelper component in the IsoMap.Instance.TCHPrefab!!";
            }

            if (err != null)
            {
                Debug.LogError(err);
                return;
            }

            _tch.transform.SetParent(transform);
            _tch.transform.localPosition = Vector3.zero;
            _tch.Init();
        }

        [MethodBTN(false)]
        public void DropToTheFloorCollider()
        {
            cCollider.DropToFloor();
        }


        private void OnValidate()
        {
            if (isActiveAndEnabled)
                findTCH();
        }
        void findTCH()
        {
            if (_tch == null)
                _tch = GetComponentInChildren<TallCharacterHelper>();
        }
        #endregion
#endif
    }
}