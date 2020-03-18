using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.XILRuntime.Internal.CLRRedirection
{
    public unsafe static class UnityDebugCLR
    {
        /// <summary>
        /// Debug.Log(object message)
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        public unsafe static StackObject* Log(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //ILRuntime的调用约定为被调用者清理堆栈，因此执行这个函数后需要将参数从堆栈清理干净，并把返回值放在栈顶，具体请看ILRuntime实现原理文档
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            //这个是最后方法返回后esp栈指针的值，应该返回清理完参数并指向返回值，这里是只需要返回清理完参数的值即可
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            //取Log方法的参数，如果有两个参数的话，第一个参数是esp - 2,第二个参数是esp -1, 因为Mono的bug，直接-2值会错误，所以要调用ILIntepreter.Minus
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);

            //这里是将栈指针上的值转换成object，如果是基础类型可直接通过ptr->Value和ptr->ValueLow访问到值，具体请看ILRuntime实现原理文档
            object message = typeof(object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            //所有非基础类型都得调用Free来释放托管堆栈
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            //我们在输出信息后面加上DLL堆栈
            UnityEngine.Debug.Log(message + "\n" + stacktrace);

            return __ret;
        }

        /// <summary>
        /// Debug.Log(object message, UnityEngine.Object context)
        /// </summary>
        /// <param name="__intp"></param>
        /// <param name="__esp"></param>
        /// <param name="__mStack"></param>
        /// <param name="__method"></param>
        /// <param name="isNewObj"></param>
        /// <returns></returns>
        public static StackObject* Log_with_context(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Object @context = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object @message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            UnityEngine.Debug.Log(@message + "\n" + stacktrace, @context);

            return __ret;
        }
    }
}
