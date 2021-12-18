using System.Linq;
using TinaX.Core.Helper.LogColor;
using TinaX.Core.Utils;
using TinaX.XILRuntime.Consts;

namespace TinaX.XILRuntime
{
    public static class LogOptions
    {
        //------日志开关------
        private static bool _enable = true;         //总开关
        private static bool _enable_log = true;     //"Log"级别

        //------堆栈跟踪开关------
        private static bool _stackTrace_log = true; //"Log"级别

        //------前缀------
        private static bool _enablePrefix = true;
        private static string _prefixText = "[ILRuntime]";

        /// <summary>
        /// 是否输出日志的全局总开关
        /// </summary>
        public static bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                GetStackTrace(out bool fromILRuntime, out string stackTrace);
                if (!fromILRuntime)
                {
                    if (LocalizationUtil.IsHans())
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime环境的Log功能已{(value ? "启用" : "禁用")}</color>, 跟踪堆栈:\n{stackTrace}\n");
                    else
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime log is {(value ? "enabled" : "disabled")}</color>, stackTrace:\n{stackTrace}\n");
                }
            }
        }

        /// <summary>
        /// 是否输出"Log"级别的日志
        /// </summary>
        public static bool EnableLog
        {
            get => _enable_log;
            set
            {
                _enable_log = value;
                GetStackTrace(out bool fromILRuntime, out string stackTrace);
                if (!fromILRuntime)
                {
                    if (LocalizationUtil.IsHans())
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime环境的Log功能（级别：\"Log\"）已{(value ? "启用" : "禁用")}</color>, 跟踪堆栈:\n{stackTrace}\n");
                    else
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime log (Type:\"Log\") is {(value ? "enabled" : "disabled")}</color>, stackTrace:\n{stackTrace}\n");
                }
            }
        }



        public static bool StackTraceLog
        {
            get => _stackTrace_log;
            set
            {
                _stackTrace_log = value;
                GetStackTrace(out bool fromILRuntime, out string stackTrace);
                if (!fromILRuntime)
                {
                    if (LocalizationUtil.IsHans())
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>ILRuntime环境的Log（级别：\"Log\"）已{(value ? "启用" : "禁用")}输出堆栈跟踪</color>, 跟踪堆栈:\n{stackTrace}\n");
                    else
                        UnityEngine.Debug.Log($"[{XILConsts.ModuleName}]<color=#{LogColorHelper.Color_Primary_16}>The printing stack trace function of ilruntime log (Tyoe:\"Log\") is {(value ? "enabled" : "disabled")}</color>, stackTrace:\n{stackTrace}\n");
                }
            }
        }



        public static bool EnablePrefix
        {
            get => _enablePrefix;
            set
            {
                _enablePrefix = value;
            }
        }

        public static string PrefixText
        {
            get => _prefixText;
            set
            {
                _prefixText = value;
            }
        }

        /// <summary>
        /// 获取当前程序调用堆栈
        /// </summary>
        /// <param name="fromILRuntime">调用是否来自ILRuntime</param>
        /// <param name="stackTrace"></param>
        private static void GetStackTrace(out bool fromILRuntime, out string stackTrace)
        {
            stackTrace = UnityEngine.StackTraceUtility.ExtractStackTrace();
            if (stackTrace.Contains("TinaX.XILRuntime.Redirects.RedirectXIL"))//热更工程调用到这里，必须走一遍CLR重定向，所以判断这个地方就行
            {
                fromILRuntime = true;
            }
            else
            {
                fromILRuntime = false;
                //对于来自原生工程的跟踪堆栈，前面三行是没用的，它们来自这个类的内部，给删掉
                var str_arr = stackTrace.Split('\n');
                if(str_arr.Length > 3)
                {
                    stackTrace = string.Join("\n", str_arr.Skip(3));
                }
            }
        }
    }
}
