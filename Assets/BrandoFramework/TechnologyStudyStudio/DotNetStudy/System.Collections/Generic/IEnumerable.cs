#region Head

// Author:            LinYuzhou
// Email:             

#endregion



namespace Study.DotNet.System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
    {
        new IEnumerator<T> GetEnumerator();
    }
}

