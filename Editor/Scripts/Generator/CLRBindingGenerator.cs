using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ILRuntime.Runtime.CLRBinding;
using TinaX;
using TinaX.Core.Helper.LogColor;
using TinaX.Core.Utils;
using TinaX.XILRuntime.Adaptors;
using TinaX.XILRuntime.Consts;
using TinaXEditor.XILRuntime.ConfigAssets;
using TinaXEditor.XILRuntime.Consts;
using UnityEditor;
using UnityEngine;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace TinaXEditor.XILRuntime.Generator
{
    /// <summary>
    /// CLR绑定代码生成器
    /// </summary>
    public class CLRBindingGenerator
    {
        

        public static void GenerateByAnalysisFromConfiguration()
        {
            var conf = XILRuntimeEditorConfigAsset.GetInstance();
            var output_path = conf.BindingCodeOutputPath;
            if (!output_path.StartsWith("Assets/"))
            {
                Debug.LogError($"[{XILConsts.ModuleName}]Generate failed: Output folder path is invalid : {output_path}");
                return;
            }
            GenerateByAnalysis(conf.EditorLoadAssemblies.Select(a => a.AssemblyPath), output_path);
            AssetDatabase.Refresh();
        }

        [MenuItem("TinaX/ILRuntime/Generator/Generate clr binding code")]
        public static void GenerateByAnalysis_Menu()
        {
            GenerateByAnalysisFromConfiguration();
        }

        /// <summary>
        /// 分析Assembly并生成代码
        /// </summary>
        /// <param name="outputPath">输出路径</param>
        public static void GenerateByAnalysis(IEnumerable<string> assemblyPaths, string outputPath, List<Type> valueTypeBinders = null, List<Type> delegateTypes = null, params string[] excludeFiles)
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

            if(valueTypeBinders == null)
                valueTypeBinders = new List<Type>();
            valueTypeBinders.AddRange(CLRBindingGenerateConsts.ValueTypeConst);

            if(delegateTypes == null)
                delegateTypes = new List<Type>();
            delegateTypes.AddRange(CLRBindingGenerateConsts.DelegateTypes);

            //反射所有配置接口
            Type t_define = typeof(ICLRBindingGenerateDefine);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(t_define)))
                .ToArray();
            if (types.Length > 0)
            {
                foreach (var type in types)
                {
                    ICLRBindingGenerateDefine define_obj = (ICLRBindingGenerateDefine)Activator.CreateInstance(type);

                    //调用初始化方法
                    define_obj.Initialize(ref appDomain);

                    var _valueTypes = define_obj.ValueTypeBinders;
                    if (_valueTypes != null && _valueTypes.Count > 0)
                    {
                        foreach (var __valueType in _valueTypes)
                        {
                            if (!valueTypeBinders.Contains(__valueType))
                                valueTypeBinders.Add(__valueType);
                        }
                    }

                    var _delegates = define_obj.DelegateTypes;
                    if (_delegates != null && _delegates.Count > 0)
                    {
                        foreach (var __delegate in _delegates)
                        {
                            if (!delegateTypes.Contains(__delegate))
                                delegateTypes.Add(__delegate);
                        }
                    }
                }
            }


            //注册内置的跨域绑定适配器们
            XILAdaptorRegisters.RegisterCrossBindingAdaptors(appDomain);
            BindingCodeGenerator.GenerateBindingCode(appDomain, outputPath, valueTypeBinders, delegateTypes, excludeFiles);

            disposableGroup.Dispose();
            Debug.Log(LogColorHelper.PrimaryLog("Generate code finish."));
        }
    }
}