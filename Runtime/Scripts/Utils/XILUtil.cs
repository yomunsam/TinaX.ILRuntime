/*
 * 此处部分搬运CatLib代码（MIT License）
 * https://github.com/CatLib/CatLib.ILRuntime/blob/master/src/Redirect/Helper.cs
 * 让我们感谢神奇的喵大
 */

using CatLib;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TinaX.XILRuntime.Utils
{
    public static class XILUtil
    {
        //参考 https://github.com/CatLib/CatLib.ILRuntime/blob/de7ad644efbb5a41db7a0ba33d4fa22a727c8795/src/ILRuntimeApplication.cs
        internal static string GetCatLibServiceName(PropertyInfo property)
        {
            if (property is ILRuntimePropertyInfo)
                return property.PropertyType.FullName;
            else
                return XCore.GetMainInstance().Services.CatApplication.Type2Service(property.PropertyType);
        }

        /// <summary>
        /// 获取CatLib中的服务名
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static string GetCatLibServiceName(FieldInfo field)
        {
            if (field is ILRuntimeFieldInfo)
                return field.FieldType.FullName;
            else
                return XCore.GetMainInstance().Services.CatApplication.Type2Service(field.FieldType);
        }

        internal static string GetCatLibServiceName(Type type)
        {
            if (type is ILRuntimeType || type is ILRuntimeWrapperType)
                return type.FullName;
            else
                return XCore.GetMainInstance().Services.CatApplication.Type2Service(type);
        }


        /// <summary>
        /// 将IType转为字符串
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>字符串</returns>
        public static string ITypeToService(IType type)
        {
            var ilType = type as ILType;
            return ilType != null ? ilType.FullName : XCore.GetMainInstance().Services.Type2ServiceName(type.TypeForCLR);
        }

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
