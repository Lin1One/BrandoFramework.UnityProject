using UnityEngine;

namespace GameWorld
{
    public class BlendShape2CombinedMap : MonoBehaviour
    {
        public SerializableSourceBlendShape2Combined srcToCombinedMap;

        public SerializableSourceBlendShape2Combined GetMap()
        {
            if (srcToCombinedMap == null)
            {
                srcToCombinedMap = new SerializableSourceBlendShape2Combined();
            }

            return srcToCombinedMap;
        }
    }
}
