using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Anonym.Util
{    
    [System.Serializable]
    public class TmpTexture2D
    {
        [SerializeField]
        public Texture2D texture = null;

        public bool bCorrupted = false;

        public Texture2D MakeIcon(ref Sprite[] sprites, ref Color[] colors)
        {
            string conditions = sprites.Select(s => s.name).Concat(colors.Select(c => c.ToString())).Aggregate((i, j) => i + j);
            if (bCorrupted || texture == null || !texture.name.Equals(conditions))
            {
                Texture2D _texture = null;
                bool bAccumulate = false;
                for (int i = 0; i < sprites.Length; ++i)
                {
                    Sprite sprite = sprites[i];
                    if (sprite == null)
                        continue;

                    Color color = colors != null && colors.Length > i ? colors[i] : Color.white;
                    bAccumulate |= OverrideColoredTexture2D(sprite.texture, sprite.textureRect, ref _texture, color, bAccumulate);
                }
                _texture.name = conditions;
                return texture = _texture;
            }
            return texture = null;
        }

        public Texture2D MakeRenderImage(List<GameObject> _target, Camera camera_original, Color baseColor, int width = 128, int height = 128, string extraCondition = "", int iLayer = 0)
        {
            List<SpriteRenderer> sprites = new List<SpriteRenderer>();
            _target.ForEach(go => sprites.AddRange(go.GetComponentsInChildren<SpriteRenderer>()));
            if (sprites.All(r => r == null || r.sprite == null))
                return texture = null;

            string conditions = sprites.Select(s => s.sprite == null ? "_" : s.sprite.name).Concat(
                sprites.Select(s => s.sprite == null ? "_" : s.color.ToString())).Aggregate((i, j) => i + j) + extraCondition;

            if (bCorrupted || texture == null || !texture.name.Equals(conditions))
            {

                var flags = BindingFlags.Static | BindingFlags.NonPublic;
                var propInfo = typeof(Camera).GetProperty("PreviewCullingLayer", flags);
                int previewLayer = iLayer != 0 ? iLayer : (int)propInfo.GetValue(null, new object[0]);

                var targets = _target.Where(_t => _t != null).Select(_t => GameObject.Instantiate(_t).gameObject).ToList();
                if (targets.Count == 0)
                    return null;

                targets.ForEach(t =>
                {
                    t.hideFlags = HideFlags.HideAndDontSave;
                    t.layer = previewLayer;
                    foreach (Transform transform in t.GetComponentsInChildren<Transform>())
                    {
                        transform.gameObject.layer = previewLayer;
                    }
                });

                Texture2D _texture = null;

                if (camera_original == null)
                    camera_original = Object.FindObjectOfType<Camera>();

                Camera camera = Camera.Instantiate(camera_original) as Camera;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 100f;
                camera.orthographic = true;
                camera.pixelRect = new Rect(0, 0, width, height);
                camera.cullingMask = 1 << previewLayer;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = baseColor;

                Isometric.IsoTransform isoTransform = targets.Select(t => t.GetComponentInChildren<Isometric.IsoTransform>()).First();
                if (isoTransform != null)
                {
                    Quaternion isometricQuaternion = Quaternion.Euler(isoTransform.transform.eulerAngles);
                    SSHelper.FocusCameraOnGameObject(camera, targets, isometricQuaternion * Vector3.back, 0.6f);
                }

                _texture = SSHelper.TakeScreenshot(camera, targets, null);

                targets.ForEach(t => GameObject.DestroyImmediate(t, true));
                GameObject.DestroyImmediate(camera.gameObject, true);
                _texture.name = conditions;
                return texture = _texture;
            }
            return null;
        }

        public Texture2D MakeRenderImage(GameObject _target, Camera camera_original, Color baseColor, int width = 128, int height = 128, string extranCondition = "")
        {
            var sprites = _target.gameObject.GetComponentsInChildren<SpriteRenderer>();
            if (sprites.All(r => r == null || r.sprite == null))
                return texture = null;

            string conditions = sprites.Select(s => s.sprite == null ? "null" : s.sprite.name).Concat(
                sprites.Select(s => s.color.ToString())).Aggregate((i, j) => i + j) + extranCondition;

            if (bCorrupted || texture == null || !texture.name.Equals(conditions))
            {

                var flags = BindingFlags.Static | BindingFlags.NonPublic;
                var propInfo = typeof(Camera).GetProperty("PreviewCullingLayer", flags);
                int previewLayer = 1; // (int)propInfo.GetValue(null, new object[0]);

                GameObject target = GameObject.Instantiate(_target).gameObject;
                target.hideFlags = HideFlags.HideAndDontSave;
                target.layer = previewLayer;

                foreach (Transform transform in target.GetComponentsInChildren<Transform>())
                {
                    transform.gameObject.layer = previewLayer;
                }

                if (target != null)
                {
                    if (camera_original == null)
                        camera_original = Object.FindObjectOfType<Camera>();

                    Camera camera = Camera.Instantiate(camera_original) as Camera;
                    camera.nearClipPlane = 0.1f;
                    camera.farClipPlane = 100f;
                    camera.orthographic = true;
                    camera.pixelRect = new Rect(0, 0, width, height);
                    camera.cullingMask = 1 << previewLayer;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = baseColor;

                    Isometric.IsoTransform isoTransform = target.GetComponentInChildren<Isometric.IsoTransform>();
                    if (isoTransform != null)
                    {
                        Quaternion isometricQuaternion = Quaternion.Euler(isoTransform.transform.eulerAngles);
                        SSHelper.FocusCameraOnGameObject(camera, target, isometricQuaternion * Vector3.back);
                    }

                    Texture2D _texture = SSHelper.TakeScreenshot(camera, new List<GameObject>() { target }, null);
                    GameObject.DestroyImmediate(camera.gameObject, true);
                    GameObject.DestroyImmediate(target, true);
                    _texture.name = conditions;
                    return texture = _texture;
                }
                else
                    texture = null;
            }
            return null;
        }

        public void DrawRect(Rect rect)
        {
            if (texture != null)
                GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        private static bool OverrideColoredTexture2D(Texture2D source, Rect sourceRect, ref Texture2D destination, Color color, bool bAccumulate)
        {
            int s_x = Mathf.RoundToInt(sourceRect.x);
            int s_y = Mathf.RoundToInt(sourceRect.y);
            int s_width = Mathf.RoundToInt(sourceRect.width);
            int s_height = Mathf.RoundToInt(sourceRect.height);

            if (destination == null)
            {
                destination = new Texture2D(s_width, s_height, TextureFormat.RGBA32, false);
                destination.alphaIsTransparency = true;
                destination.filterMode = FilterMode.Point;
            }
            else
            {
                s_width = Mathf.Min(s_width, destination.width);
                s_height = Mathf.Min(s_height, destination.height);
            }

            int d_width = destination.width;
            int d_height = destination.height;
            int d_x = (d_width - s_width) / 2;
            int d_y = (d_height - s_height) / 2;

            Texture2D sourceForRead = source;
            // if (source is compressed)
            {
                RenderTexture rt = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
                RenderTexture.active = rt;
                Graphics.Blit(source, rt);

                sourceForRead = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
                sourceForRead.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
            }

            var rawdata = sourceForRead.GetRawTextureData();
            Color32[] destColor = bAccumulate ? destination.GetPixels32() : null;
            Color32[] newcolors = new Color32[s_width * s_height];
            Color col;
            int index_new_color;
            int index_source_raw;
            int index_dest_raw;

            for (int _y = 0; _y < s_height; ++_y)
            {
                for (int _x = 0; _x < s_width; _x++)
                {
                    index_new_color = _x + _y * s_width;
                    index_source_raw = 4 * ((s_x + _x) + (s_y + _y) * sourceForRead.width);
                    index_dest_raw = (d_x + _x) + (d_y + _y) * d_width;

                    col = new Color32(rawdata[index_source_raw], rawdata[index_source_raw + 1], rawdata[index_source_raw + 2], rawdata[index_source_raw + 3]) * color;
                    newcolors[index_new_color] = bAccumulate ? CustomEditorGUI.Color_AlphaBlend(col, destColor[index_dest_raw]) : col;
                }
            }

            destination.SetPixels32(d_x, d_y, s_width, s_height, newcolors);
            destination.Apply();
            return newcolors.Length > 0;
        }
    }
}