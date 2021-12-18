using System;
using System.Collections.Generic;
using CatLib.Container;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using TinaX.XILRuntime.Extensions.ServiceContainer;
using TinaX.XILRuntime.Helper;
using UnityEngine;

namespace TinaX.XILRuntime.Redirects
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_TinaX_Container_IServiceContainer()
        {
            GetMappingOrCreate(typeof(TinaX.Container.IServiceContainer), out var mapping);


            //Add Services
            mapping.Register("Singleton", 2, 0, Singleton_TService_TConcrete);
            mapping.Register("Singleton", 1, 0, Singleton_TService);
            mapping.Register("SingletonIf", 2, 1, SingletonIf_TService_TConcrete_IBindData);

            mapping.Register("Bind", 2, 0, Bind_TService_TConcrete);
            mapping.Register("Bind", 1, 0, Bind_TService);

            


        }

        //IBindData Bind<TService, TConcrete>();
        private static StackObject* Bind_TService_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            intp.Free(ptr_of_this_method);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);
            var tConcrete = XILHelper.ITypeToClrType(genericArguments[1]);

            var result_of_this_method = instance_of_this_method.Bind(serviceName, tConcrete, false);

            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }

        //IBindData Bind<TService>();
        private static StackObject* Bind_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);
            intp.Free(ptr_of_this_method);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);
            var tConcrete = XILHelper.ITypeToClrType(genericArguments[0]);

            var result_of_this_method = instance_of_this_method.Bind(serviceName, tConcrete, false);

            return ILIntepreter.PushObject(__ret, mStack, result_of_this_method);
        }


        //IBindData Singleton<TService, TConcrete>();
        private static StackObject* Singleton_TService_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);
            var tConcrete = XILHelper.ITypeToClrType(genericArguments[1]);

            var result_of_this_method = instance_of_this_method.Bind(serviceName, tConcrete, true);
            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }

        //IBindData Singleton<TService>();
        private static StackObject* Singleton_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);
            var tConcrete = XILHelper.ITypeToClrType(genericArguments[0]);

            var result_of_this_method = instance_of_this_method.Bind(serviceName, tConcrete, true);
            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }

        //bool SingletonIf<TService, TConcrete>(out IBindData bindData);
        private static StackObject* SingletonIf_TService_TConcrete_IBindData(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            CatLib.Container.IBindData bindData = (CatLib.Container.IBindData)typeof(CatLib.Container.IBindData).CheckCLRTypes(intp.RetriveObject(ptr_of_this_method, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);

            ptr_of_this_method = ILIntepreter.Minus(esp, 2);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack), (ILRuntime.CLR.Utils.Extensions.TypeFlags)0);

            var serviceName = instance_of_this_method.GetServiceNameByIType(genericArguments[0]);
            var tConcrete = XILHelper.ITypeToClrType(genericArguments[1]);
            var result_of_this_method = instance_of_this_method.BindIf(serviceName, tConcrete, true, out bindData);

            //给ref或out复制
            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            XILHelper.SetValue(ptr_of_this_method, mStack, __domain, bindData);
            intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(esp, 2);
            intp.Free(ptr_of_this_method);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;

            return __ret + 1;
        }
    }
}
