using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinaXEditor.Const;

namespace TinaXEditor.ILRuntime.Const
{
    public static class XRuntimeEditorConst
    {
        public static readonly string ProjectSetting_Node = XEditorConst.ProjectSettingRootName + "/XILRuntime";

        public static readonly string[] HotUpdatableAssembly_BlackList =
        {
            "Assembly-CSharp",
            "Assembly-CSharp-Editor",
            "TinaX.Core",
            "TinaX.Core.Editor",
            "TinaX.ILRuntime",
            "TinaX.ILRuntime.Editor",
        };
    }
}
