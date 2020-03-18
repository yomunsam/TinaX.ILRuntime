namespace TinaX.ILRuntime.Exceptions
{
    public class XRTException : XException
    {
        public XRTException(string msg, int errorCode) : base(msg, errorCode)
        {
        }

        public XRTException(string msg, XRuntimeErrorCode errorCode) : base(msg, (int)errorCode) { }
    }
}
