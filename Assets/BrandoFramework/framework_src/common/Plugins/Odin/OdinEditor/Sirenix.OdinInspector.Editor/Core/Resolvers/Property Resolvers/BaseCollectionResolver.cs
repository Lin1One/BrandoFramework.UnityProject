#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="BaseCollectionResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="BaseCollectionResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public abstract class BaseCollectionResolver<TCollection> : OdinPropertyResolver<TCollection>, ICollectionResolver
    {
        private Queue<Action> changeQueue = new Queue<Action>();
        private bool isReadOnly;
        private int isReadOnlyLastUpdateID;

        protected virtual bool ApplyToRootSelectionTarget { get { return false; } }

        public abstract Type ElementType { get; }

        public bool IsReadOnly
        {
            get
            {
                if (this.isReadOnlyLastUpdateID != this.Property.Tree.UpdateID)
                {
                    this.isReadOnlyLastUpdateID = this.Property.Tree.UpdateID;
                    this.isReadOnly = true;

                    var entry = this.Property.ValueEntry;

                    for (int i = 0; i < entry.ValueCount; i++)
                    {
                        try
                        {
                            var collection = (TCollection)entry.WeakValues[i];

                            if (object.ReferenceEquals(collection, null))
                            {
                                // Default to false in this case for consistency's sake
                                this.isReadOnly = false;
                                break;
                            }

                            if (!this.CollectionIsReadOnly(collection))
                            {
                                this.isReadOnly = false;
                                break;
                            }
                        }
                        // This defaults to a "false" assumption, because... *sigh*... because people are stupid sometimes.
                        catch (NotImplementedException) { this.isReadOnly = false; }
                    }
                }

                return this.isReadOnly;
            }
        }

        public int MaxCollectionLength { get { return this.MaxChildCountSeen; } }

        public override bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            if (!this.ApplyToRootSelectionTarget && property == property.Tree.SecretRootProperty) return false;
            return true;
        }

        public bool ApplyChanges()
        {
            bool hasChanges = changeQueue.Count > 0;

            while (changeQueue.Count > 0)
            {
                changeQueue.Dequeue()();
            }

            if (hasChanges)
            {
                this.OnCollectionChangesApplied();

                if (this.Property.SupportsPrefabModifications)
                {
                    this.Property.Update(true);

                    foreach (var child in this.Property.Children.Recurse())
                    {
                        child.Update(true);
                    }
                }

                this.Property.Children.ClearCaches();
            }

            return hasChanges;
        }

        public bool CheckHasLengthConflict()
        {
            //return base.HasChildCountConflict;

            // TODO: @Tor I have inserted this code here, based on OdinPropertyResolver<TValue>.CalculateChildCount,
            // because the state of base.HasChildCountConflict was not ready by the time PropertyValueEntry.GetValueState
            // used it to determine if there is a mismatch in collection length.
            //
            // This caused a layout error in the CollectionDrawer because the ValueState was set to None in the layout event, 
            // and correctly set to CollectionLengthConflict in the following repaint event.
            var entry = this.Property.ValueEntry;
            if (entry.ValueCount == 0)
            {
                return false;
            }

            int prevCount = this.GetChildCount(ValueEntry.Values[0]);

            for (int i = 1; i < this.ValueEntry.ValueCount; i++)
            {
                if (prevCount != this.GetChildCount(ValueEntry.Values[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public abstract bool ChildPropertyRequiresRefresh(int index, InspectorPropertyInfo info);

        public void EnqueueChange(Action action)
        {
            this.changeQueue.Enqueue(action);

            this.Property.Tree.DelayActionUntilRepaint(() =>
            {
                this.Property.Tree.RegisterPropertyDirty(this.Property);
            });
        }

        public void QueueAdd(object[] values)
        {
            Assert.IsNotNull(values);
            Assert.AreEqual(values.Length, this.Property.Tree.WeakTargets.Count);

            for (int i = 0; i < values.Length; i++)
            {
                int capture = i;
                this.EnqueueChange(() => this.Add((TCollection)this.Property.BaseValueEntry.WeakValues[capture], values[capture]));
            }
        }

        public void QueueClear()
        {
            var count = this.Property.BaseValueEntry.WeakValues.Count;

            for (int i = 0; i < count; i++)
            {
                int capture = i;
                this.EnqueueChange(() => this.Clear((TCollection)this.Property.BaseValueEntry.WeakValues[capture]));
            }
        }

        public void QueueRemove(object[] values)
        {
            Assert.IsNotNull(values);
            Assert.AreEqual(values.Length, this.Property.Tree.WeakTargets.Count);

            for (int i = 0; i < values.Length; i++)
            {
                int capture = i;
                this.EnqueueChange(() => this.Remove((TCollection)this.Property.BaseValueEntry.WeakValues[capture], values[capture]));
            }
        }

        protected abstract void Add(TCollection collection, object value);

        protected abstract void Clear(TCollection collection);

        protected abstract bool CollectionIsReadOnly(TCollection collection);

        protected virtual void OnCollectionChangesApplied()
        {
        }

        protected abstract void Remove(TCollection collection, object value);
    }
}
#endif