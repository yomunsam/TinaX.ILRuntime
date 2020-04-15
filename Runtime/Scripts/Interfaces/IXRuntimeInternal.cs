using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaX.XILRuntime.Exceptions;

namespace TinaX.XILRuntime.Internal
{
    public interface IXRuntimeInternal
    {
        XException GetStartException();
        void InvokeEntryMathod();
        Task<bool> Start();
    }
}
