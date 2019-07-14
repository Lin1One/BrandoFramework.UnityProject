using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;

    [System.Serializable]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class IsoTransform : MethodBTN_MonoBehaviour
    {
#if UNITY_EDITOR
        public Vector3 localRotation;
        public Vector3 localScale;
        public bool bAutoUpdate = false;

        protected float IsometricRotationScale = 1f;

        public void Update_IsoTransform()
        {
            AdjustRotation();
            AdjustScale();
        }

        public void AdjustRotation()
        {
            AdjustRotation((Vector3)IsoMap.instance.TileAngle);
        }

        public void AdjustRotation(Vector3 customAngle)
        {
            adjustRotation(customAngle);
        }

        protected void adjustRotation(Vector3 globalRotation)
        {
            transform.eulerAngles = localRotation + globalRotation;
        }

        virtual public void AdjustScale()
        {
            adjustScale(Vector3.one);
        }

        virtual protected void adjustScale(Vector3 vMultiplier)
        {
            Vector3 v3Tmp = vMultiplier;

            if (localScale.Equals(Vector3.zero))
            {
                localScale = v3Tmp;
            }
            else
            {
                if (IsometricRotationScale != 0f)
                    transform.localScale = Vector3.Scale(v3Tmp, localScale) * IsometricRotationScale;
                else
                    transform.localScale = localScale;
            }
        }

        private void OnValidate()
        {
            if (bAutoUpdate)
                Update_IsoTransform();
        }

        [MethodBTN(false)]
        public void Update_Transform()
        {
            Update_IsoTransform();
        }
#endif
    }
}