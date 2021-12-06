using TinaX.XILRuntime.Consts;

namespace TinaX.XILRuntime.Options
{
    public class XILRuntimeOptions
    {
        /// <summary>
        /// 启用ILRuntime
        /// </summary>
        public bool Enable { get; set; } = true;

        public string ConfigAssetLoadPath { get; set; } = XILConsts.DefaultConfigAssetName;

        public int DefaultJitFlags = 0;
    }
}
