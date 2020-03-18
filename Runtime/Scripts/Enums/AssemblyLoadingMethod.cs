using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.ILRuntime.Internal
{
    [Serializable]
    public enum AssemblyLoadingMethod
    {
        /// <summary>
        /// Use Framework's Asset Manager to load assemblies| 使用Framework的资产管理器
        /// </summary>
        FrameworkAssetsManager      = 1,

        /// <summary>
        /// Load directly using the dotnet System.IO | 使用.NET的IO库加载
        /// </summary>
        LoadFromSystemIO            = 2,
    }
}
