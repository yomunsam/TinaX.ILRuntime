using System;
using TinaX.Exceptions;
using UObject = UnityEngine.Object;

namespace TinaX.XILRuntime.Delegates
{
    public class XILDelegateRegisters
    {
        public static void RegisterDelegateAdapters(IXILRuntime xil)
        {
            xil.DelegateManager.RegisterMethodDelegate<Exception>();
            xil.DelegateManager.RegisterMethodDelegate<UObject, XException>();

            //UnityAction
            xil.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });
        }
    }
}
