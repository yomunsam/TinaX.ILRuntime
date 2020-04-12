using ILRuntime.Mono.Cecil.Cil;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime
{
    public interface IXRuntime
    {
        ILRuntime.Runtime.Enviorment.AppDomain ILAppDomain { get; }

        object CreateInstance(Type type, params object[] args);
        object Invoke(string type, string method, object instance = null, params object[] param);

        void RegisterCLRMethodRedirection(MethodBase method, CLRRedirectionDelegate func);
        void RegisterCLRMethodRedirection(IEnumerable<CLRRedirectionInfo> funcs);
        void RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor);
    }
}
