using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime.Config
{
    public class DebugLogConfig
    {
        public bool LogStackTrace { get; set; } = true;
        public bool WarningStackTrace { get; set; } = true;
        public bool ErrorStackTrace { get; set; } = true;

        public bool EnablePrefix { get; set; } = true;
        public string PrefixText { get; set; } = "[ILRuntime]";
    }
}
