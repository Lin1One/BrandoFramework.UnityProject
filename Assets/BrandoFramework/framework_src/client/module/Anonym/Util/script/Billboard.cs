using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        float _fOffsetFromCam = 0F;
        void OnEnable()
        {
            transform.Translate(Vector3.forward * _fOffsetFromCam, Camera.main.transform);
            Camera.onPreRender += PreRender;
        }
        void OnDisable() { Camera.onPreRender -= PreRender; }
        void PreRender(Camera cam)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                              Camera.main.transform.rotation * Vector3.up);
        }
    }
}