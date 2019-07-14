using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;

    [System.Serializable]
    [ExecuteInEditMode]
    public class IsoLight : MonoBehaviour
    {
        public enum ApplyType
        {
            Multiply = 0,
            Positive = 1,
            Negative = 2,
            Overlay = 3,
            Override = 4,
        }

        [Header("[Option]")]
        [SerializeField]
        bool bOnOff = true;
        public bool TurnOnOff
        {
            get { return bOnOff; }
            set { bOnOff = value; UpdateAllReciver(); }
        }

        [SerializeField]
        public bool bStaticLight = false;

        [SerializeField, Range(0, 10)]
        public int UniquePriority = 0;

        [SerializeField]
        ApplyType type = ApplyType.Multiply;

        [HideInInspector, SerializeField]
        Collider customRangeCollider = null;

        [HideInInspector, SerializeField]
        bool bSimpleShadow = false;

        [SerializeField, ConditionalHide("bSimpleShadow", conditionalSourceValue: "True", hideInInspector: true)]
        Color ShadowColor = Color.gray;

        [Header("[Color]")]
        [SerializeField]
        Gradient color = new Gradient();

        [SerializeField]
        AnimationCurve colorCurve = new AnimationCurve(new Keyframe[] {new Keyframe(0, 0), new Keyframe(1, 1) });

        [SerializeField]
        Vector2 colorCurveRange = new Vector2(0, 10);

        [SerializeField]
        Vector3 AxisWeightForDistance = Vector3.one;

        [SerializeField]
        bool bAffectOverRange = true;

        [SerializeField, HideInInspector]
        List<IsoLightReciver> targetList = new List<IsoLightReciver>();
        public List<IsoLightReciver> TargetList { get { return targetList; } }

        void OnValidate()
        {
            if (isActiveAndEnabled)
                init();
        }

        private void OnEnable()
        {
            init();
        }

        void init()
        {
            TargetList.RemoveAll(r => r == null);
            TargetList.ForEach(r =>
            {
                if (!r.Check(this))
                {
                    r.AddLight(this);
                }
            });
            UpdateAllReciver();
        }        

        void DebugLog_TotalTargetCount(string msg)
        {
            Debug.Log(string.Format("[{0}] {1} Target - Total({2})", name, msg, targetList.Count));
        }

        public void AddTarget(IsoLightReciver reciver)
        {
            if (!targetList.Contains(reciver))
                targetList.Add(reciver);
        }

        public void AddTarget(GameObject lookupGameObject, bool bIncludeChild)
        {
            if (lookupGameObject == null)
                return;

            if (bIncludeChild)
            {
                var targetObjects = lookupGameObject.GetComponentsInChildren<Iso2DBase>();
                targetList.AddRange(targetObjects.Select(r => r.gameObject.SetIsoLightReciver(this)).Where(r => !targetList.Contains(r)));
            }
            else
            {
                var targetObject = lookupGameObject.GetComponent<Iso2DBase>();
                if (targetObject != null)
                    AddTarget(targetObject.gameObject.SetIsoLightReciver(this));
            }
            DebugLog_TotalTargetCount("Add");
        }

        public void RemoveTarget(GameObject lookupGameObject, bool bIncludeChild)
        {
            if (lookupGameObject == null)
                return;

            List<IsoLightReciver> targetObjects = new List<IsoLightReciver>();
            if (bIncludeChild)
                targetObjects.AddRange(lookupGameObject.GetComponentsInChildren<IsoLightReciver>());
            else
                targetObjects.Add(lookupGameObject.GetComponent<IsoLightReciver>());

            foreach (var r in targetObjects)
            {
                if (r)
                    r.RemoveLight(this);
            }

            targetList.RemoveAll(r => r == null || targetObjects.Contains(r));
            DebugLog_TotalTargetCount("Remove");
        }

        public void RemoveReciver(IsoLightReciver reciver)
        {
            targetList.Remove(reciver);
        }

        public void AddTarget_All()
        {
            targetList.Clear();

            var targetObjects = FindObjectsOfType<Iso2DBase>();
            targetList.AddRange(targetObjects.Select(r => r.gameObject.SetIsoLightReciver(this)));

            var reciverOnly = FindObjectsOfType<IsoLightReciver>();
            reciverOnly = reciverOnly.Where(r => r.GetComponent<Iso2DBase>() == null).ToArray();
            foreach (var r in reciverOnly)
            {
                if (!r.Check(this))
                {
                    r.AddLight(this);
                }
            }

            DebugLog_TotalTargetCount("All");
        }

        public void AddTarget_All(LayerMask layerMask)
        {
            var targetObjects = FindObjectsOfType<Iso2DBase>();
            targetList.AddRange( targetObjects.
                Where(r => 
                    (layerMask & 1 << r.gameObject.layer) > 0 
                    && !targetList.Exists(s => s.gameObject == r.gameObject)).
                Select(r => r.gameObject.SetIsoLightReciver(this)));

            var reciverOnly = FindObjectsOfType<IsoLightReciver>();
            reciverOnly = reciverOnly.Where(r => r.GetComponent<Iso2DBase>() == null).ToArray();
            foreach(var r in reciverOnly)
            {
                if (!r.Check(this))
                {
                    r.AddLight(this);
                }
            }

            DebugLog_TotalTargetCount("All");
        }

        public void RemoveTarget_All(LayerMask layerMask)
        {
            var maskedList = targetList.Where(r => (layerMask & 1 << r.gameObject.layer) > 0);
            foreach (var one in maskedList)
                one.RemoveLight(this);
            targetList.RemoveAll(r => maskedList.Contains(r));
        }

        public void RemoveTarget_All()
        {
            targetList.ForEach(r =>
            {
                if (r != null)
                    r.RemoveLight(this);
            });
            targetList.Clear();
        }

        public void UpdateAllReciver()
        {
            targetList.ForEach(r => r.UpdateLightColor(bStaticLight));
        }

        public Color Calc(Color baseColor, IsoLightReciver _reciver)
        {
            if (!bOnOff || color == null)
                return baseColor;

            float fLength = 0;

            if (bSimpleShadow)
            {
                Bounds _bounds = _reciver.Sprr.bounds;
                Vector3 topOfTarget = _bounds.center + Vector3.up * _bounds.extents.y;
                fLength = Vector3.Distance(transform.position, topOfTarget);
                var hits = Physics.RaycastAll(topOfTarget, transform.position - topOfTarget, fLength, -1, QueryTriggerInteraction.Collide);
                if (!hits.All(r => _reciver.transform.IsChildOf(r.transform) || r.transform.IsChildOf(transform.root)))
                    return ShadowColor;
            }

            Vector3 position = _reciver.GetPositionWidthoutOffset() - transform.position;
            position.Scale(AxisWeightForDistance);
            fLength = position.magnitude;
            if (!bAffectOverRange && fLength > colorCurveRange.y)
                return baseColor;

            float fClampedLength = Mathf.Clamp01(Mathf.InverseLerp(colorCurveRange.x, colorCurveRange.y, fLength));
            Color lightColor = color.Evaluate(colorCurve.Evaluate(fClampedLength));
            float fLightAlpha = lightColor.a;
            
            switch (type)
            {
                case ApplyType.Positive:
                    baseColor = ClampedRGB(baseColor + fLightAlpha * lightColor);
                    break;
                case ApplyType.Negative:
                    baseColor = ClampedRGB(baseColor - fLightAlpha * lightColor);
                    break;
                case ApplyType.Override:
                    baseColor = lightColor;
                    break;
                case ApplyType.Overlay:
                    baseColor = ClampedRGB((1 - fLightAlpha) * baseColor + fLightAlpha * lightColor); 
                    break;
                case ApplyType.Multiply:
                default:
                    baseColor *= lightColor;
                    break;
            }
            baseColor.a = 1;
            return baseColor;
        }

        bool bOnApplicationQuit = false;
        private void OnApplicationQuit()
        {
            bOnApplicationQuit = true;
        }

        public void OnDestroy()
        {
            if (bOnApplicationQuit)
                return;

            RemoveTarget_All();
        }

        private void Update()
        {
            if(transform.hasChanged)
            {
                UpdateAllReciver();
                transform.hasChanged = false;
            }
        }        

        public static Color ClampedRGB(Color color)
        {
            color.r = Mathf.Clamp01(color.r);
            color.g = Mathf.Clamp01(color.g );
            color.b = Mathf.Clamp01(color.b);
            return color;
        }

        private void OnDrawGizmosSelected()
        {
            Color startColor = color.Evaluate(0);
            startColor.a = 1;
            for(int i = 0; i < 3; ++i)
            {
                Gizmos.color = startColor * (1 - i * 0.25f);
                Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.225f + i * 0.025f, colorCurveRange.x));
            }
        }

#if UNITY_EDITOR
        public void CreateDefaultSelector()
        {
            var go = new GameObject("GameObject Selector");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<GameObjectSelector>().Add_SphereColliderChild().radius = 4f;
        }
#endif
    }
}