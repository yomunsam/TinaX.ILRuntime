using System.Threading;
using Cysharp.Threading.Tasks;

namespace TinaX.XILRuntime.Internal
{
    public interface IXILRuntimeInternal
    {
        bool Initialized { get; }

        UniTask InvokeEntryMethodAsync();
        UniTask StartAsync(CancellationToken cancellationToken = default);
    }
}
