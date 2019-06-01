#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="IKeyValueMapResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="IKeyValueMapResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    public interface IKeyValueMapResolver : ICollectionResolver
    {
        object GetKey(int selectionIndex, int childIndex);

        void QueueSet(object[] keys, object[] values);

        void QueueRemoveKey(object[] keys);
    }
}
#endif