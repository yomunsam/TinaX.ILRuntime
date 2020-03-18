namespace TinaX.XILRuntime.Exceptions
{
    public class TypeNotFound : XRTException
    {
        public string TypeName { get; private protected set; }
        public TypeNotFound(string msg,string typeName) : base(msg, XRuntimeErrorCode.TypeNotFound)
        {
            TypeName = typeName;
        }

    }
}
