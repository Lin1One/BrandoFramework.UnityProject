#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Study.DotNet.System.Collections.Generic
{
    public class Stack<T> : IEnumerable<T>, ICollection
    {
        private T[] _array;     // Storage for stack elements
        private int _size;           // Number of items in the stack.
        private int _version;        // Used to keep enumerator in sync w/ collection.

        [NonSerialized]
        private Object _syncRoot;

        private const int _defaultCapacity = 4;
        static T[] _emptyArray = new T[0];

        #region 构造函数

        public Stack()
        {
            _array = _emptyArray;
            _size = 0;
            _version = 0;
        }

        public Stack(int capacity)
        {
            if (capacity < 0)
                //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired);
                _array = new T[capacity];
            _size = 0;
            _version = 0;
        }

        public Stack(IEnumerable<T> collection)
        {
            //if (collection == null)
            //ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            ICollection<T> c = collection as ICollection<T>;
            if (c != null)
            {
                int count = c.Count;
                _array = new T[count];
                c.CopyTo(_array, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                _array = new T[_defaultCapacity];

                using (IEnumerator<T> en = collection.GetEnumerator())
                {
                    while (en.MoveNext())
                    {
                        Push(en.Current);
                    }
                }
            }
        }
        #endregion

        #region ICollection 接口

        public int Count
        {
            get { return _size; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    //System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
                return _syncRoot;
            }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
            {
                //ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if (array.Rank != 1)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }

            if (array.GetLowerBound(0) != 0)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (array.Length - arrayIndex < _size)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            try
            {
                Array.Copy(_array, 0, array, arrayIndex, _size);
                Array.Reverse(array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
            }
        }

        #endregion

        #region IEnumerable 接口

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            int count = _size;

            //比较器
            EqualityComparer<T> c = EqualityComparer<T>.Default;
            while (count-- > 0)
            {
                if (((object)item) == null)
                {
                    if (((object)_array[count]) == null)
                        return true;
                }
                else if (_array[count] != null && c.Equals(_array[count], item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                //ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                //ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }

            if (array.Length - arrayIndex < _size)
            {
                //ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }

            Array.Copy(_array, 0, array, arrayIndex, _size);
            Array.Reverse(array, arrayIndex, _size);
        }

        public void TrimExcess()
        {
            int threshold = (int)(((double)_array.Length) * 0.9);
            if (_size < threshold)
            {
                T[] newarray = new T[_size];
                Array.Copy(_array, 0, newarray, 0, _size);
                _array = newarray;
                _version++;
            }
        }

        public T Peek()
        {
            //if (_size == 0)
            //ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
            return _array[_size - 1];
        }

        public T Pop()
        {
            //if (_size == 0)
            // ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
            _version++;
            T item = _array[--_size];
            _array[_size] = default(T);     // Free memory quicker.
            return item;
        }

        public void Push(T item)
        {
            if (_size == _array.Length)
            {
                T[] newArray = new T[(_array.Length == 0) ? _defaultCapacity : 2 * _array.Length];
                Array.Copy(_array, 0, newArray, 0, _size);
                _array = newArray;
            }
            _array[_size++] = item;
            _version++;
        }

        public T[] ToArray()
        {
            T[] objArray = new T[_size];
            int i = 0;
            while (i < _size)
            {
                objArray[i] = _array[_size - i - 1];
                i++;
            }
            return objArray;
        }


        #region 泛型栈迭代器
        [Serializable()]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private Stack<T> _stack;
            private int _index;
            private int _version;
            private T currentElement;

            internal Enumerator(Stack<T> stack)
            {
                _stack = stack;
                _version = _stack._version;
                _index = -2;
                currentElement = default(T);
            }

            public void Dispose()
            {
                _index = -1;
            }

            public bool MoveNext()
            {
                bool retval;
                //if (_version != _stack._version)
                //        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);

                if (_index == -2)
                {  // First call to enumerator.
                    _index = _stack._size - 1;
                    retval = (_index >= 0);
                    if (retval)
                    {
                        currentElement = _stack._array[_index];
                    }
                    return retval;
                }
                if (_index == -1)
                {  // End of enumeration.
                    return false;
                }
                retval = (--_index >= 0);
                if (retval)
                {
                    currentElement = _stack._array[_index];
                }
                else
                    currentElement = default(T);
                return retval;
            }

            /// <include file='doc\Stack.uex' path='docs/doc[@for="StackEnumerator.Current"]/*' />
            public T Current
            {
                get
                {
                    //if (_index == -2) ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
                    //if (_index == -1) ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
                    return currentElement;
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    //if (_index == -2) ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
                    //if (_index == -1) ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
                    return currentElement;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                //if (_version != _stack._version)
                //        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
                _index = -2;
                currentElement = default(T);
            }
        }

        #endregion
    }
}

