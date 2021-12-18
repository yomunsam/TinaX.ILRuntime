using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using TinaX.XILRuntime.Extensions.ServiceContainer;

namespace TinaX.XILRuntime.Redirects
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_TinaX_Container_IGetServices()
        {
            GetMappingOrCreate(typeof(TinaX.Container.Internal.IGetServices), out var mapping);

            mapping.Register("Get", 1, 1, Get_TService_ArrObject);
        }


        //TService Get<TService>(params object[] userParams);
        private static StackObject* Get_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }


            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            System.Object[] userParams = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(esp, 2);
            TinaX.Container.Internal.IGetServices instance_of_this_method = (TinaX.Container.Internal.IGetServices)typeof(TinaX.Container.Internal.IGetServices).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            intp.Free(ptr_of_this_method);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);

            var result_of_this_method = instance_of_this_method.Get(serviceName, userParams);
            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }

    }

    
}
