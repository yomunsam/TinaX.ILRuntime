using System;
using TinaX.XILRuntime.Internal.Adaptors;

namespace TinaX.XILRuntime.Internal
{
    internal unsafe static class InternalRegisters
    {
        internal static void RegisterDelegates(IXILRuntime xil)
        {
            xil.DelegateManager.RegisterMethodDelegate<TinaX.Container.IServiceContainer>();
            xil.DelegateManager.RegisterMethodDelegate<UniRx.Unit>();
            xil.DelegateManager.RegisterMethodDelegate<System.Exception>();

            xil.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

        }

        internal static void RegisterCLRMethodRedirections(IXILRuntime xil)
        {
            //CLR重定向
            Redirect.RedirectXIL.Register(xil);
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(xil.ILRuntimeAppDomain);
        }

        internal static void RegisterCrossBindingAdaptors(IXILRuntime xil)
        {
            xil.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor()); //用于Async/await
        }


    }
}
