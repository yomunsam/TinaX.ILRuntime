using UnityEngine;

namespace TinaX.XILRuntime.Internal
{
    public class XRuntimeConfig : ScriptableObject
    {
        public bool EnableILRuntime = true;
        
        public string EntryMethod; //入口方法

        public AssemblyLoadingMethod AssemblyLoadMode;

        #region If load from framework assets manager
        public string Dll_LoadPathByFrameworkAssetsManager;
        public string Symbol_LoadPathByFrameworkAssetsManager;
        #endregion



        #region Editor
        public string CLRBindingOutputFolder = "Assets/TinaX/XILRuntime/Generated";
        public string AssemblyFilePathInEditor;
        #endregion

    }
}

