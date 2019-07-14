using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    public interface IOverrideSortingOrder
    {
        int sortingOrder
        {
            get;
            set;
        }
    }
}