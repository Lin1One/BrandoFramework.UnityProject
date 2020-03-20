#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
using System.Diagnostics.Contracts;

namespace Study.DotNet.System.Collections
{
    // A simple stack of objects.  Internally it is implemented as an array,
    // so Push can be O(n).  Pop is O(1).
    public class Stack : ICollection
    {
        private object[] _array;     // Storage for stack elements
        private int _size;           // Number of items in the stack.
        private int _version;        // Used to keep enumerator in sync w/ collection.
        private const int _defaultCapacity = 10;

        [NonSerialized]
        private Object _syncRoot;

        #region 构造函数

        public Stack()
        {
            _array = new object[_defaultCapacity];
            _size = 0;
            _version = 0;
        }

        // Create a stack with a specific initial capacity.  The initial capacity
        // must be a non-negative number.
        public Stack(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                //throw new ArgumentOutOfRangeException("initialCapacity", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                return;
            }
            Contract.EndContractBlock();
            if (initialCapacity < _defaultCapacity)
            {
                initialCapacity = _defaultCapacity;  // Simplify doubling logic in Push.
            }

            _array = new object[initialCapacity];
            _size = 0;
            _version = 0;
        }

        // Fills a Stack with the contents of a particular collection.  The items are
        // pushed onto the stack in the same order they are read by the enumerator.
        //
        public Stack(ICollection col) : this((col == null ? 32 : col.Count))
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            Contract.EndContractBlock();
            IEnumerator en = col.GetEnumerator();
            while (en.MoveNext())
            {
                Push(en.Current);
            }
        }

        #endregion

        #region ICollection 接口

        public virtual int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return _size;
            }
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual Object SyncRoot
        {
            get
            {
                //if (_syncRoot == null)
                //{
                //    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                //}
                return _syncRoot;
            }
        }

        public virtual void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                //throw new ArgumentException(Environment.GetResourceString("Arg_RankMultiDimNotSupported"));
            if (index < 0)
                //throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            if (array.Length - index < _size)
                //throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            Contract.EndContractBlock();

            int i = 0;
            if (array is object[])
            {
                object[] objArray = (object[])array;
                while (i < _size)
                {
                    objArray[i + index] = _array[_size - i - 1];
                    i++;
                }
            }
            else
            {
                while (i < _size)
                {
                    array.SetValue(_array[_size - i - 1], i + index);
                    i++;
                }
            }
        }

        #endregion

        #region  IEnumerator 接口

        public virtual IEnumerator GetEnumerator()
        {
            Contract.Ensures(Contract.Result<IEnumerator>() != null);
            return new StackEnumerator(this);
        }

        #endregion

        #region ICloneable 接口
        public virtual Object Clone()
        {
            Contract.Ensures(Contract.Result<Object>() != null);

            Stack s = new Stack(_size);
            s._size = _size;
            Array.Copy(_array, 0, s._array, 0, _size);
            s._version = _version;
            return s;
        }

        #endregion


        public virtual void Clear()
        {
            // Don't need to doc this but we clear the elements so that the gc can reclaim the references.
            Array.Clear(_array, 0, _size);
            _size = 0;
            _version++;
        }

        public virtual bool Contains(object obj)
        {
            int count = _size;
            while (count-- > 0)
            {
                if (obj == null)
                {
                    if (_array[count] == null)
                    {
                        return true;
                    }  
                }
                else if (_array[count] != null && _array[count].Equals(obj))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检视栈顶，
        /// </summary>
        /// <returns></returns>
        public virtual object Peek()
        {
            if (_size == 0)
                //throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EmptyStack"));

            Contract.EndContractBlock();
            return _array[_size - 1];
        }

        /// <summary>
        /// 弹出
        /// </summary>
        /// <returns></returns>
        public virtual Object Pop()
        {
            if (_size == 0)
                //throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EmptyStack"));

            Contract.EndContractBlock();
            _version++;
            object obj = _array[--_size];
            _array[_size] = null;     // Free memory quicker.
            return obj;
        }

        /// <summary>
        /// 压入
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Push(object obj)
        {
            if (_size == _array.Length)
            {
                object[] newArray = new object[2 * _array.Length];
                Array.Copy(_array, 0, newArray, 0, _size);
                _array = newArray;
            }
            _array[_size++] = obj;
            _version++;
        }

        /// <summary>
        /// 返回一个线程安全的同步栈
        /// 各种操作前均会加锁
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static Stack Synchronized(Stack stack)
        {
            if (stack == null)
                throw new ArgumentNullException("stack");

            Contract.Ensures(Contract.Result<Stack>() != null);
            Contract.EndContractBlock();

            return new SyncStack(stack);
        }

        /// <summary>
        /// 返回数组，顺序从栈顶至栈尾
        /// </summary>
        /// <returns></returns>
        public virtual object[] ToArray()
        {
            Contract.Ensures(Contract.Result<Object[]>() != null);

            object[] objArray = new object[_size];
            int i = 0;
            while (i < _size)
            {
                objArray[i] = _array[_size - i - 1];
                i++;
            }
            return objArray;
        }

        /// <summary>
        /// 栈迭代器
        /// </summary>
        [Serializable]
        private class StackEnumerator : IEnumerator, ICloneable
        {
            private Stack _stack;
            private int _index;
            private int _version;
            private Object currentElement;

            internal StackEnumerator(Stack stack)
            {
                _stack = stack;
                _version = _stack._version;
                _index = -2;
                currentElement = null;
            }

            public Object Clone()
            {
                return MemberwiseClone();
            }

            public virtual bool MoveNext()
            {
                bool retval;
                if (_version != _stack._version)
                {
                    //throw new InvalidOperationException(Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
                }
                if (_index == -2)
                {  // First call to enumerator.
                    _index = _stack._size - 1;
                    retval = (_index >= 0);
                    if (retval)
                        currentElement = _stack._array[_index];
                    return retval;
                }
                if (_index == -1)
                {  // End of enumeration.
                    return false;
                }

                retval = (--_index >= 0);
                if (retval)
                    currentElement = _stack._array[_index];
                else
                    currentElement = null;
                return retval;
            }

            public virtual Object Current
            {
                get
                {
                    //if (_index == -2)
                        //throw new InvalidOperationException(Environment.GetResourceString(ResId.InvalidOperation_EnumNotStarted));
                    //if (_index == -1)
                            //throw new InvalidOperationException(Environment.GetResourceString(ResId.InvalidOperation_EnumEnded));
                    return currentElement;
                }
            }

            public virtual void Reset()
            {
                if (_version != _stack._version)
                    //throw new InvalidOperationException(Environment.GetResourceString(ResId.InvalidOperation_EnumFailedVersion));
                _index = -2;
                currentElement = null;
            }
        }

        /// <summary>
        /// 同步栈
        /// </summary>
        [Serializable]
        private class SyncStack : Stack
        {
            private Stack _s;
            private object _root;

            internal SyncStack(Stack stack)
            {
                _s = stack;
                _root = stack.SyncRoot;
            }

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override object SyncRoot
            {
                get
                {
                    return _root;
                }
            }

            public override int Count
            {
                get
                {
                    lock (_root)
                    {
                        return _s.Count;
                    }
                }
            }

            public override bool Contains(Object obj)
            {
                lock (_root)
                {
                    return _s.Contains(obj);
                }
            }

            public override Object Clone()
            {
                lock (_root)
                {
                    return new SyncStack((Stack)_s.Clone());
                }
            }

            public override void Clear()
            {
                lock (_root)
                {
                    _s.Clear();
                }
            }

            public override void CopyTo(Array array, int arrayIndex)
            {
                lock (_root)
                {
                    _s.CopyTo(array, arrayIndex);
                }
            }

            public override void Push(Object value)
            {
                lock (_root)
                {
                    _s.Push(value);
                }
            }

            public override Object Pop()
            {
                lock (_root)
                {
                    return _s.Pop();
                }
            }

            public override IEnumerator GetEnumerator()
            {
                lock (_root)
                {
                    return _s.GetEnumerator();
                }
            }

            public override Object Peek()
            {
                lock (_root)
                {
                    return _s.Peek();
                }
            }

            public override Object[] ToArray()
            {
                lock (_root)
                {
                    return _s.ToArray();
                }
            }
        }


    }
}

