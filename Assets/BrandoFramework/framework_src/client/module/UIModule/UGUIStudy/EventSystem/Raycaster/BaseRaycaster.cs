using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.UI.EventSystem
{
    /// <summary>
    /// ÉäÏß·¢ÉúÆ÷
    /// </summary>
    public abstract class BaseRaycaster : BrandoUIBehaviour
    {
        public abstract void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList);
        public abstract Camera eventCamera { get; }

        public virtual int sortOrderPriority
        {
            get { return int.MinValue; }
        }

        public virtual int renderOrderPriority
        {
            get { return int.MinValue; }
        }

        public override string ToString()
        {
            return "Name: " + gameObject + "\n" +
                   "eventCamera: " + eventCamera + "\n" +
                   "sortOrderPriority: " + sortOrderPriority + "\n" +
                   "renderOrderPriority: " + renderOrderPriority;
        }

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            RaycasterManager.AddRaycaster(this);
        }

        protected override void OnDisable()
        {
            RaycasterManager.RemoveRaycasters(this);
            base.OnDisable();
        }

        #endregion
    }
}
