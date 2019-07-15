using Anonym.Isometric;
using Anonym.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Client.GamePlaying.Unit
{

    [RequireComponent(typeof(CharacterController))]
    public class IsoUnitCharacterController : IsoUnitControllerBase
    {
        #region 控制对象 Character

        CharacterController CC;

        override public bool isOnGround
        {
            get
            {
                return CC.isGrounded;
            }

        }
        override public Transform unitTransform
        {
            get
            {
                return CC.transform;
            }
        }

        bool bUseCustomColliderSize = false;

        Vector2 CCSize;

        #endregion Character

        public override void Jump()
        {
            if (bJumpWithMove)
            {
                // In order to ensure the bottom check
                CC.Move(Vector3.down * 1.25f * CC.minMoveDistance);

                if (isOnGround)
                {
                    jumpStart();
                }
            }
            else
                EnQueueDirection(InGameDirection.Jump_Move);
            return;
        }

        override public int SortingOrder_Adjustment()
        {

            float fXweight = 0f;
            //if ((CC.collisionFlags & CollisionFlags.Below) == 0)
            {
                RaycastHit _hit;
                float fOffset = CC.height * 0.5f + CC.skinWidth;
                if (Physics.Raycast(unitTransform.position + CC.center, Vector3.down, out _hit,
                        CCSize.x + fOffset, CollisionLayerMask))
                {
                    fXweight = Mathf.Lerp(CCSize.x, 0f,
                        (_hit.distance - fOffset * 0.25f) / CCSize.x);
                }
            }
            Vector3 iv3Resolution = IsoMap.instance.fResolutionOfIsometric;
            return Mathf.RoundToInt(fXweight * CCSize.x * Mathf.Min(iv3Resolution.z, iv3Resolution.x) +
                (1f - fXweight) * CCSize.y * iv3Resolution.y);
        }

        #region MoveFunction
        protected override void ApplyMovement(Vector3 vMovement)
        {
            if (!vMovement.Equals(Vector3.zero))
            {
                CC.Move(vMovement);

                if ((CC.collisionFlags & CollisionFlags.Below) != 0)
                {
                    Grounding(unitTransform.localPosition, 1f);
                }
                if ((CC.collisionFlags & CollisionFlags.Sides) != 0)
                {
                    if (bSnapToGroundGrid && bRevertPositionOnCollision)
                        SetHorizontalMovement(GetRevertVector());
                }
            }
        }
        #endregion

        #region GameObject
        protected override void OnInit()
        {

            if (CC == null)
                CC = unitTransform.GetComponent<CharacterController>();
            CC.enabled = true;

            base.OnInit();

            if (CCSize.Equals(Vector2.zero) && bUseCustomColliderSize)
            {
                CCSize = new Vector2(Mathf.Max(Anonym.Isometric.Grid.fGridTolerance, CC.radius * 2f),
                    Mathf.Max(Anonym.Isometric.Grid.fGridTolerance, CC.height + CC.center.y));
            }

            SetMinMoveDistance(Mathf.Min(CC.minMoveDistance, fGridTolerance));            

            vDestinationCoordinates.Set(Mathf.RoundToInt(unitTransform.localPosition.x), 0, Mathf.RoundToInt(unitTransform.localPosition.z));
        }

        public override void OnUpdate()
        {
            RaycastHit hit;

            base.OnUpdate();

            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 5000))
            {

            }
        }        
#endregion
    }
}