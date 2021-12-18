using System.Collections.Generic;
using System.Linq;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using TinaX.Core.Helper.LogColor;
using TinaX.Core.Utils;
using TinaX.XILRuntime.Consts;
using UnityEngine;

namespace TinaX.XILRuntime.Redirects
{
    internal static unsafe partial class RedirectXIL
    {
        private static void Register_TinaX_XILRuntime_LogOptions()
        {
            GetMappingOrCreate(typeof(TinaX.XILRuntime.LogOptions), out var mapping);

            mapping.Register("set_Enable", 0, 1, il_logOptions_Enable_bool);
        }

        static StackObject* il_logOptions_Enable_bool(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            bool @value = ptr_of_this_method->Value == 1;

            TinaX.XILRuntime.LogOptions.Enable = value;
            //输出日志
            var stackTrace = __domain.DebugService.GetStackTrace(__intp);
            if (LocalizationUtil.IsHans())
                UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime环境的Log功能已{(value ? "启用" : "禁用")}</color>, 该操作来自ILRuntime环境，堆栈跟踪:\n{RemoveUselessStackTrace(stackTrace)}\n");
            else
                UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime log is {(value ? "enabled" : "disabled")}</color>, stackTrace from ilruntime appdomain:\n{RemoveUselessStackTrace(stackTrace)}\n");

            return __ret;
        }

        static string RemoveUselessStackTrace(string source)
        {
            //获取到的跟踪堆栈第一行是无用的，类似“IL_00cd: call System.Void TinaX.XILRuntime.LogOptions::set_LogEnable(System.Boolean)”这种， 这个方法把第一行删了
            var str_arr = source.Split('\n');
            if (str_arr.Length < 2)
                return source;
            else
            {
                return string.Join("\n", str_arr.Skip(1));
            }
        }
    }
}
