using System;
using ILRuntime.Reflection;
using TinaX.Core.Container;
using TinaX.XILRuntime.Consts;
using UnityEngine;

namespace TinaX.XILRuntime.Services
{
    /// <summary>
    /// TinaX.ILRuntime 实例创建器
    /// </summary>
    public class XILInstanceCreator : IInstanceCreator
    {
        private readonly IXILRuntime m_xILRntime;

        public XILInstanceCreator(IXILRuntime xil)
        {
            this.m_xILRntime = xil;
        }

        public string ImplementerName => XILConsts.ModuleName;

        public bool TryCreateInstance(Type type, out object result, object[] args)
        {
            Debug.Log("尝试通过ILRuntime创建实例");
            if (type is ILRuntimeType || type is ILRuntimeWrapperType)
            {
                Debug.Log("    真的通过ILRuntime创建了实例：" + type.FullName);
                result = m_xILRntime.ILRuntimeAppDomain.Instantiate(type.FullName, args);
                return true;
            }
            result = null;
            return false;
        }
    }
}
