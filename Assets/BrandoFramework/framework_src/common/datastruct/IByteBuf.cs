
namespace Common
{
    /// <summary>
    /// 字节数据缓存器接口。
    /// </summary>
    public interface IByteBuf
    {
        #region 读

        int ResidualLength { get; }

        byte[] GetWritedBytes();

        byte[] GetReadResidual();

        byte ReadByte();

        byte[] ReadBytes(int length);

        int ReadInt();

        short ReadShort();

        long ReadLong();

        int ReadbleCount { get; }

        #endregion

        #region 写

        void WriteByte(byte value);

        void WriteInt(int value);

        void WriteLong(long value);

        void WriteShort(short value);

        void WriteBytes(byte[] bytes);

        void WriteBytes(byte[] bytes, int length);

        #endregion

        #region 操作读写索引

        void MoveReadIndex(int offset);

        void MoveWriteIndex(int offset);

        #endregion

        byte[] LastBytes { get; set; }
    }
}
