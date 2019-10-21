using UnityEngine;

namespace GameWorld
{
    public class MeshCombineData
    {
        [SerializeField]
        public Vector3[] verts;

        [SerializeField]
        public Vector3[] normals;

        [SerializeField]
        public Vector4[] tangents;

        [SerializeField]
        public Vector2[] uvs;

        [SerializeField]
        public Vector2[] uv2s;

        [SerializeField]
        public Vector2[] uv3s;

        [SerializeField]
        public Vector2[] uv4s;

        [SerializeField]
        public Color[] colors;

        [SerializeField]
        public Matrix4x4[] bindPoses;

        [SerializeField]
        public Transform[] bones;

        //unity won't serialize these
        public BoneWeight[] boneWeights;

        public void Init()
        {
            verts = new Vector3[0];
            normals = new Vector3[0];
            tangents = new Vector4[0];
            uvs = new Vector2[0];
            uv2s = new Vector2[0];
            uv3s = new Vector2[0];
            uv4s = new Vector2[0];
            colors = new Color[0];
            bones = new Transform[0];
            bindPoses = new Matrix4x4[0];
            boneWeights = new BoneWeight[0];
        }
    }

}