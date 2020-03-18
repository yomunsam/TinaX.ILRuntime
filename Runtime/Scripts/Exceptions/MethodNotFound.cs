namespace TinaX.XILRuntime.Exceptions
{
    public class MethodNotFound : XRTException
    {
        public string MethodName { get; private protected set; }
        public string TypeName { get; private protected set; }
        public MethodNotFound(string msg,string typeName, string methodName) : base(msg, XRuntimeErrorCode.MethodNotFound)
        {
            TypeName = typeName;
            MethodName = methodName;
        }

        public MethodNotFound(string typeName,string methodName):base($"[TinaX.ILRuntime]The Method \"{methodName}\" in type \"{typeName}\" not found.", XRuntimeErrorCode.MethodNotFound)
        {
            TypeName = typeName;
            MethodName = methodName;
        }
    }
}
