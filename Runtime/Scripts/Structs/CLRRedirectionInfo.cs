using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime
{
    public struct CLRRedirectionInfo
    {
        public System.Reflection.MethodInfo method;
        public ILRuntime.Runtime.Enviorment.CLRRedirectionDelegate func;
        
    }
}
