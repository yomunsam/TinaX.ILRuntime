using ILRuntime.CLR.TypeSystem;
using TinaX.Container;

namespace TinaX.XILRuntime.Extensions.ServiceContainer
{
    public static class ServiceContainerExtensions
    {
        public static string GetServiceNameByIType(this IServiceContainer services, IType type)
        {
            var ilType = type as ILType;
            return ilType != null ? ilType.FullName : services.GetServiceName(type.TypeForCLR);
        }
    }
}
