using System;
using System.Collections.Generic;
using System.Reflection;
using TinaX.XILRuntime;

namespace TinaXEditor.XILRuntime
{
    public interface ICLRGenerateDefine
    {
        List<Type> GetCLRBindingTypes();
        HashSet<MethodBase> GetCLRBindingExcludeMethods();
        HashSet<FieldInfo> GetCLRBindingExcludeFields();

        /// <summary>
        /// 值类型绑定列表
        /// </summary>
        /// <returns></returns>
        List<Type> GetValueTypeBinders();

        List<Type> GetDelegateTypes();


        void GenerateByAssemblies_InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain);
    }
}
