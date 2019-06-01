
using ProtoBuf;
using System.IO;

namespace client_common
{
    /// <summary>
    /// Protobuf序列号器。
    /// </summary>
    public class ProtobufSerializer : ISerializer
    {
        public byte[] Serialize(object message)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        public T DeSerialize<T>(byte[] bytes) where T : class, new()
        {
            using (var ms = new MemoryStream(bytes))
            {
                var message = Serializer.Deserialize<T>(ms);
                return message;
            }
        }
    }
}