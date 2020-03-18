using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.ILRuntime.Const
{
    public static class XRuntimeConst
    {
        public const string ServiceName = "X ILRuntime";

        public const string ConfigFileName = "XILRuntime";
        public static readonly string ConfigPath_Resources = $"{TinaX.Const.FrameworkConst.Framework_Configs_Folder_Path}/{ConfigFileName}";

        public const string AssemblyFileName = "assembly.dll";
        public const string SymbolFileName = "assembly.pdb";
    
    }
}
