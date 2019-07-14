using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;

    public class KeyInputAssist : Util.Singleton<KeyInputAssist>
    {
        public static string defaultIsoMapPrefabPat = "Assets/Anonym/MapEditor/Scene/Tutorials/7.1 CharacterControllers/KeyInputAssist.prefab";

        [SerializeField]
        IsometricMovement target;

        [SerializeField]
        bool bUseDiagonalKey = true;

        [SerializeField]
        GameObject AnchorPrefab;
        GameObject AnchorInstance;

        [SerializeField]
        float fMaxDashInputInterval = 0.33f;
        float fLastInputTime = 0;

        [SerializeField, HideInInspector]
        IsometricNavMeshAgent NMAgent = null;
        [SerializeField, Header("Only available with NavMeshAgent.")]
        bool bUseClickToPathfinding = true;

        private void Start()
        {
            init();
        }

        void Update()
        {
            if (target != null && Application.isPlaying)
            {
                InputProcess();
            }
        }

        private void OnValidate()
        {
            if (isActiveAndEnabled)
                init();
        }

        public void Init()
        {
            init();
        }

        void init()
        {
            if (target == null)
                SetTarget(GetComponent<IsometricMovement>());

            if (target == null)
                SetTarget(FindObjectOfType<IsometricMovement>());
        }

        void ClickToMove()
        {
            Vector3 destination;
            if (Input.GetMouseButtonDown(0) && NMAgent.ClickToMove(out destination))
                AddAnchor(destination);
        }

        void InputProcess()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                target.Jump();

            inputProcess();

            if (NMAgent != null && bUseClickToPathfinding)
                ClickToMove();
        }

        bool keyMacro(InGameDirection direction, bool bShift,
            System.Func<IEnumerable<KeyCode>, System.Func<KeyCode, bool>, bool> action,
            System.Func<KeyCode, bool> subAction,
            System.Action<InGameDirection> Do,
            params KeyCode[] codes)
        {
            if (bShift) // Rotate
                direction = (InGameDirection) (-1 * (int) direction);

            if (action(codes, subAction))
            {
                Do(direction);
                return true;
            }
            return false;
        }

        void inputProcess()
        {
            bool bShifted = Input.GetKey(KeyCode.LeftShift);
            System.Action<InGameDirection> Do;
            System.Func<KeyCode, bool> GetKeyMethod;
            if (target.bContinuousMovement){
                Do = ContinuousMove;
                GetKeyMethod = Input.GetKey;
            } else {
                Do = EnQueueTo;
                GetKeyMethod = Input.GetKeyDown;
            }

            if (target.b8DirectionalMovement)
            {
                bool bSelected = false;
                if (bUseDiagonalKey)
                {
                    if (keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad7, KeyCode.Q) ||
                        keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad3, KeyCode.C) ||
                        keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad9, KeyCode.E) ||
                        keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad1, KeyCode.Z))
                        bSelected = true;
                }
                else
                {
                    if (keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.UpArrow)      ||
                        keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.DownArrow)   ||
                        keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.UpArrow)     ||
                        keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.DownArrow)    ||
                        keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.A, KeyCode.W)      ||
                        keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.D, KeyCode.S)   ||
                        keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.D, KeyCode.W)     ||
                        keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.All<KeyCode>, GetKeyMethod, Do, KeyCode.A, KeyCode.S))
                        bSelected = true;
                }

                if (bSelected == false)
                {
                    keyMacro(InGameDirection.Left_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad4, KeyCode.LeftArrow, KeyCode.A);
                    keyMacro(InGameDirection.Right_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad6, KeyCode.RightArrow, KeyCode.D);
                    keyMacro(InGameDirection.Top_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad8, KeyCode.UpArrow, KeyCode.W);
                    keyMacro(InGameDirection.Down_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.Keypad2, KeyCode.DownArrow, KeyCode.X, KeyCode.S);
                }
            }
            else
            {
                keyMacro(InGameDirection.LT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.LeftArrow, KeyCode.A);
                keyMacro(InGameDirection.RD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.RightArrow, KeyCode.D);
                keyMacro(InGameDirection.RT_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.UpArrow, KeyCode.W);
                keyMacro(InGameDirection.LD_Move, bShifted, Enumerable.Any<KeyCode>, GetKeyMethod, Do, KeyCode.DownArrow, KeyCode.S);
            }
        }

        void EnQueueTo(InGameDirection direction)
        {
            bool bDash = target.bMovingDirection(direction) && (Time.time - fLastInputTime < fMaxDashInputInterval);
            fLastInputTime = Time.time;

            target.EnQueueDirection(bDash ? InGameDirection.Dash : direction);
        }

        void ContinuousMove(InGameDirection direction)
        {
            if (direction < 0)
                direction = (InGameDirection ) (- 1 * (int) direction);
            Vector3 vMovement = IsometricMovement.HorizontalVector(direction);
            target.DirectTranslate(vMovement * Time.deltaTime);
        }

        public void AddAnchor(Vector3 position)
        {
            ClearAnchor();
            if (AnchorPrefab)
                AnchorInstance = Instantiate(AnchorPrefab, position + AnchorPrefab.transform.position, AnchorPrefab.transform.rotation);
        }

        public void ClearAnchor()
        {
            if (AnchorInstance)
            {
                Destroy(AnchorInstance);
                AnchorInstance = null;
            }
        }        

        public void SetTarget(IsometricMovement newTarget)
        {
            ClearAnchor();
            target = newTarget;
            NMAgent = target == null ? null : target.GetComponent<IsometricNavMeshAgent>();
        }

        public void DiagonalKey(bool bFlag)
        {
            bUseDiagonalKey = bFlag;
        }
    }
}