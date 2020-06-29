using ILRuntime.Runtime.Enviorment;
using System;
using System.Reflection;
using TinaX.XILRuntime.Config;

namespace TinaX.XILRuntime
{
    public interface IXILRuntime
    {
        bool LoadSymbol { get; set; }
        DelegateManager DelegateManager { get; }
        ILRuntime.Runtime.Enviorment.AppDomain ILRuntimeAppDomain { get; }
        DebugLogConfig DebugLogConfig { get; }

        object CreateInstance(Type type, params object[] args);
        object CreateInstanceAndInject(Type type);
        void InjectObject(object obj);
        void InvokeILMethod(string type, string method, params object[] args);
        IXILRuntime RegisterCLRMethodRedirection(MethodBase method, CLRRedirectionDelegate func);
        IXILRuntime RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor);
        IXILRuntime RegisterValueTypeBinder(Type type, ValueTypeBinder binder);
    }
}
