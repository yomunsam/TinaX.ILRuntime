using Cysharp.Threading.Tasks;
using TinaX.Options;
using TinaX.XILRuntime.Internal;
using TinaX.XILRuntime.Options;
using UnityEngine;

namespace TinaX.XILRuntime
{
    public class XILRuntimeService : IXILRuntime, IXILRuntimeInternal
    {
        private readonly IOptions<XILRuntimeOptions> m_Options;

        public XILRuntimeService(IOptions<XILRuntimeOptions> options)
        {
            this.m_Options = options;
        }

        public async UniTask StartAsync()
        {
            Debug.Log("喵1");
            var options = m_Options.Value;
            Debug.Log("喵2");
            if (options.ApplyOptionsAsync != null)
                await options.ApplyOptionsAsync(options);

            Debug.Log("XIL Service :" + options.TestProp);
            await UniTask.CompletedTask;
        }

    }
}
