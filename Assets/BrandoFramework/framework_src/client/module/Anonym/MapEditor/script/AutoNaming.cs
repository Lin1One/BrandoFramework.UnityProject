using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class AutoNaming : MonoBehaviour
    {
#if UNITY_EDITOR
        private static StringBuilder _sb = new StringBuilder();

        [HideInInspector]
        GridCoordinates _coordinates;
        [HideInInspector]
        public GridCoordinates coordinates
        {
            get
            {
                return _coordinates == null ?
                    _coordinates = GetComponent<GridCoordinates>() : _coordinates;
            }
        }

		[HideInInspector]
        SpriteRenderer[] _sprrs = null;
        [HideInInspector]
        public SpriteRenderer[] sprrs
        {
            get
            {
                return _sprrs == null || _sprrs.Length == 0 ?
                    _sprrs = GetComponentsInChildren<SpriteRenderer>() : _sprrs;
            }
        }

        void OnTransformChildrenChanged()
        {
            _sprrs = null;
        }

        [SerializeField]
        string format = "T_F{1:00}_({0:00},{2:00})";

        [SerializeField]
        bool bAutoName = true;
        [SerializeField]
        bool bAutoNamed;
		[SerializeField]
		public bool bPostfix_Sprite = true;
        void Start()
        {
            naming();
        }
        public void AutoName()
        {
            bAutoNamed = false;
            naming();
        }
        void naming()
        {
            if (bAutoName && !bAutoNamed && coordinates != null && transform != transform.root)
            {
                Vector3 xyz = coordinates._xyz;
				name = string.Format(format, xyz.x, xyz.y, xyz.z);
                if (bPostfix_Sprite)
				{
                    _sb.Length = 0;
                    if (sprrs == null || sprrs.Length == 0)
                    {
                        _sb.Append('_');
                        _sb.Append("[Empty]");
                    }
                    else
                    {
                        for(int i = 0 ; i < sprrs.Length ; i++)
                        {
                            _sb.Append('_');
                            if (sprrs[i] != null && sprrs[i].sprite != null)
                                _sb.Append(sprrs[i].sprite.name);
                            else
                                _sb.Append("[null]");
                        }
                    }
					name =  string.Format("{0}_{1}", name, _sb.ToString());
				}
                bAutoNamed = true;
            }
        }
#endif
    }
}