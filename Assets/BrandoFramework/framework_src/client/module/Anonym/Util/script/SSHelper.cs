using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Anonym.Util
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SSHelper : MethodBTN_MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        Camera cam;

        [MethodBTN(false)]
        void UseSceneviewCamera()
        {
            Camera[] cams = UnityEditor.SceneView.GetAllSceneCameras();
            if (cams.Length > 0)
                cam = cams[0];
        }

        [SerializeField]
        List<GameObject> protagonists;

        [SerializeField]
        List <GameObject> extras;

        public static string defaultIsoMapPrefabPat = "Assets/Anonym/Util/prefab/SS cam.prefab";

        const string MSG_DefaultPath = "Assets/SSHelper";
        const string MSG_fileEX = ".png";

        [SerializeField]
        string _savePath;

        bool bExtentionPath
        {
            get
            {
                return string.IsNullOrEmpty(_savePath) ? false : _savePath.IndexOf(MSG_fileEX) > 0;
            }
        }
        string savePathEX
        {
            get
            {
                return bExtentionPath ? _savePath : _savePath + MSG_fileEX;
            }
        }

        [MethodBTN(false)]
        public void Screenshot()
        {
            _savePath = Screenshot(cam, protagonists, extras, savePathEX);
        }

        public static Texture2D TakeScreenshot(Camera cam, List<GameObject> protagonists, List<GameObject> extras)
        {
            if (cam == null)
            {
                Debug.Log("Camera Field is required.");
                return null;
            }

            Rect rt_origin = cam.rect;

#if UNITY_2017_3_OR_NEWER
            int resWidth = cam.scaledPixelWidth;
            int resHeight = cam.scaledPixelHeight;
#else
            int resWidth = Mathf.RoundToInt(cam.pixelRect.width);
            int resHeight = Mathf.RoundToInt(cam.pixelRect.height);
#endif
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 32);
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            screenShot.filterMode = FilterMode.Point;

            cam.targetTexture = rt;
            cam.rect = new Rect(0, 0, 1, 1);

            var extraList = Extra_ToggleOff(extras);
            cam.Render();
            Extra_ToggleOn(extraList);

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenShot.Apply();

            cam.targetTexture = null;
            cam.rect = rt_origin;
            RenderTexture.active = null;

            GameObject.DestroyImmediate(rt);

            return screenShot;
        }

        public static string Screenshot(Camera cam, List<GameObject> protagonists, List<GameObject> extras, string path)
        {
            if (cam == null)
            {
                Debug.Log("Camera Field is required.");
                return path;
            }
            Texture2D screenShot = TakeScreenshot(cam, protagonists, extras);
            if (screenShot == null)
            {
                Debug.Log("Failed, something wrong.");
                return path;
            }

            System.IO.File.WriteAllBytes(path, screenShot.EncodeToPNG());
            UnityEditor.AssetDatabase.ImportAsset(path);
            Debug.Log("Saved: " + path);
            path  = NextFileName(path);
            GameObject.DestroyImmediate(screenShot);

            return path;
        }

        static List<GameObject> Extra_ToggleOff(List<GameObject> extras)
        {
            List<GameObject> backupedList = new List<GameObject>();
            if (extras != null && extras.Any(r => r != null && r.activeSelf))
            {
                GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
                backupedList.AddRange(allGameObjects.Where(r => r.activeSelf && r.transform == r.transform.root));
                extras.RemoveAll(r => r == null);
                // backupedList.RemoveAll(r => models.Any(rr => rr.transform.IsChildOf(r.transform)));
                backupedList.ForEach(r => r.SetActive(false));
                extras.ForEach(m => m.SetActive(true));
            }
            return backupedList;
        }

        static void Extra_ToggleOn(List<GameObject> backupedList)
        {
            if (backupedList.Count > 0)
            {
                backupedList.ForEach(r => r.SetActive(true));
            }
        }

        static string NextFileName(string path)
        {
            return UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
        }

        public static Bounds CalculateBounds(List<GameObject> goList)
        {
            var gos = goList.Where(go => go != null).GetEnumerator();
            if (gos.MoveNext())
            {
                var b = CalculateBounds(gos.Current);
                while (gos.MoveNext())
                {
                    b.Encapsulate(CalculateBounds(gos.Current));
                }
                return b;
            }
            return new Bounds();
        }

        public static Bounds CalculateBounds(GameObject go)
        {
            Bounds b = new Bounds(go.transform.position, Vector3.zero);
            Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
            foreach (Renderer r in rList)
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }

        public static void FocusCameraOnGameObject(Camera c, List<GameObject> gos, Vector3 vGap, float fScale = 1f)
        {
            FocusCameraOnBound(c, CalculateBounds(gos), vGap, fScale);
        }

        public static void FocusCameraOnGameObject(Camera c, GameObject go, Vector3 vGap, float fScale = 1f)
        {
            FocusCameraOnBound(c, CalculateBounds(go), vGap, fScale);
        }

        public static void FocusCameraOnBound(Camera c, Bounds b, Vector3 vGap, float fScale = 1f)
        {
            Vector3 max = b.size;
            float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
            float dist = radius / (Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad / 2f));
            Vector3 pos = vGap * dist + b.center;
            c.transform.position = pos;
            c.transform.LookAt(b.center);
            c.orthographicSize = radius * 0.5f * fScale;
        }

        void OnEnable()
        {
            if (string.IsNullOrEmpty(_savePath))
                _savePath = MSG_DefaultPath;

            _savePath  = NextFileName(_savePath);

            if (cam == null)
                UseSceneviewCamera();
        }
#endif
    }
}