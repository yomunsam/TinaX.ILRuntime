/*
 * 此处参考了CatLib代码（MIT License）
 * https://github.com/CatLib/CatLib.ILRuntime/blob/master/src/Redirect/RedirectApp_Bind.cs
 * 让我们感谢神奇的喵大
 */
using CatLib;
using CatLib.Container;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.XILRuntime.Utils;

namespace TinaX.XILRuntime.Internal.Redirect
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_IServiceContainer()
        {
            GetMappingOrCreate(typeof(TinaX.Container.IServiceContainer), out var mapping);

            mapping.Register("Get", 1, 1, Get_TService_ArrObject);
            mapping.Register("Get", 0, 2, new string[] {
                "System.Type",
                "System.Object[]"
            }, Get_Type_ArrObject);

            mapping.Register("TryGet", 1, 2, TryGet_TService_ArrObject);
            mapping.Register("TryGet", 0, 3, new string[] {
                "System.Type",
                "System.Object&",
                "System.Object[]"
            }, TryGet_Type_ArrObject);

            mapping.Register("TryGetBuildInService", 1, 1, TryGetBuildInService_TService);
            mapping.Register("TryGetBuildInService", 0, 2, new string[]
            {
                "System.Type",
                "System.Object&"
            }, TryGetBuildInService_Type);

            mapping.Register("Bind", 1, 0, Bind_TService);
            mapping.Register("Bind", 2, 0, Bind_TService_TConcrete);

            mapping.Register("BindIf", 1, 1, BindIf_TService_IBindData);
            mapping.Register("BindIf", 2, 1, BindIf_TService_TConcrete_IBindData);

            mapping.Register("Singleton", 1, 0, Singleton_TConcrete);
            mapping.Register("Singleton", 2, 0, Singleton_TService_TConcrete);

        }

        #region Get Services

        //TService Get<TService>(params object[] userParams);
        private static StackObject* Get_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var userParams =
                (object[])typeof(object[]).CheckCLRTypes(
                    StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            intp.Free(ptrOfThisMethod);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, XCore.GetMainInstance().Services.Get(tService, userParams));
        }



        //object Get(Type type, params object[] userParams);
        private static StackObject* Get_Type_ArrObject(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @userParams = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Type @type = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            string serviceName = XILUtil.GetCatLibServiceName(@type);
            var result_of_this_method = instance_of_this_method.Get(serviceName, @userParams);

            object obj_result_of_this_method = result_of_this_method;
            if (obj_result_of_this_method is CrossBindingAdaptorType)
            {
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        //bool TryGet<TService>(out TService service, params object[] userParams);
        private static StackObject* TryGet_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 2)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var userparams = (object[])typeof(object[]).CheckCLRTypes(
                StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);
            var service = 
                (object)typeof(ILTypeInstance).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));



            var result = XCore.MainInstance.Services.TryGet(tService, out service, userparams);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        //bool TryGet(Type type, out object service, params object[] userParams);
        private static StackObject* TryGet_Type_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (method.ParameterCount != 3)
            {
                throw new EntryPointNotFoundException();
            }


            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var userparams = (object[])typeof(object[]).CheckCLRTypes(
                StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);
            var service =
                (object)typeof(object).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            ptrOfThisMethod = ILIntepreter.Minus(esp, 3);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);
            var _type =
                (Type)typeof(Type).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));
            var tService = XILUtil.GetCatLibServiceName(_type);

            var result = XCore.MainInstance.Services.TryGet(tService, out service, userparams);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        /// <summary>
        /// bool TryGetBuildInService<TBuiltInService>(out TBuiltInService service) where TBuiltInService : IBuiltInService;
        /// </summary>
        /// <param name="intp"></param>
        /// <param name="esp"></param>
        /// <param name="mStack"></param>
        /// <param name="method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        private static StackObject* TryGetBuildInService_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var service =
                (object)typeof(ILTypeInstance).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));


            var result = XCore.MainInstance.Services.TryGetBuildInService(tService, out service);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        //bool TryGetBuildInService(Type type, out object service);
        private static StackObject* TryGetBuildInService_Type(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (method.ParameterCount != 2)
            {
                throw new EntryPointNotFoundException();
            }

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var service =
                (object)typeof(object).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);
            var _type =
                (Type)typeof(Type).CheckCLRTypes(StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));
            var tService = XILUtil.GetCatLibServiceName(_type);

            var result = XCore.MainInstance.Services.TryGetBuildInService(tService, out service);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        #endregion

        #region Binding Services

        // IBindData Bind<TService>();
        private static StackObject* Bind_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[0]);

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Bind(tService, tType, false);

            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }

        //IBindData Bind<TService, TConcrete>();
        private static StackObject* Bind_TService_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[1]);

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Bind(tService, tType, false);

            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }


        //IBindData Singleton<TService, TConcrete>();
        private static StackObject* Singleton_TService_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[1]);

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Bind(tService, tType, true);

            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }

        //IBindData Singleton<TService>();
        private static StackObject* Singleton_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[0]);

            ILRuntime.Runtime.Enviorment.AppDomain __domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.Container.IServiceContainer instance_of_this_method = (TinaX.Container.IServiceContainer)typeof(TinaX.Container.IServiceContainer).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Bind(tService, tType, true);

            return ILIntepreter.PushObject(esp, mStack, result_of_this_method);
        }

        //bool BindIf<TService>(out IBindData bindData);
        private static StackObject* BindIf_TService_IBindData(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[0]);

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var bindData =
                (IBindData)typeof(IBindData).CheckCLRTypes(
                    StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            var result = XCore.MainInstance.Services.BindIf(tService, tType, false, out bindData);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, bindData);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        //bool BindIf<TService, TConcrete>(out IBindData bindData);
        private static StackObject* BindIf_TService_TConcrete_IBindData(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 1)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[1]);

            var ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            ptrOfThisMethod = ILIntepreter.GetObjectAndResolveReference(ptrOfThisMethod);

            var bindData =
                (IBindData)typeof(IBindData).CheckCLRTypes(
                    StackObject.ToObject(ptrOfThisMethod, intp.AppDomain, mStack));

            var result = XCore.MainInstance.Services.BindIf(tService, tType, false, out bindData);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, bindData);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        #endregion

        internal static StackObject* Type2ServiceName_param(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Type @type = (System.Type)typeof(System.Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = XCore.GetMainInstance().Services.Type2ServiceName(type);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        internal static StackObject* Type2ServiceName_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack,
            CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 1 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);

            return ILIntepreter.PushObject(esp, mStack, tService);
        }
    }
}
