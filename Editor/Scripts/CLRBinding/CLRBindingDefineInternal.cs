using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;
using TinaX;
using UnityEngine;

namespace TinaXEditor.XILRuntime.Internal
{
    internal static class CLRBindingDefineInternal
    {
        public static List<Type> BindTypes => new List<Type>()
        {
            typeof(List<ILTypeInstance>),
            //C#
            typeof(int),
            typeof(float),
            typeof(long),
            typeof(object),
            typeof(string),
            typeof(Array),

            //TinaX
            typeof(IXCore),

            //Unity
            typeof(GameObject),
            typeof(UnityEngine.Object),
            typeof(Transform),
            typeof(RectTransform),
            typeof(Debug),
            typeof(RectTransform),
            typeof(Time),    

        };


    }
}
