using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    [Serializable]
    public class MBBlendShapeFrame
    {
        public float frameWeight;
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector3[] tangents;
    }

    [Serializable]
    public class BlendShape
    {
        public int gameObjectID;
        public GameObject gameObject;
        public string name;
        public int indexInSource;
        public MBBlendShapeFrame[] frames;
    }
}
