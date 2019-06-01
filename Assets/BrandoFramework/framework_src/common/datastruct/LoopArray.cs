
namespace client_common
{
    /// <summary>
    /// 循环数组。
    /// </summary>
    public class LoopArray<T>
    {
        protected T[] _buffer;

        protected LoopArray(int length)
        {
            _buffer = new T[length];
        }

        protected LoopArray(T[] array)
        {
            _buffer = array;
        }

        protected T this[int index]
        {
            get
            {
                if (index < _buffer.Length)
                {
                    return _buffer[index];
                }

                var finalIndex = index % _buffer.Length;
                return _buffer[finalIndex];
            }
            set
            {
                if (index < _buffer.Length)
                {
                    _buffer[index] = value;
                    return;
                }


                var finalIndex = index % _buffer.Length;
                _buffer[finalIndex] = value;
            }
        }

        protected int Length => _buffer.Length;

        protected void SetNetBuffer(T[] buffer)
        {
            _buffer = buffer;
        }
    }
}