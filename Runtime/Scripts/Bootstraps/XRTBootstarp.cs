using TinaX;

namespace TinaX.ILRuntime.Internal
{
    public class XRTBootstarp : IXBootstrap
    {

        public void OnInit() { }

        public void OnStart()
        {
            XCore.MainInstance.GetService<IXRuntimeInternal>().InvokeEntryMathod();
        }

        public void OnAppRestart() { }

        public void OnQuit() { }

    }
}
