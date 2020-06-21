using ILRuntime.Runtime.Enviorment;
using System;
using System.Reflection;

namespace TinaX.XILRuntime
{
    public interface IXILRuntime
    {
        bool LoadSymbol { get; set; }
        DelegateManager DelegateManager { get; }

        object CreateInstance(Type type, params object[] args);
        void InjectObject(object obj);
        IXILRuntime RegisterCLRMethodRedirection(MethodBase method, CLRRedirectionDelegate func);
        IXILRuntime RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor);
    }
}
