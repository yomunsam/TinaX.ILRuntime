using System;
using System.Collections.Generic;

namespace TinaXEditor.XILRuntime.Generator
{
    public interface ICLRBindingGenerateDefine
    {
        List<Type> ValueTypeBinders { get; }

        List<Type> DelegateTypes { get; }

        void Initialize(ref ILRuntime.Runtime.Enviorment.AppDomain appDomain);

    }
}
