using System;

namespace Client.ScriptCreate
{
    [Serializable]
    public enum ScriptType : byte
    {
        Csharp,
        
        TypeScript,
        
        Lua
    }
}