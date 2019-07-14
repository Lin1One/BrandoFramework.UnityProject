using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
 
 namespace Anonym.Util
 {
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
	public class MeshUp : MonoBehaviour {
#if UNITY_EDITOR
        public void Prefab()
        {
            ToPrefab(GetComponent<MeshFilter>().mesh);
        }
 
		public static Mesh CreateMeshXY(float width, float height)
		{
			Mesh m = new Mesh();
			m.name = "ScriptedMesh";
			m.vertices = new Vector3[] {
				new Vector3(-width, -height, 0.0f),
				new Vector3(width, -height, 0.0f),
				new Vector3(width, height, 0.0f),
				new Vector3(-width, height, 0.0f)
			};
			m.uv = new Vector2[] {
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2(1, 1),
				new Vector2 (1, 0)
			};
			m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
			m.RecalculateNormals();
				
			return m;
		}
        public static Mesh CreateMeshYZ(float width, float height)
		{
			Mesh m = new Mesh();
			m.name = "ScriptedMesh";
			m.vertices = new Vector3[] {
				new Vector3(0.0f, -width, -height ),
				new Vector3(0.0f, width, -height),
				new Vector3(0.0f, width, height),
				new Vector3(0.0f, -width, height)
			};
			m.uv = new Vector2[] {
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2(1, 1),
				new Vector2 (1, 0)
			};
			m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
			m.RecalculateNormals();
				
			return m;
		}
        public static Mesh CreateMeshXZ(float width, float height)
		{
			Mesh m = new Mesh();
			m.name = "ScriptedMesh";
			m.vertices = new Vector3[] {
				new Vector3( -width, 0.0f, -height ),
				new Vector3( width, 0.0f, -height),
				new Vector3( width, 0.0f, height),
				new Vector3( -width, 0.0f, height)
			};
			m.uv = new Vector2[] {
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2(1, 1),
				new Vector2 (1, 0)
			};
			m.triangles = new int[] { 0, 3, 2, 0, 2, 1};
			m.RecalculateNormals();
				
			return m;
		}

        public static void ToPrefab(Mesh mesh)
        {
            string path = "Assets/Anonym/Isometric/prefab/MeshUp/";
            AssetDatabase.CreateAsset(mesh, path + mesh.name + ".asset");    
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
	}
}