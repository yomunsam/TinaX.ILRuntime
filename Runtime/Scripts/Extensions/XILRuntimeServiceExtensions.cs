using System;
using TinaX.Options;
using TinaX.Systems.Configuration;
using TinaX.XILRuntime;
using TinaX.XILRuntime.Builders;
using TinaX.XILRuntime.ConfigAssets;
using TinaX.XILRuntime.Options;
using TinaX.XILRuntime.Registers;

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
            core.AddILRuntime(builder => { });
            return core;
        }

        

        public static IXCore AddILRuntime(this IXCore core, Action<XILRuntimeBuilder> ilruntimeBuilder)
        {
            //---------------------------------------------------------------------------------
            //因为这个Builder不复杂，所有我们就直接在这儿顺便实现builder模式里的Director，不另外写个class了

            var builder = new XILRuntimeBuilder(core.Services);
            ilruntimeBuilder?.Invoke(builder);
            //Options
            if (!core.Services.TryGet<IOptions<XILRuntimeOptions>>(out _))
            {
                core.Services.AddOptions();
                core.Services.Configure<XILRuntimeOptions>(options => { });
            }

            //各类注册
            var regMgr = builder.GetRegisterManager();
            if(regMgr.Count > 0)
            {
                core.Services.Singleton<RegisterManager>(regMgr);
            }

            //-------------------------------------------------------------------------------
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
