using System.Collections.Generic;
using TinaX.XILRuntime.Const;
using UnityEngine;

namespace TinaX.XILRuntime.Internal
{
#if TINAX_DEV
    [CreateAssetMenu(fileName =XILConst.ConfigFileName)]
#endif
    public class XILRTConfig : ScriptableObject
    {
        public XILRTConfig()
        {
            LoadAssemblies = new List<AssemblyAndSymbolPath>();
            EditorLoadAssemblyPaths = new List<AssemblyAndSymbolPath>();
        }
        public bool Enable = true;
        public List<AssemblyAndSymbolPath> LoadAssemblies;

        public string EntryClass;
        public string EntryMethod;



        /// <summary>
        /// 在编辑器下加载Assembly的路径
        /// </summary>
        public List<AssemblyAndSymbolPath> EditorLoadAssemblyPaths;

        /// <summary>
        /// 编辑器下，生成CLR绑定代码的输出路径
        /// </summary>
        public string EditorCLRBindingCodeOutputPath = "Assets/ILRuntime/Generated";

    }
}
