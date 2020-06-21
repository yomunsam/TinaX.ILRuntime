using TinaX.XILRuntime;

namespace TinaX.Services
{
    public static class XILRTProviderExtend
    {
        public static IXCore UseXILRuntime(this IXCore core)
        {
            core.RegisterServiceProvider(new XILRuntimeProvider());
            return core;
        }
    }
}
