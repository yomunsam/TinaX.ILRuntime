using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime.Internal.Adaptor
{
    static class AdaptorRegister
    {
        public static void Register(XRuntime runtime)
        {
            runtime.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        }
    }
}
