using Anonym.Isometric;
using Anonym.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.GamePlaying.Unit
{
    [DisallowMultipleComponent]
    public class IsoUnitControllerBase : UnitComponentBase
    {
        private UnitEntityBase unitEntity;

        protected InGameDirection _currentInGameDirection;

        public InGameDirection currentInGameDirection
        {
            get { return _currentInGameDirection; }
        }

        [SerializeField]
        Queue<InGameDirection> DirQ = new Queue<InGameDirection>();

        #region 组件生命周期

        protected override void OnInit()
        {
            GetUnitAnimator();
            jumpInit();
            InitLayerMask();
            SetMinMoveDistance(fGridTolerance);
        }

        public virtual void OnUpdate()
        {
            ///处理朝向，位置
            if (Application.isPlaying)
            {
                if (!bOnMoving && DirQ.Count > 0)
                {
                    ExecuteDir(DirQ.Dequeue());
                }
                //应用新位置
                ApplyMovement(GetMovementVector());
            }

            ///排序
            if (unitTransform.hasChanged && IsoMap.instance.bUseIsometricSorting && sortingOrder != null)
            {
                sortingOrder.iExternAdd = SortingOrder_Adjustment();
            }
        }

        protected override void OnRelease()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Layer

        protected LayerMask CollisionLayerMask = new LayerMask();

        private void InitLayerMask()
        {
            if (CollisionLayerMask == 0)
                CollisionLayerMask = 1 << LayerMask.NameToLayer("Default");
        }

        #endregion

        #region 控制参数

        //是否 8 方向移动
        public bool b8DirectionalMovement = false;

        //按网格移动
        protected bool bSnapToGroundGrid = true;

        protected bool bRevertPositionOnCollision = true;

        //连续移动
        bool _bContinuousMovement = true;
        public bool bContinuousMovement
        {
            get
            {
                return bSnapToGroundGrid == false && _bContinuousMovement == true;
            }
        }

        //跳跃可移动
        protected bool bJumpWithMove = true;

        //是否冲刺
        bool bDashing = false;

        //速度
        protected float fSpeed = 2f;

        //冲刺速度倍数
        float fDashSpeedMultiplier = 4f;

        // Tick 速度
        float fTickSpeed
        {
            get
            {
                return (bDashing ? fDashSpeedMultiplier : 1f) * fSpeed * Time.deltaTime;
            }
        }

        //最大可下落高度
        protected float fMaxDropHeight = 100f;


        int iMaxQSize = 2;

        #endregion

        #region 跳跃参数 Jump

        protected Vector3 vJumpOffset = Vector3.zero;

        //跳跃曲线
        protected AnimationCurve JumpCurve = new AnimationCurve(
                new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) }
                );

        //跳跃高度倍数
        float fJumpingHeightMultiplier = 1;

        //跳跃距离倍数
        float fJumpingDurationMultiplier = 1;

        //跳跃高度
        float fJumpingHeight
        {
            get
            {
                return JumpingHeight(Mathf.Clamp(fJumpCurveDuration - fJumpingTime, 0, fJumpCurveDuration));
            }
        }

        //滞空高度
        protected float JumpingHeight(float fNormalizedJumingTime)
        {
            return fJumpingHeightMultiplier * JumpCurve.Evaluate(fNormalizedJumingTime);
        }

        //滞空时间
        float fJumpingTime = 0;

        //是否滞空
        protected bool isOnJumping
        {
            get
            {
                return fJumpingTime > 0f;
            }
        }

        //跳跃持续时间
        float fJumpCurveDuration = 1;

        
        protected float fTotalJumpingDuration
        {
            get
            {
                return fJumpCurveDuration * fJumpingDurationMultiplier;
            }
        }

        public virtual void Jump()
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

        //跳跃开始
        protected void jumpStart()
        {
            fJumpingTime = fJumpCurveDuration;
        }

        protected virtual void jumpReset()
        {
            fJumpingTime = 0;
            vJumpOffset = Vector3.zero;
        }

        #endregion Jump

        #region 控制对象 Character

        //朝向
        protected bool bRotateToDirection = false;

        //是否在地面
        public virtual bool isOnGround
        {
            get { return true; }
        }

        public virtual Transform unitTransform
        {
            get
            {
                return unitEntity.UnitTrans.Trans;
            }
        }

        /// <summary>
        /// 碰撞器
        /// </summary>
        protected Collider characterCollider;
        private Collider cCollider
        {
            get
            {
                //if (characterCollider == null)
                //    characterCollider = GetComponentInChildren<CapsuleCollider>();

                //if (characterCollider == null)
                //    characterCollider = GetComponentInChildren<CharacterController>();

                //if (characterCollider == null)
                //    characterCollider = GetComponentInChildren<Collider>();

                return characterCollider;
            }
        }

        /// <summary>
        /// 碰撞器边界
        /// </summary>
        /// <returns></returns>
        public virtual Bounds GetColliderBounds()
        {
            return cCollider.bounds;
        }
        #endregion

        #region 动画机 Animator

        //Animator 参数
        const string Name_X_Axis_Param = "X-Dir";
        const string Name_Z_Axis_Param = "Z-Dir";
        const string Name_Moving_Param = "OnMoving";
        /// <summary>
        /// 动画机
        /// </summary>
        protected Animator animator;

        private void GetUnitAnimator()
        {
            ////if (animator == null)
            ////    animator = gameObject.GetComponent<Animator>();
            ////if (animator == null)
            ////    animator = gameObject.GetComponentInChildren<Animator>();
        }


        protected bool bUseParameterForAnimator = false;

        protected bool bUseXAxisFlipAnimation = false;

        /// <summary>
        /// 更新动画
        /// </summary>
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
                animator.SetBool(Name_Moving_Param, bMovingFlag);
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

        #region 移动状态MovingStatus

        InGameDirection LastDirection = InGameDirection.Top_Move;

        //目标点
        protected GameObject destination;


        protected bool bOnMoving = false;

        public bool bMovingDirection(InGameDirection direction)
        {
            return bOnMoving && LastDirection.Equals(direction);
        }

        /// <summary>
        /// 水平面移动量
        /// </summary>
        [SerializeField]
        protected Vector3 vHorizontalMovement;

        /// <summary>
        /// 目标物体坐标
        /// </summary>
        [SerializeField]
        protected Vector3 vDestinationCoordinates;

        [SerializeField]
        protected Vector3 vLastCoordinates;

        protected float fMinMovement = 0f;
        protected float fSqrtMinMovement = 0f;

        protected void SetMinMoveDistance(float _fMin)
        {
            fMinMovement = Mathf.Abs(_fMin);
            fSqrtMinMovement = fMinMovement * fMinMovement;
        }
        #endregion

        #region 地面位置相关方法 Grounding        
        [Header("Ground")]
        [SerializeField]
        protected float fGridTolerance = 0.05f;

        //所在地块
        IsoTileBulk groundBulk;

        protected Anonym.Isometric.Grid groundGrid
        {
            get
            {
                return groundBulk == null ? IsoMap.instance.gGrid : groundBulk.coordinates.grid;
            }
        }

        Vector3 groundGridUnSnapedPosition(Vector3 position)
        {
            return groundGridPos(groundGridCoordinates(position, true) - groundGridCoordinates(position, false), false);
        }

        protected Vector3 groundGridPos(Vector3 coordinates, bool bSnap)
        {
            return groundGrid ?
                groundGrid.CoordinatesToPosition(coordinates, bSnap) :
                SnapPosition(coordinates, bSnap);
        }

        Vector3 groundGridCoordinates(Vector3 position, bool bSnap)
        {
            return groundGrid ?
                groundGrid.PositionToCoordinates(position, bSnap) :
                SnapPosition(position, bSnap);
        }

        void groundBulkUpdate(GameObject groundObject)
        {
            groundBulk = groundObject.GetComponentInParent<IsoTileBulk>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fRayLength"></param>
        /// <returns></returns>
        public virtual bool Grounding(Vector3 position, float fRayLength)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(position, Vector3.down, out raycastHit,
                fRayLength, 0, QueryTriggerInteraction.Ignore))
            {
                groundBulkUpdate(raycastHit.collider.gameObject);
                return true;
            }
            return false;
        }
        #endregion

        #region 排序 SortingOrder

        IsometricSortingOrder _so = null;

        public IsometricSortingOrder sortingOrder
        {
            get
            {
                return _so;// != null ? _so : _so = GetComponent<IsometricSortingOrder>();
            }
        }

        virtual public int SortingOrder_Adjustment()
        {
            return sortingOrder ? sortingOrder.iExternAdd : 0;
        }
        #endregion

        #region 移动函数 MoveFunction

        /// <summary>
        /// 处理方向
        /// </summary>
        /// <param name="dir"></param>
        protected void ExecuteDir(InGameDirection dir)
        {
            //大于 0 为移动，小于 0 为转向
            bool bMove = dir > 0;
            if (!bMove)
            {
                dir = (InGameDirection)(-1 * (int)dir);
            }

            //冲刺，没有在移动，且在地面或可在空中移动
            if (dir.Equals(InGameDirection.Dash) ||
                (!bOnMoving && (isOnGround || bJumpWithMove)))
            {
                if (!bJumpWithMove && dir.Equals(InGameDirection.Jump_Move))
                {
                    jumpStart();
                    dir = LastDirection;
                }

                if (dir.Equals(InGameDirection.Dash))
                {
                    bDashing = true;
                    dir = LastDirection;
                }
                else
                {
                    bDashing = false;
                    LastDirection = dir;
                }
                if (bRotateToDirection)
                {
                    RotateTo(dir);
                }

                if (bMove)
                {
                    Vector3 v3TmpCoordinates = IsometricMovement.HorizontalVector(dir);
                    if (!bDashing && bSnapToGroundGrid)
                    {
                        Vector3 vUnSnapedPosition = groundGridUnSnapedPosition(unitTransform.position);
                        if (vUnSnapedPosition.magnitude > 0.2f &&
                            Vector3.Dot(v3TmpCoordinates.normalized, vUnSnapedPosition.normalized) > 0.866f) //0.866 means angle < 30
                        {
                            v3TmpCoordinates = -vUnSnapedPosition;
                        }
                    }

                    Vector3 v3TmpPosition = unitTransform.position + groundGridPos(v3TmpCoordinates, bSnapToGroundGrid);
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
                        {
                            vLastCoordinates = vDestinationCoordinates;
                        }

                        vDestinationCoordinates += v3TmpCoordinates;
                        SetHorizontalMovement(v3TmpPosition - unitTransform.position);
                    }
                }
            }
            else
            {
                EnQueueDirection(dir);
            }
        }

        /// <summary>
        /// 冲刺
        /// </summary>
        void Dash()
        {
            if (!bDashing)
            {
                ExecuteDir(InGameDirection.Dash);
            }
        }

        public virtual bool EnQueueDirection(InGameDirection dir)
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

        /// <summary>
        /// 直接移动
        /// </summary>
        /// <param name="vTranslate"></param>
        public virtual void DirectTranslate(Vector3 vTranslate)
        {
            Bounds bounds = GetColliderBounds();
            var vRadius = bounds.extents;
            var vDir = vTranslate.normalized;
            vTranslate = vTranslate * fSpeed;
            if (Grounding(bounds.center + vTranslate + vDir * (vRadius.x + vRadius.z) * 0.5f, fMaxDropHeight))
            {
                SetHorizontalMovement(vTranslate);
            }
                
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

        /// <summary>
        /// 转向
        /// </summary>
        /// <param name="dir"></param>
        void RotateTo(InGameDirection dir)
        {
            Vector3 vDir = HorizontalVector(dir);
            UpdateAnimatorParams(false, vDir.x, vDir.z);
        }

        protected Vector3 GetRevertVector()
        {
            Vector3 vDest = Vector3.zero;
            if (bSnapToGroundGrid && bRevertPositionOnCollision)
            {
                vDest = groundGrid ? 
                    groundGrid.PositionToCoordinates(unitTransform.position, false) : 
                    unitTransform.position;
                vDest.y = 0;
                float fLength = (vLastCoordinates - vDest).magnitude;
                vLastCoordinates = Vector3.MoveTowards(vDest, vLastCoordinates, fLength - Mathf.FloorToInt(fLength));
                vDestinationCoordinates = vLastCoordinates;
                vDest = groundGridPos(vDestinationCoordinates, bSnapToGroundGrid) - unitTransform.position;
            }
            return vDest;
        }


        /// <summary>
        /// 获取位移量
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3 GetMovementVector()
        {
            //竖直位置
            Vector3 vTmp = GetVerticalMovementVector();
            if (isOnJumping)
            {
                Vector3 vGap = vTmp - vJumpOffset;
                vJumpOffset = vTmp;
                vTmp = vGap;
            }
            var acturalMovementVector = GetHorizontalMovementVector() + vTmp;
            return acturalMovementVector;
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

        /// <summary>
        /// 获取水平方向位置
        /// </summary>
        /// <returns></returns>
        virtual protected Vector3 GetHorizontalMovementVector()
        {
            Vector3 vMovementTmp = Vector3.zero;

            if (bOnMoving)
            {
                if (vHorizontalMovement.magnitude <= fMinMovement)
                {
                    Grounding(unitTransform.localPosition, 1f);
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

        protected Vector3 GetTickMovementVector(Vector3 destination)
        {
            return Vector3.MoveTowards(Vector3.zero, destination, fTickSpeed);
        }

        /// <summary>
        /// 位移
        /// </summary>
        /// <param name="vMovement"></param>
        protected virtual void ApplyMovement(Vector3 vMovement)
        {
            if (!vMovement.Equals(Vector3.zero))
                unitTransform.position += vMovement;
        }


        protected virtual void Arrival()
        {
            vHorizontalMovement = Vector3.zero;
            bOnMoving = bDashing = false;
            UpdateAnimatorParams();
        }

        #endregion

        #region Static Tools Methods

        //确认位置
        protected static Vector3 SnapPosition(Vector3 position, bool bSnap)
        {
            //将位置取整
            if (bSnap)
            {
                return new Vector3(
                    Mathf.RoundToInt(position.x), 
                    Mathf.RoundToInt(position.y), 
                    Mathf.RoundToInt(position.z));
            } 
            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vDir"></param>
        /// <returns></returns>
        protected Vector3 HorizontalVector(Vector3 vDir)
        {
            return HorizontalVector(Convert(vDir));
        }

        /// <summary>
        /// 角度换算至 45 度世界
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Vector3 HorizontalVector(InGameDirection dir)
        {
            float fMultiplier = 1f;
            if (dir.Equals(InGameDirection.Left_Move) || 
                dir.Equals(InGameDirection.Top_Move)||
                dir.Equals(InGameDirection.Right_Move) || 
                dir.Equals(InGameDirection.Down_Move))
            {
                fMultiplier *= 1.414f;
            }
                
            return Quaternion.AngleAxis(AngleOfForward(dir), Vector3.up) * Vector3.forward * fMultiplier;
        }
        protected static float AngleOfForward(InGameDirection dir)
        {
            return (int)dir * 45f;
        }

        //角度换算，八方向角度值
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
        }

        /// <summary>
        /// 转换为 4 方向角度枚举
        /// </summary>
        /// <param name="vMoveTo"></param>
        /// <returns></returns>
        static InGameDirection convert_4Dir(Vector3 vMoveTo)
        {
            bool isXAxis = Mathf.Abs(vMoveTo.x) >= Mathf.Abs(vMoveTo.z);
            InGameDirection result = InGameDirection.Jump_Move;
            if (isXAxis)
            {
                if (vMoveTo.x > 0)
                    result = InGameDirection.RD_Move;           //右下
                else
                    result = InGameDirection.LT_Move;           //左上
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

        /// <summary>
        /// 转换为 8 方向枚举
        /// </summary>
        /// <param name="vMoveTo"></param>
        /// <returns></returns>
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
                if (bPositiveX) result = InGameDirection.RD_Move;
                else if (!bPositiveX) result = InGameDirection.LT_Move;
                else if (bPositiveZ) result = InGameDirection.RT_Move;
                else if (!bPositiveZ) result = InGameDirection.LD_Move;
            }
            else if (absX / absZ > 1.5f)
                result = bPositiveX ? InGameDirection.RD_Move : InGameDirection.LT_Move;
            else if (absX / absZ < 0.5f)
                result = bPositiveZ ? InGameDirection.RT_Move : InGameDirection.LD_Move;
            else
            {
                if (bPositiveX)
                    result = bPositiveZ ? InGameDirection.Right_Move : InGameDirection.Down_Move;
                else
                    result = bPositiveZ ? InGameDirection.Top_Move : InGameDirection.Left_Move;
            }

            return result;
        }

        /// <summary>
        /// 是否相反方向
        /// </summary>
        /// <param name="dirA"></param>
        /// <param name="dirB"></param>
        /// <returns></returns>
        protected static bool IsOppositSide(InGameDirection dirA, InGameDirection dirB)
        {
            return Mathf.Abs(dirA - dirB) == (int)InGameDirection.OppositeDir;
        }
        #endregion

    }
}