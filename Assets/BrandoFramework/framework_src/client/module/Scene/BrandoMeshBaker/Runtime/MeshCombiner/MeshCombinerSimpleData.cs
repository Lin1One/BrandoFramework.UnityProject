using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// Manages a single combined mesh.This class is the core of the mesh combining API.
    /// 
    /// It is not a component so it can be can be instantiated and used like a normal c sharp class.
    /// </summary>
    public partial class MeshCombineHandler : MeshCombineHandlerBase
    {
        public enum MeshCreationConditions
        {
            NoMesh,
            CreatedInEditor,
            CreatedAtRuntime,
            AssignedByUser,
        }

        //Used for comparing if skinned meshes use the same bone and bindpose.
        //Skinned meshes must be bound with the same TRS to share a bone.
        public struct BoneAndBindpose
        {
            public Transform bone;
            public Matrix4x4 bindPose;

            public BoneAndBindpose(Transform t, Matrix4x4 bp)
            {
                bone = t;
                bindPose = bp;
            }

            public override bool Equals(object obj)
            {
                if (obj is BoneAndBindpose)
                {
                    if (bone == ((BoneAndBindpose)obj).bone && bindPose == ((BoneAndBindpose)obj).bindPose)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                //OK if don't check bindPose well because bp should be the same
                return (bone.GetInstanceID() % 2147483647) ^ (int)bindPose[0, 0];
            }
        }
    }
}