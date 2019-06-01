#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="IOrderedCollectionResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="IOrderedCollectionResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    public interface IOrderedCollectionResolver : ICollectionResolver
    {
        void QueueRemoveAt(int index);

        void QueueInsertAt(int index, object[] values);
    }
}
#endif