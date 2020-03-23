using System;
using System.Reflection;
using TinaX.XILRuntime.Internal.CLRRedirection;
using UnityEngine;

namespace TinaX.XILRuntime.Internal
{
    internal class CLRRedirectionDefineInternal
    {
        private static readonly BindingFlags flag = (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        public unsafe static CLRRedirectionInfo[] Redirections => new CLRRedirectionInfo[]
        {
            //TinaX.Core-----------------------------
            new CLRRedirectionInfo()
            {
                method = typeof(XCore).GetMethod("CreateInstance",flag,null,new Type[]{ typeof(Type), typeof(object[]) },null),
                func = TinaXCoreCLR.CreateInstance,
            },
            new CLRRedirectionInfo()
            {
                method = typeof(IXCore).GetMethod("CreateInstance",flag,null,new Type[]{ typeof(Type), typeof(object[]) },null),
                func = TinaXCoreCLR.CreateInstance,
            },

            //Unity------------------------------------
            new CLRRedirectionInfo() //Debug.Log
            {
                method = typeof(Debug).GetMethod("Log",flag,null,new Type[]{ typeof(object)},null),
                func = UnityDebugCLR.Log
            },
            new CLRRedirectionInfo() //Debug.Log
            {
                method = typeof(Debug).GetMethod("Log",flag,null,new Type[]{ typeof(object), typeof(UnityEngine.Object)}, null),
                func = UnityDebugCLR.Log_with_context
            },
        };
    }
}
