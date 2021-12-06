using System.Collections.Generic;
using TinaX.XILRuntime.Consts;
using TinaX.XILRuntime.Structs;
using UnityEngine;

namespace TinaX.XILRuntime.ConfigAssets
{
#if TINAX_DEV
    [CreateAssetMenu(fileName = XILConsts.DefaultConfigAssetName, menuName = "TinaX Dev/Create ILRuntime Config Asset", order = 10)]
#endif
    public class XILRuntimeConfigAsset : ScriptableObject
    {
        public XILRuntimeConfigAsset()
        {
            LoadAssemblies = new List<AssemblyLoadInfo>();
        }

        public bool Enable = true;
        public List<AssemblyLoadInfo> LoadAssemblies;

        public string EntryClass;
        public string EntryMethod;

        public AssemblyLoaderAsset AssemblyLoaderAsset;
    }
}
