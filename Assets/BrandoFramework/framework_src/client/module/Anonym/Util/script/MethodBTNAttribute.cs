using System;
using UnityEngine;

namespace Anonym.Util
{
    public abstract class MethodBTN_MonoBehaviour : MonoBehaviour
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class MethodBTNAttribute : Attribute
        {
            public bool bPlayModeOnly = true;
            public object[] args;
            public MethodBTNAttribute(bool _bPlayModeOnly = true, params object[] arguments)
            {
                bPlayModeOnly = _bPlayModeOnly;
                args = arguments;
            }
        }
    }
}