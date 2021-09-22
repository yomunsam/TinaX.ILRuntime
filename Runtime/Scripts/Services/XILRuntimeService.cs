using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Exceptions;
using TinaX.Options;
using TinaX.Services.ConfigAssets;
using TinaX.XILRuntime.ConfigAssets;
using TinaX.XILRuntime.Internal;
using TinaX.XILRuntime.Options;

namespace TinaX.XILRuntime
{
    public class XILRuntimeService : IXILRuntime, IXILRuntimeInternal
    {
        private readonly IOptions<XILRuntimeOptions> m_Options;
        private readonly IConfigAssetService m_ConfigAssetService;

        public XILRuntimeService(IOptions<XILRuntimeOptions> options,
            IConfigAssetService configAssetService)
        {
            this.m_Options = options;
            this.m_ConfigAssetService = configAssetService;
        }

        private XILRuntimeConfigAsset m_ConfigAsset;


        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            var options = m_Options.Value;
            if (!options.Enable)
                return;

            //加载配置资产
            m_ConfigAsset = await LoadConfigAssetAsync(options.ConfigAssetLoadPath, cancellationToken);
            if(m_ConfigAsset == null)
            {
                throw new XException($"Failed to load configuration assets \"{options.ConfigAssetLoadPath}\" ");
            }

            await UniTask.CompletedTask;
        }


        private UniTask<XILRuntimeConfigAsset> LoadConfigAssetAsync(string loadPath, CancellationToken cancellationToken)
        {
            return m_ConfigAssetService.GetConfigAsync<XILRuntimeConfigAsset>(loadPath, cancellationToken);
        }

    }
}
