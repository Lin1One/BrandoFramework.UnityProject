#region Head

// Author:            LinYuzhou
// Email:             

#endregion

using System;

namespace Study.DotNet.System.Collections.Generic
{
    public interface IEnumerator<out T> : IDisposable, IEnumerator
    {
        new T Current
        {
            get;
        }
    }
}

