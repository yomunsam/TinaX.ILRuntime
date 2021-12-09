using System;
using ILRuntime.Reflection;
using TinaX.Core.Activator;
using TinaX.XILRuntime.Consts;

namespace TinaX.XILRuntime.CreateInstance
{
    /// <summary>
    /// TinaX.ILRuntime 实例创建器
    /// </summary>
    public class XILInstanceCreator : ICreateInstance
    {
        private readonly IXILRuntime m_xILRntime;

        public XILInstanceCreator(IXILRuntime xil)
        {
            m_xILRntime = xil;
        }


        public string ProviderName => XILConsts.ModuleName;

        public bool TryCreateInstance(Type type, out object result, params object[] args)
        {
            if (type is ILRuntimeType || type is ILRuntimeWrapperType)
            {
                result = m_xILRntime.ILRuntimeAppDomain.Instantiate(type.FullName, args);
                return true;
            }
            result = null;
            return false;
        }
    }
}
