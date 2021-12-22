using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Container;
using TinaX.Core.Behaviours;
using TinaX.XILRuntime.Internal;

namespace TinaX.XILRuntime.Behaviour
{
    public class XILRuntimeStartBehaviour : IStartAsync
    {
        private readonly IServiceContainer m_Services;

        public XILRuntimeStartBehaviour(IServiceContainer services)
        {
            this.m_Services = services;
        }

        public int StartOrder { get; set; } = 0;

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            var ilruntimeService = m_Services.Get<IXILRuntimeInternal>();
            if (ilruntimeService.Initialized)
                await ilruntimeService.InvokeEntryMethodAsync();
        }
    }
}
