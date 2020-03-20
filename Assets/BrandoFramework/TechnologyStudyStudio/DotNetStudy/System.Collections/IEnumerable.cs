#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.DotNet.System.Collections
{
    public interface IEnumerable 
    {
        // Returns an IEnumerator for this enumerable Object.  The enumerator provides
        // a simple way to access all the contents of a collection.
        IEnumerator GetEnumerator();
    }
}

