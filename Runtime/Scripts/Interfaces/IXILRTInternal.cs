using System.Threading.Tasks;

namespace TinaX.XILRuntime.Internal
{
    public interface IXILRTInternal
    {
        void InvokeEntryMethod();
        Task<XException> StartAsync();
    }
}
