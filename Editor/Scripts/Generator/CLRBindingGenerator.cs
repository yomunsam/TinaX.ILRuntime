using System;
using System.Collections;
using System.Collections.Generic;
using TinaX;
using TinaX.Systems;
using UnityEngine;
using ILRuntime.Runtime.CLRBinding;
using TinaX.XILRuntime.Const;
using System.Linq;
using UnityEditor;
using System.Reflection;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using TinaX.Utils;
using System.IO;
using NUnit.Framework;

namespace TinaXEditor.XILRuntime.Generator
{
    static class CLRBindingGenerator
    {
        private static List<Type> s_InternalDefineTypes = new List<Type>
        {
            //C#
            typeof(int),
            typeof(long),
            typeof(object),
            typeof(string),
            typeof(Array),

            //TinaX
            typeof(IXCore),
            typeof(TinaX.Container.IServiceContainer),
            typeof(ITimeTicket),
            typeof(TimeMachine),
            typeof(IEventTicket),
            typeof(XEvent),

            //Unity
            typeof(GameObject),
            typeof(UnityEngine.Object),
            typeof(Transform),
            typeof(Debug),
            typeof(RectTransform),
            typeof(Time),
        };

        private static HashSet<MethodBase> s_InternalExcludeMethods = new HashSet<MethodBase>
        {
            typeof(float).GetMethod("IsFinite"),
        };

        private static HashSet<FieldInfo> s_InternalExcludeFields = new HashSet<FieldInfo>
        {

        };

        /// <summary>
        /// 值类型
        /// </summary>
        private static List<Type> s_InternalValueTypes = new List<Type>
        {
            typeof(Vector3),
            typeof(Vector2),
            typeof(Quaternion),
            typeof(Color),
        };

        private static List<Type> s_InternalDelegateTypes = new List<Type>
        {

        };


        [MenuItem("TinaX/X ILRuntime/Generator/Generate by define")]
        public static void GenCodeByTypes()
        {
            var config = XConfig.GetConfig<TinaX.XILRuntime.Internal.XILRTConfig>(XILConst.ConfigPath_Resources);
            if(config == null)
            {
                Debug.LogError($"[{XILConst.ServiceName}] Generate CLRBinding code failed: config file not found.");
                return;
            }
            string output_path = config.EditorCLRBindingCodeOutputPath;
            if (!output_path.StartsWith("Assets/"))
            {
                Debug.LogError($"[{XILConst.ServiceName}]Generate failed: Output folder path is invalid :" + output_path);
                return;
            }

            List<Type> gen_types = new List<Type>(s_InternalDefineTypes);
            HashSet<MethodBase> gen_exclude_methods = new HashSet<MethodBase>(s_InternalExcludeMethods);
            HashSet<FieldInfo> gen_exclude_fields = new HashSet<FieldInfo>(s_InternalExcludeFields);
            List<Type> gen_valueTypes = new List<Type>(s_InternalValueTypes);
            List<Type> gen_delegates = new List<Type>(s_InternalDelegateTypes);


            Type t_define = typeof(ICLRGenerateDefine);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(t_define)))
                .ToArray();
            if(types.Length > 0)
            {
                foreach(var type in types)
                {
                    ICLRGenerateDefine define_obj = (ICLRGenerateDefine)Activator.CreateInstance(type);
                    var _types = define_obj.GetCLRBindingTypes();
                    if(_types != null && _types.Count > 0)
                    {
                        foreach(var __type in _types)
                        {
                            if (!gen_types.Contains(__type))
                                gen_types.Add(__type);
                        }
                    }

                    var _exclude_methods = define_obj.GetCLRBindingExcludeMethods();
                    if(_exclude_methods != null && _exclude_methods.Count > 0)
                    {
                        foreach(var __method in _exclude_methods)
                        {
                            if (!gen_exclude_methods.Contains(__method))
                                gen_exclude_methods.Add(__method);
                        }
                    }

                    var _exclude_fields = define_obj.GetCLRBindingExcludeFields();
                    if (_exclude_fields != null && _exclude_fields.Count > 0)
                    {
                        foreach (var __field in _exclude_fields)
                        {
                            if (!gen_exclude_fields.Contains(__field))
                                gen_exclude_fields.Add(__field);
                        }
                    }

                    var _valueTypes = define_obj.GetValueTypeBinders();
                    if (_valueTypes != null && _valueTypes.Count > 0)
                    {
                        foreach (var __valueType in _valueTypes)
                        {
                            if (!gen_valueTypes.Contains(__valueType))
                                gen_valueTypes.Add(__valueType);
                        }
                    }

                    var _delegates = define_obj.GetDelegateTypes();
                    if (_delegates != null && _delegates.Count > 0)
                    {
                        foreach (var __delegate in _delegates)
                        {
                            if (!gen_delegates.Contains(__delegate))
                                gen_delegates.Add(__delegate);
                        }
                    }
                }
            }


            BindingCodeGenerator.GenerateBindingCode(gen_types, output_path, gen_exclude_methods, gen_exclude_fields, gen_valueTypes, gen_delegates);
            Debug.Log($"<color=#{TinaX.Internal.XEditorColorDefine.Color_Safe_16}>Generate code finish.</color>");
            AssetDatabase.Refresh();
        }


        [MenuItem("TinaX/X ILRuntime/Generator/Generate by assembly")]
        public static void GenCodeByAssemblies()
        {
            var config = XConfig.GetConfig<TinaX.XILRuntime.Internal.XILRTConfig>(XILConst.ConfigPath_Resources);
            if (config == null)
            {
                Debug.LogError($"[{XILConst.ServiceName}] Generate CLRBinding code failed: config file not found.");
                return;
            }
            string output_path = config.EditorCLRBindingCodeOutputPath;
            if (!output_path.StartsWith("Assets/"))
            {
                Debug.LogError($"[{XILConst.ServiceName}]Generate failed: Output folder path is invalid :" + output_path);
                return;
            }

            ILAppDomain domain = new ILAppDomain();
            DisposableGroup disGroup = new DisposableGroup();
            foreach (var path in config.EditorLoadAssemblyPaths)
            {
                if (!path.AssemblyPath.IsNullOrEmpty())
                {
                    var fs = new FileStream(path.AssemblyPath, FileMode.Open, FileAccess.Read);
                    disGroup.Register(fs);
                    domain.LoadAssembly(fs);
                }
            }

            List<Type> gen_valueTypes = new List<Type>(s_InternalValueTypes);
            List<Type> gen_delegates = new List<Type>(s_InternalDelegateTypes);

            Type t_define = typeof(ICLRGenerateDefine);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(t_define)))
                .ToArray();
            if (types.Length > 0)
            {
                foreach (var type in types)
                {
                    ICLRGenerateDefine define_obj = (ICLRGenerateDefine)Activator.CreateInstance(type);
                    define_obj.GenerateByAssemblies_InitILRuntime(domain);

                    var _valueTypes = define_obj.GetValueTypeBinders();
                    if (_valueTypes != null && _valueTypes.Count > 0)
                    {
                        foreach (var __valueType in _valueTypes)
                        {
                            if (!gen_valueTypes.Contains(__valueType))
                                gen_valueTypes.Add(__valueType);
                        }
                    }

                    var _delegates = define_obj.GetDelegateTypes();
                    if (_delegates != null && _delegates.Count > 0)
                    {
                        foreach (var __delegate in _delegates)
                        {
                            if (!gen_delegates.Contains(__delegate))
                                gen_delegates.Add(__delegate);
                        }
                    }
                }
            }



            BindingCodeGenerator.GenerateBindingCode(domain, output_path, gen_valueTypes, gen_delegates);
            disGroup.Dispose();
            Debug.Log($"<color=#{TinaX.Internal.XEditorColorDefine.Color_Safe_16}>Generate code finish.</color>");
            AssetDatabase.Refresh();
        }
    }
}

