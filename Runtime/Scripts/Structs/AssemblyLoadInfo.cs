using System.IO;

namespace TinaX.XILRuntime.Internal
{
    public struct AssemblyLoadInfo
    {
        public string AssemblyLoadPath;
        public string SymbolLoadPath;

        public Stream AssemblyStream;
        public Stream SymbolStream;
    }
}
