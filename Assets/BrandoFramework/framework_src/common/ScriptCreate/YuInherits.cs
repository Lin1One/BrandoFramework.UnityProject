using System;

namespace Common.ScriptCreate
{
    /// <summary>
    /// 脚本类型。
    /// </summary>
    [Serializable]
    public enum Inherits : byte
    {
        Class,

        AbstractClass,

        Enum,

        Interface,
    }

    [Serializable]
    public enum ScriptType : byte
    {
        Csharp,
        
        TypeScript,
        
        Lua
    }
}