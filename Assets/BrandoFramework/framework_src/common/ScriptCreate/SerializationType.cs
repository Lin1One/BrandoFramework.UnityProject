using System;

namespace Common.ScriptCreate
{
    /// <summary>
    /// 脚本类型。
    /// </summary>
    [Serializable]
    public enum SerializationType : byte
    {
        ProtoBuff,
        Json
    }

}