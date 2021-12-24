//using System;
//using System.Reflection;
//using CatLib.Container;
//using ILRuntime.Runtime;
//using ILRuntime.Runtime.Intepreter;
//using TinaX.Container;
//using TinaX.Core.Container;
//using TinaX.Exceptions;
//using TinaX.XILRuntime.Extensions.ServiceContainer;
//using UnityEngine;

//namespace TinaX.XILRuntime.ServiceInjector
//{
//    public class XILServiceInjector : IServiceInjector
//    {
//        /// <summary>
//        /// 帮助CatLib实现属性注入
//        /// </summary>
//        /// <param name="makeServiceBindData"></param>
//        /// <param name="makeServiceInstance"></param>
//        /// <param name="serviceContainer"></param>
//        /// <returns></returns>
//        public bool TryServiceAttributeInject(ref Bindable makeServiceBindData, ref object makeServiceInstance, IServiceContainer serviceContainer)
//        {
//            if(makeServiceInstance is ILTypeInstance)
//            {
//                //Debug.Log("一处属性注入由ILRuntime代劳：" + makeServiceInstance.ToString());
//                BindingFlags _bindFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
//                Type actualType = makeServiceInstance.GetActualType();
//                foreach(var property in actualType.GetProperties(_bindFlag))
//                {
//                    //Debug.Log("     property:" + property.Name);
//                    var attr = property.GetCustomAttribute<TinaX.InjectAttribute>(true);
//                    if (attr == null)
//                        continue;
//                    string serviceName = serviceContainer.GetServiceNameByProperty(property);

//                    if(serviceContainer.TryGet(serviceName, out var _service_instance))
//                    {
//                        property.SetValue(makeServiceInstance, _service_instance);
//                        continue;
//                    }

//                    if (attr.Nullable)
//                        continue;
//                    else
//                        throw new XException($"Service not found {property.PropertyType}"); //抛异常
//                }

//                return true;
//            }
//            return false;
//        }
//    }
//}
