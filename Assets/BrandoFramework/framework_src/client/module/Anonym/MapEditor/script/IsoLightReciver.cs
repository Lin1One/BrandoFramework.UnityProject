using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    using Util;

    [System.Serializable]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class IsoLightReciver : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer sprr;
        public SpriteRenderer Sprr { get { return sprr; } }

        [SerializeField, Tooltip("This value is the original color before IsoLight is applied." +
            "If you change this value, the value applied with the lighting color will be applied automatically.")]
        Color BackupedSpriteRendererColor = Color.white;
        public Color GetBaseColor { get { return BackupedSpriteRendererColor; } }
        Color SetBaseColor_Undoable { set {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(this, "IsoLight Color");
#endif
                BackupedSpriteRendererColor = value;
            }
        }

        [SerializeField]
        Color StaticLightColor = Color.white;

        [SerializeField]
        Color DynamicLightColor = Color.white;

        [SerializeField]
        List<IsoLight> IsoStaticLightList = new List<IsoLight>();

        [SerializeField]
        List<IsoLight> IsoDynamicLightList = new List<IsoLight>();
        public IsoLight[] GetAllLightList()
        {
            return IsoStaticLightList.Concat(IsoDynamicLightList).ToArray();
        }
        List<IsoLight> GetLightList(bool isStaticLight)
        {
            return isStaticLight ? IsoStaticLightList : IsoDynamicLightList;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                UpdateLightColor();
                transform.hasChanged = false;
            }
        }

        private void Awake()
        {
            PreventlinkMissing();
        }

        private void OnValidate()
        {
            if (isActiveAndEnabled)
            {
                PreventlinkMissing();
                UpdateLightColor();
            }
        }

        bool bOnApplicationQuit = false;
        private void OnApplicationQuit()
        {
            bOnApplicationQuit = true;
        }

        void OnDestroy()
        {
            if (bOnApplicationQuit)
                return;

            ClearAllLights();
        }

        public void ClearAllLights()
        {
            clearLightList(IsoStaticLightList);
            clearLightList(IsoDynamicLightList);
        }

        void clearLightList(List<IsoLight> _list){
            if (_list == null || _list.Count == 0)
                return;
            _list.ForEach(r => r.RemoveReciver(this));
            _list.Clear();
        }

        public void PreventlinkMissing()
        {
            IsoStaticLightList.RemoveAll(r => r == null);
            IsoStaticLightList.ForEach(r => r.AddTarget(this));
            IsoDynamicLightList.RemoveAll(r => r == null);
            IsoDynamicLightList.ForEach(r => r.AddTarget(this));
        }

        public void Init()
        {
            sprr = GetComponent<SpriteRenderer>();
            BackUpSpriteRendererColor();
        }

        public Vector3 GetPositionWidthoutOffset()
        {
            Iso2DObject iso2D = GetComponent<Iso2DObject>();
            if (iso2D != null)
                return iso2D.GetPosition_WithoutFudge();
            return transform.position;
        }

        public bool Check(IsoLight light)
        {
            PreventlinkMissing();

            bool bLightTypeChanged;
            if (light.bStaticLight)
                bLightTypeChanged = IsoStaticLightList.Contains(light) && !IsoDynamicLightList.Contains(light);
            else
                bLightTypeChanged = !IsoStaticLightList.Contains(light) && IsoDynamicLightList.Contains(light);

            if (bLightTypeChanged)
                GetLightList(light.bStaticLight).Sort((x, y) => x.UniquePriority.CompareTo(y.UniquePriority));

            return bLightTypeChanged;
        }

        public void AddLight(IsoLight light)
        {
            if (light.bStaticLight)
                removeLightFromDynamicList(light);
            else
                removeLightFromStaticList(light);

            List<IsoLight> _list = GetLightList(light.bStaticLight);
            if (!_list.Contains(light))
                _list.Add(light);

            _list.Sort((x, y) => x.UniquePriority.CompareTo(y.UniquePriority));

            UpdateLightColor(light.bStaticLight);
            ApplyLightColor();
        }

        public void RemoveLight(IsoLight light)
        {
            removeLightFromStaticList(light);
            removeLightFromDynamicList(light);

            if (IsoDynamicLightList.Count == 0 && (IsoStaticLightList.Count == 0 && !Application.isPlaying))
            {
                RevertSpriteRendererColor();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        GameObject.DestroyImmediate(this);
                    };
                }
                else
#else
#endif
                    Destroy(this);
            }
            else
            {
                ApplyLightColor();
            }
        }

        void removeLightFromStaticList(IsoLight light)
        {
            if (IsoStaticLightList.Contains(light))
            {
                IsoStaticLightList.RemoveAll(r => r == light);
                UpdateLightColor(true);
            }
        }

        void removeLightFromDynamicList(IsoLight light)
        {
            if (IsoDynamicLightList.Contains(light))
            {
                IsoDynamicLightList.RemoveAll(r => r == light);
                UpdateLightColor(false);
            }
        }

        public void UpdateLightColor()
        {
            UpdateLightColor(true, true);
            UpdateLightColor(false, true);
            ApplyLightColor();
        }

        public void UpdateBaseColor(Color newColor)
        {
            SetBaseColor_Undoable = newColor;
            UpdateLightColor();
        }

        public void UpdateLightColor(bool isStaticLight, bool bWithOutApplyColor = false)
        {
            if (!this || (isStaticLight && Application.isPlaying))
                return;

            List<IsoLight> _list = GetLightList(isStaticLight);

            Color color = Color.white;

            _list.ForEach(r =>
            {
                if (r)
                    color = r.Calc(color, this);
            });

            if (isStaticLight)
                StaticLightColor = color;
            else
                DynamicLightColor = color;

            if (!bWithOutApplyColor)
                ApplyLightColor();
        }

        void applySprrColor(Color color)
        {
            if (sprr && sprr.color != color)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(sprr, "IsoLight Color");
#endif
                sprr.color = color;
            }
        }
        void ApplyLightColor()
        {
            applySprrColor(BackupedSpriteRendererColor * StaticLightColor * DynamicLightColor);
        }
        void BackUpSpriteRendererColor()
        {
            if (sprr)
                SetBaseColor_Undoable = sprr.color;
        }
        public void RevertSpriteRendererColor()
        {
            applySprrColor(BackupedSpriteRendererColor);
        }
    }

    public static class IsoLightReciverUtility
    {
        public static IsoLightReciver SetIsoLightReciver(this GameObject obj, IsoLight light)
        {
            IsoLightReciver reciver = obj.GetComponent<IsoLightReciver>();
            if (reciver == null)
            {
                reciver = obj.AddComponent<IsoLightReciver>();
                reciver.Init();
            }
            reciver.AddLight(light);
            return reciver;
        }
    }
}