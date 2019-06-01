
using System;

namespace client_common
{


    public class ByteBuf : LoopArray<byte>, IByteBuf
    {
        private int readIndex;
        private int writeIndex;

        public int ReadbleCount => writeIndex - readIndex;

        public ByteBuf(int length) : base(length)
        {
        }

        public ByteBuf(byte[] array, int readIndex = 0) : base(array)
        {
            this.readIndex = readIndex;
        }

        private void EnsureReadble(int length)
        {
            if (ReadbleCount < length)
                throw new Exception($"没有长度为{length}可读数据！");
        }

        public byte[] ReadBytes(int length)
        {
            EnsureReadble(length);
            var bytes = BytesPool.Take(length);
            for (var index = 0; index < length; index++)
            {
                var b = this[readIndex];
                readIndex++;
                bytes[index] = b;
            }

            return bytes;
        }

        public byte[] GetWritedBytes()
        {
            var bytes = new byte[writeIndex];
            for (var index = 0; index < writeIndex; index++)
            {
                bytes[index] = this[index];
            }

            readIndex = writeIndex;

            return bytes;
        }

        #region 读写操作

        public void WriteBytes(byte[] bytes)
        {
            foreach (var bit in bytes)
            {
                this[writeIndex] = bit;
                writeIndex++;
            }
        }

        public void WriteBytes(byte[] bytes, int length)
        {
            for (var i = 0; i < length; i++)
            {
                this[writeIndex] = bytes[i];
                writeIndex++;
            }
        }

        public void MoveReadIndex(int offset) => readIndex += offset;

        public void MoveWriteIndex(int offset) => writeIndex += offset;
        public byte[] LastBytes { get; set; }

        public void WriteByte(byte b)
        {
            this[writeIndex] = b;
            writeIndex++;
        }

        public void WriteInt(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(bytes);
        }

        public void WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(bytes);
        }

        public void WriteShort(short value)
        {
            var bytes = BitConverter.GetBytes(value);
            WriteBytes(bytes);
        }

        public byte ReadByte()
        {
            EnsureReadble(1);
            var result = this[readIndex];
            readIndex++;
            return result;
        }

        public int ReadInt()
        {
            EnsureReadble(4);
            var result = BitConverter.ToInt32(_buffer, readIndex);
            readIndex += 4;
            return result;
        }

        public short ReadShort()
        {
            EnsureReadble(2);
            var result = BitConverter.ToInt16(_buffer, readIndex);
            readIndex += 2;
            return result;
        }

        public long ReadLong()
        {
            EnsureReadble(8);
            var result = BitConverter.ToInt64(_buffer, readIndex);
            readIndex += 8;
            return result;
        }

        public int ResidualLength => ReadbleCount - writeIndex;

        public byte[] GetReadResidual()
        {
            var result = new byte[ResidualLength];
            Buffer.BlockCopy(_buffer, readIndex,
                result, 0, ResidualLength);
            return result;
        }

        #endregion
    }
}
