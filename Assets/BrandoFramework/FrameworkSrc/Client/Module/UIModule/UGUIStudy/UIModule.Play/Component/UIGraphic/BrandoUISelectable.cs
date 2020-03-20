using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using Client.UI.EventSystem;

namespace Client.UI
{
    [ExecuteAlways]
    [SelectionBase]
    [DisallowMultipleComponent]
    /// <summary>
    /// 可选择对象
    /// </summary>
    public class BrandoUISelectable : BrandoUIBehaviour,
        IMoveHandler,
        IPointerDownHandler, 
        IPointerUpHandler,
        IPointerEnterHandler, 
        IPointerExitHandler,
        ISelectHandler, 
        IDeselectHandler
    {
        private static List<BrandoUISelectable> s_List = new List<BrandoUISelectable>();

        /// <summary>
        /// 当前场景内可见的可选择UI list.
        /// </summary>
        public static List<BrandoUISelectable> allSelectables { get { return s_List; } }

        [FormerlySerializedAs("highlightGraphic")]
        [FormerlySerializedAs("m_HighlightGraphic")]
        [SerializeField]
        private BrandoUIGraphic m_TargetGraphic;
        /// <summary>
        /// Graphic that will be transitioned upon.
        /// </summary>
        public BrandoUIGraphic targetGraphic
        {
            get
            {
                return m_TargetGraphic;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref m_TargetGraphic, value))
                    OnSetProperty();
            }
        }

        /// <summary>
        /// Convenience function that converts the referenced Graphic to a Image, if possible.
        /// </summary>
        public BrandoUIImage image
        {
            get { return m_TargetGraphic as BrandoUIImage; }
            set { m_TargetGraphic = value; }
        }


        protected BrandoUISelectable()
        { }

        #region 生命周期

        protected override void Awake()
        {
            if (m_TargetGraphic == null)
                m_TargetGraphic = GetComponent<BrandoUIGraphic>();
        }

        // Select on enable and add to the list.
        protected override void OnEnable()
        {
            base.OnEnable();

            s_List.Add(this);
            var state = SelectionState.Normal;

            // The button will be highlighted even in some cases where it shouldn't.
            // For example: We only want to set the State as Highlighted if the StandaloneInputModule.m_CurrentInputMode == InputMode.Buttons
            // But we dont have access to this, and it might not apply to other InputModules.
            // TODO: figure out how to solve this. Case 617348.
            if (hasSelection)
                state = SelectionState.Highlighted;

            m_CurrentSelectionState = state;
            InternalEvaluateAndTransitionToSelectionState(true);
        }

        // Remove from the list.
        protected override void OnDisable()
        {
            s_List.Remove(this);
            InstantClearState();
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Colors.fadeDuration = Mathf.Max(m_Colors.fadeDuration, 0.0f);

            // OnValidate can be called before OnEnable, this makes it unsafe to access other components
            // since they might not have been initialized yet.
            // OnSetProperty potentially access Animator or Graphics. (case 618186)
            if (isActiveAndEnabled)
            {
                if (!interactable && BrandoEventSystem.current != null && BrandoEventSystem.current.currentSelectedGameObject == gameObject)
                    BrandoEventSystem.current.SetSelectedGameObject(null);
                // Need to clear out the override image on the target...
                DoSpriteSwap(null);

                // If the transition mode got changed, we need to clear all the transitions, since we don't know what the old transition mode was.
                StartColorTween(Color.white, true);
                TriggerAnimation(m_AnimationTriggers.normalTrigger);

                // And now go to the right state.
                InternalEvaluateAndTransitionToSelectionState(true);
            }
        }
#endif // if UNITY_EDITOR
        protected override void Reset()
        {
            m_TargetGraphic = GetComponent<BrandoUIGraphic>();
        }

        #endregion

        #region UI导航 Navigation
        /// <summary>
        /// 导航信息
        /// </summary>
        [FormerlySerializedAs("navigation")]
        [SerializeField]
        private BrandoUINavigation m_Navigation = BrandoUINavigation.defaultNavigation;

        public BrandoUINavigation navigation
        {
            get
            {
                return m_Navigation;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Navigation, value))
                    OnSetProperty();
            }
        }

        /// <summary>
        /// Finds the selectable object next to this one.
        /// </summary>
        /// <remarks>
        /// The direction is determined by a Vector3 variable.
        /// </remarks>
        /// <param name="dir">The direction in which to search for a neighbouring Selectable object.</param>
        /// <returns>The neighbouring Selectable object. Null if none found.</returns>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     //Sets the direction as "Up" (Y is in positive).
        ///     public Vector3 direction = new Vector3(0, 1, 0);
        ///     public Button btnMain;
        ///
        ///     public void Start()
        ///     {
        ///         //Finds and assigns the selectable above the main button
        ///         Selectable newSelectable = btnMain.FindSelectable(direction);
        ///
        ///         Debug.Log(newSelectable.name);
        ///     }
        /// }
        /// </code>
        /// </example>
        public BrandoUISelectable FindSelectable(Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;
            BrandoUISelectable bestPick = null;
            for (int i = 0; i < s_List.Count; ++i)
            {
                var sel = s_List[i];

                if (sel == this || sel == null)
                    continue;

                if (!sel.IsInteractable() || sel.navigation.mode == BrandoUINavigation.Mode.None)
                    continue;

#if UNITY_EDITOR
                // Apart from runtime use, FindSelectable is used by custom editors to
                // draw arrows between different selectables. For scene view cameras,
                // only selectables in the same stage should be considered.
                if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                    continue;
#endif

                var selRect = sel.transform as RectTransform;
                Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
                Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;

                // Value that is the distance out along the direction.
                float dot = Vector3.Dot(dir, myVector);

                // Skip elements that are in the wrong direction or which have zero distance.
                // This also ensures that the scoring formula below will not have a division by zero error.
                if (dot <= 0)
                    continue;

                // This scoring function has two priorities:
                // - Score higher for positions that are closer.
                // - Score higher for positions that are located in the right direction.
                // This scoring function combines both of these criteria.
                // It can be seen as this:
                //   Dot (dir, myVector.normalized) / myVector.magnitude
                // The first part equals 1 if the direction of myVector is the same as dir, and 0 if it's orthogonal.
                // The second part scores lower the greater the distance is by dividing by the distance.
                // The formula below is equivalent but more optimized.
                //
                // If a given score is chosen, the positions that evaluate to that score will form a circle
                // that touches pos and whose center is located along dir. A way to visualize the resulting functionality is this:
                // From the position pos, blow up a circular balloon so it grows in the direction of dir.
                // The first Selectable whose center the circular balloon touches is the one that's chosen.
                float score = dot / myVector.sqrMagnitude;

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = sel;
                }
            }
            return bestPick;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;
            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }

        // Convenience function -- change the selection to the specified object if it's not null and happens to be active.
        void Navigate(AxisEventData eventData, BrandoUISelectable sel)
        {
            if (sel != null && sel.IsActive())
                eventData.selectedObject = sel.gameObject;
        }

        /// <summary>
        /// Find the selectable object to the left of this one.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public Button btnMain;
        ///
        ///     // Disables the selectable UI element directly to the left of the Start Button
        ///     public void IgnoreSelectables()
        ///     {
        ///         //Finds the selectable UI element to the left the start button and assigns it to a variable of type "Selectable"
        ///         Selectable secondButton = startButton.FindSelectableOnLeft();
        ///         //Disables interaction with the selectable UI element
        ///         secondButton.interactable = false;
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual BrandoUISelectable FindSelectableOnLeft()
        {
            if (m_Navigation.mode == BrandoUINavigation.Mode.Explicit)
            {
                return m_Navigation.selectOnLeft;
            }
            if ((m_Navigation.mode & BrandoUINavigation.Mode.Horizontal) != 0)
            {
                return FindSelectable(transform.rotation * Vector3.left);
            }
            return null;
        }

        /// <summary>
        /// Find the selectable object to the right of this one.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public Button btnMain;
        ///
        ///     // Disables the selectable UI element directly to the right the Start Button
        ///     public void IgnoreSelectables()
        ///     {
        ///         //Finds the selectable UI element to the right the start button and assigns it to a variable of type "Selectable"
        ///         Selectable secondButton = startButton.FindSelectableOnRight();
        ///         //Disables interaction with the selectable UI element
        ///         secondButton.interactable = false;
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual BrandoUISelectable FindSelectableOnRight()
        {
            if (m_Navigation.mode == BrandoUINavigation.Mode.Explicit)
            {
                return m_Navigation.selectOnRight;
            }
            if ((m_Navigation.mode & BrandoUINavigation.Mode.Horizontal) != 0)
            {
                return FindSelectable(transform.rotation * Vector3.right);
            }
            return null;
        }

        /// <summary>
        /// The Selectable object above current
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///     public Button btnMain;
        ///
        ///     // Disables the selectable UI element directly above the Start Button
        ///     public void IgnoreSelectables()
        ///     {
        ///         //Finds the selectable UI element above the start button and assigns it to a variable of type "Selectable"
        ///         Selectable secondButton = startButton.FindSelectableOnUp();
        ///         //Disables interaction with the selectable UI element
        ///         secondButton.interactable = false;
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual BrandoUISelectable FindSelectableOnUp()
        {
            if (m_Navigation.mode == BrandoUINavigation.Mode.Explicit)
            {
                return m_Navigation.selectOnUp;
            }
            if ((m_Navigation.mode & BrandoUINavigation.Mode.Vertical) != 0)
            {
                return FindSelectable(transform.rotation * Vector3.up);
            }
            return null;
        }

        /// <summary>
        /// Find the selectable object below this one.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Button startButton;
        ///
        ///     // Disables the selectable UI element directly below the Start Button
        ///     public void IgnoreSelectables()
        ///     {
        ///         //Finds the selectable UI element below the start button and assigns it to a variable of type "Selectable"
        ///         Selectable secondButton = startButton.FindSelectableOnDown();
        ///         //Disables interaction with the selectable UI element
        ///         secondButton.interactable = false;
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual BrandoUISelectable FindSelectableOnDown()
        {
            if (m_Navigation.mode == BrandoUINavigation.Mode.Explicit)
            {
                return m_Navigation.selectOnDown;
            }
            if ((m_Navigation.mode & BrandoUINavigation.Mode.Vertical) != 0)
            {
                return FindSelectable(transform.rotation * Vector3.down);
            }
            return null;
        }

        /// <summary>
        /// Determine in which of the 4 move directions the next selectable object should be found.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IMoveHandler
        /// {
        ///     //When the focus moves to another selectable object, Invoke this Method.
        ///     public void OnMove(AxisEventData eventData)
        ///     {
        ///         //Assigns the move direction and the raw input vector representing the direction from the event data.
        ///         MoveDirection moveDir = eventData.moveDir;
        ///         Vector2 moveVector = eventData.moveVector;
        ///
        ///         //Displays the information in the console
        ///         Debug.Log(moveDir + ", " + moveVector);
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnMove(AxisEventData eventData)
        {
            switch (eventData.moveDir)
            {
                case MoveDirection.Right:
                    Navigate(eventData, FindSelectableOnRight());
                    break;

                case MoveDirection.Up:
                    Navigate(eventData, FindSelectableOnUp());
                    break;

                case MoveDirection.Left:
                    Navigate(eventData, FindSelectableOnLeft());
                    break;

                case MoveDirection.Down:
                    Navigate(eventData, FindSelectableOnDown());
                    break;
            }
        }

        #endregion

        #region UI图形交互状态，及过渡 Transition

        /// <summary>
        /// 是否可交互
        /// </summary>
        [Tooltip("Can the Selectable be interacted with?")]
        [SerializeField]
        private bool m_Interactable = true;

        private bool m_GroupsAllowInteraction = true;

        /// <summary>
        /// Is this object interactable.
        /// </summary>
        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Interactable, value))
                {
                    if (!m_Interactable && BrandoEventSystem.current != null && 
                        BrandoEventSystem.current.currentSelectedGameObject == gameObject)
                        BrandoEventSystem.current.SetSelectedGameObject(null);
                    if (m_Interactable)
                        UpdateSelectionState(null);
                    OnSetProperty();
                }
            }
        }

        /// <summary>
        /// Is the object interactable.
        /// </summary>
        public virtual bool IsInteractable()
        {
            return m_GroupsAllowInteraction && m_Interactable;
        }

        /// <summary>
        /// 过渡方式枚举
        /// </summary>
        public enum Transition
        {
            /// <summary>
            /// No Transition.
            /// </summary>
            None,

            /// <summary>
            /// Use an color tint transition.染色过渡
            /// </summary>
            ColorTint,

            /// <summary>
            /// Use a sprite swap transition.
            /// </summary>
            SpriteSwap,

            /// <summary>
            /// Use an animation transition.
            /// </summary>
            Animation
        }

        /// <summary>
        /// 过渡方式
        /// </summary>
        [FormerlySerializedAs("transition")]
        [SerializeField]
        private Transition m_Transition = Transition.ColorTint;

        /// <summary>
        /// The type of transition that will be applied to the targetGraphic when the state changes.
        /// </summary>
        public Transition transition
        {
            get
            {
                return m_Transition;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Transition, value))
                    OnSetProperty();
            }
        }

        // Colors used for a color tint 
        [FormerlySerializedAs("colors")]
        [SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        /// <summary>
        /// The ColorBlock for this selectable object.
        /// </summary>
        public ColorBlock colors
        {
            get
            {
                return m_Colors;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_Colors, value))
                    OnSetProperty();
            }
        }

        // Sprites used for a Image swap-based transition.
        [FormerlySerializedAs("spriteState")]
        [SerializeField]
        private SpriteState m_SpriteState;

        /// <summary>
        /// The SpriteState for this selectable object.
        /// </summary>
        /// <remarks>
        /// Modifications will not be visible if transition is not SpriteSwap.
        /// </remarks>
        public SpriteState spriteState
        {
            get
            {
                return m_SpriteState;
            }
            set
            {
                if (SetPropertyUtility.SetStruct(ref m_SpriteState, value))
                    OnSetProperty();
            }
        }

        [FormerlySerializedAs("animationTriggers")]
        [SerializeField]
        private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

        /// <summary>
        /// The AnimationTriggers for this selectable object.
        /// </summary>
        /// <remarks>
        /// Modifications will not be visible if transition is not Animation.
        /// </remarks>
        public AnimationTriggers animationTriggers
        {
            get
            {
                return m_AnimationTriggers;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref m_AnimationTriggers, value))
                    OnSetProperty();
            }
        }

        /// <summary>
        /// Convenience function to get the Animator component on the GameObject.
        /// </summary>
        public Animator animator
        {
            get { return GetComponent<Animator>(); }
        }

        /// <summary>
        /// 颜色过渡 Tween
        /// </summary>
        /// <param name="targetColor"></param>
        /// <param name="instant"></param>
        void StartColorTween(Color targetColor, bool instant)
        {
            if (m_TargetGraphic == null)
                return;

            m_TargetGraphic.CrossFadeColor(targetColor, instant ? 0f : m_Colors.fadeDuration, true, true);
        }

        void DoSpriteSwap(Sprite newSprite)
        {
            if (image == null)
                return;

            image.overrideSprite = newSprite;
        }

        void TriggerAnimation(string triggername)
        {
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
                return;

            animator.ResetTrigger(m_AnimationTriggers.normalTrigger);
            animator.ResetTrigger(m_AnimationTriggers.pressedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.highlightedTrigger);
            animator.ResetTrigger(m_AnimationTriggers.disabledTrigger);

            animator.SetTrigger(triggername);
        }

        /// <summary>
        /// An enumeration of selected states of objects
        /// 选择状态
        /// </summary>
        protected enum SelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled
        }

        private SelectionState m_CurrentSelectionState;

        protected SelectionState currentSelectionState
        {
            get { return m_CurrentSelectionState; }
        }

        protected virtual void InstantClearState()
        {
            string triggerName = m_AnimationTriggers.normalTrigger;

            isPointerInside = false;
            isPointerDown = false;
            hasSelection = false;

            switch (m_Transition)
            {
                case Transition.ColorTint:
                    StartColorTween(Color.white, true);
                    break;
                case Transition.SpriteSwap:
                    DoSpriteSwap(null);
                    break;
                case Transition.Animation:
                    TriggerAnimation(triggerName);
                    break;
            }
        }

        /// <summary>
        /// Transition the Selectable to the entered state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        /// <param name="instant">Should the transition occur instantly.</param>
        protected virtual void DoStateTransition(SelectionState state, bool instant)
        {
            Color tintColor;
            Sprite transitionSprite;
            string triggerName;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = m_Colors.normalColor;
                    transitionSprite = null;
                    triggerName = m_AnimationTriggers.normalTrigger;
                    break;
                case SelectionState.Highlighted:
                    tintColor = m_Colors.highlightedColor;
                    transitionSprite = m_SpriteState.highlightedSprite;
                    triggerName = m_AnimationTriggers.highlightedTrigger;
                    break;
                case SelectionState.Pressed:
                    tintColor = m_Colors.pressedColor;
                    transitionSprite = m_SpriteState.pressedSprite;
                    triggerName = m_AnimationTriggers.pressedTrigger;
                    break;
                case SelectionState.Disabled:
                    tintColor = m_Colors.disabledColor;
                    transitionSprite = m_SpriteState.disabledSprite;
                    triggerName = m_AnimationTriggers.disabledTrigger;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    triggerName = string.Empty;
                    break;
            }

            if (gameObject.activeInHierarchy)
            {
                switch (m_Transition)
                {
                    case Transition.ColorTint:
                        StartColorTween(tintColor * m_Colors.colorMultiplier, instant);
                        break;
                    case Transition.SpriteSwap:
                        DoSpriteSwap(transitionSprite);
                        break;
                    case Transition.Animation:
                        TriggerAnimation(triggerName);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns whether the selectable is currently 'highlighted' or not.
        /// </summary>
        /// <remarks>
        /// Use this to check if the selectable UI element is currently highlighted.
        /// </remarks>
        /// <example>
        /// <code>
        /// //Create a UI element. To do this go to Create>UI and select from the list. Attach this script to the UI GameObject to see this script working. The script also works with non-UI elements, but highlighting works better with UI.
        ///
        /// using UnityEngine;
        /// using UnityEngine.Events;
        /// using UnityEngine.EventSystems;
        /// using UnityEngine.UI;
        ///
        /// //Use the Selectable class as a base class to access the IsHighlighted method
        /// public class Example : Selectable
        /// {
        ///     //Use this to check what Events are happening
        ///     BaseEventData m_BaseEvent;
        ///
        ///     void Update()
        ///     {
        ///         //Check if the GameObject is being highlighted
        ///         if (IsHighlighted(m_BaseEvent) == true)
        ///         {
        ///             //Output that the GameObject was highlighted, or do something else
        ///             Debug.Log("Selectable is Highlighted");
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        protected bool IsHighlighted(BaseEventData eventData)
        {
            if (!IsActive())
                return false;

            if (IsPressed())
                return false;

            bool selected = hasSelection;
            if (eventData is PointerEventData)
            {
                var pointerData = eventData as PointerEventData;
                selected |=
                    (isPointerDown && !isPointerInside && pointerData.pointerPress == gameObject) // This object pressed, but pointer moved off
                    || (!isPointerDown && isPointerInside && pointerData.pointerPress == gameObject) // This object pressed, but pointer released over (PointerUp event)
                    || (!isPointerDown && isPointerInside && pointerData.pointerPress == null); // Nothing pressed, but pointer is over
            }
            else
            {
                selected |= isPointerInside;
            }
            return selected;
        }

        [Obsolete("Is Pressed no longer requires eventData", false)]
        protected bool IsPressed(BaseEventData eventData)
        {
            return IsPressed();
        }

        /// <summary>
        /// Whether the current selectable is being pressed.
        /// </summary>
        protected bool IsPressed()
        {
            if (!IsActive())
                return false;

            return isPointerInside && isPointerDown;
        }

        private bool isPointerInside { get; set; }
        private bool isPointerDown { get; set; }
        private bool hasSelection { get; set; }
        #endregion

        #region 交互事件接口实现

        // Change the button to the correct state
        private void EvaluateAndTransitionToSelectionState(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            UpdateSelectionState(eventData);
            InternalEvaluateAndTransitionToSelectionState(false);
        }
        /// <summary>
        /// Internally update the selection state of the Selectable.
        /// </summary>
        protected void UpdateSelectionState(BaseEventData eventData)
        {
            if (IsPressed())
            {
                m_CurrentSelectionState = SelectionState.Pressed;
                return;
            }

            if (IsHighlighted(eventData))
            {
                m_CurrentSelectionState = SelectionState.Highlighted;
                return;
            }

            m_CurrentSelectionState = SelectionState.Normal;
        }

        private void InternalEvaluateAndTransitionToSelectionState(bool instant)
        {
            var transitionState = m_CurrentSelectionState;
            if (IsActive() && !IsInteractable())
                transitionState = SelectionState.Disabled;
            DoStateTransition(transitionState, instant);
        }


        /// <summary>
        /// Evaluate current state and transition to pressed state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IPointerDownHandler// required interface when using the OnPointerDown method.
        /// {
        ///     //Do this when the mouse is clicked over the selectable object this script is attached to.
        ///     public void OnPointerDown(PointerEventData eventData)
        ///     {
        ///         Debug.Log(this.gameObject.name + " Was Clicked.");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // Selection tracking
            if (IsInteractable() && navigation.mode != BrandoUINavigation.Mode.None && BrandoEventSystem.current != null)
                BrandoEventSystem.current.SetSelectedGameObject(gameObject, eventData);

            isPointerDown = true;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Evaluate eventData and transition to appropriate state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IPointerUpHandler, IPointerDownHandler// These are the interfaces the OnPointerUp method requires.
        /// {
        ///     //OnPointerDown is also required to receive OnPointerUp callbacks
        ///     public void OnPointerDown(PointerEventData eventData)
        ///     {
        ///     }
        ///
        ///     //Do this when the mouse click on this selectable UI object is released.
        ///     public void OnPointerUp(PointerEventData eventData)
        ///     {
        ///         Debug.Log("The mouse click was released");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = false;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Evaluate current state and transition to appropriate state.
        /// New state could be pressed or hover depending on pressed state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IPointerEnterHandler// required interface when using the OnPointerEnter method.
        /// {
        ///     //Do this when the cursor enters the rect area of this selectable UI object.
        ///     public void OnPointerEnter(PointerEventData eventData)
        ///     {
        ///         Debug.Log("The cursor entered the selectable UI element.");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Evaluate current state and transition to normal state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IPointerExitHandler// required interface when using the OnPointerExit method.
        /// {
        ///     //Do this when the cursor exits the rect area of this selectable UI object.
        ///     public void OnPointerExit(PointerEventData eventData)
        ///     {
        ///         Debug.Log("The cursor exited the selectable UI element.");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Set selection and transition to appropriate state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, ISelectHandler// required interface when using the OnSelect method.
        /// {
        ///     //Do this when the selectable UI object is selected.
        ///     public void OnSelect(BaseEventData eventData)
        ///     {
        ///         Debug.Log(this.gameObject.name + " was selected");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnSelect(BaseEventData eventData)
        {
            hasSelection = true;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Unset selection and transition to appropriate state.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour, IDeselectHandler //This Interface is required to receive OnDeselect callbacks.
        /// {
        ///     public void OnDeselect(BaseEventData data)
        ///     {
        ///         Debug.Log("Deselected");
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void OnDeselect(BaseEventData eventData)
        {
            hasSelection = false;
            EvaluateAndTransitionToSelectionState(eventData);
        }

        /// <summary>
        /// Selects this Selectable.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // required when using UI elements in scripts
        /// using UnityEngine.EventSystems;// Required when using Event data.
        ///
        /// public class ExampleClass : MonoBehaviour// required interface when using the OnSelect method.
        /// {
        ///     public InputField myInputField;
        ///
        ///     //Do this OnClick.
        ///     public void SaveGame()
        ///     {
        ///         //Makes the Input Field the selected UI Element.
        ///         myInputField.Select();
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual void Select()
        {
            if (BrandoEventSystem.current == null || BrandoEventSystem.current.alreadySelecting)
                return;

            BrandoEventSystem.current.SetSelectedGameObject(gameObject);
        }

        #endregion 

        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();
        protected override void OnCanvasGroupChanged()
        {
            // Figure out if parent groups allow interaction
            // If no interaction is alowed... then we need
            // to not do that :)
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction
                    // we need to break
                    if (!m_CanvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break
                    // as we should not consider parents
                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            if (groupAllowInteraction != m_GroupsAllowInteraction)
            {
                m_GroupsAllowInteraction = groupAllowInteraction;
                OnSetProperty();
            }
        }

        // Call from unity if animation properties have changed
        protected override void OnDidApplyAnimationProperties()
        {
            OnSetProperty();
        }


        private void OnSetProperty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                InternalEvaluateAndTransitionToSelectionState(true);
            else
#endif
            InternalEvaluateAndTransitionToSelectionState(false);
        }
    }
}
