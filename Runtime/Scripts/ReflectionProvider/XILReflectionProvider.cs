using System;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using TinaX.Core.ReflectionProvider;

namespace TinaX.XILRuntime.ReflectionProvider
{
#nullable enable

    /// <summary>
    /// 反射提供者
    /// 为框架提供ILRuntime环境中对象的反射
    /// </summary>
    public class XILReflectionProvider : IReflectionProvider
    {
        public bool TryGetType(ref object sourceObject, out Type? type)
        {
            if (sourceObject is ILTypeInstance || sourceObject is CrossBindingAdaptorType)
            {
                type = sourceObject.GetActualType();
                return true;
            }
            else
            {
                type = null;
                return false;
            }
        }
    }

#nullable disable
}
