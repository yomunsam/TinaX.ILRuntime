using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.XILRuntime.Adaptors.Async;

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
            xil.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        }
    }
}
