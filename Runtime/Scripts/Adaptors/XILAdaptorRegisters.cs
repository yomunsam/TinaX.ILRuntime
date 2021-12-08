using TinaX.XILRuntime.Adaptors.Async;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace TinaX.XILRuntime.Adaptors
{
    /// <summary>
    /// TinaX ILRuntime 模块 内部适配器 注册
    /// </summary>
    public class XILAdaptorRegisters
    {
        /// <summary>
        /// 注册 跨域委托适配器
        /// </summary>
        /// <param name="xil"></param>
        public static void RegisterCrossBindingAdaptors(IXILRuntime xil)
        {
            RegisterCrossBindingAdaptors(xil.ILRuntimeAppDomain);
        }

        public static void RegisterCrossBindingAdaptors(ILAppDomain appdomain) 
        {
            appdomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        }
    }
}
