
using System;
using UnityEngine;

namespace GameWorld
{
    public class MeshBakerUtility
    {

        public static bool DO_INTEGRITY_CHECKS = false;

        public static Texture2D createTextureCopy(Texture2D source)
        {
            Texture2D newTex = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
            newTex.SetPixels(source.GetPixels());
            return newTex;
        }

        public static bool ArrayBIsSubsetOfA(System.Object[] a, System.Object[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                bool foundBinA = false;
                for (int j = 0; j < a.Length; j++)
                {
                    if (a[j] == b[i])
                    {
                        foundBinA = true;
                        break;
                    }
                }
                if (foundBinA == false) return false;
            }
            return true;
        }

        public static Material[] GetGOMaterials(GameObject go)
        {
            if (go == null) return null;
            Material[] sharedMaterials = null;
            Mesh mesh = null;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                sharedMaterials = mr.sharedMaterials;
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf == null)
                {
                    throw new Exception("Object " + go + " has a MeshRenderer but no MeshFilter.");
                }
                mesh = mf.sharedMesh;
            }

            SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                sharedMaterials = smr.sharedMaterials;
                mesh = smr.sharedMesh;
            }

            if (sharedMaterials == null)
            {
                Debug.LogError("Object " + go.name + " does not have a MeshRenderer or a SkinnedMeshRenderer component");
                return new Material[0];
            }
            else if (mesh == null)
            {
                Debug.LogError("Object " + go.name + " has a MeshRenderer or SkinnedMeshRenderer but no mesh.");
                return new Material[0];
            }
            else
            {
                if (mesh.subMeshCount < sharedMaterials.Length)
                {
                    Debug.LogWarning("Object " + go + " has only " + mesh.subMeshCount + " submeshes and has " + sharedMaterials.Length + " materials. Extra materials do nothing.");
                    Material[] newSharedMaterials = new Material[mesh.subMeshCount];
                    Array.Copy(sharedMaterials, newSharedMaterials, newSharedMaterials.Length);
                    sharedMaterials = newSharedMaterials;
                }
                return sharedMaterials;
            }
        }

        public static Mesh GetMesh(GameObject go)
        {
            if (go == null) return null;
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                return mf.sharedMesh;
            }

            SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                return smr.sharedMesh;
            }

            return null;
        }

        public static void SetMesh(GameObject go, Mesh m)
        {
            if (go == null) return;
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                mf.sharedMesh = m;
            }
            else
            {
                SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    smr.sharedMesh = m;
                }
            }
        }

        public static Renderer GetRenderer(GameObject go)
        {
            if (go == null) return null;
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null) return mr;


            SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) return smr;
            return null;
        }

        public static void DisableRendererInSource(GameObject go)
        {
            if (go == null) return;
            MeshRenderer mf = go.GetComponent<MeshRenderer>();
            if (mf != null)
            {
                mf.enabled = false;
                return;
            }

            SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.enabled = false;
                return;
            }
        }

        public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds)
        {
            MeshAnalysisResult mar = new MeshAnalysisResult();
            bool outVal = hasOutOfBoundsUVs(m, ref mar);
            uvBounds = mar.uvRect;
            return outVal;
        }

        /// <summary>
        /// 判断网格超出 UV
        /// </summary>
        /// <param name="m"></param>
        /// <param name="putResultHere"></param>
        /// <param name="submeshIndex"></param>
        /// <param name="uvChannel"></param>
        /// <returns></returns>
        public static bool hasOutOfBoundsUVs(Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1, int uvChannel = 0)
        {
            if (m == null)
            {
                putResultHere.hasOutOfBoundsUVs = false;
                return putResultHere.hasOutOfBoundsUVs;
            }
            Vector2[] uvs;
            if (uvChannel == 0)
            {
                uvs = m.uv;
            }
            else if (uvChannel == 1)
            {
                uvs = m.uv2;
            }
            else if (uvChannel == 2)
            {
                uvs = m.uv3;
            }
            else
            {

                uvs = m.uv4;
            }
            return hasOutOfBoundsUVs(uvs, m, ref putResultHere, submeshIndex);
        }

        public static bool hasOutOfBoundsUVs(Vector2[] uvs, Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1)
        {
            putResultHere.hasUVs = true;
            if (uvs.Length == 0)
            {
                putResultHere.hasUVs = false;
                putResultHere.hasOutOfBoundsUVs = false;
                putResultHere.uvRect = new Rect();
                return putResultHere.hasOutOfBoundsUVs;
            }
            float minx, miny, maxx, maxy;
            if (submeshIndex >= m.subMeshCount)
            {
                putResultHere.hasOutOfBoundsUVs = false;
                putResultHere.uvRect = new Rect();
                return putResultHere.hasOutOfBoundsUVs;
            }
            else if (submeshIndex >= 0)
            {
                //checking specific submesh
                int[] tris = m.GetTriangles(submeshIndex);
                if (tris.Length == 0)
                {
                    putResultHere.hasOutOfBoundsUVs = false;
                    putResultHere.uvRect = new Rect();
                    return putResultHere.hasOutOfBoundsUVs;
                }
                minx = maxx = uvs[tris[0]].x;
                miny = maxy = uvs[tris[0]].y;
                for (int idx = 0; idx < tris.Length; idx++)
                {
                    int i = tris[idx];
                    if (uvs[i].x < minx) minx = uvs[i].x;
                    if (uvs[i].x > maxx) maxx = uvs[i].x;
                    if (uvs[i].y < miny) miny = uvs[i].y;
                    if (uvs[i].y > maxy) maxy = uvs[i].y;
                }
            }
            else
            {
                //checking all UVs
                minx = maxx = uvs[0].x;
                miny = maxy = uvs[0].y;
                for (int i = 0; i < uvs.Length; i++)
                {
                    if (uvs[i].x < minx) minx = uvs[i].x;
                    if (uvs[i].x > maxx) maxx = uvs[i].x;
                    if (uvs[i].y < miny) miny = uvs[i].y;
                    if (uvs[i].y > maxy) maxy = uvs[i].y;
                }
            }
            Rect uvBounds = new Rect();
            uvBounds.x = minx;
            uvBounds.y = miny;
            uvBounds.width = maxx - minx;
            uvBounds.height = maxy - miny;
            if (maxx > 1f || minx < 0f || maxy > 1f || miny < 0f)
            {
                putResultHere.hasOutOfBoundsUVs = true;
            }
            else
            {
                putResultHere.hasOutOfBoundsUVs = false;
            }
            putResultHere.uvRect = uvBounds;
            return putResultHere.hasOutOfBoundsUVs;
        }

        public static void setSolidColor(Texture2D t, Color c)
        {
            Color[] cs = t.GetPixels();
            for (int i = 0; i < cs.Length; i++)
            {
                cs[i] = c;
            }
            t.SetPixels(cs);
            t.Apply();
        }

        public static Texture2D resampleTexture(Texture2D source, int newWidth, int newHeight)
        {
            TextureFormat f = source.format;
            if (f == TextureFormat.ARGB32 ||
                f == TextureFormat.RGBA32 ||
                f == TextureFormat.BGRA32 ||
                f == TextureFormat.RGB24 ||
                f == TextureFormat.Alpha8 ||
                f == TextureFormat.DXT1)
            {
                Texture2D newTex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, true);
                float w = newWidth;
                float h = newHeight;
                for (int i = 0; i < newWidth; i++)
                {
                    for (int j = 0; j < newHeight; j++)
                    {
                        float u = i / w;
                        float v = j / h;
                        newTex.SetPixel(i, j, source.GetPixelBilinear(u, v));
                    }
                }
                newTex.Apply();
                return newTex;
            }
            else
            {
                Debug.LogError("Can only resize textures in formats ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT");
                return null;
            }
        }

        class MB_Triangle
        {
            int submeshIdx;
            int[] vs = new int[3];

            public bool isSame(object obj)
            {
                MB_Triangle tobj = (MB_Triangle)obj;
                if (vs[0] == tobj.vs[0] &&
                    vs[1] == tobj.vs[1] &&
                    vs[2] == tobj.vs[2] &&
                    submeshIdx != tobj.submeshIdx)
                {
                    return true;
                }
                return false;
            }

            public bool sharesVerts(MB_Triangle obj)
            {
                if (vs[0] == obj.vs[0] ||
                    vs[0] == obj.vs[1] ||
                    vs[0] == obj.vs[2])
                {
                    if (submeshIdx != obj.submeshIdx) return true;
                }
                if (vs[1] == obj.vs[0] ||
                    vs[1] == obj.vs[1] ||
                    vs[1] == obj.vs[2])
                {
                    if (submeshIdx != obj.submeshIdx) return true;
                }
                if (vs[2] == obj.vs[0] ||
                    vs[2] == obj.vs[1] ||
                    vs[2] == obj.vs[2])
                {
                    if (submeshIdx != obj.submeshIdx) return true;
                }
                return false;
            }

            public void Initialize(int[] ts, int idx, int sIdx)
            {
                vs[0] = ts[idx];
                vs[1] = ts[idx + 1];
                vs[2] = ts[idx + 2];
                submeshIdx = sIdx;
                Array.Sort(vs);
            }
        }

        /// <summary>
        /// 判断是否所有材质都不同
        /// </summary>
        /// <param name="sharedMaterials"></param>
        /// <returns></returns>
        public static bool AreAllSharedMaterialsDistinct(Material[] sharedMaterials)
        {
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                for (int j = i + 1; j < sharedMaterials.Length; j++)
                {
                    if (sharedMaterials[i] == sharedMaterials[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static int doSubmeshesShareVertsOrTris(Mesh m, ref MeshAnalysisResult mar)
        {
            MB_Triangle consider = new MB_Triangle();
            MB_Triangle other = new MB_Triangle();
            //cache all triangles
            int[][] tris = new int[m.subMeshCount][];
            for (int i = 0; i < m.subMeshCount; i++)
            {
                tris[i] = m.GetTriangles(i);
            }
            bool sharesVerts = false;
            bool sharesTris = false;
            for (int i = 0; i < m.subMeshCount; i++)
            {
                int[] smA = tris[i];
                for (int j = i + 1; j < m.subMeshCount; j++)
                {
                    int[] smB = tris[j];
                    for (int k = 0; k < smA.Length; k += 3)
                    {
                        consider.Initialize(smA, k, i);
                        for (int l = 0; l < smB.Length; l += 3)
                        {
                            other.Initialize(smB, l, j);
                            if (consider.isSame(other))
                            {
                                sharesTris = true;
                                break;
                            }
                            if (consider.sharesVerts(other))
                            {
                                sharesVerts = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (sharesTris)
            {
                mar.hasOverlappingSubmeshVerts = true;
                mar.hasOverlappingSubmeshTris = true;
                return 2;
            }
            else if (sharesVerts)
            {
                mar.hasOverlappingSubmeshVerts = true;
                mar.hasOverlappingSubmeshTris = false;
                return 1;
            }
            else
            {
                mar.hasOverlappingSubmeshTris = false;
                mar.hasOverlappingSubmeshVerts = false;
                return 0;
            }
        }

        public static bool GetBounds(GameObject go, out Bounds b)
        {
            if (go == null)
            {
                Debug.LogError("go paramater was null");
                b = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }
            Renderer r = GetRenderer(go);
            if (r == null)
            {
                Debug.LogError("GetBounds must be called on an object with a Renderer");
                b = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }
            if (r is MeshRenderer)
            {
                b = r.bounds;
                return true;
            }
            else if (r is SkinnedMeshRenderer)
            {
                b = r.bounds;
                return true;
            }
            Debug.LogError("GetBounds must be called on an object with a MeshRender or a SkinnedMeshRenderer.");
            b = new Bounds(Vector3.zero, Vector3.zero);
            return false;
        }

        public static void Destroy(UnityEngine.Object o)
        {
            if (Application.isPlaying)
            {
                MonoBehaviour.Destroy(o);
            }
            else
            {
                //			string p = AssetDatabase.GetAssetPath(o);
                //			if (p != null && p.Equals("")) // don't try to destroy assets
                MonoBehaviour.DestroyImmediate(o, false);
            }
        }

        #region GraphicsUtility

        #region MeshUtility

        public static void OptimizeMesh(Mesh m)
        {
            UnityEditor.MeshUtility.Optimize(m);
        }


        public static bool IsRunningAndMeshNotReadWriteable(Mesh m)
        {
            if (Application.isPlaying)
            {
                return !m.isReadable;
            }
            else
            {
                return false;
            }
        }

        static Vector2 _HALF_UV = new Vector2(.5f, .5f);
        public static Vector2[] GetMeshUV1s(Mesh m)
        {
            Vector2[] uv;
            Debug.Log("UV1 does not exist in Unity 5+");
            uv = m.uv;
            if (uv.Length == 0)
            {
                Debug.Log("Mesh " + m + " has no uv1s. Generating");
                Debug.LogWarning("Mesh " + m + " didn't have uv1s. Generating uv1s.");
                uv = new Vector2[m.vertexCount];
                for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
            }
            return uv;
        }

        public static Vector2[] GetMeshUV3orUV4(Mesh m, bool get3)
        {
            Vector2[] uvs;

            if (get3) uvs = m.uv3;
            else uvs = m.uv4;
            if (uvs.Length == 0)
            {
                Debug.Log("Mesh " + m + " has no uv" + (get3 ? "3" : "4") + ". Generating");
                uvs = new Vector2[m.vertexCount];
                for (int i = 0; i < uvs.Length; i++) { uvs[i] = _HALF_UV; }
            }
            return uvs;
        }

        public static void MeshClear(Mesh m, bool t)
        {
            m.Clear(t);
        }

        public static void MeshAssignUV3(Mesh m, Vector2[] uv3s)
        {
            m.uv3 = uv3s;
        }

        public static void MeshAssignUV4(Mesh m, Vector2[] uv4s)
        {
            m.uv4 = uv4s;
        }

        /// <summary>
        /// 最大网格顶点数
        /// </summary>
        /// <returns></returns>
        public static int MaxMeshVertexCount()
        {
            return 2147483646;
        }

        public static void SetMeshIndexFormatAndClearMesh(Mesh m, int numVerts, bool vertices, bool justClearTriangles)
        {
#if UNITY_2017_3_OR_NEWER
            if (vertices && numVerts > 65534 && m.indexFormat == UnityEngine.Rendering.IndexFormat.UInt16)
            {
                MeshClear(m, false);
                m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                return;
            }
            else if (vertices && numVerts <= 65534 && m.indexFormat == UnityEngine.Rendering.IndexFormat.UInt32)
            {
                MeshClear(m, false);
                m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
                return;
            }
#endif
            if (justClearTriangles)
            {
                //仅清空三角形
                MeshClear(m, true);
            }
            else
            {//clear all the data and start with a blank mesh
                MeshClear(m, false);
            }
        }

        public static bool GraphicsUVStartsAtTop()
        {
#if UNITY_2017_1_OR_NEWER
            return SystemInfo.graphicsUVStartsAtTop;
#else
            if (SystemInfo.graphicsDeviceVersion.Contains("metal"))
            {
                return false;
            }
            else
            {
                // "opengl es, direct3d"
                return true;
            }
#endif
        }

        #region LightMap

        public static Vector4 GetLightmapTilingOffset(Renderer r)
        {
            return r.lightmapScaleOffset; //r.lightmapScaleOffset ;
        }

        #endregion

        #endregion

        #region BlendShape

        public static int GetBlendShapeFrameCount(Mesh m, int shapeIndex)
        {
            return m.GetBlendShapeFrameCount(shapeIndex);

        }

        public static float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex)
        {
            return m.GetBlendShapeFrameWeight(shapeIndex, frameIndex);
        }

        public static void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            m.GetBlendShapeFrameVertices(shapeIndex, frameIndex, vs, ns, ts);
        }

        public static void ClearBlendShapes(Mesh m)
        {
            m.ClearBlendShapes();
        }

        public static void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            m.AddBlendShapeFrame(nm, wt, vs, ns, ts);
        }

        #endregion

        #region Bone
        public static Transform[] GetBones(Renderer r)
        {
            if (r is SkinnedMeshRenderer)
            {
                Transform[] bone = ((SkinnedMeshRenderer)r).bones;
#if UNITY_EDITOR
                if (bone.Length == 0)
                {
                    Mesh m = ((SkinnedMeshRenderer)r).sharedMesh;
                    if (m.bindposes.Length != bone.Length) Debug.LogError("SkinnedMesh (" + r.gameObject + ") in the list of objects to combine has no bones. Check that 'optimize game object' is not checked in the 'Rig' tab of the asset importer. Mesh Baker cannot combine optimized skinned meshes because the bones are not available.");
                }
#endif
                return bone;
            }
            else if (r is MeshRenderer)
            {
                Transform[] bone = new Transform[1];
                bone[0] = r.transform;
                return bone;
            }
            else
            {
                Debug.LogError("Could not getBones. Object does not have a renderer");
                return null;
            }
        }

        #endregion

        #endregion


        #region UnityVersion

        public static int GetMajorVersion()
        {
            string v = Application.unityVersion;
            string[] vs = v.Split(new char[] { '.' });
            return int.Parse(vs[0]);
        }

        public static int GetMinorVersion()
        {
            string v = Application.unityVersion;
            string[] vs = v.Split(new char[] { '.' });
            return int.Parse(vs[1]);
        }

        #endregion

        #region GameObject

        public static bool GetActive(GameObject go)
        {
            return go.activeInHierarchy;
        }

        public static void SetActive(GameObject go, bool isActive)
        {
            go.SetActive(isActive);
        }

        public static void SetActiveRecursively(GameObject go, bool isActive)
        {
            go.SetActive(isActive);
        }

        public static UnityEngine.Object[] FindSceneObjectsOfType(Type t)
        {
            return GameObject.FindObjectsOfType(t);
        }

        #endregion
    }
}
