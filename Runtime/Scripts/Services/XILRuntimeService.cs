using System;
using Cysharp.Threading.Tasks;
using TinaX.Exceptions;
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
            var options = m_Options.Value;
            if (options.ApplyOptionsAsync != null)
                await options.ApplyOptionsAsync(options);

            await UniTask.CompletedTask;
            throw new XException("喵异常");

        }

    }
}
