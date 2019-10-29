using System;

namespace Common.ScriptCreate
{
    [Serializable]
    public enum ScriptType : byte
    {
        Csharp,
        
        TypeScript,
        
        Lua
    }
}