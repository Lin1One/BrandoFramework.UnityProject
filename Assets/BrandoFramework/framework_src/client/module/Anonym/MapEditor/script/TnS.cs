using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    public interface IFloatValue
    {
        float FloatValue { get; set; }
    }
    [System.Serializable]
    public class TnS : IGameObject
    {
        public Transform _Transform;
        public SpriteRenderer _Sprr;
        
        public GameObject gameObject { get { return _Transform != null ? _Transform.gameObject : null; } }

        public void Init(GameObject _obj)
        {
            _Transform = _obj.transform;
            _Sprr = _obj.GetComponent<SpriteRenderer>();
        }
    }

    [System.Serializable]
    public class Attachment_TnS : Attachment<TnS>, IFloatValue
    {
        public float fDepthFudge = 0;
        public float FloatValue
        {
            get { return fDepthFudge; }
            set { fDepthFudge = value; }
        }

        override public bool Init(GameObject _obj, int _indentLevel)
        {
            base.Init(_obj, _indentLevel);
            AttachedObj.Init(_obj);
            fDepthFudge = 0;
            return AttachedObj != null;
        }
    }

    [System.Serializable]
    public class IsoHierarchy_TnS : AttachmentHierarchy<Attachment_TnS> { }
}