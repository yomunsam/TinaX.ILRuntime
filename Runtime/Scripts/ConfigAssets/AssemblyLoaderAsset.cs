using System;
using TinaX.Container;
using TinaX.XILRuntime.Loader;
using UnityEngine;

namespace TinaX.XILRuntime.ConfigAssets
{
    [Serializable]
    public abstract class AssemblyLoaderAsset : ScriptableObject
    {
        public abstract IAssemblyLoader CreateAssemblyLoader(IXCore services);
    }
}
