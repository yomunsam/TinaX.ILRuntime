namespace TinaX.XILRuntime.Exceptions
{
    public class XILRTException : XException
    {
        public XILRTException(string msg) : base($"[{Const.XILConst.ServiceName}]{msg}")
        {
        }
    }
}
