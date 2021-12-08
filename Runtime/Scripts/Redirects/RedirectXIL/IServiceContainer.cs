﻿using System;
using System.Collections.Generic;
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
        private static void Register_IServiceContainer()
        {
            GetMappingOrCreate(typeof(TinaX.Container.IServiceContainer), out var mapping);

            //Add Services
            mapping.Register("Singleton", 2, 0, Singleton_TService_TConcrete);
        }

        //TService Get<TService>(params object[] userParams);
        //private static StackObject* Get_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        //{
        //    var genericArguments = method.GenericArguments;
        //    if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 1)
        //    {
        //        throw new EntryPointNotFoundException();
        //    }


        //}


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
            Debug.LogFormat("热更工程绑定：{0} --> {1}", serviceName, tConcrete.FullName);
            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }
    }
}