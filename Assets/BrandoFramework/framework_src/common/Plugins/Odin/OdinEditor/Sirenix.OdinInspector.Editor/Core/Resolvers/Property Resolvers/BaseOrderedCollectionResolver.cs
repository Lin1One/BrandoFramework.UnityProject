#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="BaseOrderedCollectionResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="BaseOrderedCollectionResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using System;
    using UnityEngine.Assertions;

    public abstract class BaseOrderedCollectionResolver<TCollection> : BaseCollectionResolver<TCollection>, IOrderedCollectionResolver
    {
        public void QueueInsertAt(int index, object[] values)
        {
            Assert.IsNotNull(values);
            Assert.AreEqual(values.Length, this.Property.Tree.WeakTargets.Count);

            for (int i = 0; i < values.Length; i++)
            {
                int capture = i;
                this.EnqueueChange(() => this.InsertAt((TCollection)this.Property.BaseValueEntry.WeakValues[capture], index, values[capture]));
            }
        }

        public void QueueRemoveAt(int index)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            var count = this.Property.Tree.WeakTargets.Count;

            for (int i = 0; i < count; i++)
            {
                int capture = i;
                this.EnqueueChange(() => this.RemoveAt((TCollection)this.Property.BaseValueEntry.WeakValues[capture], index));
            }
        }

        protected abstract void InsertAt(TCollection collection, int index, object value);

        protected abstract void RemoveAt(TCollection collection, int index);
    }
}
#endif