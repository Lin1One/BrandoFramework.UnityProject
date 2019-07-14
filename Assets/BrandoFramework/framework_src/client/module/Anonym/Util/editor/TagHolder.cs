using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    [System.Serializable]
    public class TagHolder
    {
        [SerializeField]
        public string _name = "TagHolder";

        [SerializeField]
        public TagList _tagList;

        [SerializeField]
        public Tag _tag;

        public TagHolder()
        {
             
        }

        public TagHolder(string _TagName)
        {
            _name = _TagName;
        }

        public string GetTag()
        {
            if (_tag == null)
                return "Null(not set)";
            return _tag.tag;
        }

        public void Set(TagList __tagList, Tag __tag)
        {
            _tagList = __tagList;
            _tag = __tag;
        }

        public void Set(TagHolder _tagHolder)
        {
            if (_tagHolder != null)
                Set(_tagHolder._tagList, _tagHolder._tag);
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", _tagList == null ? "Null" : _tagList.name, _tag == null ? "Null" : _tag.tag);
        }
    }
}