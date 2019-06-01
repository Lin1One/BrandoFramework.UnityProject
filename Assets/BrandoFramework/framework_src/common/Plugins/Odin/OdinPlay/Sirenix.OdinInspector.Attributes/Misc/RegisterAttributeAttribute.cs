//-----------------------------------------------------------------------// <copyright file="RegisterAttributeAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class RegisterAttributeAttribute : Attribute // TODO: Rename this.
    {
        public Type AttributeType;
        public string[] Categories;

        public RegisterAttributeAttribute(Type attributeType, params string[] categories)
        {
            this.AttributeType = attributeType;
            this.Categories = categories;
        }
    }
}