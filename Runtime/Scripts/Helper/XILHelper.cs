using System;
using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;

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

        /// <summary>
        /// 设定指定的ref或者out值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="ptrOfThisMethod">变量指针</param>
        /// <param name="mStack">调用堆栈</param>
        /// <param name="domain">域</param>
        /// <param name="value">值</param>
        internal unsafe static void SetValue<T>(StackObject* ptrOfThisMethod, IList<object> mStack, global::ILRuntime.Runtime.Enviorment.AppDomain domain, T value)
        {
            switch (ptrOfThisMethod->ObjectType)
            {
                case ObjectTypes.StackObjectReference:
                    {
                        var destination = *(StackObject**)&ptrOfThisMethod->Value;
                        object instance = value;
                        if (destination->ObjectType >= ObjectTypes.Object)
                        {
                            if (instance is CrossBindingAdaptorType)
                                instance = ((CrossBindingAdaptorType)instance).ILInstance;
                            mStack[destination->Value] = instance;
                        }
                        else
                        {
                            ILIntepreter.UnboxObject(destination, instance, mStack, domain);
                        }
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var instance = mStack[ptrOfThisMethod->Value];
                        var typeInstance = instance as ILTypeInstance;
                        if (typeInstance != null)
                        {
                            typeInstance[ptrOfThisMethod->ValueLow] = value;
                        }
                        else
                        {
                            var type = domain.GetType(instance.GetType()) as CLRType;
                            type.SetFieldValue(ptrOfThisMethod->ValueLow, ref instance, value);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var type = domain.GetType(ptrOfThisMethod->Value);
                        var ilType = type as ILType;
                        if (ilType != null)
                        {
                            ilType.StaticInstance[ptrOfThisMethod->ValueLow] = value;
                        }
                        else
                        {
                            ((CLRType)type).SetStaticFieldValue(ptrOfThisMethod->ValueLow, value);
                        }
                    }
                    break;
                case ObjectTypes.ArrayReference:
                    {
                        var instanceOfArrayReference = (T[])mStack[ptrOfThisMethod->Value];
                        instanceOfArrayReference[ptrOfThisMethod->ValueLow] = value;
                    }
                    break;
            }
        }
    }
}
