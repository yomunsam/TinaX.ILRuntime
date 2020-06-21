#pragma warning disable CA2235 // Mark all non-serializable fields

namespace TinaX.XILRuntime.Internal
{
    [System.Serializable]
    public struct AssemblyAndSymbolPath
    {
        public string AssemblyPath;
        public string SymbolPath;
    }
}

#pragma warning restore CA2235 // Mark all non-serializable fields
