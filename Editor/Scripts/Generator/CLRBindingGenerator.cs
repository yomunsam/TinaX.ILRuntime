using System.Collections.Generic;
using System.IO;
using ILRuntime.Runtime.CLRBinding;
using TinaX;
using TinaX.Core.Helper.LogColor;
using TinaX.Core.Utils;
using TinaX.XILRuntime.Adaptors;
using UnityEngine;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace TinaXEditor.XILRuntime.Generator
{
    /// <summary>
    /// CLR绑定代码生成器
    /// </summary>
    public class CLRBindingGenerator
    {
        /// <summary>
        /// 分析Assembly并生成代码
        /// </summary>
        /// <param name="outputPath">输出路径</param>
        static void GenerateByAnalysis(IEnumerable<string> assemblyPaths, string outputPath)
        {
            if(!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            ILAppDomain appDomain = new ILAppDomain();
            DisposableGroup disposableGroup = new DisposableGroup();
            foreach(var path in assemblyPaths)
            {
                if (path.IsNullOrEmpty())
                    continue;
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                disposableGroup.Register(fs);
                appDomain.LoadAssembly(fs);
            }

            //注册内置的跨域绑定适配器们
            XILAdaptorRegisters.RegisterCrossBindingAdaptors(appDomain);
            BindingCodeGenerator.GenerateBindingCode(appDomain, outputPath);

            disposableGroup.Dispose();
            Debug.Log(LogColorHelper.PrimaryLog("Generate code finish."));
        }
    }
}