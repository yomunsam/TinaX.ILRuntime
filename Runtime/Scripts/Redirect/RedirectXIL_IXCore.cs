using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using TinaX.XILRuntime.Utils;

namespace TinaX.XILRuntime.Internal.Redirect
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_IXCore()
        {
            GetMappingOrCreate(typeof(TinaX.IXCore), out var mapping);

            mapping.Register("GetService", 1, 1, GetService_TService_ArrObject);
            mapping.Register("GetService", 0, 2, new string[] {
                "System.Type",
                "System.Object[]"
            }, GetService_Type_ArrObject);

            mapping.Register("TryGetService", 1, 2, TryGetService_TService_ArrObject);
            mapping.Register("TryGetService", 0, 3, new string[] {
                "System.Type",
                "System.Object&",
                "System.Object[]"
            }, TryGetService_Type_ArrObject);

            mapping.Register("TryGetBuildInService", 1, 1, IXCore_TryGetBuildInService_TService);
            mapping.Register("BindService", 2, 0, BindService_TService_TConcrete);
            mapping.Register("BindSingletonService", 2, 0, BindSingletonService_TService_TConcrete);

            mapping.Register("InjectObject", 0, 1, XCore_Inject_Object);
        }



        //TService Get<TService>(params object[] userParams);
        private static StackObject* GetService_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
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

        //object GetService(Type type, params object[] userParams);
        private static StackObject* GetService_Type_ArrObject(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
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

            

            string serviceName = XILUtil.GetCatLibServiceName(@type);
            var result_of_this_method = XCore.MainInstance.Services.Get(serviceName, @userParams);

            object obj_result_of_this_method = result_of_this_method;
            if (obj_result_of_this_method is CrossBindingAdaptorType)
            {
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        //bool TryGetService<TService>(out TService service, params object[] userParams);
        private static StackObject* TryGetService_TService_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
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


            intp.Free(ptrOfThisMethod);

            var result = XCore.MainInstance.Services.TryGet(tService, out service, userparams);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        //bool TryGetService(Type type, out object service, params object[] userParams);
        private static StackObject* TryGetService_Type_ArrObject(ILIntepreter intp, StackObject* esp, IList<object> mStack,
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
            intp.Free(ptrOfThisMethod);

            var result = XCore.MainInstance.Services.TryGet(tService, out service, userparams);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 2);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        private static StackObject* IXCore_TryGetBuildInService_TService(ILIntepreter intp, StackObject* esp, IList<object> mStack,
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

            intp.Free(ptrOfThisMethod);
            var result = XCore.MainInstance.Services.TryGetBuildInService(tService, out service);

            ptrOfThisMethod = ILIntepreter.Minus(esp, 1);
            XILUtil.SetValue(ptrOfThisMethod, mStack, intp.AppDomain, service);

            return ILIntepreter.PushObject(ILIntepreter.Minus(esp, 1), mStack, result);
        }

        //void BindService<TService, TConcrete>();
        static StackObject* BindService_TService_TConcrete(ILIntepreter __intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            var genericArguments = method.GenericArguments;
            if (genericArguments == null || genericArguments.Length != 2 || method.ParameterCount != 0)
            {
                throw new EntryPointNotFoundException();
            }

            var tService = XILUtil.ITypeToService(genericArguments[0]);
            var tType = XILUtil.ITypeToClrType(genericArguments[1]);

            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            TinaX.IXCore instance_of_this_method = (TinaX.IXCore)typeof(TinaX.IXCore).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Services.Bind(tService, tType,false);
            //instance_of_this_method.BindService<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            return __ret;
        }

        static StackObject* BindSingletonService_TService_TConcrete(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
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
            TinaX.IXCore instance_of_this_method = (TinaX.IXCore)typeof(TinaX.IXCore).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, mStack));
            intp.Free(ptr_of_this_method);

            instance_of_this_method.Services.Bind(tService, tType, true);
            //instance_of_this_method.BindSingletonService<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            return __ret;
        }

        private static StackObject* XCore_Inject_Object(ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain domain = intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(esp, 1);
            System.Object target = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, domain, mStack));
            intp.Free(ptr_of_this_method);
            
            XCore.MainInstance.Services.Get<IXILRuntime>().InjectObject(target);

            return __ret;
        }

        

    }
}
