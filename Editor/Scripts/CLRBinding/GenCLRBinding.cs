using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILRuntime.Runtime.CLRBinding;
using UnityEngine;
using System.IO;
using TinaX.IO;
using UnityEditor;

namespace TinaXEditor.XILRuntime.Internal
{
    class GenCLRBinding
    {
        #region Menu

        [MenuItem("TinaX/X ILRuntime/CLR Binding/Generate By Types",false, 1)]
        static void Menu_GenCode()
        {
            GenCode();
        }
        

        #endregion
        public static void GenCode()
        {
            //config 
            var config = TinaX.XConfig.GetConfig<TinaX.XILRuntime.Internal.XRuntimeConfig>(TinaX.XILRuntime.Const.XRuntimeConst.ConfigPath_Resources, TinaX.AssetLoadType.Resources, false);
            if(config == null)
            {
                Debug.LogError("Generate failed: X ILRuntime config file not found.");
                return;
            }

            string output_path = config.CLRBindingOutputFolder;
            if (!output_path.StartsWith("Assets/"))
            {
                Debug.LogError("Generate failed: Output folder path in config file is invalid :" + output_path);
                return;
            }
            

            List<Type> list_types = new List<Type>(CLRBindingDefineInternal.BindTypes);
            Type t_interface = typeof(ICLRBindingData);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(t_interface)))
                .ToArray();
            if (types.Count() > 0)
            {
                foreach(var type in types)
                {
                    ICLRBindingData obj = (ICLRBindingData)Activator.CreateInstance(type);
                    var _types = obj.GetCLRBindingTypes();
                    if(_types != null && _types.Count > 0)
                    {
                        foreach (var _type in _types)
                        {
                            if (!list_types.Contains(_type))
                            {
                                list_types.Add(_type);
                            }
                        }
                    }
                    
                }
            }

            //gen
            BindingCodeGenerator.GenerateBindingCode(list_types, output_path);
        }

    }
}
