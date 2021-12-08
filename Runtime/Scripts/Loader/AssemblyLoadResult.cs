﻿using System.IO;
using TinaX.XILRuntime.Structs;

namespace TinaX.XILRuntime.Loader
{
    public class AssemblyLoadResult
    {
        public string AssemblyPath { get; set; }
        /// <summary>
        /// Assembly 流
        /// </summary>
        public MemoryStream AssemblyStream { get; set; }

        public string SymbolPath { get; set; }
        public MemoryStream SymbolStream { get; set; }


        public AssemblyLoadResult() { }

        public AssemblyLoadResult(ref AssemblyLoadInfo loadInfo)
        {
            this.AssemblyPath = loadInfo.AssemblyPath;
            this.SymbolPath = loadInfo.SymbolPath;
        }
    }
}