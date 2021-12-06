using System.Threading;
using Cysharp.Threading.Tasks;
using TinaX.XILRuntime.Structs;

namespace TinaX.XILRuntime.Loader
{
    /// <summary>
    /// Assembly 加载器
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// 该加载器是否支持异步加载
        /// </summary>
        bool SupportAsynchronous { get; }


        /// <summary>
        /// 异步加载Assembly
        /// </summary>
        /// <param name="loadInfo"></param>
        /// <param name="loadSymbol">需要加载调试符号文件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        UniTask<AssemblyLoadResult> LoadAssemblyAsync(AssemblyLoadInfo loadInfo, bool loadSymbol = true, CancellationToken cancellationToken = default);

        AssemblyLoadResult LoadAssembly(AssemblyLoadInfo loadInfo, bool loadSymbol = true);
    }
}
