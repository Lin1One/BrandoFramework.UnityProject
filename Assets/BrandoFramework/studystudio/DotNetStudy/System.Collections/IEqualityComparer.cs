#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using System;

namespace Study.DotNet.System.Collections
{
    public interface IEqualityComparer 
    {
        bool Equals(object x, object y);
        int GetHashCode(object obj);
    }
}

