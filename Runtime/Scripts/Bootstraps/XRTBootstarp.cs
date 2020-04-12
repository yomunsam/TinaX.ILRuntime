using TinaX;
using System;

namespace TinaX.XILRuntime.Internal
{
    public class XRTBootstarp : IXBootstrap
    {

        public void OnInit() { }

        public void OnStart()
        {
            if (XCore.MainInstance.TryGetService<IXRuntimeInternal>(out var service))
            {
                try
                {
                    service.InvokeEntryMathod();
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        public void OnAppRestart() { }

        public void OnQuit() { }

    }
}
