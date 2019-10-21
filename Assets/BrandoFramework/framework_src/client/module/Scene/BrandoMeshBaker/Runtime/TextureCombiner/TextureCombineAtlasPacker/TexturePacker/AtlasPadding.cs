using System;

namespace GameWorld
{

    /// <summary>
    /// Atlas 填充
    /// </summary>
    [Serializable]
    public struct AtlasPadding
    {
        public int topBottom;
        public int leftRight;

        public AtlasPadding(int p)
        {
            topBottom = p;
            leftRight = p;
        }

        public AtlasPadding(int px, int py)
        {
            topBottom = py;
            leftRight = px;
        }
    }

    
}
