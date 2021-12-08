using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using UnityEngine;

namespace TinaX.XILRuntime.Redirects
{

    //关于UnityEngine.Debug的重定向

    internal static unsafe partial class RedirectXIL
    {
        private static void Register_UnityEngine_Debug()
        {
            GetMappingOrCreate(typeof(UnityEngine.Debug), out var mapping);

            mapping.Register("Log", 0, 1, Log_Message);
        }

        static StackObject* Log_Message(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            //清理堆栈
            __intp.Free(ptr_of_this_method);

            //if (LogStackTrace)
            //{
            //    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            //    UnityEngine.Debug.Log($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}");
            //}
            //else
            //    UnityEngine.Debug.Log(GetLogMessage(message));

            string stackTrace = __domain.DebugService.GetStackTrace(__intp);
            Debug.Log(string.Format("{0}\n{1}", message, stackTrace));

            return __ret;
        }
    }
}
