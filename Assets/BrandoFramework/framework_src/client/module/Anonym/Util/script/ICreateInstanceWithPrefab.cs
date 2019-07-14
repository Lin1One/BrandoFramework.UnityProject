using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    public interface ICreateInstanceWithPrefab<T>
    {
        string DefaultPrefabPath { get; }
        T CreateInstanceWithPrefab();
    }
}
