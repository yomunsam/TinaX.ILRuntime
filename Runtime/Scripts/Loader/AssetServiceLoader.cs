using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.Services;
using TinaX.XILRuntime.Structs;
using UnityEngine;

namespace TinaX.XILRuntime.Loader
{
    public class AssetServiceLoader : IAssemblyLoader
    {
        private readonly IAssetService m_AssetService;

        public AssetServiceLoader(IAssetService assetService)
        {
            this.m_AssetService = assetService;
        }

        /// <summary>
        /// 该加载器支持异步加载
        /// </summary>
        public bool SupportAsynchronous => true;

        /// <summary>
        /// 异步加载Assembly
        /// </summary>
        /// <param name="loadInfo"></param>
        /// <returns></returns>
        public async UniTask<AssemblyLoadResult> LoadAssemblyAsync(AssemblyLoadInfo loadInfo, bool loadSymbol = true, CancellationToken cancellationToken = default)
        {
            if (loadInfo.AssemblyPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(loadInfo.AssemblyPath));

            var result = new AssemblyLoadResult(ref loadInfo);

            var assembly_textasset = await m_AssetService.LoadAsync<TextAsset>(loadInfo.AssemblyPath, cancellationToken);
            result.AssemblyStream = new System.IO.MemoryStream(assembly_textasset.bytes);
            m_AssetService.Release(assembly_textasset);

            if (loadSymbol) //加载调试符号
            {
                if (loadInfo.SymbolPath.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(loadInfo.SymbolPath));

                var symbol_textasset = await m_AssetService.LoadAsync<TextAsset>(loadInfo.SymbolPath, cancellationToken);
                result.SymbolStream = new System.IO.MemoryStream(symbol_textasset.bytes);
                m_AssetService.Release(symbol_textasset);
            }

            return result;
        }

        //当加载器提供异步加载实现时，则只会使用异步加载方法，不会使用同步加载方法
        public AssemblyLoadResult LoadAssembly(AssemblyLoadInfo loadInfo, bool loadSymbol = true)
        {
            throw new System.NotImplementedException();
        }
    }
}
