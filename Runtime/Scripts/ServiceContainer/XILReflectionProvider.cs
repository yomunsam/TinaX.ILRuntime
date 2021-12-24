using System;
using System.Reflection;
using ILRuntime.Runtime;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using TinaX.Core.Container;
using UnityEngine;

namespace TinaX.XILRuntime.ServiceContainer
{
    /// <summary>
    /// 为TinaX.Core的服务容器提供反射相关功能
    /// </summary>
    public class XILReflectionProvider : IReflectionProvider
    {
        public bool TryGetType(ref object sourceObject, out Type type)
        {
            if(sourceObject is ILTypeInstance || sourceObject is CrossBindingAdaptorType)
            {
                type = sourceObject.GetActualType();
#if TINAX_DEV
                Debug.LogFormat("获取了IL中实例的类型：{0}", type.FullName);
#endif
                return true;
            }
            else
            {
                type = default;
                return false;
            }
        }

        /// <summary>
        /// 给定的属性可以被依赖注入吗
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public bool? CanInjected(ref PropertyInfo property)
        {
            return null; //好像不要处理
        }

        public bool? CanInjectedSkip(ref PropertyInfo property)
        {
            return null; //不行再处理
        }
    }
}
