using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime
{
    public enum XRuntimeErrorCode
    {

        Unknow                                          = 0,
        /// <summary>
        /// Framework's assets manager interface invalid | 框架的资源管理接口无效
        /// </summary>
        FrameworkAssetsManagerInterfaceInValid          = 1,
        LoadAssemblyFailed                              = 2,
        ConfigFileInvalid                               = 3,
        AssemblyNameInvalid                             = 4,
        TypeNotFound                                    = 5,
        MethodNotFound                                  = 6,
        InvalidEntryMethod                              = 7,
    }
}
