using ILRuntime.CLR.Method;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System.Collections.Generic;
using TinaX.XILRuntime.Config;

namespace TinaX.XILRuntime.Internal.Redirect
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_UnityEngine_Debug()
        {
            GetMappingOrCreate(typeof(UnityEngine.Debug), out var mapping);

            mapping.Register("Log", 0, 1, Log_Message);
            mapping.Register("Log", 0, 2, Log_Message_Context);
            mapping.Register("LogWarning", 0, 1, LogWarning_Message);
            mapping.Register("LogWarning", 0, 2, LogWarning_Message_Context);
            mapping.Register("LogError", 0, 1, LogError_Message);
            mapping.Register("LogError", 0, 2, LogError_Message_Context);
        }

        private static DebugLogConfig _logConfig;

        private static bool LogStackTrace
        {
            get
            {
                if(_logConfig == null)
                {
                    _logConfig = XCore.GetMainInstance().Services.Get<IXILRuntime>().DebugLogConfig;
                }
                return _logConfig.LogStackTrace;
            }
        }

        private static bool WarningStackTrace
        {
            get
            {
                if (_logConfig == null)
                {
                    _logConfig = XCore.GetMainInstance().Services.Get<IXILRuntime>().DebugLogConfig;
                }
                return _logConfig.WarningStackTrace;
            }
        }

        private static bool ErrorStackTrace
        {
            get
            {
                if (_logConfig == null)
                {
                    _logConfig = XCore.GetMainInstance().Services.Get<IXILRuntime>().DebugLogConfig;
                }
                return _logConfig.ErrorStackTrace;
            }
        }

        private static object GetLogMessage(object message)
        {
            if (_logConfig == null)
            {
                _logConfig = XCore.GetMainInstance().Services.Get<IXILRuntime>().DebugLogConfig;
            }
            if (_logConfig.EnablePrefix)
                return $"{_logConfig.PrefixText}{message}";
            else
                return message;
        }

        static StackObject* Log_Message(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (LogStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.Log($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}");
            }
            else
                UnityEngine.Debug.Log(GetLogMessage(message));

            return __ret;
        }

        static StackObject* Log_Message_Context(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Object @context = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (LogStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.Log($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}", @context);
            }
            else
                UnityEngine.Debug.Log(GetLogMessage(message), @context);

            return __ret;
        }

        static StackObject* LogWarning_Message(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (WarningStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.LogWarning($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}");
            }
            else
                UnityEngine.Debug.LogWarning(GetLogMessage(message));

            return __ret;
        }

        static StackObject* LogWarning_Message_Context(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Object @context = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (WarningStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.LogWarning($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}", @context);
            }
            else
                UnityEngine.Debug.LogWarning(GetLogMessage(message), @context);

            return __ret;
        }


        static StackObject* LogError_Message(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (ErrorStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.LogError($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}");
            }
            else
                UnityEngine.Debug.LogError(GetLogMessage(message));

            return __ret;
        }

        static StackObject* LogError_Message_Context(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Object @context = (UnityEngine.Object)typeof(UnityEngine.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            if (ErrorStackTrace)
            {
                string stackTrace = __domain.DebugService.GetStackTrace(__intp);
                UnityEngine.Debug.LogError($"{GetLogMessage(message)}\n\nStackTrace:\n{stackTrace}", @context);
            }
            else
                UnityEngine.Debug.LogError(GetLogMessage(message), @context);

            return __ret;
        }

    }
}
