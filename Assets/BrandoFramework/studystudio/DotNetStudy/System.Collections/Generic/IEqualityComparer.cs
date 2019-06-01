#region Head

// Author:            LinYuzhou
// Email:             

#endregion

namespace Study.DotNet.System.Collections.Generic
{
    public interface IEqualityComparer<in T>
    {
        bool Equals(T x, T y);
        int GetHashCode(T obj);
    }
}

