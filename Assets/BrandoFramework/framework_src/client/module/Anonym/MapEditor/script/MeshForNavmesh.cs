using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// based http://luigigarcia.byethost7.com/2015/08/16/navmesh-helper-baking-scene-based-on-colliders/?i=1

#if UNITY_EDITOR
namespace Anonym.Isometric
{
    using Util;
    public static class MeshForNavmesh
    {
        static private Material diffuseDefaultMaterial = null;

        public static GameObject Bake(Collider theCollider)
        {
            GameObject fakeObject = null;
            string DEFAULT_FAKEOBJECT_NAME = theCollider.gameObject.name;

            if (theCollider is BoxCollider)
            {
                #region In case col is a box
                BoxCollider baseCollider = theCollider as BoxCollider;
                fakeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject.DestroyImmediate(fakeObject.GetComponent<Collider>());
                fakeObject.name = DEFAULT_FAKEOBJECT_NAME;
                fakeObject.transform.rotation = theCollider.gameObject.transform.rotation;
                fakeObject.transform.parent = theCollider.gameObject.transform;
                fakeObject.transform.localPosition = baseCollider.center;
                fakeObject.transform.parent = null;
                fakeObject.transform.localScale = theCollider.gameObject.transform.lossyScale;
                Vector3 tempScale = fakeObject.transform.localScale;
                tempScale.x *= baseCollider.size.x;
                tempScale.y *= baseCollider.size.y;
                tempScale.z *= baseCollider.size.z;
                fakeObject.transform.localScale = tempScale;
                #endregion
            }
            else if (theCollider is CapsuleCollider)
            {
                #region In case col is a capsule
                CapsuleCollider baseCollider = theCollider as CapsuleCollider;
                fakeObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                GameObject.DestroyImmediate(fakeObject.GetComponent<Collider>());
                fakeObject.name = DEFAULT_FAKEOBJECT_NAME;
                fakeObject.transform.rotation = theCollider.gameObject.transform.rotation;
                fakeObject.transform.parent = theCollider.gameObject.transform;
                fakeObject.transform.localPosition = baseCollider.center;
                fakeObject.transform.parent = null;
                fakeObject.transform.localScale = theCollider.gameObject.transform.lossyScale;
                const float DEFAULT_CAPSULE_RADIUS = 0.5f;
                const float DEFAULT_CAPSULE_HEIGHT = 2.0f;
                Vector3 tempScale = fakeObject.transform.localScale;

                // max(x,z) code
                if (Mathf.Abs(tempScale.x) > Mathf.Abs(tempScale.z))
                {
                    tempScale.z = tempScale.x;
                }
                else
                {
                    tempScale.x = tempScale.z;
                }

                tempScale.x *= baseCollider.radius / DEFAULT_CAPSULE_RADIUS;
                tempScale.z *= baseCollider.radius / DEFAULT_CAPSULE_RADIUS;
                tempScale.y *= baseCollider.height / DEFAULT_CAPSULE_HEIGHT;
                fakeObject.transform.localScale = tempScale;
                #endregion
            }
            else if (theCollider is SphereCollider)
            {
                #region In case col is a sphere
                SphereCollider baseCollider = theCollider as SphereCollider;
                fakeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject.DestroyImmediate(fakeObject.GetComponent<Collider>());
                fakeObject.name = DEFAULT_FAKEOBJECT_NAME;
                fakeObject.transform.rotation = theCollider.gameObject.transform.rotation;
                fakeObject.transform.parent = theCollider.gameObject.transform;
                fakeObject.transform.localPosition = baseCollider.center;
                fakeObject.transform.parent = null;
                fakeObject.transform.localScale = theCollider.gameObject.transform.lossyScale;
                const float DEFAULT_SPHERE_RADIUS = 0.5f;
                Vector3 tempScale = fakeObject.transform.localScale;

                // max(x,y,z) code
                if (Mathf.Abs(tempScale.x) > Mathf.Abs(tempScale.y))
                {
                    tempScale.y = tempScale.x;
                }
                else
                {
                    tempScale.x = tempScale.y;
                }
                if (Mathf.Abs(tempScale.x) > Mathf.Abs(tempScale.z))
                {
                    tempScale.z = tempScale.y = tempScale.x;
                }
                else
                {
                    tempScale.x = tempScale.y = tempScale.z;
                }

                tempScale.x *= baseCollider.radius / DEFAULT_SPHERE_RADIUS;
                tempScale.y *= baseCollider.radius / DEFAULT_SPHERE_RADIUS;
                tempScale.z *= baseCollider.radius / DEFAULT_SPHERE_RADIUS;
                fakeObject.transform.localScale = tempScale;
                #endregion
            }
            else if (theCollider is MeshCollider)
            {
                #region In case col is a mesh
                MeshCollider baseCollider = theCollider as MeshCollider;
                // to generate the MeshCollider object, the original MUST HAVE a MeshRenderer
                if (baseCollider.GetComponent<MeshRenderer>() != null)
                {
                    int materialsCount = baseCollider.GetComponent<MeshRenderer>().sharedMaterials.Length;
                    fakeObject = new GameObject(DEFAULT_FAKEOBJECT_NAME);
                    fakeObject.transform.position = theCollider.gameObject.transform.position;
                    fakeObject.transform.rotation = theCollider.gameObject.transform.rotation;
                    fakeObject.AddComponent<MeshFilter>().sharedMesh = baseCollider.sharedMesh;
                    if (diffuseDefaultMaterial == null)
                    {
                        SetDefaultMaterialReference();
                    }
                    Material[] mats = new Material[materialsCount];
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = diffuseDefaultMaterial;
                    }
                    fakeObject.AddComponent<MeshRenderer>().materials = mats;
                    fakeObject.transform.localScale = theCollider.gameObject.transform.lossyScale;
                }
                #endregion
            }

            if (fakeObject != null)
            {
                var mr = fakeObject.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    mr.receiveShadows = false;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    mr.allowOcclusionWhenDynamic = false;
                    mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    mr.lightmapIndex = -1;
                    mr.realtimeLightmapIndex = -1;
                    // mr.rendererPriority = int.MaxValue;
                    mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

                    UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(mr);
                    so.FindProperty("m_ScaleInLightmap").floatValue = 0;
                    so.ApplyModifiedProperties();


                    //if (UnityEditor.GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, UnityEditor.StaticEditorFlags.LightmapStatic))
                    //{
                    //    Debug.Log("Static Lightmap!");
                    //}
                }
            }
            return fakeObject;
        }

        static private void SetDefaultMaterialReference()
        {
            GameObject tempPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            diffuseDefaultMaterial = tempPrimitive.GetComponent<MeshRenderer>().sharedMaterial;
            GameObject.DestroyImmediate(tempPrimitive);
        }
    }
}
#endif