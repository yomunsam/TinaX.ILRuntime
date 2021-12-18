using System.Reflection;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using TinaX.Container;
using TinaX.Container.Internal;

namespace TinaX.XILRuntime.Extensions.ServiceContainer
{
    public static class ServiceContainerExtensions
    {
        public static string GetServiceNameByIType(this IGetServices services, IType type)
        {
            var ilType = type as ILType;
            return ilType != null ? ilType.FullName : services.GetServiceName(type.TypeForCLR);
        }

        public static string GetServiceNameByProperty(this IServiceContainer services, PropertyInfo property)
        {
            if (property is ILRuntimePropertyInfo)
                return property.PropertyType.FullName;
            else
                return services.GetServiceName(property.PropertyType);
        }
    }
}
