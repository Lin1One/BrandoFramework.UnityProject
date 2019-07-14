using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Anonym.Isometric
{
    using Util;

    [RequireComponent(typeof(NavMeshAgent))]
    public class IsometricNavMeshAgent : IsometricMovement
    {
        public static string defaultIsoMapPrefabPat = 
            "Assets/Anonym/MapEditor/Scene/Tutorials/7.1 CharacterControllers/Iso NavMeshAgent Sample.prefab";

        [SerializeField]
        bool IsCustomTravalOffMesh = true;
        bool isOnCustomOffMeshAction
        {
            get
            {
                return OffMeshAction != null;
            }
        }
        Coroutine OffMeshAction;

        override public bool isOnGround
        {
            get
            {
                return NMAgent.isOnNavMesh;
            }
        }

        bool isRemainingDistanceForNMA
        {
            get
            {
                return NMAgent.remainingDistance > fMinMovement;
            }
        }

        bool isControllable
        {
            get
            {
                return bSnapToGroundGrid || OffMeshAction == null;
            }
        }

        override protected void jumpReset()
        {
            base.jumpReset();
            NMAgent.baseOffset = _baseOffset;
        }

        [SerializeField]
        public NavMeshAgent NMAgent;

        float _baseOffset;

        #region Events and Delegates
        public delegate void DestinationArrival(IsometricNavMeshAgent isometricNavMeshAgent, GameObject destination);

        public event DestinationArrival OnDestinationArrival;
        #endregion

        public void SetDestinationGameObject(GameObject destinationObject)
        {
            destination = destinationObject;
            bOnMoving = true;
            NMAgent.isStopped = bSnapToGroundGrid;
            NMAgent.destination = destination.transform.position;
        }
        
        void ResetPath()
        {
            if (NMAgent.hasPath)
                NMAgent.ResetPath();

            if (!KeyInputAssist.IsNull)
                KeyInputAssist.instance.ClearAnchor();
        }

        bool SetDestination(ref Vector3 vDest, bool bDirectly = false)
        {
            if (!bDirectly)
                vDest = SnapPosition(vDest, bSnapToGroundGrid);
            
            if ((cTransform.position - vDest).sqrMagnitude <= fSqrtMinMovement)
                return false;

            bOnMoving = true;
            NMAgent.isStopped = bSnapToGroundGrid;
            NMAgent.destination = vDest;
            return true;
        }

        bool HorizontalRayCast(Vector3 vTranslate)
        {
            vTranslate.y = 0;
            return RayTest(NMAgent.nextPosition, NMAgent.nextPosition + vTranslate);
        }

        bool RayTest(Vector3 from, Vector3 to)
        {
            NavMeshHit nHit;
            return NavMesh.Raycast(from, to, out nHit, NavMesh.AllAreas);
        }

        bool GetTDFromPath(out Vector3 vTranslate, out Vector3 vDestination)
        {
            var corners = NMAgent.path.corners;
            vDestination = corners.Last();

            Vector3 startPosition = NMAgent.nextPosition;
            Vector3 nextPosition = startPosition;

            // Vector3 vHere = SnapPosition(startPosition, true);
            Vector3 vAcc = vTranslate = Vector3.zero;

            Vector3 vNowTranslate = corners[1] - startPosition;
            Vector3 vX = new Vector3(vNowTranslate.x, 0, 0).normalized;
            Vector3 vZ = new Vector3(0, 0, vNowTranslate.z).normalized;

            for (int i = 1; i < corners.Length; ++i)
            {
                vAcc += corners[i] - nextPosition;
                nextPosition = corners[i];
                vDestination = SnapPosition(nextPosition, true);
                vTranslate = vDestination - startPosition;
                vTranslate.y = 0;
                // vTranslate 장애물 검증하여 있을 경우 우회로로 변경
                if (vTranslate.sqrMagnitude >= fSqrtMinMovement)
                {
                    if (vTranslate.sqrMagnitude > 1)
                    {
                        if (!RayTest(startPosition, vDestination))
                            vTranslate = HorizontalVector(Convert(vTranslate));
                        else if (RayTest(startPosition, startPosition + vX))
                            vTranslate = vZ;
                        else if (RayTest(startPosition, startPosition + vZ))
                            vTranslate = vX;
                        else
                            vTranslate = HorizontalVector(Convert(vTranslate));
                    }
                    else if (HorizontalRayCast(vTranslate))
                    {
                        int angleOffset = Vector3.Cross(vTranslate, vAcc).y >= 0 ? 2 : -2; // 2 meas 90 degree
                        var sideAxis = HorizontalVector(Convert(vTranslate) + angleOffset); 
                        sideAxis = Vector3.Project(vAcc, sideAxis);
                        vTranslate = HorizontalVector(sideAxis);
                    }

                    return true;
                }
            }

            return false;
        }

        bool EnQueueDirectionWithoutReset(InGameDirection dir)
        {
            if (isControllable)
                return base.EnQueueDirection(dir);

            return false;
        }

        IEnumerator CustomOnOffMeshAction_JumpOver()
        {
            OffMeshLinkData data = NMAgent.currentOffMeshLinkData;
            Vector3 startPos = NMAgent.transform.position;
            Vector3 endPos = data.endPos;
            float normalizedJumpingTime = 0;
            float inverseJumpingDuration = 1 / fTotalJumpingDuration;
            while (normalizedJumpingTime <= 1)
            {
                float yOffset = JumpingHeight(normalizedJumpingTime);
                NMAgent.transform.position = Vector3.Lerp(startPos, endPos, normalizedJumpingTime) + yOffset * -Physics.gravity.normalized;
                normalizedJumpingTime += Time.deltaTime * inverseJumpingDuration;
                yield return null;
            }
            NMAgent.CompleteOffMeshLink();
            OffMeshAction = null;
        }

        public bool ClickToMove(out Vector3 destination)
        {
            destination = Vector3.zero;
            if (NMAgent.isOnNavMesh)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, 1000);

                float fMinDistance = float.MaxValue;
                NavMeshHit minHit = new NavMeshHit();

                var enumerator = hits.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NavMeshHit nHit;
                    RaycastHit current = (RaycastHit)enumerator.Current;
                    if (NavMesh.SamplePosition(current.point, out nHit, 10, NavMesh.AllAreas))
                    {
                        if (nHit.distance < fMinDistance)
                        {
                            fMinDistance = nHit.distance;
                            minHit = nHit;
                        }
                    }
                }

                if (fMinDistance != float.MaxValue)
                {
                    Vector3 vTmp = minHit.position;
                    if (SetDestination(ref vTmp))
                    {
                        destination = vTmp;
                        return true;
                    }
                }
            }
            return false;
        }

        override public Bounds GetBounds()
        {
            Vector3 vSize = new Vector3(NMAgent.radius, NMAgent.height, NMAgent.radius);
            return new Bounds(NMAgent.nextPosition + NMAgent.baseOffset * Vector3.up, vSize);
        }

        override public bool EnQueueDirection(InGameDirection dir)
        {
            if (EnQueueDirectionWithoutReset(dir))
            {
                ResetPath();
                return true;
            }
            return false;
        }

        override public void DirectTranslate(Vector3 vTranslate)
        {
            if (isControllable)
            {
                ResetPath();
                var vDir = vTranslate.normalized;
                vTranslate = vTranslate * fSpeed;
                if (Grounding(NMAgent.nextPosition + vTranslate + vDir * NMAgent.radius, fMaxDropHeight))
                    SetHorizontalMovement(vTranslate);
            }
        }

        override public bool Grounding(Vector3 position, float fRayLength)
        {
            return isOnGround;
            //NavMeshHit rayHit;
            //return NMAgent.Raycast(position + Vector3.down * fRayLength, out rayHit);
        }

        override public int SortingOrder_Adjustment()
        {
            return IsometricSortingOrderUtility.IsometricSortingOrder(vJumpOffset);
        }

        override protected Vector3 GetHorizontalMovementVector()
        {
            if (isOnCustomOffMeshAction)
                return Vector3.zero;
            
            // Remaing Movement First
            Vector3 vMovementTmp = Vector3.zero;
            if (bOnMoving && vHorizontalMovement.magnitude > fMinMovement)
            {
                vMovementTmp = GetTickMovementVector(vHorizontalMovement);
                vHorizontalMovement -= vMovementTmp;
                return vMovementTmp;
            }

            // Get Next Direction based on NavMeshPath
            if (bSnapToGroundGrid)
            {
                if (NMAgent.hasPath && NMAgent.pathStatus != NavMeshPathStatus.PathInvalid && isRemainingDistanceForNMA)
                {
                    Vector3 vNext, vDestination;
                    bool bStop = !GetTDFromPath(out vNext, out vDestination);

                    if (bStop)
                    {
                        if (NMAgent.path.corners.Length > 2)
                            SetDestination(ref vDestination, true);
                        else
                            Arrival();
                    }
                    else if (isControllable)
                    {
                        SetHorizontalMovement(vNext);
                        vMovementTmp = GetTickMovementVector(vHorizontalMovement);
                        vHorizontalMovement -= vMovementTmp;
                    }
                }
            }

            if (bOnMoving && vHorizontalMovement.Equals(Vector3.zero) &&
                !(NMAgent.pathPending || NMAgent.hasPath || isRemainingDistanceForNMA))
            {
                Arrival();
            }
            else if (NMAgent.hasPath && !bSnapToGroundGrid)
                UpdateAnimatorParams(bOnMoving, NMAgent.desiredVelocity.x, NMAgent.desiredVelocity.z);

            return vMovementTmp;
        }

        override protected Vector3 GetMovementVector()
        {
            if (isOnJumping)
                NMAgent.baseOffset = GetVerticalMovementVector().magnitude + _baseOffset;

            return GetHorizontalMovementVector();
        }

        override protected void ApplyMovement(Vector3 vMovement)
        {
            if (!vMovement.Equals(Vector3.zero))
            {
                NMAgent.Move(vMovement);
                if (Vector3.Distance(NMAgent.nextPosition, NMAgent.path.corners.Last()) < fMinMovement)
                    ResetPath();
            }

            if (NMAgent.isOnOffMeshLink && NMAgent.currentOffMeshLinkData.valid)
            {
                if (IsCustomTravalOffMesh)
                {
                    if (!isOnCustomOffMeshAction)
                        OffMeshAction = StartCoroutine(CustomOnOffMeshAction_JumpOver());
                }
                else
                {
                    if (NMAgent.isOnOffMeshLink && NMAgent.currentOffMeshLinkData.valid)
                    {
                        if (NMAgent.isStopped)
                            NMAgent.isStopped = false;
                    }
                    else if (!NMAgent.isOnOffMeshLink && !NMAgent.isStopped)
                    {
                        NMAgent.isStopped = true;
                    }
                }
            }
        }

        override protected void Arrival()
        {
            base.Arrival();
            if (NMAgent.isOnNavMesh) // && !isRemainingDistanceForNMA)
                ResetPath();

            // Dispatch event
            if (OnDestinationArrival != null)
                OnDestinationArrival(this, destination);

        }

        #region GameObject
        void Init()
        {
            if (NMAgent == null)
                NMAgent = GetComponentInChildren<NavMeshAgent>();

            NMAgent.autoTraverseOffMeshLink = !IsCustomTravalOffMesh;
            NMAgent.updateRotation = bRotateToDirection;

            _baseOffset = NMAgent.baseOffset;

            base.Start();
            SetMinMoveDistance(Mathf.Min(NMAgent.stoppingDistance, fGridTolerance));

            NMAgent.speed = fSpeed;

            if (NMAgent.isOnNavMesh && destination)
                NMAgent.SetDestination(destination.transform.position);
            else
                Arrival();

            if (isOnCustomOffMeshAction)
            {
                StopCoroutine(OffMeshAction);
                OffMeshAction = null;
            }
        }
        override public void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            if (isOnCustomOffMeshAction)
            {
                StopCoroutine(OffMeshAction);
                OffMeshAction = null;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (isActiveAndEnabled)
                Init();
        }
#endif
        #endregion
    }
}