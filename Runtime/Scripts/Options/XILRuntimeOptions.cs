using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TinaX.XILRuntime.Structs;

namespace TinaX.XILRuntime.Options
{
    public class XILRuntimeOptions
    {
        /// <summary>
        /// 启用ILRuntime
        /// </summary>
        public bool Enable { get; set; } = true;

        public List<AssemblyLoadInfo> LoadAssemblies { get; set; } = new List<AssemblyLoadInfo>();

        /// <summary>
        /// 入口类
        /// </summary>
        public string EntryClass { get; set; }

        /// <summary>
        /// 入口方法
        /// </summary>
        public string EntryMethod { get; set; }


        public Func<XILRuntimeOptions, UniTask> ApplyOptionsAsync;
    }
}
