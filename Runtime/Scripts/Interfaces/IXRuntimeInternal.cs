using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.ILRuntime.Exceptions;

namespace TinaX.ILRuntime.Internal
{
    public interface IXRuntimeInternal
    {
        XRTException GetStartException();
        void InvokeEntryMathod();
        Task<bool> Start();
    }
}
