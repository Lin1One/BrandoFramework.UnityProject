using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    public interface ITag
    {
        List<string> GetTags();
        bool Tag(List<string> _tags);
        bool Tag(string _tag);
        void AddTags(string _tag);
        void RemoveTags(string _tag);
        void ClearTags();
    }
}
