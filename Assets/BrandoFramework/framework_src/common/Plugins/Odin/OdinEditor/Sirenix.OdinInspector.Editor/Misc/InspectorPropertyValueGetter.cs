#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InspectorPropertyValueGetter.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="StringMemberHelper.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using System.Reflection;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///	Helper class to get values from InspectorProperties.
    /// </summary>
    public class InspectorPropertyValueGetter<TReturnType>
    {
        private string errorMessage;
        private Func<TReturnType> staticValueGetter;
        private Func<object, TReturnType> instanceValueGetter;
        private InspectorProperty memberProperty;
        private MemberInfo memberInfo;

        /// <summary>
        /// If any error occurred while looking for members, it will be stored here.
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage; } }

        /// <summary>
        /// Gets the referenced member information.
        /// </summary>
        public MemberInfo MemberInfo { get { return this.memberInfo; } }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="allowInstanceMember">If <c>true</c>, then StringMemberHelper will look for instance members.</param>
        /// <param name="allowStaticMember">If <c>true</c>, then StringMemberHelper will look for static members.</param>
        /// <exception cref="System.InvalidOperationException">Require either allowInstanceMember or allowStaticMember to be true.</exception>
        public InspectorPropertyValueGetter(InspectorProperty property, string memberName, bool allowInstanceMember = true, bool allowStaticMember = true)
        {
            this.memberProperty = property.FindParent(x => x.Info.GetMemberInfo() != null, true);
            var parentType = this.memberProperty.ParentType;

            var finder = MemberFinder.Start(parentType)
                .HasReturnType<TReturnType>(true)
                .IsNamed(memberName)
                .HasNoParameters();

            if (!allowInstanceMember && !allowStaticMember)
            {
                throw new InvalidOperationException("Require either allowInstanceMember and/or allowStaticMember to be true.");
            }
            else if (!allowInstanceMember)
            {
                finder.IsStatic();
            }
            else if (!allowStaticMember)
            {
                finder.IsInstance();
            }

            if (finder.TryGetMember(out this.memberInfo, out this.errorMessage))
            {
                if (this.memberInfo is MethodInfo)
                {
                    memberName += "()";
                }

                if (this.memberInfo.IsStatic())
                {
                    this.staticValueGetter = DeepReflection.CreateValueGetter<TReturnType>(parentType, memberName);
                }
                else
                {
                    this.instanceValueGetter = DeepReflection.CreateWeakInstanceValueGetter<TReturnType>(parentType, memberName);
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TReturnType GetValue()
        {
            if (this.staticValueGetter != null)
            {
                return this.staticValueGetter();
            }

            var instance = this.memberProperty.ParentValues[0];

            if (instance != null && this.instanceValueGetter != null)
            {
                return this.instanceValueGetter(instance);
            }

            return default(TReturnType);
        }

        /// <summary>
        /// Gets all values from all targets.
        /// </summary>
        public IEnumerable<TReturnType> GetValues()
        {
            if (this.staticValueGetter != null)
            {
                yield return this.staticValueGetter();
            }

            for (int i = 0; i < this.memberProperty.ParentValues.Count; i++)
            {
                var instance = this.memberProperty.ParentValues[i];
                if (instance != null && this.instanceValueGetter != null)
                {
                    yield return this.instanceValueGetter(instance);
                }
            }
        }
    }
}
#endif