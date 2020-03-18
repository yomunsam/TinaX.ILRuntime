using ILRuntime.Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime
{
    public interface IXRuntime
    {
        object Invoke(string type, string method, object instance = null, params object[] param);

        
    }
}
