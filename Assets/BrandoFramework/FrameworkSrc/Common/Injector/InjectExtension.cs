
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common
{
    public static class InjectExtension
    {
        public static bool HasInjectAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes(typeof(InjectAttribute), true).Any();
        }

        public static bool IsSingle(this Type type)
        {
            return type.GetCustomAttributes(typeof(SingletonAttribute), true).Any();
        }

        public static Type GetAbsTypeDefaultMapImplType(this Type type)
        {
            var defaultAttributes = type.GetCustomAttributes(typeof(DefaultInjecTypeAttribute), true);
            if (defaultAttributes.Length > 0)
                return defaultAttributes[0].As<DefaultInjecTypeAttribute>().SourceType;
            return null;


        }
    }

}