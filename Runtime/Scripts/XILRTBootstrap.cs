namespace TinaX.XILRuntime.Internal
{
    public class XILRTBootstrap : IXBootstrap
    {
        public void OnInit(IXCore core) { }

        public async void OnStart(IXCore core)
        {
            if(core.Services.TryGet<IXILRTInternal>(out var ilrt))
            {
                await ilrt.InvokeEntryMethod();
            }
        }

        public void OnAppRestart() { }
        public void OnQuit() { }
    }
}
