using System.Threading.Tasks;

namespace TinaX.XILRuntime.Internal
{
    public interface IXILRTInternal
    {
        Task InvokeEntryMethod();
        Task<XException> StartAsync();
    }
}
