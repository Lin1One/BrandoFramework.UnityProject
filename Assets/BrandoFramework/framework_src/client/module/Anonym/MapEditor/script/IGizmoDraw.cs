using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    public interface IGizmoDraw
    {
#if UNITY_EDITOR
        void GizmoDraw();
#endif
    }
}