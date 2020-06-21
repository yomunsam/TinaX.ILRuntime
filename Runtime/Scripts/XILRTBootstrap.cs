namespace TinaX.XILRuntime.Internal
{
    public class XILRTBootstrap : IXBootstrap
    {
        public void OnInit(IXCore core) { }

        public void OnStart(IXCore core)
        {
            if(core.Services.TryGet<IXILRTInternal>(out var ilrt))
            {
                ilrt.InvokeEntryMethod();
            }
        }

        public void OnAppRestart() { }
        public void OnQuit() { }
    }
}
