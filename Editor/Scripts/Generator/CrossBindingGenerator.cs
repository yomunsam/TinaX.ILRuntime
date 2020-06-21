using System;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.XILRuntime.Generator
{
    public static class CrossBindingGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPath">output path | 输出路径</param>
        /// <param name="baseType"></param>
        /// <param name="targetNamespace"></param>
        public static void Generate(string targetPath, Type baseType, string targetNamespace)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(targetPath)) //Assets/Samples/ILRuntime/1.6/Demo/Scripts/Examples/04_Inheritance/InheritanceAdapter.cs
            {
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(baseType, targetNamespace));
            }

            Debug.Log($"<color=#{TinaX.Internal.XEditorColorDefine.Color_Safe_16}>Generate cross binding code finish.</color>: " + targetPath);
            AssetDatabase.Refresh();
        }
    }
}
