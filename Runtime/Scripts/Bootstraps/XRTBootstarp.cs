using TinaX;

namespace TinaX.XILRuntime.Internal
{
    public class XRTBootstarp : IXBootstrap
    {

        public void OnInit() { }

        public void OnStart()
        {
            if (XCore.MainInstance.TryGetService<IXRuntimeInternal>(out var service))
                service.InvokeEntryMathod();
        }

        public void OnAppRestart() { }

        public void OnQuit() { }

    }
}
