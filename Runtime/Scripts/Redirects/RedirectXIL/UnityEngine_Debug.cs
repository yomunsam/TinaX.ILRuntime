using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using TinaX.Core.Helper.LogColor;
using TinaX.Core.Utils;
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

        private static bool isHans = LocalizationUtil.IsHans();

        static StackObject* Log_Message(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            //清理堆栈
            __intp.Free(ptr_of_this_method);

            
            bool log_available = true;      //可否输出
            bool enable_stackTrace = LogOptions.StackTraceLog;  //打印堆栈跟踪
            if (!LogOptions.Enable || !LogOptions.EnableLog)
            {
                log_available = false; //可否输出日志的总开关
            }

            if (log_available)
            {
                if (enable_stackTrace)
                {
                    string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                    Debug.Log($"{(LogOptions.EnablePrefix ? LogOptions.PrefixText : string.Empty)}{message}\n\n<color=#{LogColorHelper.Color_Primary_16}>{(isHans ? "堆栈跟踪：" : "StackTrace:")}</color>\n{stackTrace}\n");
                }
                else
                {
                    Debug.Log($"{(LogOptions.EnablePrefix ? LogOptions.PrefixText : string.Empty)}{message}");
                }
            }

            return __ret;
        }
    }
}
