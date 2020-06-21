using System.Threading.Tasks;
using TinaX.XILRuntime.Const;
using TinaX.XILRuntime.Internal;
using TinaX.Services;

namespace TinaX.XILRuntime
{
    public class XILRuntimeProvider : IXServiceProvider
    {
        public string ServiceName => XILConst.ServiceName;

        public Task<XException> OnInit(IXCore core) => Task.FromResult<XException>(null);

        public void OnServiceRegister(IXCore core)
        {
            core.Services.BindBuiltInService<IAppDomain, IXILRuntime, XILRT>()
                .SetAlias<IXILRTInternal>();
        }

        public Task<XException> OnStart(IXCore core)
        {
            return core.Services.Get<IXILRTInternal>().StartAsync();
        }

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;




    }
}
