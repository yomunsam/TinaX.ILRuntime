using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Module;
using TinaX.Modules;
using TinaX.XILRuntime.Consts;
using TinaX.XILRuntime.Internal;
using UnityEngine;

namespace TinaX.XILRuntime
{
    public class XILRuntimeModule : IModuleProvider
    {

        public string ModuleName => XILConsts.ModuleName;

        public UniTask<ModuleBehaviourResult> OnInit(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));

        public void ConfigureServices(IServiceContainer services)
        {
            services.Singleton<IXILRuntime, XILRuntimeService>().Alias<IXILRuntimeInternal>();
        }

        public async UniTask<ModuleBehaviourResult> OnStart(IServiceContainer services, CancellationToken cancellationToken)
        {
            Debug.Log("XILRuntime Module 开始启动");
            await services.Get<IXILRuntimeInternal>().StartAsync();
            return ModuleBehaviourResult.CreateSuccess(ModuleName);
            //return UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));
        }

        public UniTask OnRestart(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        public void OnQuit() { }
    }
}
