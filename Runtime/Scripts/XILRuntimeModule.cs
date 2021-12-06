using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Core.Behaviours;
using TinaX.Module;
using TinaX.Modules;
using TinaX.XILRuntime.Behaviour;
using TinaX.XILRuntime.Consts;
using TinaX.XILRuntime.Internal;
using UnityEngine;

namespace TinaX.XILRuntime
{
    public class XILRuntimeModule : IModuleProvider
    {

        public string ModuleName => XILConsts.ModuleName;

        public UniTask<ModuleBehaviourResult> OnInitAsync(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.FromResult(ModuleBehaviourResult.CreateSuccess(ModuleName));

        public void ConfigureBehaviours(IBehaviourManager behaviour, IServiceContainer services)
        {
            behaviour.Register(new XILRuntimeStartBehaviour(services));
        }

        public void ConfigureServices(IServiceContainer services)
        {
            services.Singleton<IXILRuntime, XILRuntimeService>().Alias<IXILRuntimeInternal>();
        }

        public async UniTask<ModuleBehaviourResult> OnStartAsync(IServiceContainer services, CancellationToken cancellationToken)
        {
#if TINAX_DEV
            Debug.Log("XILRuntime Module 开始启动");
#endif
            await services.Get<IXILRuntimeInternal>().StartAsync(cancellationToken);
            return ModuleBehaviourResult.CreateSuccess(ModuleName);
        }

        public UniTask OnRestartAsync(IServiceContainer services, CancellationToken cancellationToken)
            => UniTask.CompletedTask;

        public void OnQuit() { }

        
    }
}
