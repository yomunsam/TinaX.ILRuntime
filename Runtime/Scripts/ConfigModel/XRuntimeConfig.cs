using System.Collections.Generic;
using UnityEngine;

namespace TinaX.XILRuntime.Internal
{
    public class XRuntimeConfig : ScriptableObject
    {
        [System.Serializable]
        public struct S_AssemblyLoadPath
        {
            public string AssemblyLoadPath;
            public string SymbolFileLoadPath;
        }


        public bool EnableILRuntime = true;
        
        public string EntryMethod; //入口方法

        public List<S_AssemblyLoadPath> Assemblys;

        public bool NotLoadSymbolInNonDevelopMode = true; 

        #region Editor
        public string CLRBindingOutputFolder = "Assets/TinaX/XILRuntime/Generated";
        public string AssemblyFilePathInEditor;
        #endregion

    }
}

