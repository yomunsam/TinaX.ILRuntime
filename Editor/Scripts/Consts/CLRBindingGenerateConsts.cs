using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TinaXEditor.XILRuntime.Consts
{
    /// <summary>
    /// CLR绑定生成定义
    /// </summary>
    public static class CLRBindingGenerateConsts
    {
        public static List<Type> ValueTypeConst = new List<Type>
        {
            //UnityEngine
            typeof(Vector2),
            typeof(Vector3),

            //UniTask
            typeof(UniTask),

            //.NET
            typeof(CancellationToken)
        };

        public static List<Type> DelegateTypes = new List<Type>
        {

        };
    }
}
