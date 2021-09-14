using System;
using TinaX.Options;
using TinaX.Systems.Configuration;
using TinaX.XILRuntime;
using TinaX.XILRuntime.ConfigAssets;
using TinaX.XILRuntime.Options;

namespace TinaX.Services
{
    public static class XILRuntimeServiceExtensions
    {
        /// <summary>
        /// Add ILRuntime to TinaX Core.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="ilruntimeOptions"></param>
        /// <returns></returns>
        public static IXCore AddILRuntime(this IXCore core)
        {
            core.Services.AddOptions();
            core.Services.Configure<XILRuntimeOptions>(options =>
            {
                options.ApplyOptionsFromConfigAsset();
            });

            core.AddModule(new XILRuntimeModule());
            return core;
        }

        /// <summary>
        /// Add ILRuntime to TinaX Core.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="ilruntimeOptions"></param>
        /// <returns></returns>
        public static IXCore AddILRuntime(this IXCore core, Action<XILRuntimeOptions> ilruntimeOptions)
        {
            core.Services.AddOptions();
            core.Services.Configure<XILRuntimeOptions>(ilruntimeOptions);

            core.AddModule(new XILRuntimeModule());
            return core;
        }

        /// <summary>
        /// Add ILRuntime to TinaX Core.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="ilruntimeOptions"></param>
        /// <returns></returns>
        public static IXCore AddILRuntime(this IXCore core, Action<XILRuntimeOptions, IConfiguration> ilruntimeOptions)
        {
            core.Services.AddOptions();
            core.Services.Configure<XILRuntimeOptions>(options =>
            {
                ilruntimeOptions?.Invoke(options, core.Services.Get<IConfiguration>());
            });

            core.AddModule(new XILRuntimeModule());
            return core;
        }


        [Obsolete("Use \"AddILRuntime\"")]
        public static IXCore UseXILRuntime(this IXCore core)
        {
            core.AddILRuntime();
            return core;
        }
    }
}
