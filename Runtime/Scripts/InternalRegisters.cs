using TinaX.XILRuntime.Internal.Adaptors;

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

        internal static void RegisterCrossBindingAdaptors(IXILRuntime xil)
        {
            xil.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor()); //用于Async/await
        }


    }
}
