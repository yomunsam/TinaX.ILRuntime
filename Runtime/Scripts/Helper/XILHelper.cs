using System;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;

namespace TinaX.XILRuntime.Helper
{
    internal static unsafe class XILHelper
    {
        /// <summary>
        /// 将IType转为Clr Type类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型</returns>
        public static Type ITypeToClrType(IType type)
        {
            var ilType = type as ILType;
            return ilType != null ? new ILRuntimeType(ilType) : type.TypeForCLR;
        }
    }
}
