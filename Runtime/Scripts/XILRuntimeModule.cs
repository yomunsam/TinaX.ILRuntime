using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Module;
using TinaX.Modules;
using TinaX.XILRuntime.Consts;

namespace TinaX.XILRuntime
{
    public class XILRuntimeModule : IModuleProvider
    {

        public string ModuleName => XILConsts.ModuleName;

        public UniTask<ModuleBehaviourResult> OnInit(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));

        public void ConfigureServices(IServiceContainer services)
        {
            services.Singleton<IXILRuntime, XILRuntimeService>();
        }

        public UniTask<ModuleBehaviourResult> OnStart(IServiceContainer services, CancellationToken cancellationToken)
        {
            return UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));
        }

        public UniTask OnRestart(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        public void OnQuit() { }
    }
}
