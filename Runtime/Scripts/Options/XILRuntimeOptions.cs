using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace TinaX.XILRuntime.Options
{
    public class XILRuntimeOptions
    {
        /// <summary>
        /// 启用ILRuntime
        /// </summary>
        public bool Enable { get; set; } = true;


        public string TestProp { get; set; }

        public Func<XILRuntimeOptions, UniTask> ApplyOptionsAsync;
    }
}
