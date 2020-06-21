using ILRuntime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime.Internal
{
    internal unsafe static class InternalRegisters
    {
        internal static void RegisterDelegates(IXILRuntime xil)
        {
            xil.DelegateManager.RegisterMethodDelegate<TinaX.Container.IServiceContainer>();
        }

        internal static void RegisterCLRMethodRedirections(IXILRuntime xil)
        {
            //CLR重定向
            Redirect.RedirectXIL.Register(xil);
        }


    }
}
