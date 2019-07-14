using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Anonym.Util
{
    using Isometric;

    [RequireComponent(typeof(Camera))]
    public class SelfieStick : MonoBehaviour
    {        
        [SerializeField]
        Transform _Target;        
        public void SetTarget(Transform _t)
        {
            if (_t != null)
            {
                _Target = _t;
                ISO = _Target.GetComponentInChildren<IsometircSortingOrder>();
            }
        }
        [SerializeField]
        List<GameObject> _renderObjectList;
        [HideInInspector]
        Vector3 _arm_position;
        [HideInInspector]
        Quaternion _arm_rotation;

        [SerializeField]
        Camera _localCam;

        RenderTexture _renderTexture;

        [SerializeField]
        int _iRenderTexture_Width = 128;
        [SerializeField]
        int _iRenderTexture_Height = 128;

        Rect _TextureRT { get { return new Rect(0, 0, _iRenderTexture_Width, _iRenderTexture_Height); } }

        [SerializeField]
        TextureWrapMode textureWrapMode = TextureWrapMode.Clamp;
        [SerializeField]
        FilterMode filterMode = FilterMode.Point;

        [SerializeField]
        int iSortingOrder = 0;

        [SerializeField]
        Isometric.IsometricSortingOrder ISO;

        [SerializeField]
        SpriteRenderer sprr;

        void OnDestroy()
        {
        }

        void Start()
        {
            SetupCamera();
        }

        public void SetupCamera(Camera cam = null)
        {
            if (cam == null)
                cam = gameObject.GetComponent<Camera>();
            _localCam = cam;

            if (_localCam != null)
            {
                //NonRenderingLayer.ApplyMask(_localCam);
                _localCam.cullingMask = 1 << NonRenderingLayer.UpdateTemporaryRenderingLayer();

                _renderTexture = CreateTexture();
                SetTexture();

                AdjustSize();
            }
        }

        RenderTexture CreateTexture()
        {
            var newTexture = new RenderTexture(_iRenderTexture_Width,
                    _iRenderTexture_Height, 16, RenderTextureFormat.ARGB32);
            newTexture.wrapMode = textureWrapMode;
            newTexture.filterMode = filterMode;
            newTexture.Create();
            return newTexture;
        }

        void SetTexture()
        {
            _localCam.targetTexture = _renderTexture;

            if (sprr == null)
                return;

            if (sprr.sprite == null || !sprr.sprite.texture.isReadable)
            {
                sprr.sprite = Sprite.Create(new Texture2D(_iRenderTexture_Width, _iRenderTexture_Height, TextureFormat.ARGB32, false, false), _TextureRT, Vector2.one * 0.5f, 128);
            }
        }

        public void AdjustSize()
        {
            _localCam.orthographicSize = (float)_iRenderTexture_Height / (2f * ((sprr == null || sprr.sprite == null) ? IsoMap.instance.ReferencePPU : sprr.sprite.pixelsPerUnit));
        }

        void SetRenderObjects(List<GameObject> newLiist)
        {
            _renderObjectList.Clear();
            _renderObjectList.AddRange(newLiist);
        }

        void PrepareRenderObjects()
        {
            int iValue = NonRenderingLayer.UpdateTemporaryRenderingLayer();
            _renderObjectList.ForEach(r => SetLayerRecursively(r, iValue));
        }

        void RevertToNonRenderLayer()
        {
            int _NonRenderingLayer = NonRenderingLayer.UpdateNonRenderingLayer();
            _renderObjectList.ForEach(r => SetLayerRecursively(r, _NonRenderingLayer));
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj || obj == gameObject)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }

        void OnEnable()
        {
            _arm_position = transform.localPosition;
            _arm_rotation = transform.rotation;

            Camera.onPreCull += PreCull;
            Camera.onPostRender += UpdateRenderTexture;
        }

        void OnDisable()
        {
            Camera.onPreCull -= PreCull;
            Camera.onPostRender -= UpdateRenderTexture;
        }

        void PreCull(Camera cam)
        {
            if (_localCam != cam)
                return;

            if (_Target != null)
            {
                transform.position = _Target.position + _arm_position;
                transform.rotation = _arm_rotation;
                iSortingOrder = sprr.sortingOrder = ISO != null ? ISO.iLastSortingOrder : IsometricSortingOrderUtility.IsometricSortingOrder(_Target);
            }
            PrepareRenderObjects();
        }

        void UpdateRenderTexture(Camera cam)
        {
            if (_localCam != cam)
                return;

            RevertToNonRenderLayer();            

            if (sprr.sprite.texture.isReadable)
            {
                Graphics.CopyTexture(_renderTexture, sprr.sprite.texture);
            }
        }
    }
}