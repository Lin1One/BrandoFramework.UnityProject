#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
 
namespace Anonym.Util
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MeshUp))]
    class MeshUpEditor : Editor
    {
        MeshUp obj;
 
        void OnSceneGUI()
        {
            obj = (MeshUp)target;
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
 
            if (GUILayout.Button("XY_Quad"))
            {
                if (obj)
                {
                    obj.gameObject.GetComponent<MeshFilter>().mesh = MeshUp.CreateMeshXY(0.5f, 0.5f);
                }
            }
            if (GUILayout.Button("YZ_Quad"))
            {
                if (obj)
                {
                    obj.gameObject.GetComponent<MeshFilter>().mesh = MeshUp.CreateMeshYZ(0.5f, 0.5f);
                }
            }
            if (GUILayout.Button("XZ_Quad"))
            {
                if (obj)
                {
                    obj.gameObject.GetComponent<MeshFilter>().mesh = MeshUp.CreateMeshXZ(0.5f, 0.5f);
                }
            }
            if (GUILayout.Button("Union_Quad"))
            {
                if (obj)
                {
                    // obj.gameObject.GetComponent<MeshFilter>().mesh = MeshUp.CreateMeshYZ(0.5f, 0.5f);
                }
            }
 
 
            if (GUILayout.Button("To Prefab"))
            {
                if (obj)
                {
                   obj.Prefab();
                }
            }
        }
    }
#endif
}