using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
        //2D arrays are not serializable but arrays  of arrays are.
        [System.Serializable]
        public class SerializableIntArray
        {
            public int[] data;

            public SerializableIntArray() { }

            public SerializableIntArray(int len)
            {
                data = new int[len];
            }
        }

        
    
}